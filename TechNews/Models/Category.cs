using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TechNews.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва категорії обов'язкова")]
        [Display(Name = "Назва категорії")]
        public string Name { get; set; }

        // Навігаційна властивість: Одна категорія має багато новин
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}
