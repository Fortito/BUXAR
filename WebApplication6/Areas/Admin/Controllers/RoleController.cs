using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication6.Data;

namespace WebApplication6.Areas;
[Area(nameof(Admin))]
[Authorize(Roles = "Admin")]
public class RoleController : Controller
    {
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }
    public IActionResult Index()
    {
        var roles = _roleManager.Roles.ToList();

        
        return View(roles);
    }
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(IdentityRole identityRole)
    {
        if (!ModelState.IsValid)
        {
            return View(identityRole);
        }

        IdentityResult result = await _roleManager.CreateAsync(identityRole);

        if (result.Succeeded)
        {
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(identityRole);

    }
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null) return NotFound();

        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();

        return View(role);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();

        var result = await _roleManager.DeleteAsync(role);

        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(role);
    }


}
