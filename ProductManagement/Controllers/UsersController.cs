using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new List<(IdentityUser User, bool IsAdmin)>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add((user, roles.Contains("Admin")));
            }

            return View(userRoles);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleAdminRole(string userId, bool makeAdmin)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (makeAdmin)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
