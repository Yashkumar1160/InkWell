using InkWell.Category.DTOs;
using InkWell.Category.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InkWell.Category.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        // ICategoryService instance    
        private ICategoryService categoryService;

        // Constructor Dependency Injection
        public CategoryController(ICategoryService service)
        {
            categoryService = service;
        }

        // --- Category Endpoints ---

        // POST /api/category/create
        // Create Categories (Admin only)
        [HttpPost("create")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO dto)
        {
            try
            {
                // create category using categoryService 
                CategoryResponseDTO result = await categoryService.CreateCategory(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/category/all
        // Get all categories
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories()
        {
            // get all categories using categoryService
            List<CategoryResponseDTO> categories = await categoryService.GetAllCategories();
            return Ok(categories);
        }

        // GET /api/category/5
        // Get category by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                // get category by id using categoryService
                CategoryResponseDTO category = await categoryService.GetCategoryById(id);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET /api/category/slug/technology
        // Get category by slug
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            try
            {   
                // get category by slug using categoryService
                CategoryResponseDTO category = await categoryService.GetCategoryBySlug(slug);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET /api/category/children/5
        // get child categories of a parent
        [HttpGet("children/{parentId}")]
        public async Task<IActionResult> GetChildren(int parentId)
        {
            // get child categories using categoryService
            List<CategoryResponseDTO> children = await categoryService.GetChildCategories(parentId);
            return Ok(children);
        }

        // PUT /api/category/update/5
        // Update Categories(Admin only)
        [HttpPut("update/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDTO dto)
        {
            try
            {
                // update categories using categoryService
                CategoryResponseDTO result = await categoryService.UpdateCategory(id, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /api/category/delete/5
        // delete categories (admin only)
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await categoryService.DeleteCategory(id);
                return Ok(new { message = "Category deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // --- Tag Endpoints ---

        // POST /api/category/tag/create
        // Create tags(admin only)
        [HttpPost("tag/create")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagDTO dto)
        {
            try
            {
                TagResponseDTO result = await categoryService.CreateTag(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/category/tag/all
        // Get all tags
        [HttpGet("tag/all")]
        public async Task<IActionResult> GetAllTags()
        {
            List<TagResponseDTO> tags = await categoryService.GetAllTags();
            return Ok(tags);
        }

        // GET /api/category/tag/trending
        // Get top 10 trending tags
        [HttpGet("tag/trending")]
        public async Task<IActionResult> GetTrendingTags()
        {
            List<TagResponseDTO> tags = await categoryService.GetTrendingTags();
            return Ok(tags);
        }

        // GET /api/category/tag/5
        // Get tag by id
        [HttpGet("tag/{id}")]
        public async Task<IActionResult> GetTagById(int id)
        {
            try
            {
                TagResponseDTO tag = await categoryService.GetTagById(id);
                return Ok(tag);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET /api/category/tag/slug/dotnet
        // Get tag by slug
        [HttpGet("tag/slug/{slug}")]
        public async Task<IActionResult> GetTagBySlug(string slug)
        {
            try
            {
                TagResponseDTO tag = await categoryService.GetTagBySlug(slug);
                return Ok(tag);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE /api/category/tag/delete/5
        // Delete tag (Admin only)
        [HttpDelete("tag/delete/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                await categoryService.DeleteTag(id);
                return Ok(new { message = "Tag deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // --- PostTag Endpoints ---

        // POST /api/category/tag/assign
        // Assign tag to post (Admin, Author)
        [HttpPost("tag/assign")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> AssignTag([FromBody] AssignTagDTO dto)
        {
            try
            {
                await categoryService.AddTagToPost(dto.PostId, dto.TagId);
                return Ok(new { message = "Tag assigned to post." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /api/category/tag/remove
        // Remove tag from post (Admin, Author)
        [HttpDelete("tag/remove")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> RemoveTag([FromBody] AssignTagDTO dto)
        {
            try
            {
                await categoryService.RemoveTagFromPost(dto.PostId, dto.TagId);
                return Ok(new { message = "Tag removed from post." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/category/tag/post/5
        // Get all tags on a post
        [HttpGet("tag/post/{postId}")]
        public async Task<IActionResult> GetTagsByPost(int postId)
        {
            List<TagResponseDTO> tags = await categoryService.GetTagsByPost(postId);
            return Ok(tags);
        }

        // POST /api/category/assign
        // Assign category to a post
        [HttpPost("assign")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> AssignCategory([FromBody] AssignCategoryDTO dto)
        {
            try
            {
                await categoryService.AddCategoryToPost(dto.PostId, dto.CategoryId);
                return Ok(new { message = "Category assigned to post." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /api/category/unassign
        // Remove category from a post (Admin, Author)
        [HttpDelete("unassign")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> UnassignCategory([FromBody] AssignCategoryDTO dto)
        {
            try
            {
                await categoryService.RemoveCategoryFromPost(dto.PostId, dto.CategoryId);
                return Ok(new { message = "Category removed from post." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/category/post/5
        // get all categories a post belongs to
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetCategoriesByPost(int postId)
        {
            List<CategoryResponseDTO> categories = await categoryService.GetCategoriesByPost(postId);
            return Ok(categories);
        }
    }
}