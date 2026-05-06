using System.Security.Claims;
using InkWell.Comment.DTOs;
using InkWell.Comment.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InkWell.Comment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        // ICommentService instance
        private ICommentService commentService;

        // Constructor Dependency Injection
        public CommentController(ICommentService service)
        {
            commentService = service;
        }


        // Add a comment
        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] CreateCommentDTO dto)
        {
            try
            {
                // get author id 
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int authorId = int.Parse(idStr);

                // get author name from claims
                string actorName = User.FindFirstValue("FullName") ?? User.FindFirstValue(ClaimTypes.Name);

                // Add comment using commentService
                CommentResponseDTO result = await commentService.AddComment(authorId, actorName, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Anyone can see comments on a post
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetByPost(int postId)
        {
            // get comments on a post using commentService 
            List<CommentResponseDTO> comments = await commentService.GetByPost(postId);
            return Ok(comments);
        }

        // Get only top level comments (not replies)
        [HttpGet("post/{postId}/top-level")]
        public async Task<IActionResult> GetTopLevel(int postId)
        {
            // get top level comments using commentService
            List<CommentResponseDTO> comments = await commentService.GetTopLevel(postId);
            return Ok(comments);
        }

        // Get all replies to a specific comment
        [HttpGet("replies/{parentCommentId}")]
        public async Task<IActionResult> GetReplies(int parentCommentId)
        {
            // Get replies on a comment using commentService
            List<CommentResponseDTO> replies = await commentService.GetReplies(parentCommentId);
            return Ok(replies);
        }

        // Get comment by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                // get comment by id 
                CommentResponseDTO comment = await commentService.GetById(id);
                return Ok(comment);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Get total comment count on a post
        [HttpGet("count/{postId}")]
        public async Task<IActionResult> GetCount(int postId)
        {
            // get comment count using commentService
            int count = await commentService.GetCommentCount(postId);
            return Ok(new { count = count });
        }

        // Edit comment (User)
        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCommentDTO dto)
        {
            try
            {
                // get author id from jwt token claims
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int authorId = int.Parse(idStr);

                // update comment
                CommentResponseDTO result = await commentService.UpdateComment(id, authorId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // User deletes their comment (Admin can delete any comment)
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // get author id using jwt token claims
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int authorId = int.Parse(idStr);

                // get role
                string callerRole = User.FindFirstValue(ClaimTypes.Role);

                // soft delete comment
                await commentService.SoftDeleteComment(id, authorId, callerRole);
                return Ok(new { message = "Comment deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Author approves comment on own post (admin approves any comment)
        [HttpPut("approve/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                // approve comments on post using commentService
                await commentService.ApproveComment(id);
                return Ok(new { message = "Comment approved." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Reject inappropriate comment
        [HttpPut("reject/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                // reject comment using commentService
                await commentService.RejectComment(id);
                return Ok(new { message = "Comment rejected." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Logged in user likes a comment
        [HttpPost("like/{id}")]
        [Authorize]
        public async Task<IActionResult> Like(int id)
        {
            try
            {
                // like a comment using commentService
                await commentService.LikeComment(id);
                return Ok(new { message = "Comment liked." });
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
                // unlike comment (decrease like using commentService)
                await commentService.UnlikeComment(id);
                return Ok(new { message = "Comment unliked." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Filters all comments by status (Admin only)
        [HttpGet("status")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetByStatus([FromQuery] string status)
        {
            try
            {
                // Filter comments by status using commentService
                List<CommentResponseDTO> comments = await commentService.GetByStatus(status);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // admin turns moderation mode on or off
        // when on all new comments go to PENDING instead of APPROVED
        [HttpPut("moderation")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult SetModeration([FromQuery] bool enabled)
        {
            commentService.SetModerationMode(enabled);

            string status = enabled ? "enabled" : "disabled";
            return Ok(new { message = "Moderation mode " + status });
        }

        // check if moderation mode is currently on or off
        [HttpGet("moderation")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetModeration()
        {
            bool enabled = commentService.GetModerationMode();
            return Ok(new { moderationEnabled = enabled });
        }
    }
}