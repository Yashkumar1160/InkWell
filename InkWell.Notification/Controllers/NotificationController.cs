using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using InkWell.Notification.DTOs;
using InkWell.Notification.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace InkWell.Notification.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private INotificationService notificationService;

        public NotificationController(INotificationService service)
        {
            notificationService = service;
        }

        // GET /api/notification/my
        // logged in user gets their own notifications
        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMy()
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int recipientId = int.Parse(idStr);

            List<NotificationResponseDTO> notifications =
                await notificationService.GetByRecipient(recipientId);
            return Ok(notifications);
        }

        // GET /api/notification/unread
        // get only unread notifications
        [HttpGet("unread")]
        [Authorize]
        public async Task<IActionResult> GetUnread()
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int recipientId = int.Parse(idStr);

            List<NotificationResponseDTO> notifications =
                await notificationService.GetUnread(recipientId);
            return Ok(notifications);
        }

        // GET /api/notification/unread-count
        // get unread count for notification bell badge
        [HttpGet("unread-count")]
        [Authorize]
        public async Task<IActionResult> GetUnreadCount()
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int recipientId = int.Parse(idStr);

            int count = await notificationService.GetUnreadCount(recipientId);
            return Ok(new { count = count });
        }

        // PUT /api/notification/read/5
        // mark single notification as read
        [HttpPut("read/{id}")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int recipientId = int.Parse(idStr);

            await notificationService.MarkAsRead(id, recipientId);
            return Ok(new { message = "Notification marked as read." });
        }

        // PUT /api/notification/read-all
        // mark all notifications as read
        [HttpPut("read-all")]
        [Authorize]
        public async Task<IActionResult> MarkAllRead()
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int recipientId = int.Parse(idStr);

            await notificationService.MarkAllRead(recipientId);
            return Ok(new { message = "All notifications marked as read." });
        }

        // DELETE /api/notification/delete/5
        // delete a single notification
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int recipientId = int.Parse(idStr);

            await notificationService.DeleteNotification(id, recipientId);
            return Ok(new { message = "Notification deleted." });
        }

        // DELETE /api/notification/delete-read
        // delete all read notifications for logged in user
        [HttpDelete("delete-read")]
        [Authorize]
        public async Task<IActionResult> DeleteRead()
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int recipientId = int.Parse(idStr);

            await notificationService.DeleteRead(recipientId);
            return Ok(new { message = "Read notifications deleted." });
        }

        // GET /api/notification/all
        // admin sees all notifications platform wide
        [HttpGet("all")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            List<NotificationResponseDTO> notifications =
                await notificationService.GetAll();
            return Ok(notifications);
        }

        // POST /api/notification/broadcast
        // admin sends notification to multiple users
        [HttpPost("broadcast")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Broadcast([FromBody] BroadcastDTO dto)
        {
            await notificationService.SendBulk(dto);
            return Ok(new { message = "Broadcast sent." });
        }

        // GET /api/notification/by-type?type=NEW_COMMENT
        // admin filters notifications by type
        [HttpGet("by-type")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetByType([FromQuery] string type)
        {
            List<NotificationResponseDTO> notifications =
                await notificationService.GetByType(type);
            return Ok(notifications);
        }

        // GET /api/notification/by-related/5
        // get all notifications for a specific post or comment
        [HttpGet("by-related/{relatedId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetByRelatedId(int relatedId)
        {
            List<NotificationResponseDTO> notifications =
                await notificationService.GetByRelatedId(relatedId);
            return Ok(notifications);
        }
    }
}