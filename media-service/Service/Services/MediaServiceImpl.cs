using InkWell.Media.DTOs;
using InkWell.Media.Models;
using InkWell.Media.Repository.Interfaces;
using InkWell.Media.Services.Interfaces;

namespace InkWell.Media.Services.Services
{
    public class MediaServiceImpl : IMediaService
    {
        // IMediaRepository instance
        private IMediaRepository mediaRepository;

        // IConfiguration instance
        private IConfiguration configuration;

        // Constructor Dependency Injection
        public MediaServiceImpl(IMediaRepository repository, IConfiguration config)
        {
            mediaRepository = repository;
            configuration = config;
        }

        // UPLOAD MEDIA
        // saves file to local wwwroot/uploads folder
        // saves metadata to database
        public async Task<MediaResponseDTO> UploadMedia(int uploaderId, IFormFile file)
        {
            // check file is not empty
            if (file == null || file.Length == 0)
            {
                throw new Exception("No file provided.");
            }

            // check file size does not exceed 10MB
            long maxSizeBytes = 10 * 1024 * 1024;
            if (file.Length > maxSizeBytes)
            {
                throw new Exception("File size exceeds 10MB limit.");
            }

            // check file type is allowed
            string mimeType = file.ContentType.ToLower();
            bool isAllowed = mimeType == "image/jpeg"
                || mimeType == "image/png"
                || mimeType == "image/gif"
                || mimeType == "image/webp"
                || mimeType == "application/pdf";

            if (isAllowed == false)
            {
                throw new Exception("File type not allowed. Use JPEG, PNG, GIF, WebP or PDF.");
            }

            // generate unique file name to avoid conflicts
            string uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
            string fileName = uniqueId + "-" + file.FileName;

            // get upload folder path from config
            string uploadFolder = configuration["MediaSettings:UploadFolder"];

            // create folder if it does not exist
            if (Directory.Exists(uploadFolder) == false)
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // full path where file will be saved on disk
            string filePath = Path.Combine(uploadFolder, fileName);

            // save file to disk
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // build the public url that angular uses to display the file
            string baseUrl = configuration["MediaSettings:BaseUrl"];
            string fileUrl = baseUrl + "/uploads/" + fileName;

            // save metadata to database
            MediaModel newMedia = new MediaModel();
            newMedia.UploaderId = uploaderId;
            newMedia.FileName = fileName;
            newMedia.OriginalName = file.FileName;
            newMedia.Url = fileUrl;
            newMedia.MimeType = file.ContentType;
            newMedia.SizeKb = file.Length / 1024;
            newMedia.AltText = "";
            newMedia.UploadedAt = DateTime.UtcNow;
            newMedia.IsDeleted = false;

            MediaModel saved = await mediaRepository.Add(newMedia);
            return MapToDTO(saved);
        }

        // Method to get media by id
        public async Task<MediaResponseDTO> GetById(int id)
        {
            MediaModel media = await mediaRepository.GetById(id);

            if (media == null)
            {
                throw new Exception("Media not found.");
            }

            return MapToDTO(media);
        }

        // Method to get media by uploader
        public async Task<List<MediaResponseDTO>> GetByUploader(int uploaderId)
        {
            List<MediaModel> files = await mediaRepository.GetByUploaderId(uploaderId);
            List<MediaResponseDTO> result = new List<MediaResponseDTO>();

            foreach (MediaModel media in files)
            {
                result.Add(MapToDTO(media));
            }

            return result;
        }

        // Method to get all media linked to a post
        public async Task<List<MediaResponseDTO>> GetByPost(int postId)
        {
            List<MediaModel> files = await mediaRepository.GetByLinkedPostId(postId);
            List<MediaResponseDTO> result = new List<MediaResponseDTO>();

            foreach (MediaModel media in files)
            {
                result.Add(MapToDTO(media));
            }

            return result;
        }

        // Method to get all media files (Admin only)
        public async Task<List<MediaResponseDTO>> GetAll()
        {
            List<MediaModel> files = await mediaRepository.GetAll();
            List<MediaResponseDTO> result = new List<MediaResponseDTO>();

            foreach (MediaModel media in files)
            {
                result.Add(MapToDTO(media));
            }

            return result;
        }

