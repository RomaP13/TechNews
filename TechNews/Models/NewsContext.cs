using Microsoft.EntityFrameworkCore;

namespace TechNews.Models
{
    public class NewsContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public NewsContext(DbContextOptions<NewsContext> options)
            : base(options)
        {
            // Гарантує, що БД створена.
            Database.EnsureCreated(); 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Заповнюємо БД початковими даними (Seed Data)
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Технології" },
                new Category { Id = 2, Name = "Програмування" },
                new Category { Id = 3, Name = "Гаджети" },
                new Category { Id = 4, Name = "Штучний Інтелект" }
            );
        }
    }
}
