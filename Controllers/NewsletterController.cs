using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using InkWell.Newsletter.DTOs;
using InkWell.Newsletter.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InkWell.Newsletter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsletterController : ControllerBase
    {
        // INewsletterService interface instance
        private INewsletterService newsletterService;

        // IConfiguration instance
        private IConfiguration configuration;

        // Constructor Dependency Injection
        public NewsletterController(INewsletterService service, IConfiguration config)
        {
            newsletterService = service;
            configuration = config;
        }

        // POST /api/newsletter/subscribe
        // anyone can subscribe 
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeDTO dto)
        {
            try
            {
                SubscriberResponseDTO result = await newsletterService.Subscribe(dto);
                return Ok(new { message = "Subscription created. Please check your email to confirm.", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/newsletter/confirm/{token}
        // user clicks this link in their confirmation email
        [HttpGet("confirm/{token}")]
        public async Task<IActionResult> Confirm(string token)
        {
            try
            {
                await newsletterService.ConfirmSubscription(token);
                return Ok(new { message = "Subscription confirmed. Welcome to InkWell!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/newsletter/unsubscribe/{token}
        // user clicks this link in any newsletter email
        [HttpGet("unsubscribe/{token}")]
        public async Task<IActionResult> Unsubscribe(string token)
        {
            try
            {
                await newsletterService.Unsubscribe(token);
                return Ok(new { message = "You have been unsubscribed." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/newsletter/all
        // admin sees all subscribers with status
        [HttpGet("all")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            List<SubscriberResponseDTO> subscribers = await newsletterService.GetAllSubscribers();
            return Ok(subscribers);
        }

        // GET /api/newsletter/by-status?status=ACTIVE
        // admin filters subscribers by status
        [HttpGet("by-status")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetByStatus([FromQuery] string status)
        {
            try
            {
                List<SubscriberResponseDTO> subscribers = await newsletterService.GetByStatus(status);
                return Ok(subscribers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/newsletter/count
        // get count of active subscribers
        [HttpGet("count")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetCount()
        {
            int count = await newsletterService.GetSubscriberCount();
            return Ok(new { count = count });
        }

        // POST /api/newsletter/send
        // admin sends a newsletter campaign
        [HttpPost("send")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> SendNewsletter([FromBody] SendNewsletterDTO dto)
        {
            try
            {
                await newsletterService.SendNewsletter(dto);
                return Ok(new { message = "Newsletter sent successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST /api/newsletter/notify-post
        // called internally by Post Service via HttpClient
        // protected by internal API key not JWT
        [HttpPost("notify-post")]
        public async Task<IActionResult> NotifyPost(
            [FromBody] NewPostNotificationDTO dto,
            [FromHeader(Name = "X-Internal-Key")] string internalKey)
        {
            // verify internal api key
            string expectedKey = configuration["InternalApi:Key"];
            if (internalKey != expectedKey)
            {
                return Unauthorized(new { message = "Invalid internal key." });
            }

            try
            {
                await newsletterService.SendPostNotification(dto);
                return Ok(new { message = "Post notification sent." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/newsletter/preferences/{token}
        // subscriber updates their preferences via token
        [HttpPut("preferences/{token}")]
        public async Task<IActionResult> UpdatePreferences(string token, [FromBody] UpdatePreferencesDTO dto)
        {
            try
            {
                await newsletterService.UpdatePreferences(token, dto);
                return Ok(new { message = "Preferences updated." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/newsletter/by-email?email=yash@gmail.com
        // admin looks up a specific subscriber by email
        [HttpGet("by-email")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            try
            {
                SubscriberResponseDTO subscriber = await newsletterService.GetByEmail(email);
                return Ok(subscriber);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE /api/newsletter/delete/5
        // admin permanently removes a subscriber
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteSubscriber(int id)
        {
            try
            {
                await newsletterService.DeleteSubscriber(id);
                return Ok(new { message = "Subscriber deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // PUT /api/newsletter/my-preferences
        // logged in user updates their own preferences
        [HttpPut("my-preferences")]
        [Authorize]
        public async Task<IActionResult> UpdateMyPreferences([FromBody] UpdatePreferencesDTO dto)
        {
            try
            {
                string idStr = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
                int userId = int.Parse(idStr);

                await newsletterService.UpdatePreferencesByUserId(userId, dto);
                return Ok(new { message = "Preferences updated." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}