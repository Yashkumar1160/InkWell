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
        // IPostService instance
        private IPostService postService;

        // Controller Dependecy Injection
        public PostController(IPostService service)
        {
            postService = service;
        }

        // only authors and admins can create posts
        [HttpPost("create")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Create([FromBody] CreatePostDTO dto)
        {
            try
            {
                // get author id and name from JWT token
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int authorId = int.Parse(idStr);
                string authorName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

                // Create post
                PostResponseDTO result = await postService.CreatePost(authorId, authorName, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Anyone can see published posts (no login needed)
        [HttpGet("published")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> GetPublished()
        {
            // Get publised posts
            List<PostResponseDTO> posts = await postService.GetPublished();
            return Ok(posts);
        }

        // Anyone can read a post by its slug url
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            try
            {
                // Get current user id from token if they are logged in
                int userId = 0;
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(idStr)) {
                    userId = int.Parse(idStr);
                }

                // Increment view count every time someone reads the post
                PostResponseDTO post = await postService.GetBySlug(slug, userId);
                await postService.IncrementViews(post.PostId);
                return Ok(post);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Get a post by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                // Get current user id from token if they are logged in
                int userId = 0;
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(idStr)) {
                    userId = int.Parse(idStr);
                }

                // Get post by id using post service
                PostResponseDTO post = await postService.GetById(id, userId);
                return Ok(post);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        // Author sees their own posts in dashboard
        [HttpGet("my-posts")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> GetMyPosts()
        {
            //Get author id from token
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int authorId = int.Parse(idStr);

            // Get post by author using postService
            List<PostResponseDTO> posts = await postService.GetByAuthor(authorId);
            return Ok(posts);
        }

        // Get all published posts by a specific author
        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetByAuthor(int authorId)
        {
            // Find all posts with specific author using postService
            List<PostResponseDTO> posts = await postService.GetByAuthor(authorId);
            return Ok(posts);
        }


        // Search posts by keyword
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            // Find all posts that contains keyword in title using postService
            List<PostResponseDTO> posts = await postService.Search(keyword);
            return Ok(posts);
        }

        // Admin sees all posts regardless of status
        [HttpGet("all")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            List<PostResponseDTO> posts = await postService.GetAll();
            return Ok(posts);
        }

        // Author updates their own post
        [HttpPut("update/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePostDTO dto)
        {
            try
            {
                // Find authorId
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int authorId = int.Parse(idStr);

                // Update post using postService
                PostResponseDTO result = await postService.UpdatePost(id, authorId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Author publishes their post
        [HttpPut("publish/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Publish(int id)
        {
            try
            {
                // Find authorID
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int authorId = int.Parse(idStr);

                // publish post using postService
                PostResponseDTO result = await postService.PublishPost(id, authorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Author hides their post
        [HttpPut("unpublish/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Unpublish(int id)
        {
            try
            {
                // Find author id
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int authorId = int.Parse(idStr);

                // Unpublish post using postService
                PostResponseDTO result = await postService.UnpublishPost(id, authorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Author deletes their own post , Admin can delete any post
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int authorId = int.Parse(idStr);

                string callerRole = User.FindFirstValue(ClaimTypes.Role);

                // Delete post using postService
                await postService.DeletePost(id, authorId, callerRole);
                return Ok(new { message = "Post deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Logged in user likes a post
        [HttpPost("like/{id}")]
        [Authorize]
        public async Task<IActionResult> Like(int id)
        {
            try
            {
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int actorId = int.Parse(idStr);

                string actorName = User.FindFirstValue("FullName") ?? User.FindFirstValue(ClaimTypes.Name);

                await postService.LikePost(id, actorId, actorName);
                return Ok(new { message = "Post liked." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Logged in user removes their like
        [HttpPost("unlike/{id}")]
        [Authorize]
        public async Task<IActionResult> Unlike(int id)
        {
            try
            {
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int actorId = int.Parse(idStr);

                //  Remove like using postService
                await postService.UnlikePost(id, actorId);
                return Ok(new { message = "Post unliked." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // Author archives their post
        [HttpPut("archive/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Archive(int id)
        {
            try
            {
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int authorId = int.Parse(idStr);

                PostResponseDTO result = await postService.ArchivePost(id, authorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Author restores archived post
        [HttpPut("unarchive/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Unarchive(int id)
        {
            try
            {
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int authorId = int.Parse(idStr);

                PostResponseDTO result = await postService.UnarchivePost(id, authorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Admin filters posts by status
        [HttpGet("by-status")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetByStatus([FromQuery] string status)
        {
            try
            {
                List<PostResponseDTO> posts = await postService.GetByStatus(status);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Get total post count for an author
        [HttpGet("count/{authorId}")]
        [Authorize]
        public async Task<IActionResult> GetCount(int authorId)
        {
            int count = await postService.GetPostCount(authorId);
            return Ok(new { count = count });
        }

        // Admin pins post to top
        [HttpPut("feature/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Feature(int id)
        {
            try
            {
                PostResponseDTO result = await postService.FeaturePost(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Admin removes pin from post
        [HttpPut("unfeature/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Unfeature(int id)
        {
            try
            {
                PostResponseDTO result = await postService.UnfeaturePost(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}