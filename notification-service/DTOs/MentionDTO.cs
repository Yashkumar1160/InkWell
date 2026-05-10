using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InkWell.Notification.DTOs
{
    // data comment service sends when user is mentioned
    public class MentionDTO
    {
        public int MentionedUserId { get; set; }
        public int ActorId { get; set; }
        public int CommentId { get; set; }
        public int PostId { get; set; }
    }
}