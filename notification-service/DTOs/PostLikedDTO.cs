using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InkWell.Notification.DTOs
{
    // data post service sends when post is liked
    public class PostLikedDTO
    {
        public int PostId { get; set; }
        public int PostAuthorId { get; set; }
        public int ActorId { get; set; }
    }
}