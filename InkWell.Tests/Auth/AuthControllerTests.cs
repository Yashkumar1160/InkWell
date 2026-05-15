using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using InkWell.Auth.Controllers;
using InkWell.Auth.Services.Interfaces;
using InkWell.Auth.DTOs;

namespace InkWell.Auth.Tests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthService> authServiceMock;
        private AuthController authController;

        [SetUp]
        public void Setup()
        {
            authServiceMock = new Mock<IAuthService>();
            authController = new AuthController(authServiceMock.Object);
        }

        // Helper: set authenticated user on controller context
        private void SetUser(string userId, string role = "READER")
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Role, role)
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "TestAuthType");
            authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        // Test to register a new user with valid details
        [Test]
        public async Task Register_ValidDto_ReturnsOkWithToken()
        {
            RegisterDTO dto = new RegisterDTO { Email = "test@test.com", Password = "Pass@123", FullName = "Test User" };
            AuthResponseDTO mockResponse = new AuthResponseDTO { Token = "some.jwt.token" };
            authServiceMock.Setup(s => s.Register(dto)).ReturnsAsync(mockResponse);

            IActionResult result = await authController.Register(dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            AuthResponseDTO response = okResult.Value as AuthResponseDTO;
            Assert.AreEqual("some.jwt.token", response.Token);
        }

        // Test to register with duplicate email that returns bad request
        [Test]
        public async Task Register_DuplicateEmail_ReturnsBadRequest()
        {
            RegisterDTO dto = new RegisterDTO { Email = "exists@test.com", Password = "Pass@123", FullName = "Test User" };
            authServiceMock.Setup(s => s.Register(dto)).ThrowsAsync(new Exception("Email already in use."));

            IActionResult result = await authController.Register(dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(400, badRequest.StatusCode);
        }

        // Test login with correct email and password
        [Test]
        public async Task Login_ValidCredentials_ReturnsTokenAndOk()
        {
            LoginDTO dto = new LoginDTO { Email = "test@test.com", Password = "Pass@123" };
            AuthResponseDTO mockResponse = new AuthResponseDTO { Token = "some.jwt.token" };
            authServiceMock.Setup(s => s.Login(dto)).ReturnsAsync(mockResponse);

            IActionResult result = await authController.Login(dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            AuthResponseDTO response = okResult.Value as AuthResponseDTO;
            Assert.AreEqual("some.jwt.token", response.Token);
        }

        // Test login with wrong password returns error
        [Test]
        public async Task Login_WrongPassword_ReturnsBadRequest()
        {
            LoginDTO dto = new LoginDTO { Email = "test@test.com", Password = "wrongpassword" };
            authServiceMock.Setup(s => s.Login(dto)).ThrowsAsync(new Exception("Wrong email or password"));

            IActionResult result = await authController.Login(dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test getting own profile when logged in
        [Test]
        public async Task GetProfile_AuthenticatedUser_ReturnsOk()
        {
            SetUser("1");
            object mockProfile = new { UserId = 1, FullName = "Test User" };
            authServiceMock.Setup(s => s.GetProfile(1)).ReturnsAsync(mockProfile);

            IActionResult result = await authController.GetProfile();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        // Test viewing another user's public profile
        [Test]
        public async Task GetPublicProfile_ValidId_ReturnsOk()
        {
            object mockProfile = new { UserId = 1, FullName = "Test Author" };
            authServiceMock.Setup(s => s.GetProfile(1)).ReturnsAsync(mockProfile);

            IActionResult result = await authController.GetPublicProfile(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        // Test viewing non-existent profile returns not found
        [Test]
        public async Task GetPublicProfile_InvalidId_ReturnsNotFound()
        {
            authServiceMock.Setup(s => s.GetProfile(999)).ThrowsAsync(new Exception("User not found."));

            IActionResult result = await authController.GetPublicProfile(999);

            NotFoundObjectResult notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        // Test updating own profile info
        [Test]
        public async Task UpdateProfile_ValidDto_ReturnsOkWithUpdatedProfile()
        {
            SetUser("1");
            UpdateProfileDTO dto = new UpdateProfileDTO { FullName = "New Name", Bio = "My bio" };
            object mockUpdated = new { FullName = "New Name" };
            authServiceMock.Setup(s => s.UpdateProfile(1, dto)).ReturnsAsync(mockUpdated);

            IActionResult result = await authController.UpdateProfile(dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        // Test updating profile with invalid data
        [Test]
        public async Task UpdateProfile_ServiceThrows_ReturnsBadRequest()
        {
            SetUser("1");
            UpdateProfileDTO dto = new UpdateProfileDTO { FullName = "" };
            authServiceMock.Setup(s => s.UpdateProfile(1, dto)).ThrowsAsync(new Exception("Invalid data."));

            IActionResult result = await authController.UpdateProfile(dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test changing password successfully
        [Test]
        public async Task ChangePassword_ValidDto_ReturnsOk()
        {
            SetUser("1");
            ChangePasswordDTO dto = new ChangePasswordDTO { OldPassword = "OldPass@1", NewPassword = "NewPass@2" };
            authServiceMock.Setup(s => s.ChangePassword(1, dto)).Returns(Task.CompletedTask);

            IActionResult result = await authController.ChangePassword(dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("Password changed"));
        }

        // Test changing password with wrong current password
        [Test]
        public async Task ChangePassword_WrongOldPassword_ReturnsBadRequest()
        {
            SetUser("1");
            ChangePasswordDTO dto = new ChangePasswordDTO { OldPassword = "wrongOld", NewPassword = "NewPass@2" };
            authServiceMock.Setup(s => s.ChangePassword(1, dto)).ThrowsAsync(new Exception("Old password is incorrect."));

            IActionResult result = await authController.ChangePassword(dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test admin changing user role
        [Test]
        public async Task ChangeRole_ValidRole_ReturnsOk()
        {
            SetUser("99", "ADMIN");
            authServiceMock.Setup(s => s.ChangeRole(1, "AUTHOR")).Returns(Task.CompletedTask);

            IActionResult result = await authController.ChangeRole(1, "AUTHOR");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("Role updated"));
        }

        // Test admin setting an invalid role
        [Test]
        public async Task ChangeRole_InvalidRole_ReturnsBadRequest()
        {
            SetUser("99", "ADMIN");
            authServiceMock.Setup(s => s.ChangeRole(1, "GOD")).ThrowsAsync(new Exception("Role must be READER, AUTHOR or ADMIN."));

            IActionResult result = await authController.ChangeRole(1, "GOD");

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test admin getting all users list
        [Test]
        public async Task GetAllUsers_ReturnsOkWithUserList()
        {
            SetUser("99", "ADMIN");
            List<object> mockUsers = new List<object> { new { UserId = 1 }, new { UserId = 2 } };
            authServiceMock.Setup(s => s.GetAllUsers()).ReturnsAsync(mockUsers);

            IActionResult result = await authController.GetAllUsers();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<object> users = okResult.Value as List<object>;
            Assert.AreEqual(2, users.Count);
        }

        // Test admin finding user by email
        [Test]
        public async Task GetByEmail_ExistingEmail_ReturnsOk()
        {
            SetUser("99", "ADMIN");
            object mockUser = new { UserId = 1, Email = "user@test.com" };
            authServiceMock.Setup(s => s.GetUserByEmail("user@test.com")).ReturnsAsync(mockUser);

            IActionResult result = await authController.GetByEmail("user@test.com");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        // Test admin finding non-existent user by email
        [Test]
        public async Task GetByEmail_NonExistentEmail_ReturnsNotFound()
        {
            SetUser("99", "ADMIN");
            authServiceMock.Setup(s => s.GetUserByEmail("ghost@test.com")).ThrowsAsync(new Exception("User not found."));

            IActionResult result = await authController.GetByEmail("ghost@test.com");

            NotFoundObjectResult notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        // Test admin filtering users by role
        [Test]
        public async Task GetByRole_ValidRole_ReturnsOkWithUsers()
        {
            SetUser("99", "ADMIN");
            List<object> mockUsers = new List<object> { new { UserId = 1, Role = "AUTHOR" } };
            authServiceMock.Setup(s => s.GetUsersByRole("AUTHOR")).ReturnsAsync(mockUsers);

            IActionResult result = await authController.GetByRole("AUTHOR");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        // Test admin filtering by invalid role
        [Test]
        public async Task GetByRole_InvalidRole_ReturnsBadRequest()
        {
            SetUser("99", "ADMIN");
            authServiceMock.Setup(s => s.GetUsersByRole("SUPERUSER")).ThrowsAsync(new Exception("Invalid role."));

            IActionResult result = await authController.GetByRole("SUPERUSER");

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test admin searching users by name
        [Test]
        public async Task SearchUsers_ValidKeyword_ReturnsMatchingUsers()
        {
            SetUser("99", "ADMIN");
            List<object> mockUsers = new List<object> { new { UserId = 1, FullName = "Alice" } };
            authServiceMock.Setup(s => s.SearchUsers("ali")).ReturnsAsync(mockUsers);

            IActionResult result = await authController.SearchUsers("ali");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<object> users = okResult.Value as List<object>;
            Assert.AreEqual(1, users.Count);
        }

        // Test admin deactivating a user account
        [Test]
        public async Task Deactivate_ValidId_ReturnsOk()
        {
            SetUser("99", "ADMIN");
            authServiceMock.Setup(s => s.Deactivate(1)).Returns(Task.CompletedTask);

            IActionResult result = await authController.Deactivate(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("deactivated"));
        }

        // Test admin reactivating an already active account
        [Test]
        public async Task Reactivate_AlreadyActiveUser_ReturnsBadRequest()
        {
            SetUser("99", "ADMIN");
            authServiceMock.Setup(s => s.Reactivate(1)).ThrowsAsync(new Exception("Account is already active."));

            IActionResult result = await authController.Reactivate(1);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test admin reactivating a disabled account
        [Test]
        public async Task Reactivate_DeactivatedUser_ReturnsOk()
        {
            SetUser("99", "ADMIN");
            authServiceMock.Setup(s => s.Reactivate(2)).Returns(Task.CompletedTask);

            IActionResult result = await authController.Reactivate(2);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("reactivated"));
        }

        // Test admin permanently deleting a user
        [Test]
        public async Task DeleteUser_ValidId_ReturnsOk()
        {
            SetUser("99", "ADMIN");
            authServiceMock.Setup(s => s.DeleteUser(5)).Returns(Task.CompletedTask);

            IActionResult result = await authController.DeleteUser(5);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("deleted"));
        }

        // Test admin deleting non-existent user
        [Test]
        public async Task DeleteUser_InvalidId_ReturnsBadRequest()
        {
            SetUser("99", "ADMIN");
            authServiceMock.Setup(s => s.DeleteUser(999)).ThrowsAsync(new Exception("User not found."));

            IActionResult result = await authController.DeleteUser(999);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test logging out current user
        [Test]
        public async Task Logout_AuthenticatedUser_ReturnsOk()
        {
            SetUser("1");
            authServiceMock.Setup(s => s.Logout()).Returns(Task.CompletedTask);

            IActionResult result = await authController.Logout();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("Logged out"));
        }

        // Test refreshing jwt token
        [Test]
        public async Task Refresh_ValidToken_ReturnsNewToken()
        {
            AuthResponseDTO mockResponse = new AuthResponseDTO { Token = "new.jwt.token" };
            authServiceMock.Setup(s => s.RefreshToken("old.token")).ReturnsAsync(mockResponse);

            IActionResult result = await authController.Refresh("old.token");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            AuthResponseDTO response = okResult.Value as AuthResponseDTO;
            Assert.AreEqual("new.jwt.token", response.Token);
        }

        // Test refreshing an expired token returns error
        [Test]
        public async Task Refresh_ExpiredToken_ReturnsUnauthorized()
        {
            authServiceMock.Setup(s => s.RefreshToken("expired.token")).ThrowsAsync(new Exception("Token has expired."));

            IActionResult result = await authController.Refresh("expired.token");

            UnauthorizedObjectResult unauthorized = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorized);
        }

        // Test validating a good jwt token
        [Test]
        public void Validate_ValidToken_ReturnsOkWithValidTrue()
        {
            authServiceMock.Setup(s => s.ValidateToken("valid.token")).Returns(true);

            IActionResult result = authController.Validate("valid.token");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("True"));
        }

        // Test validating a fake jwt token
        [Test]
        public void Validate_InvalidToken_ReturnsUnauthorized()
        {
            authServiceMock.Setup(s => s.ValidateToken("bad.token")).Returns(false);

            IActionResult result = authController.Validate("bad.token");

            UnauthorizedObjectResult unauthorized = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorized);
        }
    }
}
