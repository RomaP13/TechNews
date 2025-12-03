using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TechNews.Controllers;
using TechNews.Models;
using Xunit;

namespace TechNews.Tests
{
    public class CommentsApiTests
    {
        [Fact]
        public void Post_AddsComment_WhenUserIsAuthorized()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<NewsContext>()
                .UseInMemoryDatabase(databaseName: "CommentsTestDb")
                .Options;

            using (var context = new NewsContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            using (var context = new NewsContext(options))
            {
                var controller = new CommentsApiController(context);

                // Імітуємо залогіненого юзера
                var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "commenter@test.com"),
                }, "mock"));

                controller.ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user }
                };

                // Act
                var result = controller.Post(1, "Cool article!");

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var comment = Assert.IsType<Comment>(okResult.Value);
                
                Assert.Equal("Cool article!", comment.Content);
                Assert.Equal("commenter@test.com", comment.AuthorEmail);
                
                // Перевіряємо, що в базі з'явився запис
                Assert.Equal(1, context.Comments.Count());
            }
        }
    }
}
