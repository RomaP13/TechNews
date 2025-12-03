using Microsoft.EntityFrameworkCore;
using TechNews.Controllers;
using TechNews.Data;
using TechNews.Models;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;

namespace TechNews.Tests
{
    public class HomeSearchTests
    {
        [Fact]
        public async Task Index_ReturnsOnlyMatchingPosts_WhenSearchStringIsProvided()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<NewsContext>()
                .UseInMemoryDatabase(databaseName: "SearchTestDb")
                .Options;

            using (var context = new NewsContext(options))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                context.Posts.AddRange(
                    new Post 
                    { 
                        Title = "Apple iPhone 15", 
                        Content = "Review", 
                        CategoryId = 1,
                        ShortDescription = "Test desc",
                        ImageUrl = "http://img.com"
                    },
                    new Post 
                    { 
                        Title = "Samsung Galaxy", 
                        Content = "Review", 
                        CategoryId = 1,
                        ShortDescription = "Test desc",
                        ImageUrl = "http://img.com"
                    },
                    new Post 
                    { 
                        Title = "Apple MacBook", 
                        Content = "Laptop", 
                        CategoryId = 1,
                        ShortDescription = "Test desc",
                        ImageUrl = "http://img.com"
                    }
                );
                await context.SaveChangesAsync();
            }

            using (var context = new NewsContext(options))
            {
                var mockLogger = Mock.Of<ILogger<HomeController>>();
                var controller = new HomeController(mockLogger, context);

                // Act
                var result = await controller.Index("Apple", null, 1);

                // Assert
                var viewResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(result);
                var model = Assert.IsAssignableFrom<PaginatedList<Post>>(viewResult.Model);

                Assert.Equal(2, model.Count); 
                Assert.Contains(model, p => p.Title == "Apple iPhone 15");
                Assert.DoesNotContain(model, p => p.Title == "Samsung Galaxy");
            }
        }
    }
}
