using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using InkWell.Comment.Controllers;
using InkWell.Comment.Services.Interfaces;
using InkWell.Comment.DTOs;

namespace InkWell.Comment.Tests
{
    [TestFixture]
    public class CommentControllerTests
    {
        private Mock<ICommentService> commentServiceMock;
        private CommentController commentController;

        [SetUp]
        public void Setup()
        {
            commentServiceMock = new Mock<ICommentService>();
            commentController = new CommentController(commentServiceMock.Object);
        }

        // Helper: configure authenticated user
        private void SetUser(string userId, string role = "READER", string fullName = "Test User")
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role),
                new Claim("FullName", fullName)
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "TestAuthType");
            commentController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        // Test adding a new comment to a post
        [Test]
        public async Task Add_ValidDto_ReturnsOkWithComment()
        {
            SetUser("1");
            CreateCommentDTO dto = new CreateCommentDTO { PostId = 1, Content = "Great post!", PostAuthorId = 2 };
            CommentResponseDTO mockResponse = new CommentResponseDTO { CommentId = 1, Content = "Great post!" };
            commentServiceMock.Setup(s => s.AddComment(1, It.IsAny<string>(), dto)).ReturnsAsync(mockResponse);

            IActionResult result = await commentController.Add(dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            CommentResponseDTO comment = okResult.Value as CommentResponseDTO;
            Assert.AreEqual("Great post!", comment.Content);
        }

        // Test adding comment fails for non-existent post
        [Test]
        public async Task Add_ServiceThrows_ReturnsBadRequest()
        {
            SetUser("1");
            CreateCommentDTO dto = new CreateCommentDTO { PostId = 999, Content = "Test", PostAuthorId = 2 };
            commentServiceMock.Setup(s => s.AddComment(1, It.IsAny<string>(), dto))
                               .ThrowsAsync(new Exception("Post not found."));

            IActionResult result = await commentController.Add(dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test getting all comments for a post
        [Test]
        public async Task GetByPost_ValidPostId_ReturnsOkWithComments()
        {
            List<CommentResponseDTO> mockComments = new List<CommentResponseDTO>
            {
                new CommentResponseDTO { CommentId = 1, Content = "First comment" },
                new CommentResponseDTO { CommentId = 2, Content = "Second comment" }
            };
            commentServiceMock.Setup(s => s.GetByPost(1)).ReturnsAsync(mockComments);

            IActionResult result = await commentController.GetByPost(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<CommentResponseDTO> comments = okResult.Value as List<CommentResponseDTO>;
            Assert.AreEqual(2, comments.Count);
        }

        // Test getting comments for a post with no comments
        [Test]
        public async Task GetByPost_NoComments_ReturnsEmptyList()
        {
            commentServiceMock.Setup(s => s.GetByPost(99)).ReturnsAsync(new List<CommentResponseDTO>());

            IActionResult result = await commentController.GetByPost(99);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<CommentResponseDTO> comments = okResult.Value as List<CommentResponseDTO>;
            Assert.AreEqual(0, comments.Count);
        }

        // Test getting only main comments, not replies
        [Test]
        public async Task GetTopLevel_ValidPostId_ReturnsOnlyTopLevelComments()
        {
            List<CommentResponseDTO> mockComments = new List<CommentResponseDTO>
            {
                new CommentResponseDTO { CommentId = 1, Content = "Top level comment" }
            };
            commentServiceMock.Setup(s => s.GetTopLevel(1)).ReturnsAsync(mockComments);

            IActionResult result = await commentController.GetTopLevel(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<CommentResponseDTO> comments = okResult.Value as List<CommentResponseDTO>;
            Assert.AreEqual(1, comments.Count);
        }

        // Test getting all replies for a specific comment
        [Test]
        public async Task GetReplies_ValidParentId_ReturnsReplies()
        {
            List<CommentResponseDTO> mockReplies = new List<CommentResponseDTO>
            {
                new CommentResponseDTO { CommentId = 5, Content = "A reply" }
            };
            commentServiceMock.Setup(s => s.GetReplies(1)).ReturnsAsync(mockReplies);

            IActionResult result = await commentController.GetReplies(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<CommentResponseDTO> replies = okResult.Value as List<CommentResponseDTO>;
            Assert.AreEqual(1, replies.Count);
        }

        // Test finding a specific comment by id
        [Test]
        public async Task GetById_ValidId_ReturnsOk()
        {
            CommentResponseDTO mockComment = new CommentResponseDTO { CommentId = 3, Content = "Some comment" };
            commentServiceMock.Setup(s => s.GetById(3)).ReturnsAsync(mockComment);

            IActionResult result = await commentController.GetById(3);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            CommentResponseDTO comment = okResult.Value as CommentResponseDTO;
            Assert.AreEqual(3, comment.CommentId);
        }

        // Test finding non-existent comment by id
        [Test]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            commentServiceMock.Setup(s => s.GetById(999)).ThrowsAsync(new Exception("Comment not found."));

            IActionResult result = await commentController.GetById(999);

            NotFoundObjectResult notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        // Test getting the total comment count for a post
        [Test]
        public async Task GetCount_ValidPostId_ReturnsCommentCount()
        {
            commentServiceMock.Setup(s => s.GetCommentCount(1)).ReturnsAsync(7);

            IActionResult result = await commentController.GetCount(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("7"));
        }

        // Test user updating their own comment content
        [Test]
        public async Task Update_ValidDto_ReturnsOkWithUpdated()
        {
            SetUser("1");
            UpdateCommentDTO dto = new UpdateCommentDTO { Content = "Edited content" };
            CommentResponseDTO mockResponse = new CommentResponseDTO { CommentId = 1, Content = "Edited content" };
            commentServiceMock.Setup(s => s.UpdateComment(1, 1, dto)).ReturnsAsync(mockResponse);

            IActionResult result = await commentController.Update(1, dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            CommentResponseDTO comment = okResult.Value as CommentResponseDTO;
            Assert.AreEqual("Edited content", comment.Content);
        }

        // Test user trying to edit someone else's comment
        [Test]
        public async Task Update_NotOwnComment_ReturnsBadRequest()
        {
            SetUser("1");
            UpdateCommentDTO dto = new UpdateCommentDTO { Content = "Attempt" };
            commentServiceMock.Setup(s => s.UpdateComment(99, 1, dto))
                               .ThrowsAsync(new Exception("You are not the author."));

            IActionResult result = await commentController.Update(99, dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test user deleting their own comment
        [Test]
        public async Task Delete_ValidId_ReturnsOk()
        {
            SetUser("1", "READER");
            commentServiceMock.Setup(s => s.SoftDeleteComment(1, 1, "READER")).Returns(Task.CompletedTask);

            IActionResult result = await commentController.Delete(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("deleted"));
        }

        // Test user deleting a comment they don't own
        [Test]
        public async Task Delete_NotOwnComment_ReturnsBadRequest()
        {
            SetUser("1", "READER");
            commentServiceMock.Setup(s => s.SoftDeleteComment(99, 1, "READER"))
                               .ThrowsAsync(new Exception("You are not the author."));

            IActionResult result = await commentController.Delete(99);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test admin/author approving a pending comment
        [Test]
        public async Task Approve_PendingComment_ReturnsOk()
        {
            SetUser("2", "AUTHOR");
            commentServiceMock.Setup(s => s.ApproveComment(1)).Returns(Task.CompletedTask);

            IActionResult result = await commentController.Approve(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("approved"));
        }

        // Test approving a comment that doesn't exist
        [Test]
        public async Task Approve_NotFound_ReturnsBadRequest()
        {
            SetUser("2", "AUTHOR");
            commentServiceMock.Setup(s => s.ApproveComment(999)).ThrowsAsync(new Exception("Comment not found."));

            IActionResult result = await commentController.Approve(999);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test admin/author rejecting a pending comment
        [Test]
        public async Task Reject_PendingComment_ReturnsOk()
        {
            SetUser("2", "AUTHOR");
            commentServiceMock.Setup(s => s.RejectComment(1)).Returns(Task.CompletedTask);

            IActionResult result = await commentController.Reject(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("rejected"));
        }

        // Test rejecting a comment that doesn't exist
        [Test]
        public async Task Reject_NotFound_ReturnsBadRequest()
        {
            SetUser("2", "AUTHOR");
            commentServiceMock.Setup(s => s.RejectComment(999)).ThrowsAsync(new Exception("Comment not found."));

            IActionResult result = await commentController.Reject(999);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test liking a comment
        [Test]
        public async Task Like_ValidComment_ReturnsOk()
        {
            SetUser("1");
            commentServiceMock.Setup(s => s.LikeComment(1)).Returns(Task.CompletedTask);

            IActionResult result = await commentController.Like(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("liked"));
        }

        // Test liking a non-existent comment
        [Test]
        public async Task Like_CommentNotFound_ReturnsBadRequest()
        {
            SetUser("1");
            commentServiceMock.Setup(s => s.LikeComment(999)).ThrowsAsync(new Exception("Comment not found."));

            IActionResult result = await commentController.Like(999);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test removing a like from a comment
        [Test]
        public async Task Unlike_ValidComment_ReturnsOk()
        {
            SetUser("1");
            commentServiceMock.Setup(s => s.UnlikeComment(1)).Returns(Task.CompletedTask);

            IActionResult result = await commentController.Unlike(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("unliked"));
        }

        // Test admin filtering comments by status (e.g. pending)
        [Test]
        public async Task GetByStatus_ValidStatus_ReturnsFilteredComments()
        {
            SetUser("99", "ADMIN");
            List<CommentResponseDTO> mockComments = new List<CommentResponseDTO>
            {
                new CommentResponseDTO { CommentId = 1, Content = "Pending comment" }
            };
            commentServiceMock.Setup(s => s.GetByStatus("PENDING")).ReturnsAsync(mockComments);

            IActionResult result = await commentController.GetByStatus("PENDING");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<CommentResponseDTO> comments = okResult.Value as List<CommentResponseDTO>;
            Assert.AreEqual(1, comments.Count);
        }

        // Test filtering by an invalid status string
        [Test]
        public async Task GetByStatus_InvalidStatus_ReturnsBadRequest()
        {
            SetUser("99", "ADMIN");
            commentServiceMock.Setup(s => s.GetByStatus("GARBAGE")).ThrowsAsync(new Exception("Invalid status."));

            IActionResult result = await commentController.GetByStatus("GARBAGE");

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test admin enabling comment moderation mode
        [Test]
        public void SetModeration_Enable_ReturnsOk()
        {
            SetUser("99", "ADMIN");

            IActionResult result = commentController.SetModeration(true);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("enabled"));
        }

        // Test admin disabling comment moderation mode
        [Test]
        public void SetModeration_Disable_ReturnsOk()
        {
            SetUser("99", "ADMIN");

            IActionResult result = commentController.SetModeration(false);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("disabled"));
        }

        // Test checking if moderation is currently enabled
        [Test]
        public void GetModeration_WhenEnabled_ReturnsTrue()
        {
            SetUser("99", "ADMIN");
            commentServiceMock.Setup(s => s.GetModerationMode()).Returns(true);

            IActionResult result = commentController.GetModeration();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("True"));
        }

        // Test checking if moderation is currently disabled
        [Test]
        public void GetModeration_WhenDisabled_ReturnsFalse()
        {
            SetUser("99", "ADMIN");
            commentServiceMock.Setup(s => s.GetModerationMode()).Returns(false);

            IActionResult result = commentController.GetModeration();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("False"));
        }
    }
}