        // Method to soft delete media file
        public async Task SoftDelete(int mediaId, int uploaderId, string callerRole)
        {
            MediaModel media = await mediaRepository.GetById(mediaId);

            if (media == null)
            {
                throw new Exception("Media not found.");
            }

            // admin can delete any file
            // uploader can only delete their own files
            if (callerRole != "ADMIN" && media.UploaderId != uploaderId)
            {
                throw new Exception("You can only delete your own files.");
            }

            media.IsDeleted = true;

            await mediaRepository.Update(media);
        }

        // Method to update alt text
        public async Task<MediaResponseDTO> UpdateAltText(int mediaId, int uploaderId, UpdateAltTextDTO dto, string callerRole)
        {
            MediaModel media = await mediaRepository.GetById(mediaId);

            if (media == null)
            {
                throw new Exception("Media not found.");
            }

            // admin can update alt text on any file
            // uploader can only update their own files
            if (callerRole != "ADMIN" && media.UploaderId != uploaderId)
            {
                throw new Exception("You can only update alt text on your own files.");
            }

            media.AltText = dto.AltText;
            MediaModel updated = await mediaRepository.Update(media);

            return MapToDTO(updated);
        }

        // Method to connect a media file to a specific post
        public async Task<MediaResponseDTO> LinkToPost(int mediaId, int postId)
        {
            MediaModel media = await mediaRepository.GetById(mediaId);

            if (media == null)
            {
                throw new Exception("Media not found.");
            }

            media.LinkedPostId = postId;
            MediaModel updated = await mediaRepository.Update(media);

            return MapToDTO(updated);
        }


        // Method to remove the connection between media and post
        public async Task<MediaResponseDTO> UnlinkFromPost(int mediaId)
        {
            MediaModel media = await mediaRepository.GetById(mediaId);
            if (media == null)
            {
                throw new Exception("Media not found.");
            }

            media.LinkedPostId = null;
            MediaModel updated = await mediaRepository.Update(media);

            return MapToDTO(updated);
        }

        // Method to permanently delete all soft deleted files(Admin only)
        public async Task CleanupDeleted()
        {
            List<MediaModel> deletedFiles = await mediaRepository.GetDeleted();
            string uploadFolder = configuration["MediaSettings:UploadFolder"];

            foreach (MediaModel media in deletedFiles)
            {
                // delete actual file from disk if it exists
                string filePath = Path.Combine(uploadFolder, media.FileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // hard delete the database record
                await mediaRepository.DeleteByMediaId(media.MediaId);
            }
        }

        // Method to get media count
        public async Task<int> GetMediaCount(int uploaderId)
        {
            int count = await mediaRepository.CountByUploaderId(uploaderId);

            return count;
        }

        // Method to convert Media model to MediaResponseDTO
        private MediaResponseDTO MapToDTO(MediaModel media)
        {
            MediaResponseDTO dto = new MediaResponseDTO();
            dto.MediaId = media.MediaId;
            dto.UploaderId = media.UploaderId;
            dto.FileName = media.FileName;
            dto.OriginalName = media.OriginalName;
            dto.Url = media.Url;
            dto.MimeType = media.MimeType;
            dto.SizeKb = media.SizeKb;
            dto.AltText = media.AltText;
            dto.LinkedPostId = media.LinkedPostId;
            dto.UploadedAt = media.UploadedAt;
            dto.IsDeleted = media.IsDeleted;

            return dto;
        }

        // Method to get media by mime type
        public async Task<List<MediaResponseDTO>> GetByMimeType(string mimeType)
        {
            List<MediaModel> files = await mediaRepository.GetByMimeType(mimeType);
            List<MediaResponseDTO> result = new List<MediaResponseDTO>();

            foreach (MediaModel media in files)
            {
                result.Add(MapToDTO(media));
            }

            return result;
        }

        // Method to get all soft deleted media files
        public async Task<List<MediaResponseDTO>> GetDeletedFiles()
        {
            List<MediaModel> files = await mediaRepository.GetDeleted();
            List<MediaResponseDTO> result = new List<MediaResponseDTO>();

            foreach (MediaModel media in files)
            {
                result.Add(MapToDTO(media));
            }
            
            return result;
        }
    }
}