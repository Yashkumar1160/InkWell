using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InkWell.Category.Models
{
    public class Tag
    {
        // primary key
        [Key]
        public int TagId { get; set; }

        // tag name
        public string Name { get; set; }

        // slug
        public string Slug { get; set; }

        // post count
        public int PostCount { get; set; }

        // when this tag was created
        public DateTime CreatedAt { get; set; }

        // Constructor for setting default values
        public Tag()
        {
            Name = "";
            Slug = "";
            PostCount = 0;
            CreatedAt = DateTime.UtcNow;
        }
    }
}