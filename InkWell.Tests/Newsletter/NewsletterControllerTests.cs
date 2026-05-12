using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using InkWell.Newsletter.Controllers;
using InkWell.Newsletter.Services.Interfaces;
using InkWell.Newsletter.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InkWell.Newsletter.Tests
{
    [TestFixture]
    public class NewsletterControllerTests
    {
        private Mock<INewsletterService> newsletterServiceMock;
        private NewsletterController newsletterController;

        [SetUp]
        public void Setup()
        {
            newsletterServiceMock = new Mock<INewsletterService>();
            newsletterController = new NewsletterController(newsletterServiceMock.Object);
        }

        // Helper: set admin user on controller
        private void SetAdmin(string userId = "99")
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "ADMIN")
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "TestAuthType");
            newsletterController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        // Helper: set authenticated regular user
        private void SetUser(string userId)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "READER")
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "TestAuthType");
            newsletterController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        // Test a new user subscribing to the newsletter
        [Test]
        public async Task Subscribe_ValidEmail_ReturnsOkWithMessage()
        {
            SubscribeDTO dto = new SubscribeDTO { Email = "user@example.com", FullName = "Test User" };
            SubscriberResponseDTO mockResponse = new SubscriberResponseDTO { Email = "user@example.com" };
            newsletterServiceMock.Setup(s => s.Subscribe(dto)).ReturnsAsync(mockResponse);

            IActionResult result = await newsletterController.Subscribe(dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("Subscription created"));
        }

        // Test subscribing with an email that is already registered
        [Test]
        public async Task Subscribe_DuplicateEmail_ReturnsBadRequest()
        {
            SubscribeDTO dto = new SubscribeDTO { Email = "already@example.com", FullName = "Test User" };
            newsletterServiceMock.Setup(s => s.Subscribe(dto))
                                  .ThrowsAsync(new Exception("Email already subscribed."));

            IActionResult result = await newsletterController.Subscribe(dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test confirming a subscription via email token
        [Test]
        public async Task Confirm_ValidToken_ReturnsOkWithWelcomeMessage()
        {
            newsletterServiceMock.Setup(s => s.ConfirmSubscription("valid-token")).Returns(Task.CompletedTask);

            IActionResult result = await newsletterController.Confirm("valid-token");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("confirmed"));
        }

        // Test confirmation fails with an invalid or old token
        [Test]
        public async Task Confirm_InvalidToken_ReturnsBadRequest()
        {
            newsletterServiceMock.Setup(s => s.ConfirmSubscription("bad-token"))
                                  .ThrowsAsync(new Exception("Invalid or expired token."));

            IActionResult result = await newsletterController.Confirm("bad-token");

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test a user unsubscribing using their unique link
        [Test]
        public async Task Unsubscribe_ValidToken_ReturnsOkWithMessage()
        {
            newsletterServiceMock.Setup(s => s.Unsubscribe("valid-unsub-token")).Returns(Task.CompletedTask);

            IActionResult result = await newsletterController.Unsubscribe("valid-unsub-token");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("unsubscribed"));
        }

        // Test unsubscribing fails if the token is wrong
        [Test]
        public async Task Unsubscribe_InvalidToken_ReturnsBadRequest()
        {
            newsletterServiceMock.Setup(s => s.Unsubscribe("bad-token"))
                                  .ThrowsAsync(new Exception("Token not found."));

            IActionResult result = await newsletterController.Unsubscribe("bad-token");

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test admin viewing all people on the mailing list
        [Test]
        public async Task GetAll_AdminUser_ReturnsOkWithSubscriberList()
        {
            SetAdmin();
            List<SubscriberResponseDTO> mockSubscribers = new List<SubscriberResponseDTO>
            {
                new SubscriberResponseDTO { Email = "a@example.com" },
                new SubscriberResponseDTO { Email = "b@example.com" }
            };
            newsletterServiceMock.Setup(s => s.GetAllSubscribers()).ReturnsAsync(mockSubscribers);

            IActionResult result = await newsletterController.GetAll();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<SubscriberResponseDTO> subscribers = okResult.Value as List<SubscriberResponseDTO>;
            Assert.AreEqual(2, subscribers.Count);
        }

        // Test admin filtering subscribers by status (e.g. pending vs confirmed)
        [Test]
        public async Task GetByStatus_ValidStatus_ReturnsFilteredSubscribers()
        {
            SetAdmin();
            List<SubscriberResponseDTO> mockSubscribers = new List<SubscriberResponseDTO>
            {
                new SubscriberResponseDTO { Email = "active@example.com" }
            };
            newsletterServiceMock.Setup(s => s.GetByStatus("ACTIVE")).ReturnsAsync(mockSubscribers);

            IActionResult result = await newsletterController.GetByStatus("ACTIVE");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<SubscriberResponseDTO> subscribers = okResult.Value as List<SubscriberResponseDTO>;
            Assert.AreEqual(1, subscribers.Count);
        }

        // Test filtering by an invalid status name
        [Test]
        public async Task GetByStatus_InvalidStatus_ReturnsBadRequest()
        {
            SetAdmin();
            newsletterServiceMock.Setup(s => s.GetByStatus("INVALID"))
                                  .ThrowsAsync(new Exception("Invalid status value."));

            IActionResult result = await newsletterController.GetByStatus("INVALID");

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test admin getting the total number of subscribers
        [Test]
        public async Task GetCount_AdminUser_ReturnsSubscriberCount()
        {
            SetAdmin();
            newsletterServiceMock.Setup(s => s.GetSubscriberCount()).ReturnsAsync(42);

            IActionResult result = await newsletterController.GetCount();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("42"));
        }

        // Test admin broadcasting a new email to all subscribers
        [Test]
        public async Task SendNewsletter_ValidDto_ReturnsOk()
        {
            SetAdmin();
            SendNewsletterDTO dto = new SendNewsletterDTO { Subject = "Monthly Update", Body = "Hello subscribers!" };
            newsletterServiceMock.Setup(s => s.SendNewsletter(dto)).Returns(Task.CompletedTask);

            IActionResult result = await newsletterController.SendNewsletter(dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("sent successfully"));
        }

        // Test sending newsletter fails if there are no active subscribers
        [Test]
        public async Task SendNewsletter_ServiceThrows_ReturnsBadRequest()
        {
            SetAdmin();
            SendNewsletterDTO dto = new SendNewsletterDTO { Subject = "", Body = "" };
            newsletterServiceMock.Setup(s => s.SendNewsletter(dto))
                                  .ThrowsAsync(new Exception("No active subscribers."));

            IActionResult result = await newsletterController.SendNewsletter(dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test user updating their email frequency via a token link
        [Test]
        public async Task UpdatePreferences_ValidToken_ReturnsOk()
        {
            UpdatePreferencesDTO dto = new UpdatePreferencesDTO { Preferences = "weekly" };
            newsletterServiceMock.Setup(s => s.UpdatePreferences("valid-token", dto)).Returns(Task.CompletedTask);

            IActionResult result = await newsletterController.UpdatePreferences("valid-token", dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("Preferences updated"));
        }

        // Test updating frequency fails with a bad link token
        [Test]
        public async Task UpdatePreferences_InvalidToken_ReturnsBadRequest()
        {
            UpdatePreferencesDTO dto = new UpdatePreferencesDTO { Preferences = "none" };
            newsletterServiceMock.Setup(s => s.UpdatePreferences("bad-token", dto))
                                  .ThrowsAsync(new Exception("Token not found."));

            IActionResult result = await newsletterController.UpdatePreferences("bad-token", dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test admin finding a specific subscriber by their email
        [Test]
        public async Task GetByEmail_ExistingEmail_ReturnsOk()
        {
            SetAdmin();
            SubscriberResponseDTO mockSub = new SubscriberResponseDTO { Email = "user@example.com" };
            newsletterServiceMock.Setup(s => s.GetByEmail("user@example.com")).ReturnsAsync(mockSub);

            IActionResult result = await newsletterController.GetByEmail("user@example.com");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        // Test admin searching for a non-existent email
        [Test]
        public async Task GetByEmail_NonExistentEmail_ReturnsNotFound()
        {
            SetAdmin();
            newsletterServiceMock.Setup(s => s.GetByEmail("ghost@example.com"))
                                  .ThrowsAsync(new Exception("Subscriber not found."));

            IActionResult result = await newsletterController.GetByEmail("ghost@example.com");

            NotFoundObjectResult notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        // Test admin removing someone from the mailing list
        [Test]
        public async Task DeleteSubscriber_ValidId_ReturnsOk()
        {
            SetAdmin();
            newsletterServiceMock.Setup(s => s.DeleteSubscriber(5)).Returns(Task.CompletedTask);

            IActionResult result = await newsletterController.DeleteSubscriber(5);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("deleted"));
        }

        // Test admin deleting a subscriber that doesn't exist
        [Test]
        public async Task DeleteSubscriber_NotFound_ReturnsBadRequest()
        {
            SetAdmin();
            newsletterServiceMock.Setup(s => s.DeleteSubscriber(999))
                                  .ThrowsAsync(new Exception("Subscriber not found."));

            IActionResult result = await newsletterController.DeleteSubscriber(999);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test logged-in user updating their newsletter settings
        [Test]
        public async Task UpdateMyPreferences_ValidDto_ReturnsOk()
        {
            SetUser("1");
            UpdatePreferencesDTO dto = new UpdatePreferencesDTO { Preferences = "weekly" };
            newsletterServiceMock.Setup(s => s.UpdatePreferencesByUserId(1, dto)).Returns(Task.CompletedTask);

            IActionResult result = await newsletterController.UpdateMyPreferences(dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("Preferences updated"));
        }

        // Test updating settings fails if user isn't subscribed
        [Test]
        public async Task UpdateMyPreferences_ServiceThrows_ReturnsBadRequest()
        {
            SetUser("1");
            UpdatePreferencesDTO dto = new UpdatePreferencesDTO { Preferences = "none" };
            newsletterServiceMock.Setup(s => s.UpdatePreferencesByUserId(1, dto))
                                  .ThrowsAsync(new Exception("No subscription found for this user."));

            IActionResult result = await newsletterController.UpdateMyPreferences(dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }
    }
}
