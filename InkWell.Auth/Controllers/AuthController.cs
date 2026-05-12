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
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            AuthResponseDTO result = await authService.Register(dto);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            AuthResponseDTO result = await authService.Login(dto);
            return Ok(result);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(id);

            object profile = await authService.GetProfile(userId);
            return Ok(profile);
        }

        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetPublicProfile(int id)
        {
            object profile = await authService.GetProfile(id);
            return Ok(profile);
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(id);

            var profile = await authService.UpdateProfile(userId, dto);
            return Ok(profile);
        }

        [HttpPut("password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(id);

            await authService.ChangePassword(userId, dto);
            return Ok(new { message = "Password changed" });
        }

        [HttpPut("role/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] string role)
        {
            await authService.ChangeRole(id, role);
            return Ok(new { message = "Role updated." });
        }

        [HttpGet("users")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllUsers()
        {
            List<object> users = await authService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("users/by-email")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            object user = await authService.GetUserByEmail(email);
            return Ok(user);
        }

        [HttpGet("users/by-role")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetByRole([FromQuery] string role)
        {
            List<object> users = await authService.GetUsersByRole(role);
            return Ok(users);
        }

        [HttpGet("search")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> SearchUsers([FromQuery] string keyword)
        {
            List<object> users = await authService.SearchUsers(keyword);
            return Ok(users);
        }

        [HttpDelete("deactivate/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await authService.Deactivate(id);
            return Ok(new { message = "Account deactivated." });
        }

        [HttpPost("reactivate/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Reactivate(int id)
        {
            await authService.Reactivate(id);
            return Ok(new { message = "Account reactivated successfully." });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await authService.DeleteUser(id);
            return Ok(new { message = "User permanently deleted." });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await authService.Logout();
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string token)
        {
            AuthResponseDTO result = await authService.RefreshToken(token);
            return Ok(result);
        }

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