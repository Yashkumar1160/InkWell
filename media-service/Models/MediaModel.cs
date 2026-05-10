using System;
using System.ComponentModel.DataAnnotations;

namespace InkWell.Media.Models
{
    public class MediaModel
    {
        // primary key
        [Key]
        public int MediaId { get; set; }

        // who uploaded this file
        // links to UserId in auth service
        public int UploaderId { get; set; }

        // the file name we saved on disk
        // we generate a unique name to avoid conflicts
        // example: a3f2b1c4-image.jpg
        public string FileName { get; set; }

        // the original file name the user uploaded
        // example: my-holiday-photo.jpg
        public string OriginalName { get; set; }

        // full url to access the file
        // example: http://localhost:5005/uploads/a3f2b1c4-image.jpg
        public string Url { get; set; }

        // file type
        // example: image/jpeg, image/png, application/pdf
        public string MimeType { get; set; }

        // file size in kilobytes
        public long SizeKb { get; set; }

        // accessibility text for the image
        // used in html img alt attribute
        public string AltText { get; set; }

        // which post this media is linked to
        // null means not linked to any post yet
        public int? LinkedPostId { get; set; }

        // when the file was uploaded
        public DateTime UploadedAt { get; set; }

        // soft delete flag
        // true means file is deleted but record kept in db
        // this keeps post image urls working after deletion
        public bool IsDeleted { get; set; }

        public MediaModel()
        {
            FileName = "";
            OriginalName = "";
            Url = "";
            MimeType = "";
            SizeKb = 0;
            AltText = "";
            LinkedPostId = null;
            UploadedAt = DateTime.UtcNow;
            IsDeleted = false;
        }
    }
}