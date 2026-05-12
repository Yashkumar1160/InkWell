using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using InkWell.Category.Controllers;
using InkWell.Category.Service.Interfaces;
using InkWell.Category.DTOs;

namespace InkWell.Category.Tests
{
    [TestFixture]
    public class CategoryControllerTests
    {
        private Mock<ICategoryService> categoryServiceMock;
        private CategoryController categoryController;

        [SetUp]
        public void Setup()
        {
            categoryServiceMock = new Mock<ICategoryService>();
            categoryController = new CategoryController(categoryServiceMock.Object);
        }

        // Helper: set admin user
        private void SetAdmin()
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "ADMIN")
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "TestAuthType");
            categoryController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        // Helper: set author user
        private void SetAuthor()
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "2"),
                new Claim(ClaimTypes.Role, "AUTHOR")
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "TestAuthType");
            categoryController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        // Helper: set authenticated user on controller context
        private void SetUser(string userId, string role = "READER")
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Role, role)
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "TestAuthType");
            categoryController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        // Test admin creating a new category
        [Test]
        public async Task CreateCategory_ValidDto_ReturnsOk()
        {
            SetAdmin();
            CreateCategoryDTO dto = new CreateCategoryDTO { Name = "Technology" };
            CategoryResponseDTO mockResponse = new CategoryResponseDTO { CategoryId = 1, Name = "Technology", Slug = "technology" };
            categoryServiceMock.Setup(s => s.CreateCategory(dto)).ReturnsAsync(mockResponse);

            IActionResult result = await categoryController.CreateCategory(dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            CategoryResponseDTO response = okResult.Value as CategoryResponseDTO;
            Assert.AreEqual("Technology", response.Name);
        }

        // Test creating category with duplicate slug returns error
        [Test]
        public async Task CreateCategory_DuplicateSlug_ReturnsBadRequest()
        {
            SetAdmin();
            CreateCategoryDTO dto = new CreateCategoryDTO { Name = "Technology" };
            categoryServiceMock.Setup(s => s.CreateCategory(dto)).ThrowsAsync(new Exception("Slug already exists."));

            IActionResult result = await categoryController.CreateCategory(dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test getting all categories list
        [Test]
        public async Task GetAllCategories_ReturnsOkWithList()
        {
            List<CategoryResponseDTO> mockCategories = new List<CategoryResponseDTO>
            {
                new CategoryResponseDTO { CategoryId = 1, Name = "Tech", Slug = "tech" },
                new CategoryResponseDTO { CategoryId = 2, Name = "Science", Slug = "science" }
            };
            categoryServiceMock.Setup(s => s.GetAllCategories()).ReturnsAsync(mockCategories);

            IActionResult result = await categoryController.GetAllCategories();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<CategoryResponseDTO> categories = okResult.Value as List<CategoryResponseDTO>;
            Assert.AreEqual(2, categories.Count);
        }

        // Test finding category by its id
        [Test]
        public async Task GetById_ValidId_ReturnsOk()
        {
            CategoryResponseDTO mockCategory = new CategoryResponseDTO { CategoryId = 1, Name = "Tech", Slug = "tech" };
            categoryServiceMock.Setup(s => s.GetCategoryById(1)).ReturnsAsync(mockCategory);

            IActionResult result = await categoryController.GetById(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            CategoryResponseDTO response = okResult.Value as CategoryResponseDTO;
            Assert.AreEqual(1, response.CategoryId);
        }

        // Test finding non-existent category by id
        [Test]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            categoryServiceMock.Setup(s => s.GetCategoryById(999)).ThrowsAsync(new Exception("Category not found."));

            IActionResult result = await categoryController.GetById(999);

            NotFoundObjectResult notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        // Test finding category by its slug
        [Test]
        public async Task GetBySlug_ValidSlug_ReturnsOk()
        {
            CategoryResponseDTO mockCategory = new CategoryResponseDTO { CategoryId = 1, Name = "Tech", Slug = "tech" };
            categoryServiceMock.Setup(s => s.GetCategoryBySlug("tech")).ReturnsAsync(mockCategory);

            IActionResult result = await categoryController.GetBySlug("tech");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            CategoryResponseDTO response = okResult.Value as CategoryResponseDTO;
            Assert.AreEqual("tech", response.Slug);
        }

        // Test finding category by wrong slug
        [Test]
        public async Task GetBySlug_InvalidSlug_ReturnsNotFound()
        {
            categoryServiceMock.Setup(s => s.GetCategoryBySlug("no-such")).ThrowsAsync(new Exception("Category not found."));

            IActionResult result = await categoryController.GetBySlug("no-such");

            NotFoundObjectResult notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        // Test getting subcategories for a parent category
        [Test]
        public async Task GetChildren_ValidParentId_ReturnsOkWithChildren()
        {
            List<CategoryResponseDTO> mockChildren = new List<CategoryResponseDTO>
            {
                new CategoryResponseDTO { CategoryId = 3, Name = "AI", Slug = "ai" }
            };
            categoryServiceMock.Setup(s => s.GetChildCategories(1)).ReturnsAsync(mockChildren);

            IActionResult result = await categoryController.GetChildren(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<CategoryResponseDTO> children = okResult.Value as List<CategoryResponseDTO>;
            Assert.AreEqual(1, children.Count);
        }

        // Test admin updating category details
        [Test]
        public async Task UpdateCategory_ValidDto_ReturnsOkWithUpdated()
        {
            SetAdmin();
            UpdateCategoryDTO dto = new UpdateCategoryDTO { Name = "Updated Tech" };
            CategoryResponseDTO mockResponse = new CategoryResponseDTO { CategoryId = 1, Name = "Updated Tech" };
            categoryServiceMock.Setup(s => s.UpdateCategory(1, dto)).ReturnsAsync(mockResponse);

            IActionResult result = await categoryController.UpdateCategory(1, dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            CategoryResponseDTO response = okResult.Value as CategoryResponseDTO;
            Assert.AreEqual("Updated Tech", response.Name);
        }

        // Test admin updating non-existent category
        [Test]
        public async Task UpdateCategory_NotFound_ReturnsBadRequest()
        {
            SetAdmin();
            UpdateCategoryDTO dto = new UpdateCategoryDTO { Name = "Ghost" };
            categoryServiceMock.Setup(s => s.UpdateCategory(999, dto)).ThrowsAsync(new Exception("Category not found."));

            IActionResult result = await categoryController.UpdateCategory(999, dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test admin deleting a category
        [Test]
        public async Task DeleteCategory_ValidId_ReturnsOk()
        {
            SetAdmin();
            categoryServiceMock.Setup(s => s.DeleteCategory(1)).Returns(Task.CompletedTask);

            IActionResult result = await categoryController.DeleteCategory(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("deleted"));
        }

        // Test admin deleting non-existent category
        [Test]
        public async Task DeleteCategory_NotFound_ReturnsBadRequest()
        {
            SetAdmin();
            categoryServiceMock.Setup(s => s.DeleteCategory(999)).ThrowsAsync(new Exception("Category not found."));

            IActionResult result = await categoryController.DeleteCategory(999);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test admin creating a new tag
        [Test]
        public async Task CreateTag_ValidDto_ReturnsOk()
        {
            SetAdmin();
            CreateTagDTO dto = new CreateTagDTO { Name = "dotnet" };
            TagResponseDTO mockTag = new TagResponseDTO { TagId = 1, Name = "dotnet", Slug = "dotnet" };
            categoryServiceMock.Setup(s => s.CreateTag(dto)).ReturnsAsync(mockTag);

            IActionResult result = await categoryController.CreateTag(dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            TagResponseDTO tag = okResult.Value as TagResponseDTO;
            Assert.AreEqual("dotnet", tag.Name);
        }

        // Test creating tag with duplicate slug
        [Test]
        public async Task CreateTag_DuplicateSlug_ReturnsBadRequest()
        {
            SetAdmin();
            CreateTagDTO dto = new CreateTagDTO { Name = "dotnet" };
            categoryServiceMock.Setup(s => s.CreateTag(dto)).ThrowsAsync(new Exception("Tag slug already exists."));

            IActionResult result = await categoryController.CreateTag(dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test getting all tags list
        [Test]
        public async Task GetAllTags_ReturnsOkWithTags()
        {
            List<TagResponseDTO> mockTags = new List<TagResponseDTO>
            {
                new TagResponseDTO { TagId = 1, Name = "dotnet" },
                new TagResponseDTO { TagId = 2, Name = "angular" }
            };
            categoryServiceMock.Setup(s => s.GetAllTags()).ReturnsAsync(mockTags);

            IActionResult result = await categoryController.GetAllTags();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<TagResponseDTO> tags = okResult.Value as List<TagResponseDTO>;
            Assert.AreEqual(2, tags.Count);
        }

        // Test getting trending tags
        [Test]
        public async Task GetTrendingTags_ReturnsOkWithTopTags()
        {
            List<TagResponseDTO> mockTags = new List<TagResponseDTO>
            {
                new TagResponseDTO { TagId = 1, Name = "dotnet" }
            };
            categoryServiceMock.Setup(s => s.GetTrendingTags()).ReturnsAsync(mockTags);

            IActionResult result = await categoryController.GetTrendingTags();

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        // Test finding tag by id
        [Test]
        public async Task GetTagById_ValidId_ReturnsOk()
        {
            TagResponseDTO mockTag = new TagResponseDTO { TagId = 1, Name = "dotnet", Slug = "dotnet" };
            categoryServiceMock.Setup(s => s.GetTagById(1)).ReturnsAsync(mockTag);

            IActionResult result = await categoryController.GetTagById(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        // Test finding non-existent tag by id
        [Test]
        public async Task GetTagById_InvalidId_ReturnsNotFound()
        {
            categoryServiceMock.Setup(s => s.GetTagById(999)).ThrowsAsync(new Exception("Tag not found."));

            IActionResult result = await categoryController.GetTagById(999);

            NotFoundObjectResult notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        // Test finding tag by slug
        [Test]
        public async Task GetTagBySlug_ValidSlug_ReturnsOk()
        {
            TagResponseDTO mockTag = new TagResponseDTO { TagId = 1, Name = "dotnet", Slug = "dotnet" };
            categoryServiceMock.Setup(s => s.GetTagBySlug("dotnet")).ReturnsAsync(mockTag);

            IActionResult result = await categoryController.GetTagBySlug("dotnet");

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            TagResponseDTO tag = okResult.Value as TagResponseDTO;
            Assert.AreEqual("dotnet", tag.Slug);
        }

        // Test finding tag by wrong slug
        [Test]
        public async Task GetTagBySlug_InvalidSlug_ReturnsNotFound()
        {
            categoryServiceMock.Setup(s => s.GetTagBySlug("ghost")).ThrowsAsync(new Exception("Tag not found."));

            IActionResult result = await categoryController.GetTagBySlug("ghost");

            NotFoundObjectResult notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        // Test admin deleting a tag
        [Test]
        public async Task DeleteTag_ValidId_ReturnsOk()
        {
            SetAdmin();
            categoryServiceMock.Setup(s => s.DeleteTag(1)).Returns(Task.CompletedTask);

            IActionResult result = await categoryController.DeleteTag(1);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("deleted"));
        }

        // Test admin deleting non-existent tag
        [Test]
        public async Task DeleteTag_NotFound_ReturnsBadRequest()
        {
            SetAdmin();
            categoryServiceMock.Setup(s => s.DeleteTag(999)).ThrowsAsync(new Exception("Tag not found."));

            IActionResult result = await categoryController.DeleteTag(999);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test assigning a tag to a post
        [Test]
        public async Task AssignTag_ValidDto_ReturnsOk()
        {
            SetAuthor();
            AssignTagDTO dto = new AssignTagDTO { PostId = 1, TagId = 2 };
            categoryServiceMock.Setup(s => s.AddTagToPost(1, 2)).Returns(Task.CompletedTask);

            IActionResult result = await categoryController.AssignTag(dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("assigned"));
        }

        // Test assigning same tag twice returns error
        [Test]
        public async Task AssignTag_AlreadyAssigned_ReturnsBadRequest()
        {
            SetAuthor();
            AssignTagDTO dto = new AssignTagDTO { PostId = 1, TagId = 2 };
            categoryServiceMock.Setup(s => s.AddTagToPost(1, 2)).ThrowsAsync(new Exception("Tag already assigned."));

            IActionResult result = await categoryController.AssignTag(dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test removing tag from a post
        [Test]
        public async Task RemoveTag_ValidDto_ReturnsOk()
        {
            SetAuthor();
            AssignTagDTO dto = new AssignTagDTO { PostId = 1, TagId = 2 };
            categoryServiceMock.Setup(s => s.RemoveTagFromPost(1, 2)).Returns(Task.CompletedTask);

            IActionResult result = await categoryController.RemoveTag(dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("removed"));
        }

        // Test getting all tags for a specific post
        [Test]
        public async Task GetTagsByPost_ValidPostId_ReturnsOkWithTags()
        {
            List<TagResponseDTO> mockTags = new List<TagResponseDTO>
            {
                new TagResponseDTO { TagId = 1, Name = "dotnet" }
            };
            categoryServiceMock.Setup(s => s.GetTagsByPost(5)).ReturnsAsync(mockTags);

            IActionResult result = await categoryController.GetTagsByPost(5);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        // Test assigning a category to a post
        [Test]
        public async Task AssignCategory_ValidDto_ReturnsOk()
        {
            SetAuthor();
            AssignCategoryDTO dto = new AssignCategoryDTO { PostId = 1, CategoryId = 3 };
            categoryServiceMock.Setup(s => s.AddCategoryToPost(1, 3)).Returns(Task.CompletedTask);

            IActionResult result = await categoryController.AssignCategory(dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("assigned"));
        }

        // Test assigning same category twice
        [Test]
        public async Task AssignCategory_AlreadyAssigned_ReturnsBadRequest()
        {
            SetAuthor();
            AssignCategoryDTO dto = new AssignCategoryDTO { PostId = 1, CategoryId = 3 };
            categoryServiceMock.Setup(s => s.AddCategoryToPost(1, 3)).ThrowsAsync(new Exception("Category already assigned."));

            IActionResult result = await categoryController.AssignCategory(dto);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        // Test removing category from a post
        [Test]
        public async Task UnassignCategory_ValidDto_ReturnsOk()
        {
            SetAuthor();
            AssignCategoryDTO dto = new AssignCategoryDTO { PostId = 1, CategoryId = 3 };
            categoryServiceMock.Setup(s => s.RemoveCategoryFromPost(1, 3)).Returns(Task.CompletedTask);

            IActionResult result = await categoryController.UnassignCategory(dto);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.Value.ToString().Contains("removed"));
        }

        // Test getting all categories for a specific post
        [Test]
        public async Task GetCategoriesByPost_ValidPostId_ReturnsOkWithCategories()
        {
            List<CategoryResponseDTO> mockCategories = new List<CategoryResponseDTO>
            {
                new CategoryResponseDTO { CategoryId = 1, Name = "Tech" }
            };
            categoryServiceMock.Setup(s => s.GetCategoriesByPost(5)).ReturnsAsync(mockCategories);

            IActionResult result = await categoryController.GetCategoriesByPost(5);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            List<CategoryResponseDTO> categories = okResult.Value as List<CategoryResponseDTO>;
            Assert.AreEqual(1, categories.Count);
        }
    }
}
