using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Areas.Admin.ViewModels;
using WebApplication6.Models;

namespace WebApplication6.Areas.Admin.Controllers;
[Area(nameof(Admin))]
[Authorize(Roles = "Admin")]

public class UserController : Controller
{
     private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IActionResult Index()
        {
            var users =_userManager.Users.ToList();
            return View(users);
        }
    [HttpGet]
    public async Task<IActionResult> AddRole (string userId)
    {
        if (string.IsNullOrEmpty(userId)) return NotFound();
        AppUser findUser = await _userManager.FindByIdAsync(userId);
        if (findUser == null) return NotFound();
        var userRole = await _userManager.GetRolesAsync(findUser);
       IEnumerable<string> roles = await _roleManager.Roles
            .Select(x => x.Name)
            .Except(userRole)
            .ToListAsync();
        UserRoleVm userRoleVm = new()
        {
            AppUser = findUser,
            Roles = roles
        };
        
        
        return View(userRoleVm);
    }
    [HttpPost]
    public async Task<IActionResult> AddRole(string userId, string role)
    {
        if (string.IsNullOrEmpty(role))
        {
            TempData["Error"] = "Chose Roll.";
            return RedirectToAction("AddRole", new { userId });
        }

        var findUser = await _userManager.FindByIdAsync(userId);
        if (findUser == null) return NotFound();

        await _userManager.AddToRoleAsync(findUser, role);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> EditRole(string userId)
    {
        var findUser = await _userManager.FindByIdAsync(userId);
        return View(findUser);
    }
    public async Task<IActionResult> DeleteRole(string userId, string role)
    {
        var findUser = await _userManager.FindByIdAsync(userId);
        IdentityResult result = await _userManager.RemoveFromRoleAsync(findUser, role);
        return RedirectToAction("Index");
    }
}


