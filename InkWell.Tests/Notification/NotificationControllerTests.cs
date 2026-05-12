using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using InkWell.Notification.Controllers;
using InkWell.Notification.Services.Interfaces;
using InkWell.Notification.DTOs;

namespace InkWell.Notification.Tests
{
    [TestFixture]
    public class NotificationControllerTests
    {
        private Mock<INotificationService> notificationServiceMock;
        private NotificationController notificationController;

        [SetUp]
        public void Setup()
        {
            notificationServiceMock = new Mock<INotificationService>();
            notificationController = new NotificationController(notificationServiceMock.Object);
        }

        // Helper: configure authenticated user
        private void SetUser(string userId, string role = "READER")
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "TestAuthType");
            notificationController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        // Test user getting a list of all their notifications
        [Test]
        public async Task GetMy_AuthenticatedUser_ReturnsAllNotifications()
        {
            SetUser("1");
            List<NotificationResponseDTO> mockNotifications = new List<NotificationResponseDTO>
            {
                new NotificationResponseDTO { NotificationId = 1, Message = "Someone liked your post", IsRead = false },
                new NotificationResponseDTO { NotificationId = 2, Message = "New comment on your post", IsRead = true }
            };
            notificationServiceMock.Setup(s => s.GetByRecipient(1)).ReturnsAsync(mockNotifications);

            IActionResult result = await notificationController.GetMy();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<NotificationResponseDTO> notifications = okResult.Value as List<NotificationResponseDTO>;
            Assert.AreEqual(2, notifications.Count);
        }

        // Test getting notifications when the inbox is empty
        [Test]
        public async Task GetMy_NoNotifications_ReturnsEmptyList()
        {
            SetUser("2");
            notificationServiceMock.Setup(s => s.GetByRecipient(2)).ReturnsAsync(new List<NotificationResponseDTO>());

            IActionResult result = await notificationController.GetMy();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<NotificationResponseDTO> notifications = okResult.Value as List<NotificationResponseDTO>;
            Assert.AreEqual(0, notifications.Count);
        }

        // Test user filtering to see only unread notifications
        [Test]
        public async Task GetUnread_AuthenticatedUser_ReturnsUnreadOnly()
        {
            SetUser("1");
            List<NotificationResponseDTO> mockNotifications = new List<NotificationResponseDTO>
            {
                new NotificationResponseDTO { NotificationId = 1, Message = "Unread notification", IsRead = false }
            };
            notificationServiceMock.Setup(s => s.GetUnread(1)).ReturnsAsync(mockNotifications);

            IActionResult result = await notificationController.GetUnread();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<NotificationResponseDTO> notifications = okResult.Value as List<NotificationResponseDTO>;
            Assert.AreEqual(1, notifications.Count);
            Assert.IsFalse(notifications[0].IsRead);
        }

        // Test filtering for unread when everything is already read
        [Test]
        public async Task GetUnread_NoUnreadNotifications_ReturnsEmptyList()
        {
            SetUser("1");
            notificationServiceMock.Setup(s => s.GetUnread(1)).ReturnsAsync(new List<NotificationResponseDTO>());

            IActionResult result = await notificationController.GetUnread();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<NotificationResponseDTO> notifications = okResult.Value as List<NotificationResponseDTO>;
            Assert.AreEqual(0, notifications.Count);
        }

