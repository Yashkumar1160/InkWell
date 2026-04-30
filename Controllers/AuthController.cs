using System.Security.Claims;
using InkWell.Auth.Services.Interfaces;
using InkWell.Auth.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InkWell.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // IAuthService instance
        private IAuthService authService;

        // Constructor Dependency Injection
        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        
        // Register New User
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            try
            {
                AuthResponseDTO result = await authService.Register(dto);

                // return Ok with token and user info
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Login User
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                AuthResponseDTO result = await authService.Login(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // Get Profile
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            // read user id from JWT token claims
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(id);

            object profile = await authService.GetProfile(userId);
            return Ok(profile);
        }

        // Get Public Profile by ID (No login needed)
        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetPublicProfile(int id)
        {
            try
            {
                object profile = await authService.GetProfile(id);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        // Logged user can update their profile
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
        {
            // read user id from JWT token claims
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(id);

            await authService.UpdateProfile(userId, dto);
            return Ok(new { message = "Profile updated" });
        }

        // Change Password 
        [HttpPut("password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            try
            {
                // read user id from JWT token
                string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int userId = int.Parse(id);

                // Change password using auth service
                await authService.ChangePassword(userId, dto);
                return Ok(new { message = "Password changed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // Change Role (ADMIN only)
        [HttpPut("role/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] string role)
        {
            try
            {
                // change role using authService
                await authService.ChangeRole(id, role);
                return Ok(new { message = "Role updated." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        // Get all users
        [HttpGet("users")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllUsers()
        {
            // get all users using authService
            List<object> users = await authService.GetAllUsers();
            return Ok(users);
        }


        // Get user by specific email
        [HttpGet("users/by-email")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            try
            {
                // get user by email using authService
                object user = await authService.GetUserByEmail(email);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Get users by specific role
        [HttpGet("users/by-role")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetByRole([FromQuery] string role)
        {
            try
            {
                // get user by specific role using authService
                List<object> users = await authService.GetUsersByRole(role);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Search users by username
        [HttpGet("search")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> SearchUsers([FromQuery] string keyword)
        {
            List<object> users = await authService.SearchUsers(keyword);
            return Ok(users);
        }

        // Deactivate account (ADMIN only)
        [HttpDelete("deactivate/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await authService.Deactivate(id);
            return Ok(new { message = "Account deactivated." });
        }


        // Reactivate account (ADMIN only)
        [HttpPost("reactivate/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Reactivate(int id)
        {
            try
            {
                await authService.Reactivate(id);
                return Ok(new { message = "Account reactivated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Delete user from database (ADMIN only)
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await authService.DeleteUser(id);
                return Ok(new { message = "User permanently deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // Logout user account
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await authService.Logout();

            return Ok(new { message = "Logged out successfully" });
        }

        // Refresh jwt token
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string token)
        {
            try
            {
                AuthResponseDTO result = await authService.RefreshToken(token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // Validate token
        [HttpPost("validate")]
        public IActionResult Validate([FromBody] string token)
        {
            bool isValid = authService.ValidateToken(token);

            if (isValid)
            {
                return Ok(new { valid = true });
            }

            return Unauthorized(new { valid = false });
        }
    }
}