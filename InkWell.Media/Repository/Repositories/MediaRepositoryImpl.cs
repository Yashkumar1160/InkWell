using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InkWell.Media.Context;
using InkWell.Media.Models;
using InkWell.Media.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InkWell.Media.Repository.Repositories
{
    public class MediaRepositoryImpl : IMediaRepository
    {
        // MediaDbContext instance
        private MediaDbContext dbContext;

        // Constructor Dependency Injection
        public MediaRepositoryImpl(MediaDbContext context)
        {
            dbContext = context;
        }

        // Method to get media by id
        public async Task<MediaModel> GetById(int id)
        {
            MediaModel media = await dbContext.MediaFiles.FindAsync(id);

            return media;
        }

        // Method to get all files by a specific uploader

        public async Task<List<MediaModel>> GetByUploaderId(int uploaderId)
        {
            List<MediaModel> files = await dbContext.MediaFiles
                .Where(m => m.UploaderId == uploaderId && m.IsDeleted == false)
                .OrderByDescending(m => m.UploadedAt)
                .ToListAsync();

            return files;
        }

        // Method to get all files linked to a specific post
        public async Task<List<MediaModel>> GetByLinkedPostId(int postId)
        {
            List<MediaModel> files = await dbContext.MediaFiles
                .Where(m => m.LinkedPostId == postId && m.IsDeleted == false)
                .ToListAsync();

            return files;
        }

        // Method to get all non deleted files - admin use
        public async Task<List<MediaModel>> GetAll()
        {
            List<MediaModel> files = await dbContext.MediaFiles
                .Where(m => m.IsDeleted == false)
                .OrderByDescending(m => m.UploadedAt)
                .ToListAsync();

            return files;
        }

        // Method to get all soft deleted files
        public async Task<List<MediaModel>> GetDeleted()
        {
            return await GetByIsDeleted(true);
        }

        // Method to count files uploaded by a user
        public async Task<int> CountByUploaderId(int uploaderId)
        {
            int count = await dbContext.MediaFiles
                .CountAsync(m => m.UploaderId == uploaderId && m.IsDeleted == false);

            return count;
        }

        // Method to save new media record to database
        public async Task<MediaModel> Add(MediaModel media)
        {
            dbContext.MediaFiles.Add(media);
            await dbContext.SaveChangesAsync();

            return media;
        }

        // Method to save changes to existing media record
        public async Task<MediaModel> Update(MediaModel media)
        {
            dbContext.MediaFiles.Update(media);
            await dbContext.SaveChangesAsync();

            return media;
        }

        // Method to hard delete a media record by its id
        public async Task DeleteByMediaId(int id)
        {
            MediaModel media = await dbContext.MediaFiles.FindAsync(id);
           
            if (media != null)
            {
                dbContext.MediaFiles.Remove(media);
                await dbContext.SaveChangesAsync();
            }
        }

        // Method to get files filtered by mime type
        public async Task<List<MediaModel>> GetByMimeType(string mimeType)
        {
            List<MediaModel> files = await dbContext.MediaFiles
                .Where(m => m.MimeType == mimeType && m.IsDeleted == false)
                .OrderByDescending(m => m.UploadedAt)
                .ToListAsync();

            return files;
        }

        // Method to get files filtered by IsDeleted flag
        public async Task<List<MediaModel>> GetByIsDeleted(bool isDeleted)
        {
            List<MediaModel> files = await dbContext.MediaFiles
                .Where(m => m.IsDeleted == isDeleted)
                .OrderByDescending(m => m.UploadedAt)
                .ToListAsync();

            return files;
        }
    }
}