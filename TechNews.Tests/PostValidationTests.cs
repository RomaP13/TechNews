using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures; // Потрібно для TempData
using Microsoft.EntityFrameworkCore;
using Moq; // Потрібно для Mock
using System.Security.Claims;
using TechNews.Controllers;
using TechNews.Models;
using Xunit;

namespace TechNews.Tests
{
    public class PostValidationTests
    {
        [Fact]
        public async Task Create_ReturnsView_WhenModelStateIsInvalid()
        {
            var options = new DbContextOptionsBuilder<NewsContext>()
                .UseInMemoryDatabase(databaseName: "ValidationTestDb")
                .Options;

            using (var context = new NewsContext(options))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }

            using (var context = new NewsContext(options))
            {
                var controller = new PostsController(context);
                
                controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

                controller.ModelState.AddModelError("Title", "Required");

                var newPost = new Post 
                { 
                    Title = "Ignored Title", 
                    ShortDescription = "Ignored Desc",
                    ImageUrl = "http://ignored.com",
                    Content = "Some content", 
                    CategoryId = 1,
                    CreatedAt = DateTime.Now
                };

                var result = await controller.Create(newPost);
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
                
                if (!context.Categories.Any())
                {
                    context.Categories.Add(new Category { Id = 1, Name = "Tech" });
                    await context.SaveChangesAsync();
                }
            }

            using (var context = new NewsContext(options))
            {
                var controller = new PostsController(context);

                // 1. Мокаємо Юзера
                var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "admin@test.com"),
                }, "mock"));

                controller.ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user }
                };

                controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
                
                var newPost = new Post 
                { 
                    Title = "Valid Title", 
                    Content = "Valid Content", 
                    ShortDescription = "Valid Desc",
                    ImageUrl = "http://valid.com",
                    CategoryId = 1,
                    AuthorEmail = "admin@test.com",
                    CreatedAt = DateTime.Now
                };

                var result = await controller.Create(newPost);
                var redirectResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirectResult.ActionName);
            }
        }
    }
}
