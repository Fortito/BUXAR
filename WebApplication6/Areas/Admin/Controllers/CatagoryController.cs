using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication6.Data;
using WebApplication6.Models;

namespace WebApplication6.Areas.Admin.Controllers;
[Area(nameof(Admin))]
[Authorize(Roles = "Admin")]
public class CatagoryController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public CatagoryController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public IActionResult Index()
    {
        var catagories = _context.catagories.ToList();
        return View(catagories);
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Catagory catagory)
    {
        bool exists = _context.catagories.Any(c => c.Name.ToLower() == catagory.Name.ToLower());

        if (exists)
        {
            ModelState.AddModelError("Name", "This category already exists.");
            return View(catagory);
        }
        await _context.catagories.AddAsync(catagory);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var catagory = await _context.catagories.FindAsync(id);
        if (catagory == null)
        {
            TempData["Error"] = "Category not found.";
            return RedirectToAction(nameof(Index));
        }

        _context.catagories.Remove(catagory);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"{catagory.Name} category was deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

}
