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
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService service)
        {
            categoryService = service;
        }

        // --- Category Endpoints ---

        [HttpPost("create")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO dto)
        {
            CategoryResponseDTO result = await categoryService.CreateCategory(dto);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories()
        {
            List<CategoryResponseDTO> categories = await categoryService.GetAllCategories();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            CategoryResponseDTO category = await categoryService.GetCategoryById(id);
            return Ok(category);
        }

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            CategoryResponseDTO category = await categoryService.GetCategoryBySlug(slug);
            return Ok(category);
        }

        [HttpGet("children/{parentId}")]
        public async Task<IActionResult> GetChildren(int parentId)
        {
            List<CategoryResponseDTO> children = await categoryService.GetChildCategories(parentId);
            return Ok(children);
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDTO dto)
        {
            CategoryResponseDTO result = await categoryService.UpdateCategory(id, dto);
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await categoryService.DeleteCategory(id);
            return Ok(new { message = "Category deleted." });
        }

        // --- Tag Endpoints ---

        [HttpPost("tag/create")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagDTO dto)
        {
            TagResponseDTO result = await categoryService.CreateTag(dto);
            return Ok(result);
        }

        [HttpGet("tag/all")]
        public async Task<IActionResult> GetAllTags()
        {
            List<TagResponseDTO> tags = await categoryService.GetAllTags();
            return Ok(tags);
        }

        [HttpGet("tag/trending")]
        public async Task<IActionResult> GetTrendingTags()
        {
            List<TagResponseDTO> tags = await categoryService.GetTrendingTags();
            return Ok(tags);
        }

        [HttpGet("tag/{id}")]
        public async Task<IActionResult> GetTagById(int id)
        {
            TagResponseDTO tag = await categoryService.GetTagById(id);
            return Ok(tag);
        }

        [HttpGet("tag/slug/{slug}")]
        public async Task<IActionResult> GetTagBySlug(string slug)
        {
            TagResponseDTO tag = await categoryService.GetTagBySlug(slug);
            return Ok(tag);
        }

        [HttpDelete("tag/delete/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            await categoryService.DeleteTag(id);
            return Ok(new { message = "Tag deleted." });
        }


        // --- PostTag Endpoints ---

        [HttpPost("tag/assign")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> AssignTag([FromBody] AssignTagDTO dto)
        {
            await categoryService.AddTagToPost(dto.PostId, dto.TagId);
            return Ok(new { message = "Tag assigned to post." });
        }

        [HttpDelete("tag/remove")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> RemoveTag([FromBody] AssignTagDTO dto)
        {
            await categoryService.RemoveTagFromPost(dto.PostId, dto.TagId);
            return Ok(new { message = "Tag removed from post." });
        }

        [HttpGet("tag/post/{postId}")]
        public async Task<IActionResult> GetTagsByPost(int postId)
        {
            List<TagResponseDTO> tags = await categoryService.GetTagsByPost(postId);
            return Ok(tags);
        }

        [HttpPost("assign")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> AssignCategory([FromBody] AssignCategoryDTO dto)
        {
            await categoryService.AddCategoryToPost(dto.PostId, dto.CategoryId);
            return Ok(new { message = "Category assigned to post." });
        }

        [HttpDelete("unassign")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> UnassignCategory([FromBody] AssignCategoryDTO dto)
        {
            await categoryService.RemoveCategoryFromPost(dto.PostId, dto.CategoryId);
            return Ok(new { message = "Category removed from post." });
        }

        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetCategoriesByPost(int postId)
        {
            List<CategoryResponseDTO> categories = await categoryService.GetCategoriesByPost(postId);
            return Ok(categories);
        }
    }
}