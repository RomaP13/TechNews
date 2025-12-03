using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechNews.Models;

namespace TechNews.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Список користувачів (Index)
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();

            if (TempData["ErrorMessage"] != null)
            {
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRolesViewModel.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return View(userRolesViewModel);
        }

        // Редагування прав (GET)
        public async Task<IActionResult> Edit(string userId)
        {
            if (userId == null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            
            if (user.Id == _userManager.GetUserId(User))
            {
                TempData["ErrorMessage"] = "Ви не можете змінити власні права доступу. Вийдіть і спробуйте з іншого акаунту Адміністратора.";
                return RedirectToAction(nameof(Index));
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.ToListAsync();

            var model = new ChangeRoleViewModel
            {
                UserId = user.Id,
                UserEmail = user.Email,
                UserRoles = userRoles,
                AllRoles = allRoles
            };

            return View(model);
        }

        // Редагування прав (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var loggedInUserId = _userManager.GetUserId(User);

            var isTargetAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (user.Id == loggedInUserId)
            {
                TempData["ErrorMessage"] = "Саморедагування ролей заборонено з міркувань безпеки.";
                return RedirectToAction(nameof(Index));
            }
            
            if (isTargetAdmin && !roles.Contains("Admin")) // Якщо він був адміном, і ми намагаємось забрати роль
            {
                 TempData["ErrorMessage"] = $"Ви не можете понизити в правах іншого Адміністратора ({user.Email}).";
                 return RedirectToAction(nameof(Index));
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            
            var addedRoles = roles.Except(userRoles);
            var removedRoles = userRoles.Except(roles);

            await _userManager.AddToRolesAsync(user, addedRoles);
            await _userManager.RemoveFromRolesAsync(user, removedRoles);

            return RedirectToAction(nameof(Index));
        }

        // Видалення користувача (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var loggedInUserId = _userManager.GetUserId(User);

                // Заборона видаляти самого себе
                if (user.Id == loggedInUserId)
                {
                    TempData["ErrorMessage"] = "Ви не можете видалити власний акаунт!";
                    return RedirectToAction(nameof(Index));
                }

                // Заборона видаляти іншого Адміністратора
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    TempData["ErrorMessage"] = $"Видалення іншого Адміністратора ({user.Email}) заборонено.";
                    return RedirectToAction(nameof(Index));
                }
                
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
