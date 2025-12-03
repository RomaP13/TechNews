using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechNews.Controllers;
using TechNews.Models;
using System.Security.Claims;
using Xunit;

namespace TechNews.Tests
{
    public class PostValidationTests
    {
        [Fact]
        public async Task Create_ReturnsView_WhenModelStateIsInvalid()
        {
            var options = new DbContextOptionsBuilder<NewsContext>()
                .UseInMemoryDatabase(databaseName: "ValidationTestDb") // Унікальна назва бази
                .Options;

            // Очищаємо базу перед тестом, щоб уникнути конфліктів
            using (var context = new NewsContext(options))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }

            using (var context = new NewsContext(options))
            {
                var controller = new PostsController(context);
                // Імітуємо помилку валідації
                controller.ModelState.AddModelError("Title", "Required");

                var newPost = new Post 
                { 
                    Content = "Some content", 
                    CategoryId = 1 // Категорія 1 вже створена в EnsureCreated
                };

                var result = await controller.Create(newPost);

                // Перевіряємо, що повернувся ViewResult (сторінка з помилками)
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Equal(newPost, viewResult.Model);
            }
        }

        [Fact]
        public async Task Create_Redirects_WhenModelIsValid()
        {
            var options = new DbContextOptionsBuilder<NewsContext>()
                .UseInMemoryDatabase(databaseName: "SuccessTestDb")
                .Options;

            using (var context = new NewsContext(options))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }

            using (var context = new NewsContext(options))
            {
                var controller = new PostsController(context);

                var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "admin@test.com"),
                }, "mock"));

                controller.ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user }
                };
                
                var newPost = new Post 
                { 
                    Title = "Valid Title", 
                    Content = "Valid Content", 
                    CategoryId = 1,
                    ShortDescription = "Test",
                    ImageUrl = "http://img.com"
                };

                var result = await controller.Create(newPost);

                var redirectResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirectResult.ActionName);
            }
        }
    }
}
