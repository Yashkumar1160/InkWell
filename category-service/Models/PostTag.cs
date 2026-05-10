using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InkWell.Category.Models
{
    public class PostTag
    {
        public int PostTagId { get; set; }

        // which post this tag is assigned to
        public int PostId { get; set; }

        // which tag is assigned
        public int TagId { get; set; }
    }
}