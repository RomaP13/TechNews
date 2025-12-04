using Microsoft.AspNetCore.Mvc;
using TechNews.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace TechNews.Controllers
{
    [Route("api/[controller]")]
    public class CommentsApiController : Controller
    {
        private readonly NewsContext _context;

        public CommentsApiController(NewsContext context)
        {
            _context = context;
        }

        // GET: Відкритий для всіх
        [HttpGet("{postId}")]
        public IEnumerable<object> Get(int postId)
        {
            return _context.Comments
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new { 
                    id = c.Id, 
                    authorEmail = c.AuthorEmail, 
                    content = c.Content, 
                    date = c.CreatedAt.ToString("dd.MM.yyyy HH:mm") 
                })
                .ToList();
        }

        // POST: Тільки для авторизованих
        [HttpPost]
        [Authorize] 
        public IActionResult Post(int postId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) 
            {
                return BadRequest("Коментар не може бути пустим");
            }

            string author = User.Identity?.Name ?? "Anonymous";

            var comment = new Comment
            {
                PostId = postId,
                Content = content,
                AuthorEmail = author, 
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return Ok(comment);
        }
    }
}
