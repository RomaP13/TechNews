using Microsoft.EntityFrameworkCore;
using TechNews.Data;
using TechNews.Models;
using Xunit;

namespace TechNews.Tests
{
    public class PaginationTests
    {
        [Fact]
        public async Task PaginatedList_Calculates_TotalPages_Correctly()
        {
            var options = new DbContextOptionsBuilder<NewsContext>()
                .UseInMemoryDatabase(databaseName: "PaginationTestDb")
                .Options;

            // Очищення
            using (var context = new NewsContext(options))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                for (int i = 0; i < 10; i++)
                {
                    context.Posts.Add(new Post 
                    { 
                        Title = $"News {i}", 
                        Content = "Test", 
                        CategoryId = 1,
                        ShortDescription = "Test desc",
                        ImageUrl = "http://test.com/img.jpg"
                    });
                }
                await context.SaveChangesAsync();
            }

            using (var context = new NewsContext(options))
            {
                var query = context.Posts.AsQueryable();
                var result = await PaginatedList<Post>.CreateAsync(query, 1, 3);
                
                // 10 елементів по 3 на сторінку = 4 сторінки
                Assert.Equal(4, result.TotalPages);
                Assert.True(result.HasNextPage);
            }
        }
    }
}
