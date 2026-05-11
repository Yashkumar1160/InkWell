using System.Collections.Generic;
using System.Threading.Tasks;
using InkWell.Media.Models;

namespace InkWell.Media.Repository.Interfaces
{
    public interface IMediaRepository
    {
        // Method to get media by id
        Task<MediaModel> GetById(int id);

        // Method to get all media uploaded by a specific user
        Task<List<MediaModel>> GetByUploaderId(int uploaderId);

        // Method to get all media linked to a specific post
        Task<List<MediaModel>> GetByLinkedPostId(int postId);

        // Method to get all media files (Admin only)
        Task<List<MediaModel>> GetAll();

        // Method to get all soft deleted media
        Task<List<MediaModel>> GetDeleted();

        // Method to count how many files a user has uploaded
        Task<int> CountByUploaderId(int uploaderId);

        // Method to save new media record
        Task<MediaModel> Add(MediaModel media);

        // Method to save changes to existing media record
        Task<MediaModel> Update(MediaModel media);

        // Method to get all media files filtered by mime type
        Task<List<MediaModel>> GetByMimeType(string mimeType);

        // Method to get media filtered by deleted status
        Task<List<MediaModel>> GetByIsDeleted(bool isDeleted);

        // Method to hard delete a media record by its id
        Task DeleteByMediaId(int id);
    }
}