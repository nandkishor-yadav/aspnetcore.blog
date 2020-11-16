using System;
using System.ComponentModel.DataAnnotations;

namespace aspnetcore.blog.Models
{
    public class Model
    {
        [Key]
        public int Id { get; set; }

        public DateTime LastModified { get; set; }

        public DateTime PubDate { get; set; }

        public bool IsPublished { get; set; }
    }
}
