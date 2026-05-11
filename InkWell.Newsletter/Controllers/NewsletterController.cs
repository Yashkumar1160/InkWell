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
        private INewsletterService newsletterService;

        public NewsletterController(INewsletterService service)
        {
            newsletterService = service;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeDTO dto)
        {
            SubscriberResponseDTO result = await newsletterService.Subscribe(dto);
            return Ok(new { message = "Subscription created. Please check your email to confirm.", data = result });
        }

        [HttpGet("confirm/{token}")]
        public async Task<IActionResult> Confirm(string token)
        {
            await newsletterService.ConfirmSubscription(token);
            return Ok(new { message = "Subscription confirmed. Welcome to InkWell!" });
        }

        [HttpGet("unsubscribe/{token}")]
        public async Task<IActionResult> Unsubscribe(string token)
        {
            await newsletterService.Unsubscribe(token);
            return Ok(new { message = "You have been unsubscribed." });
        }

        [HttpGet("all")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            List<SubscriberResponseDTO> subscribers = await newsletterService.GetAllSubscribers();
            return Ok(subscribers);
        }

        [HttpGet("by-status")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetByStatus([FromQuery] string status)
        {
            List<SubscriberResponseDTO> subscribers = await newsletterService.GetByStatus(status);
            return Ok(subscribers);
        }

        [HttpGet("count")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetCount()
        {
            int count = await newsletterService.GetSubscriberCount();
            return Ok(new { count = count });
        }

        [HttpPost("send")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> SendNewsletter([FromBody] SendNewsletterDTO dto)
        {
            await newsletterService.SendNewsletter(dto);
            return Ok(new { message = "Newsletter sent successfully." });
        }

        [HttpPut("preferences/{token}")]
        public async Task<IActionResult> UpdatePreferences(string token, [FromBody] UpdatePreferencesDTO dto)
        {
            await newsletterService.UpdatePreferences(token, dto);
            return Ok(new { message = "Preferences updated." });
        }

        [HttpGet("by-email")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            SubscriberResponseDTO subscriber = await newsletterService.GetByEmail(email);
            return Ok(subscriber);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteSubscriber(int id)
        {
            await newsletterService.DeleteSubscriber(id);
            return Ok(new { message = "Subscriber deleted." });
        }

        [HttpPut("my-preferences")]
        [Authorize]
        public async Task<IActionResult> UpdateMyPreferences([FromBody] UpdatePreferencesDTO dto)
        {
            string idStr = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            int userId = int.Parse(idStr);

            await newsletterService.UpdatePreferencesByUserId(userId, dto);
            return Ok(new { message = "Preferences updated." });
        }
    }
}