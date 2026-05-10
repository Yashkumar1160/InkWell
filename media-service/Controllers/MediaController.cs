using System.Security.Claims;
using InkWell.Media.DTOs;
using InkWell.Media.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InkWell.Media.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        // IMediaService instance
        private IMediaService mediaService;

        // Constructor Dependency Injection
        public MediaController(IMediaService service)
        {
            mediaService = service;
        }

        // POST /api/media/upload
        // author uploads a file
        // file is sent as multipart/form-data not JSON
        [HttpPost("upload")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int uploaderId = int.Parse(idStr);

                MediaResponseDTO result = await mediaService.UploadMedia(uploaderId, file);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/media/5
        // get single media file by id
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                MediaResponseDTO result = await mediaService.GetById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET /api/media/my-files
        // author sees their own media library
        [HttpGet("my-files")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> GetMyFiles()
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int uploaderId = int.Parse(idStr);

            List<MediaResponseDTO> files = await mediaService.GetByUploader(uploaderId);
            return Ok(files);
        }

        // GET /api/media/post/5
        // get all media linked to a specific post
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetByPost(int postId)
        {
            List<MediaResponseDTO> files = await mediaService.GetByPost(postId);
            return Ok(files);
        }

        // GET /api/media/all
        // admin sees all media platform wide
        [HttpGet("all")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            List<MediaResponseDTO> files = await mediaService.GetAll();
            return Ok(files);
        }

        // GET /api/media/count
        // get media count for logged in user
        [HttpGet("count")]
        [Authorize]
        public async Task<IActionResult> GetCount()
        {
            string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int uploaderId = int.Parse(idStr);

            int count = await mediaService.GetMediaCount(uploaderId);
            return Ok(new { count = count });
        }

        // DELETE /api/media/delete/5
        // soft delete a media file
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int uploaderId = int.Parse(idStr);
                string callerRole = User.FindFirstValue(ClaimTypes.Role);

                await mediaService.SoftDelete(id, uploaderId, callerRole);
                return Ok(new { message = "File deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/media/alt-text/5
        // update alt text for accessibility
        [HttpPut("alt-text/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> UpdateAltText(int id, [FromBody] UpdateAltTextDTO dto)
        {
            try
            {
                string idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int uploaderId = int.Parse(idStr);

                // pass caller role so admin can update any file
                string callerRole = User.FindFirstValue(ClaimTypes.Role);

                MediaResponseDTO result = await mediaService.UpdateAltText(id, uploaderId, dto, callerRole);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/media/link/5
        // link media file to a post
        [HttpPut("link/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> LinkToPost(int id, [FromBody] LinkPostDTO dto)
        {
            try
            {
                MediaResponseDTO result = await mediaService.LinkToPost(id, dto.PostId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/media/unlink/5
        // remove link between media and post
        [HttpPut("unlink/{id}")]
        [Authorize(Roles = "AUTHOR,ADMIN")]
        public async Task<IActionResult> UnlinkFromPost(int id)
        {
            try
            {
                MediaResponseDTO result = await mediaService.UnlinkFromPost(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /api/media/cleanup
        // admin permanently removes all soft deleted files
        [HttpDelete("cleanup")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Cleanup()
        {
            await mediaService.CleanupDeleted();
            return Ok(new { message = "Deleted files cleaned up." });
        }

        // GET /api/media/by-type?mimeType=image/jpeg
        // admin filters media by file type
        [HttpGet("by-type")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetByMimeType([FromQuery] string mimeType)
        {
            List<MediaResponseDTO> files = await mediaService.GetByMimeType(mimeType);
            return Ok(files);
        }

        // GET /api/media/deleted
        // admin can see all soft deleted files before cleanup
        [HttpGet("deleted")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetDeleted()
        {
            List<MediaResponseDTO> files = await mediaService.GetDeletedFiles();
            return Ok(files);
        }
    }
}