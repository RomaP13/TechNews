using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechNews.Models;

namespace TechNews.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly NewsContext _context;

    public HomeController(ILogger<HomeController> logger, NewsContext context)
    {
        _logger = logger;
        _context = context;
    }

    // GET: Home/Index
    public async Task<IActionResult> Index(string searchString, int? categoryId, int? pageNumber)
    {
        var postsQuery = _context.Posts.Include(p => p.Category).AsQueryable();

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            postsQuery = postsQuery.Where(p => p.CategoryId == categoryId);
        }

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            // Розбиваємо запит на слова (наприклад "Apple 16" -> ["Apple", "16"])
            var searchTerms = searchString.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var term in searchTerms)
            {
                // Кожне слово має зустрічатися хоча б в одному з полів
                // (Заголовок АБО Опис АБО Текст статті)
                postsQuery = postsQuery.Where(p => 
                    p.Title.ToLower().Contains(term) || 
                    p.ShortDescription.ToLower().Contains(term) ||
                    p.Content.ToLower().Contains(term));
            }
        }

        // Сортування (нові зверху)
        postsQuery = postsQuery.OrderByDescending(p => p.CreatedAt);

        // Пагінація
        int pageSize = 6; // Показувати по 6 новин
        
        // Зберігаємо дані для View
        ViewData["Categories"] = await _context.Categories.ToListAsync();
        ViewData["CurrentFilter"] = searchString;
        ViewData["CurrentCategory"] = categoryId;

        // Повертаємо список
        return View(await PaginatedList<Post>.CreateAsync(postsQuery, pageNumber ?? 1, pageSize));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var post = await _context.Posts
            .Include(p => p.Category)
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (post == null) return NotFound();

        return View(post);
    }

    // Сторінка "Про нас"
    public IActionResult About()
    {
        ViewData["Title"] = "Про нас";
        return View();
    }

    // Сторінка "Контакти"
    public IActionResult Contact()
    {
        ViewData["Title"] = "Контакти";
        return View();
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int postId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return RedirectToAction("Details", new { id = postId });
        }

        var comment = new Comment
        {
            PostId = postId,
            Content = content,
            AuthorEmail = User.Identity.Name, // Беремо email поточного користувача
            CreatedAt = DateTime.Now
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = postId });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
