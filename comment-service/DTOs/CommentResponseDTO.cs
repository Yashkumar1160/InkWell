using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InkWell.Comment.DTOs
{
    // what we send back to angular
    public class CommentResponseDTO
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public int AuthorId { get; set; }
        public int? ParentCommentId { get; set; }
        public string Content { get; set; }
        public int LikesCount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}