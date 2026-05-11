using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InkWell.Media.DTOs
{
    // what we send back after upload or any media query
    public class MediaResponseDTO
    {
        public int MediaId { get; set; }
        public int UploaderId { get; set; }
        public string FileName { get; set; }
        public string OriginalName { get; set; }
        public string Url { get; set; }
        public string MimeType { get; set; }
        public long SizeKb { get; set; }
        public string AltText { get; set; }
        public int? LinkedPostId { get; set; }
        public DateTime UploadedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}