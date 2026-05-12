using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using InkWell.Media.Controllers;
using InkWell.Media.Services.Interfaces;
using InkWell.Media.DTOs;

namespace InkWell.Media.Tests
{
    [TestFixture]
    public class MediaControllerTests
    {
        private Mock<IMediaService> mediaServiceMock;
        private MediaController mediaController;

        [SetUp]
        public void Setup()
        {
            mediaServiceMock = new Mock<IMediaService>();
            mediaController = new MediaController(mediaServiceMock.Object);
        }

        // Helper: set authenticated user
        private void SetUser(string userId, string role = "AUTHOR")
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "TestAuthType");
            mediaController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        // Test uploading a new media file (image/video)
        [Test]
        public async Task Upload_ValidFile_ReturnsOkWithMediaInfo()
        {
            SetUser("1", "AUTHOR");
            Mock<IFormFile> fileMock = new Mock<IFormFile>();
            MediaResponseDTO mockResponse = new MediaResponseDTO { MediaId = 1, Url = "http://test.com/img.jpg" };
            mediaServiceMock.Setup(s => s.UploadMedia(1, fileMock.Object)).ReturnsAsync(mockResponse);

            IActionResult result = await mediaController.Upload(fileMock.Object);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            MediaResponseDTO response = okResult.Value as MediaResponseDTO;
            Assert.AreEqual("http://test.com/img.jpg", response.Url);
        }

