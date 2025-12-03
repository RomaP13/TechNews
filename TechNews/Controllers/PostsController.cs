using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechNews.Models;

namespace TechNews.Controllers
{
    // Дозволяємо доступ АБО Адміну, АБО Редактору
    [Authorize(Roles = "Admin,Editor")]
    public class PostsController : Controller
    {
        private readonly NewsContext _context;

        public PostsController(NewsContext context)
        {
            _context = context;
        }

        // Список новин (Таблиця адміна)
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts.Include(p => p.Category).OrderByDescending(p => p.CreatedAt).ToListAsync();
            return View(posts);
        }

        // Створення (GET)
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // Створення (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            ModelState.Remove("Category");
            
            if (ModelState.IsValid)
            {
                post.CreatedAt = DateTime.Now;
                post.AuthorEmail = User.Identity?.Name;
                
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // Редагування (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // Редагування (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post)
        {
            if (id != post.Id) return NotFound();

            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                try
                {
                    // Зберігаємо оригінального автора та дату, якщо вони не міняються
                    var oldPost = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (oldPost != null)
                    {
                        post.CreatedAt = oldPost.CreatedAt;
                        post.AuthorEmail = oldPost.AuthorEmail;
                    }

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Posts.Any(e => e.Id == post.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }
        
        // Видалення (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var post = await _context.Posts.Include(p => p.Category).FirstOrDefaultAsync(m => m.Id == id);
            if (post == null) return NotFound();
            return View(post);
        }

        // Видалення (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null) _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
