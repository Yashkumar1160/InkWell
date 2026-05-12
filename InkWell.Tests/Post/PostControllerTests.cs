using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using InkWell.Post.Controllers;
using InkWell.Post.Service.Interfaces;
using InkWell.Post.DTOs;

namespace InkWell.Post.Tests
{
    [TestFixture]
    public class PostControllerTests
    {
        private Mock<IPostService> postServiceMock;
        private PostController postController;

        [SetUp]
        public void Setup()
        {
            postServiceMock = new Mock<IPostService>();
            postController = new PostController(postServiceMock.Object);
        }

        // Helper: configure controller context for a given user
        private void SetUser(string userId, string name = "Test Author", string role = "AUTHOR")
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role),
                new Claim("FullName", name)
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "TestAuthType");
            postController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        // Test getting all published posts for the public feed
        [Test]
        public async Task GetPublished_ReturnsOkWithPostList()
        {
            List<PostResponseDTO> mockPosts = new List<PostResponseDTO>
            {
                new PostResponseDTO { PostId = 1, Title = "First Post", Status = "PUBLISHED" },
                new PostResponseDTO { PostId = 2, Title = "Second Post", Status = "PUBLISHED" }
            };
            postServiceMock.Setup(s => s.GetPublished()).ReturnsAsync(mockPosts);

            IActionResult result = await postController.GetPublished();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            List<PostResponseDTO> posts = okResult.Value as List<PostResponseDTO>;
            Assert.AreEqual(2, posts.Count);
        }

        // Test getting published posts when none exist returns empty list
        [Test]
        public async Task GetPublished_NoPostsExist_ReturnsEmptyList()
        {
            postServiceMock.Setup(s => s.GetPublished()).ReturnsAsync(new List<PostResponseDTO>());

            IActionResult result = await postController.GetPublished();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<PostResponseDTO> posts = okResult.Value as List<PostResponseDTO>;
            Assert.AreEqual(0, posts.Count);
        }

        // Test viewing a single post by its unique slug
        [Test]
        public async Task GetBySlug_ValidSlug_ReturnsOk()
        {
            SetUser("1");
            PostResponseDTO mockPost = new PostResponseDTO { PostId = 1, Title = "First Post", Slug = "first-post" };
            postServiceMock.Setup(s => s.GetBySlug("first-post", It.IsAny<int>())).ReturnsAsync(mockPost);
            postServiceMock.Setup(s => s.IncrementViews(1)).Returns(Task.CompletedTask);

            IActionResult result = await postController.GetBySlug("first-post");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            PostResponseDTO post = okResult.Value as PostResponseDTO;
            Assert.AreEqual("first-post", post.Slug);
        }

        // Test viewing a post with an invalid slug returns error
        [Test]
        public async Task GetBySlug_InvalidSlug_ReturnsNotFound()
        {
            SetUser("1");
            postServiceMock.Setup(s => s.GetBySlug("no-such-post", It.IsAny<int>()))
                            .ThrowsAsync(new Exception("Post not found."));

            IActionResult result = await postController.GetBySlug("no-such-post");

            NotFoundObjectResult notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        // Test finding a post by its numeric id
        [Test]
        public async Task GetById_ValidId_ReturnsOk()
        {
            SetUser("1");
            PostResponseDTO mockPost = new PostResponseDTO { PostId = 5, Title = "Post Five" };
            postServiceMock.Setup(s => s.GetById(5, It.IsAny<int>())).ReturnsAsync(mockPost);

            IActionResult result = await postController.GetById(5);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            PostResponseDTO post = okResult.Value as PostResponseDTO;
            Assert.AreEqual(5, post.PostId);
        }

        // Test finding non-existent post by id
        [Test]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            SetUser("1");
            postServiceMock.Setup(s => s.GetById(999, It.IsAny<int>()))
                            .ThrowsAsync(new Exception("Post not found."));

            IActionResult result = await postController.GetById(999);

            NotFoundObjectResult notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        // Test author creating a new draft post
        [Test]
        public async Task Create_ValidPost_ReturnsDraftPost()
        {
            SetUser("1", "Test Author", "AUTHOR");
            CreatePostDTO dto = new CreatePostDTO { Title = "New Post", Content = "Some content", Excerpt = "Short" };
            PostResponseDTO mockResponse = new PostResponseDTO { PostId = 1, Title = "New Post", Status = "DRAFT" };
            postServiceMock.Setup(s => s.CreatePost(1, "Test Author", dto)).ReturnsAsync(mockResponse);

            IActionResult result = await postController.Create(dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            PostResponseDTO post = okResult.Value as PostResponseDTO;
            Assert.AreEqual("DRAFT", post.Status);
        }

        // Test post creation fails with missing required data
        [Test]
        public async Task Create_ServiceThrows_ReturnsBadRequest()
        {
            SetUser("1", "Test Author", "AUTHOR");
            CreatePostDTO dto = new CreatePostDTO { Title = "", Content = "" };
            postServiceMock.Setup(s => s.CreatePost(1, "Test Author", dto))
                            .ThrowsAsync(new Exception("Title is required."));

            IActionResult result = await postController.Create(dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test author getting a list of their own posts
        [Test]
        public async Task GetMyPosts_ReturnsOkWithAuthorPosts()
        {
            SetUser("1", "Test Author", "AUTHOR");
            List<PostResponseDTO> mockPosts = new List<PostResponseDTO>
            {
                new PostResponseDTO { PostId = 1, Title = "My Draft", Status = "DRAFT" }
            };
            postServiceMock.Setup(s => s.GetByAuthor(1)).ReturnsAsync(mockPosts);

            IActionResult result = await postController.GetMyPosts();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<PostResponseDTO> posts = okResult.Value as List<PostResponseDTO>;
            Assert.AreEqual(1, posts.Count);
        }

        // Test getting all public posts by a specific author
        [Test]
        public async Task GetByAuthor_ValidAuthorId_ReturnsOkWithPosts()
        {
            List<PostResponseDTO> mockPosts = new List<PostResponseDTO>
            {
                new PostResponseDTO { PostId = 1, Title = "Author Post", Status = "PUBLISHED" }
            };
            postServiceMock.Setup(s => s.GetByAuthor(3)).ReturnsAsync(mockPosts);

            IActionResult result = await postController.GetByAuthor(3);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        // Test searching for published posts by keyword
        [Test]
        public async Task Search_ValidKeyword_ReturnsMatchingPosts()
        {
            List<PostResponseDTO> mockResults = new List<PostResponseDTO>
            {
                new PostResponseDTO { PostId = 1, Title = "Angular Guide" }
            };
            postServiceMock.Setup(s => s.Search("angular")).ReturnsAsync(mockResults);

            IActionResult result = await postController.Search("angular");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<PostResponseDTO> posts = okResult.Value as List<PostResponseDTO>;
            Assert.AreEqual(1, posts.Count);
        }

        // Test searching with a keyword that has no matches
        [Test]
        public async Task Search_NoMatchingPosts_ReturnsEmptyList()
        {
            postServiceMock.Setup(s => s.Search("xyz123")).ReturnsAsync(new List<PostResponseDTO>());

            IActionResult result = await postController.Search("xyz123");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<PostResponseDTO> posts = okResult.Value as List<PostResponseDTO>;
            Assert.AreEqual(0, posts.Count);
        }

        // Test admin viewing all posts in the system
        [Test]
        public async Task GetAll_AdminUser_ReturnsAllPosts()
        {
            SetUser("99", "Admin", "ADMIN");
            List<PostResponseDTO> mockPosts = new List<PostResponseDTO>
            {
                new PostResponseDTO { PostId = 1, Status = "DRAFT" },
                new PostResponseDTO { PostId = 2, Status = "PUBLISHED" }
            };
            postServiceMock.Setup(s => s.GetAll()).ReturnsAsync(mockPosts);

            IActionResult result = await postController.GetAll();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<PostResponseDTO> posts = okResult.Value as List<PostResponseDTO>;
            Assert.AreEqual(2, posts.Count);
        }

        // Test author updating their own post content
        [Test]
        public async Task Update_ValidPost_ReturnsOkWithUpdated()
        {
            SetUser("1", "Test Author", "AUTHOR");
            UpdatePostDTO dto = new UpdatePostDTO { Title = "Updated Title" };
            PostResponseDTO mockResponse = new PostResponseDTO { PostId = 1, Title = "Updated Title" };
            postServiceMock.Setup(s => s.UpdatePost(1, 1, dto)).ReturnsAsync(mockResponse);

            IActionResult result = await postController.Update(1, dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            PostResponseDTO post = okResult.Value as PostResponseDTO;
            Assert.AreEqual("Updated Title", post.Title);
        }

        // Test user trying to update someone else's post returns error
        [Test]
        public async Task Update_NotOwnPost_ReturnsBadRequest()
        {
            SetUser("1", "Test Author", "AUTHOR");
            UpdatePostDTO dto = new UpdatePostDTO { Title = "Attempt" };
            postServiceMock.Setup(s => s.UpdatePost(99, 1, dto))
                            .ThrowsAsync(new Exception("You are not the author."));

            IActionResult result = await postController.Update(99, dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test author changing post status to published
        [Test]
        public async Task Publish_ValidPost_ReturnsPublishedPost()
        {
            SetUser("1", "Test Author", "AUTHOR");
            PostResponseDTO mockResponse = new PostResponseDTO { PostId = 1, Title = "Post", Status = "PUBLISHED" };
            postServiceMock.Setup(s => s.PublishPost(1, 1)).ReturnsAsync(mockResponse);

            IActionResult result = await postController.Publish(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            PostResponseDTO post = okResult.Value as PostResponseDTO;
            Assert.AreEqual("PUBLISHED", post.Status);
        }

        // Test publishing a post that doesn't belong to the user
        [Test]
        public async Task Publish_NotOwnPost_ReturnsBadRequest()
        {
            SetUser("1", "Test Author", "AUTHOR");
            postServiceMock.Setup(s => s.PublishPost(99, 1))
                            .ThrowsAsync(new Exception("Post not found."));

            IActionResult result = await postController.Publish(99);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test author taking a published post back to draft
        [Test]
        public async Task Unpublish_ValidPost_ReturnsUnpublishedPost()
        {
            SetUser("1", "Test Author", "AUTHOR");
            PostResponseDTO mockResponse = new PostResponseDTO { PostId = 1, Status = "UNPUBLISHED" };
            postServiceMock.Setup(s => s.UnpublishPost(1, 1)).ReturnsAsync(mockResponse);

            IActionResult result = await postController.Unpublish(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            PostResponseDTO post = okResult.Value as PostResponseDTO;
            Assert.AreEqual("UNPUBLISHED", post.Status);
        }

        // Test author deleting their own post
        [Test]
        public async Task Delete_ValidPost_ReturnsOk()
        {
            SetUser("1", "Test Author", "AUTHOR");
            postServiceMock.Setup(s => s.DeletePost(1, 1, "AUTHOR")).Returns(Task.CompletedTask);

            IActionResult result = await postController.Delete(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("deleted"));
        }

        // Test trying to delete a post without permission
        [Test]
        public async Task Delete_NotOwnPost_ReturnsBadRequest()
        {
            SetUser("1", "Test Author", "AUTHOR");
            postServiceMock.Setup(s => s.DeletePost(99, 1, "AUTHOR"))
                            .ThrowsAsync(new Exception("You are not the author."));

            IActionResult result = await postController.Delete(99);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test liking a post
        [Test]
        public async Task Like_ValidPost_ReturnsOk()
        {
            SetUser("1", "Test Author");
            postServiceMock.Setup(s => s.LikePost(1, 1, It.IsAny<string>())).Returns(Task.CompletedTask);

            IActionResult result = await postController.Like(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("liked"));
        }

        // Test liking the same post twice returns error
        [Test]
        public async Task Like_AlreadyLiked_ReturnsBadRequest()
        {
            SetUser("1", "Test Author");
            postServiceMock.Setup(s => s.LikePost(1, 1, It.IsAny<string>()))
                            .ThrowsAsync(new Exception("Already liked."));

            IActionResult result = await postController.Like(1);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test removing a like from a post
        [Test]
        public async Task Unlike_LikedPost_ReturnsOk()
        {
            SetUser("1", "Test Author");
            postServiceMock.Setup(s => s.UnlikePost(1, 1)).Returns(Task.CompletedTask);

            IActionResult result = await postController.Unlike(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("unliked"));
        }

        // Test archiving a post to hide it from public
        [Test]
        public async Task Archive_ValidPost_ReturnsArchivedPost()
        {
            SetUser("1", "Test Author", "AUTHOR");
            PostResponseDTO mockResponse = new PostResponseDTO { PostId = 1, Status = "ARCHIVED" };
            postServiceMock.Setup(s => s.ArchivePost(1, 1)).ReturnsAsync(mockResponse);

            IActionResult result = await postController.Archive(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            PostResponseDTO post = okResult.Value as PostResponseDTO;
            Assert.AreEqual("ARCHIVED", post.Status);
        }

        // Test restoring an archived post back to draft
        [Test]
        public async Task Unarchive_ArchivedPost_ReturnsDraftPost()
        {
            SetUser("1", "Test Author", "AUTHOR");
            PostResponseDTO mockResponse = new PostResponseDTO { PostId = 1, Status = "DRAFT" };
            postServiceMock.Setup(s => s.UnarchivePost(1, 1)).ReturnsAsync(mockResponse);

            IActionResult result = await postController.Unarchive(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            PostResponseDTO post = okResult.Value as PostResponseDTO;
            Assert.AreEqual("DRAFT", post.Status);
        }

        // Test admin filtering posts by their current status
        [Test]
        public async Task GetByStatus_ValidStatus_ReturnsOkWithFilteredPosts()
        {
            SetUser("99", "Admin", "ADMIN");
            List<PostResponseDTO> mockPosts = new List<PostResponseDTO>
            {
                new PostResponseDTO { PostId = 1, Status = "DRAFT" }
            };
            postServiceMock.Setup(s => s.GetByStatus("DRAFT")).ReturnsAsync(mockPosts);

            IActionResult result = await postController.GetByStatus("DRAFT");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<PostResponseDTO> posts = okResult.Value as List<PostResponseDTO>;
            Assert.AreEqual(1, posts.Count);
        }

        // Test filtering posts with an invalid status string
        [Test]
        public async Task GetByStatus_InvalidStatus_ReturnsBadRequest()
        {
            SetUser("99", "Admin", "ADMIN");
            postServiceMock.Setup(s => s.GetByStatus("INVALID"))
                            .ThrowsAsync(new Exception("Invalid status value."));

            IActionResult result = await postController.GetByStatus("INVALID");

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test getting the total number of posts for an author
        [Test]
        public async Task GetCount_ValidAuthorId_ReturnsCount()
        {
            SetUser("1");
            postServiceMock.Setup(s => s.GetPostCount(1)).ReturnsAsync(5);

            IActionResult result = await postController.GetCount(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("5"));
        }

        // Test admin pinning a post to the top of the feed
        [Test]
        public async Task Feature_ValidPost_ReturnsFeaturedPost()
        {
            SetUser("99", "Admin", "ADMIN");
            PostResponseDTO mockResponse = new PostResponseDTO { PostId = 1, Title = "Big Post" };
            postServiceMock.Setup(s => s.FeaturePost(1)).ReturnsAsync(mockResponse);

            IActionResult result = await postController.Feature(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        // Test pinning a non-existent post returns error
        [Test]
        public async Task Feature_PostNotFound_ReturnsBadRequest()
        {
            SetUser("99", "Admin", "ADMIN");
            postServiceMock.Setup(s => s.FeaturePost(999)).ThrowsAsync(new Exception("Post not found."));

            IActionResult result = await postController.Feature(999);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test admin removing the pinned status from a post
        [Test]
        public async Task Unfeature_FeaturedPost_ReturnsOk()
        {
            SetUser("99", "Admin", "ADMIN");
            PostResponseDTO mockResponse = new PostResponseDTO { PostId = 1, Title = "Big Post" };
            postServiceMock.Setup(s => s.UnfeaturePost(1)).ReturnsAsync(mockResponse);

            IActionResult result = await postController.Unfeature(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        // Test unpinning a post that doesn't exist
        [Test]
        public async Task Unfeature_PostNotFound_ReturnsBadRequest()
        {
            SetUser("99", "Admin", "ADMIN");
            postServiceMock.Setup(s => s.UnfeaturePost(999)).ThrowsAsync(new Exception("Post not found."));

            IActionResult result = await postController.Unfeature(999);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }
    }
}
