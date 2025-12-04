using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace TechNews.Models
{
    // Модель для списку користувачів (Index)
    public class UserRolesViewModel
    {
        public required string UserId { get; set; }
        public required string Email { get; set; }
        public required  List<string> Roles { get; set; }
    }

    // Модель для редагування прав (Edit)
    public class ChangeRoleViewModel
    {
        public required string UserId { get; set; }
        public required string UserEmail { get; set; }
        public List<IdentityRole> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
        
        public ChangeRoleViewModel()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}
