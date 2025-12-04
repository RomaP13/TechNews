using System;
using System.ComponentModel.DataAnnotations;

namespace TechNews.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Коментар")]
        public required string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Автор коментаря (email користувача)
        public required string AuthorEmail { get; set; }

        // Зв'язок з новиною (Зовнішній ключ)
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
    }
}
