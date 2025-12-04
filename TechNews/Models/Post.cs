using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TechNews.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Заголовок обов'язковий")]
        [Display(Name = "Заголовок")]
        public required string Title { get; set; }

        [Display(Name = "Короткий опис")]
        public required string ShortDescription { get; set; }

        [Required(ErrorMessage = "Текст новини обов'язковий")]
        [Display(Name = "Зміст")]
        public required string Content { get; set; }

        [Display(Name = "Зображення (URL)")]
        public required string ImageUrl { get; set; }

        [Display(Name = "Дата публікації")]
        public required DateTime CreatedAt { get; set; } = DateTime.Now;

        // Зв'язок з категорією
        [Display(Name = "Категорія")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        
        public string? AuthorEmail { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