        // Test getting the badge count for unread notifications
        [Test]
        public async Task GetUnreadCount_AuthenticatedUser_ReturnsCount()
        {
            SetUser("1");
            notificationServiceMock.Setup(s => s.GetUnreadCount(1)).ReturnsAsync(5);

            IActionResult result = await notificationController.GetUnreadCount();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("5"));
        }

        // Test unread count is zero when all are read
        [Test]
        public async Task GetUnreadCount_NoUnread_ReturnsZero()
        {
            SetUser("1");
            notificationServiceMock.Setup(s => s.GetUnreadCount(1)).ReturnsAsync(0);

            IActionResult result = await notificationController.GetUnreadCount();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("0"));
        }

        // Test marking a single notification as read
        [Test]
        public async Task MarkAsRead_ValidId_ReturnsOk()
        {
            SetUser("1");
            notificationServiceMock.Setup(s => s.MarkAsRead(1, 1)).Returns(Task.CompletedTask);

            IActionResult result = await notificationController.MarkAsRead(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("marked as read"));
        }

        // Test user trying to mark someone else's notification as read
        [Test]
        public async Task MarkAsRead_NotOwnNotification_ReturnsBadRequest()
        {
            SetUser("1");
            notificationServiceMock.Setup(s => s.MarkAsRead(99, 1))
                                    .ThrowsAsync(new Exception("Notification not found."));

            IActionResult result = await notificationController.MarkAsRead(99);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test user marking their entire inbox as read
        [Test]
        public async Task MarkAllRead_AuthenticatedUser_ReturnsOk()
        {
            SetUser("1");
            notificationServiceMock.Setup(s => s.MarkAllRead(1)).Returns(Task.CompletedTask);

            IActionResult result = await notificationController.MarkAllRead();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("All notifications marked as read"));
        }

        // Test user deleting a specific notification
        [Test]
        public async Task Delete_ValidId_ReturnsOk()
        {
            SetUser("1");
            notificationServiceMock.Setup(s => s.DeleteNotification(1, 1)).Returns(Task.CompletedTask);

            IActionResult result = await notificationController.Delete(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("deleted"));
        }

        // Test user deleting a notification they don't own
        [Test]
        public async Task Delete_NotOwnNotification_ReturnsBadRequest()
        {
            SetUser("1");
            notificationServiceMock.Setup(s => s.DeleteNotification(99, 1))
                                    .ThrowsAsync(new Exception("Notification not found."));

            IActionResult result = await notificationController.Delete(99);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test user cleaning up their inbox by deleting all read items
        [Test]
        public async Task DeleteRead_AuthenticatedUser_ReturnsOk()
        {
            SetUser("1");
            notificationServiceMock.Setup(s => s.DeleteRead(1)).Returns(Task.CompletedTask);

            IActionResult result = await notificationController.DeleteRead();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("Read notifications deleted"));
        }

        // Test admin viewing every notification in the system
        [Test]
        public async Task GetAll_AdminUser_ReturnsAllNotifications()
        {
            SetUser("99", "ADMIN");
            List<NotificationResponseDTO> mockNotifications = new List<NotificationResponseDTO>
            {
                new NotificationResponseDTO { NotificationId = 1, Message = "User 1 notification" },
                new NotificationResponseDTO { NotificationId = 2, Message = "User 2 notification" }
            };
            notificationServiceMock.Setup(s => s.GetAll()).ReturnsAsync(mockNotifications);

            IActionResult result = await notificationController.GetAll();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<NotificationResponseDTO> notifications = okResult.Value as List<NotificationResponseDTO>;
            Assert.AreEqual(2, notifications.Count);
        }

        // Test admin sending a manual notification to specific users
        [Test]
        public async Task Broadcast_ValidDto_ReturnsOk()
        {
            SetUser("99", "ADMIN");
            BroadcastDTO dto = new BroadcastDTO
            {
                Title = "Platform Update",
                Message = "We have new features!",
                RecipientIds = new List<int> { 1, 2, 3 }
            };
            notificationServiceMock.Setup(s => s.SendBulk(dto)).Returns(Task.CompletedTask);

            IActionResult result = await notificationController.Broadcast(dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("Broadcast sent"));
        }

        // Test admin broadcast fails if no users are selected
        [Test]
        public async Task Broadcast_EmptyRecipientList_ReturnsBadRequest()
        {
            SetUser("99", "ADMIN");
            BroadcastDTO dto = new BroadcastDTO
            {
                Message = "Hello",
                RecipientIds = new List<int>()
            };
            notificationServiceMock.Setup(s => s.SendBulk(dto))
                                    .ThrowsAsync(new Exception("RecipientIds cannot be empty."));

            IActionResult result = await notificationController.Broadcast(dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test admin filtering all notifications by type (e.g. LIKES)
        [Test]
        public async Task GetByType_ValidType_ReturnsFilteredNotifications()
        {
            SetUser("99", "ADMIN");
            List<NotificationResponseDTO> mockNotifications = new List<NotificationResponseDTO>
            {
                new NotificationResponseDTO { NotificationId = 1, Message = "New comment", Type = "NEW_COMMENT" }
            };
            notificationServiceMock.Setup(s => s.GetByType("NEW_COMMENT")).ReturnsAsync(mockNotifications);

            IActionResult result = await notificationController.GetByType("NEW_COMMENT");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<NotificationResponseDTO> notifications = okResult.Value as List<NotificationResponseDTO>;
            Assert.AreEqual(1, notifications.Count);
        }

        // Test admin filtering by type with no results
        [Test]
        public async Task GetByType_NoMatchingNotifications_ReturnsEmptyList()
        {
            SetUser("99", "ADMIN");
            notificationServiceMock.Setup(s => s.GetByType("POST_LIKE")).ReturnsAsync(new List<NotificationResponseDTO>());

            IActionResult result = await notificationController.GetByType("POST_LIKE");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<NotificationResponseDTO> notifications = okResult.Value as List<NotificationResponseDTO>;
            Assert.AreEqual(0, notifications.Count);
        }

        // Test admin finding notifications linked to a specific post/comment
        [Test]
        public async Task GetByRelatedId_ValidRelatedId_ReturnsOk()
        {
            SetUser("99", "ADMIN");
            List<NotificationResponseDTO> mockNotifications = new List<NotificationResponseDTO>
            {
                new NotificationResponseDTO { NotificationId = 1, Message = "Post 5 related" }
            };
            notificationServiceMock.Setup(s => s.GetByRelatedId(5)).ReturnsAsync(mockNotifications);

            IActionResult result = await notificationController.GetByRelatedId(5);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<NotificationResponseDTO> notifications = okResult.Value as List<NotificationResponseDTO>;
            Assert.AreEqual(1, notifications.Count);
        }

        // Test admin finding notifications for a non-existent related object
        [Test]
        public async Task GetByRelatedId_NoRelatedNotifications_ReturnsEmptyList()
        {
            SetUser("99", "ADMIN");
            notificationServiceMock.Setup(s => s.GetByRelatedId(999)).ReturnsAsync(new List<NotificationResponseDTO>());

            IActionResult result = await notificationController.GetByRelatedId(999);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<NotificationResponseDTO> notifications = okResult.Value as List<NotificationResponseDTO>;
            Assert.AreEqual(0, notifications.Count);
        }
    }
}
