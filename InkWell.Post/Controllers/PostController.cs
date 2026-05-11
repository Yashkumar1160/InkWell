using System.Security.Claims;
using InkWell.Post.DTOs;
using InkWell.Post.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InkWell.Post.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService postService;

        public PostController(IPostService service)
        {
            postService = service;
        }

        [HttpPost("create")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Create([FromBody] CreatePostDTO dto)
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int authorId = int.Parse(idStr);
            string authorName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

            PostResponseDTO result = await postService.CreatePost(authorId, authorName, dto);
            return Ok(result);
        }

        [HttpGet("published")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> GetPublished()
        {
            List<PostResponseDTO> posts = await postService.GetPublished();
            return Ok(posts);
        }

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            int userId = 0;
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(idStr))
            {
                userId = int.Parse(idStr);
            }

            PostResponseDTO post = await postService.GetBySlug(slug, userId);
            await postService.IncrementViews(post.PostId);
            return Ok(post);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            int userId = 0;
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(idStr))
            {
                userId = int.Parse(idStr);
            }

            PostResponseDTO post = await postService.GetById(id, userId);
            return Ok(post);
        }

        [HttpGet("my-posts")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> GetMyPosts()
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int authorId = int.Parse(idStr);

            List<PostResponseDTO> posts = await postService.GetByAuthor(authorId);
            return Ok(posts);
        }

        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetByAuthor(int authorId)
        {
            List<PostResponseDTO> posts = await postService.GetByAuthor(authorId);
            return Ok(posts);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            List<PostResponseDTO> posts = await postService.Search(keyword);
            return Ok(posts);
        }

        [HttpGet("all")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            List<PostResponseDTO> posts = await postService.GetAll();
            return Ok(posts);
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePostDTO dto)
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int authorId = int.Parse(idStr);

            PostResponseDTO result = await postService.UpdatePost(id, authorId, dto);
            return Ok(result);
        }

        [HttpPut("publish/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Publish(int id)
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int authorId = int.Parse(idStr);

            PostResponseDTO result = await postService.PublishPost(id, authorId);
            return Ok(result);
        }

        [HttpPut("unpublish/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Unpublish(int id)
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int authorId = int.Parse(idStr);

            PostResponseDTO result = await postService.UnpublishPost(id, authorId);
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int authorId = int.Parse(idStr);
            string callerRole = User.FindFirstValue(ClaimTypes.Role);

            await postService.DeletePost(id, authorId, callerRole);
            return Ok(new { message = "Post deleted." });
        }

        [HttpPost("like/{id}")]
        [Authorize]
        public async Task<IActionResult> Like(int id)
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int actorId = int.Parse(idStr);
            string actorName = User.FindFirstValue("FullName") ?? User.FindFirstValue(ClaimTypes.Name);

            await postService.LikePost(id, actorId, actorName);
            return Ok(new { message = "Post liked." });
        }

        [HttpPost("unlike/{id}")]
        [Authorize]
        public async Task<IActionResult> Unlike(int id)
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int actorId = int.Parse(idStr);

            await postService.UnlikePost(id, actorId);
            return Ok(new { message = "Post unliked." });
        }

        [HttpPut("archive/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Archive(int id)
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int authorId = int.Parse(idStr);

            PostResponseDTO result = await postService.ArchivePost(id, authorId);
            return Ok(result);
        }

        [HttpPut("unarchive/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Unarchive(int id)
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int authorId = int.Parse(idStr);

            PostResponseDTO result = await postService.UnarchivePost(id, authorId);
            return Ok(result);
        }

        [HttpGet("by-status")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetByStatus([FromQuery] string status)
        {
            List<PostResponseDTO> posts = await postService.GetByStatus(status);
            return Ok(posts);
        }

        [HttpGet("count/{authorId}")]
        [Authorize]
        public async Task<IActionResult> GetCount(int authorId)
        {
            int count = await postService.GetPostCount(authorId);
            return Ok(new { count = count });
        }

        [HttpPut("feature/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Feature(int id)
        {
            PostResponseDTO result = await postService.FeaturePost(id);
            return Ok(result);
        }

        [HttpPut("unfeature/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Unfeature(int id)
        {
            PostResponseDTO result = await postService.UnfeaturePost(id);
            return Ok(result);
        }
    }
}