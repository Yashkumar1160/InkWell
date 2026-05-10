using InkWell.Media.DTOs;

namespace InkWell.Media.Services.Interfaces
{
    public interface IMediaService
    {
        // Method to upload a file and save to local storage
        Task<MediaResponseDTO> UploadMedia(int uploaderId, IFormFile file);

        // Method to get media by id
        Task<MediaResponseDTO> GetById(int id);

        // Method to get all media by uploader 
        Task<List<MediaResponseDTO>> GetByUploader(int uploaderId);

        // Method to get all media linked to a post
        Task<List<MediaResponseDTO>> GetByPost(int postId);

        // Method to get all media platform wide (Admin only)
        Task<List<MediaResponseDTO>> GetAll();

        // Method to soft delete (sets IsDeleted to true)
        Task SoftDelete(int mediaId, int uploaderId, string callerRole);

        // Method to update the alt text of a media file
        Task<MediaResponseDTO> UpdateAltText(int mediaId, int uploaderId, UpdateAltTextDTO dto, string callerRole);

        // Method to link media file to a post
        Task<MediaResponseDTO> LinkToPost(int mediaId, int postId);

        // Method to remove link between media and post
        Task<MediaResponseDTO> UnlinkFromPost(int mediaId);

        // Method to hard delete all soft deleted files (Admin only)
        Task CleanupDeleted();

        // Method to count files by uploader
        Task<int> GetMediaCount(int uploaderId);

        // Method to get media filtered by mime type
        Task<List<MediaResponseDTO>> GetByMimeType(string mimeType);

        // Method to get all soft deleted files
        Task<List<MediaResponseDTO>> GetDeletedFiles();
    }
}