        // Test uploading fails with invalid file type
        [Test]
        public async Task Upload_ServiceThrows_ReturnsBadRequest()
        {
            SetUser("1", "AUTHOR");
            Mock<IFormFile> fileMock = new Mock<IFormFile>();
            mediaServiceMock.Setup(s => s.UploadMedia(1, fileMock.Object))
                             .ThrowsAsync(new Exception("Invalid file type."));

            IActionResult result = await mediaController.Upload(fileMock.Object);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test getting details of a specific media file
        [Test]
        public async Task GetById_ValidId_ReturnsOk()
        {
            SetUser("1");
            MediaResponseDTO mockResponse = new MediaResponseDTO { MediaId = 5, FileName = "test.jpg" };
            mediaServiceMock.Setup(s => s.GetById(5)).ReturnsAsync(mockResponse);

            IActionResult result = await mediaController.GetById(5);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            MediaResponseDTO response = okResult.Value as MediaResponseDTO;
            Assert.AreEqual(5, response.MediaId);
        }

        // Test getting non-existent media file returns not found
        [Test]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            SetUser("1");
            mediaServiceMock.Setup(s => s.GetById(999)).ThrowsAsync(new Exception("File not found."));

            IActionResult result = await mediaController.GetById(999);

            NotFoundObjectResult notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        // Test user viewing a list of their uploaded files
        [Test]
        public async Task GetMyFiles_ReturnsOkWithList()
        {
            SetUser("1", "AUTHOR");
            List<MediaResponseDTO> mockFiles = new List<MediaResponseDTO> { new MediaResponseDTO { MediaId = 1 } };
            mediaServiceMock.Setup(s => s.GetByUploader(1)).ReturnsAsync(mockFiles);

            IActionResult result = await mediaController.GetMyFiles();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<MediaResponseDTO> files = okResult.Value as List<MediaResponseDTO>;
            Assert.AreEqual(1, files.Count);
        }

        // Test getting all media files attached to a specific post
        [Test]
        public async Task GetByPost_ValidPostId_ReturnsOk()
        {
            List<MediaResponseDTO> mockFiles = new List<MediaResponseDTO> { new MediaResponseDTO { MediaId = 2 } };
            mediaServiceMock.Setup(s => s.GetByPost(10)).ReturnsAsync(mockFiles);

            IActionResult result = await mediaController.GetByPost(10);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        // Test admin viewing all uploaded files in the system
        [Test]
        public async Task GetAll_AdminUser_ReturnsAllFiles()
        {
            SetUser("99", "ADMIN");
            List<MediaResponseDTO> mockFiles = new List<MediaResponseDTO> { new MediaResponseDTO { MediaId = 1 }, new MediaResponseDTO { MediaId = 2 } };
            mediaServiceMock.Setup(s => s.GetAll()).ReturnsAsync(mockFiles);

            IActionResult result = await mediaController.GetAll();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<MediaResponseDTO> files = okResult.Value as List<MediaResponseDTO>;
            Assert.AreEqual(2, files.Count);
        }

        // Test getting total number of files uploaded by a user
        [Test]
        public async Task GetCount_ReturnsFileCount()
        {
            SetUser("1");
            mediaServiceMock.Setup(s => s.GetMediaCount(1)).ReturnsAsync(15);

            IActionResult result = await mediaController.GetCount();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("15"));
        }

        // Test user deleting their own uploaded file
        [Test]
        public async Task Delete_ValidId_ReturnsOk()
        {
            SetUser("1", "AUTHOR");
            mediaServiceMock.Setup(s => s.SoftDelete(1, 1, "AUTHOR")).Returns(Task.CompletedTask);

            IActionResult result = await mediaController.Delete(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("File deleted"));
        }

        // Test updating the accessibility alt text for an image
        [Test]
        public async Task UpdateAltText_ValidDto_ReturnsOk()
        {
            SetUser("1", "AUTHOR");
            UpdateAltTextDTO dto = new UpdateAltTextDTO { AltText = "Updated alt" };
            MediaResponseDTO mockResponse = new MediaResponseDTO { MediaId = 1, AltText = "Updated alt" };
            mediaServiceMock.Setup(s => s.UpdateAltText(1, 1, dto, "AUTHOR")).ReturnsAsync(mockResponse);

            IActionResult result = await mediaController.UpdateAltText(1, dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            MediaResponseDTO response = okResult.Value as MediaResponseDTO;
            Assert.AreEqual("Updated alt", response.AltText);
        }

        // Test linking an existing media file to a post
        [Test]
        public async Task LinkToPost_ValidDto_ReturnsOk()
        {
            SetUser("1", "AUTHOR");
            LinkPostDTO dto = new LinkPostDTO { PostId = 5 };
            MediaResponseDTO mockResponse = new MediaResponseDTO { MediaId = 1, LinkedPostId = 5 };
            mediaServiceMock.Setup(s => s.LinkToPost(1, 5)).ReturnsAsync(mockResponse);

            IActionResult result = await mediaController.LinkToPost(1, dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            MediaResponseDTO response = okResult.Value as MediaResponseDTO;
            Assert.AreEqual(5, response.LinkedPostId);
        }

        // Test removing the link between a media file and a post
        [Test]
        public async Task UnlinkFromPost_ValidId_ReturnsOk()
        {
            SetUser("1", "AUTHOR");
            MediaResponseDTO mockResponse = new MediaResponseDTO { MediaId = 1, LinkedPostId = null };
            mediaServiceMock.Setup(s => s.UnlinkFromPost(1)).ReturnsAsync(mockResponse);

            IActionResult result = await mediaController.UnlinkFromPost(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        // Test admin permanently purging deleted files from storage
        [Test]
        public async Task Cleanup_AdminUser_ReturnsOk()
        {
            SetUser("99", "ADMIN");
            mediaServiceMock.Setup(s => s.CleanupDeleted()).Returns(Task.CompletedTask);

            IActionResult result = await mediaController.Cleanup();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        // Test admin filtering files by type (e.g. image/png)
        [Test]
        public async Task GetByMimeType_ValidType_ReturnsOk()
        {
            SetUser("99", "ADMIN");
            List<MediaResponseDTO> mockFiles = new List<MediaResponseDTO> { new MediaResponseDTO { MimeType = "image/jpeg" } };
            mediaServiceMock.Setup(s => s.GetByMimeType("image/jpeg")).ReturnsAsync(mockFiles);

            IActionResult result = await mediaController.GetByMimeType("image/jpeg");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        // Test admin viewing files that are currently in the trash
        [Test]
        public async Task GetDeleted_AdminUser_ReturnsOk()
        {
            SetUser("99", "ADMIN");
            List<MediaResponseDTO> mockFiles = new List<MediaResponseDTO> { new MediaResponseDTO { IsDeleted = true } };
            mediaServiceMock.Setup(s => s.GetDeletedFiles()).ReturnsAsync(mockFiles);

            IActionResult result = await mediaController.GetDeleted();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }
    }
}
