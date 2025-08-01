using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication6.Data;
using WebApplication6.Models;

namespace WebApplication6.Areas.Admin.Controllers;
[Area(nameof(Admin))]
[Authorize(Roles = "Admin")]
public class EnterController : Controller
 {
      private readonly AppDbContext _context;
      private readonly IWebHostEnvironment _env;

    public EnterController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
    [HttpGet]
    public IActionResult Index()
        {
            List<Enter> enters = _context.enters.ToList();
        return View(enters);
        }
    [HttpGet]
    public IActionResult Create()
    { 
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Enter enter , IFormFile file)

    {
        if (file == null)
        {
            ModelState.AddModelError("error", "Photo is required");
            return View();
        }
        if (!ModelState.IsValid)
                return View();
       
        string FileName = @"/uploads/" + Guid.NewGuid() + file.FileName;
        using var stream = new FileStream(_env.WebRootPath + FileName, FileMode.Create);
        await file.CopyToAsync(stream);
        enter.PhotoUrl = FileName;
        _context.enters.Add(enter);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var enter = _context.enters.FirstOrDefault(ent => ent.Id == id);
        if (enter == null) return NotFound();

        return View(enter);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Enter enter, IFormFile file)
    {
        var enterDb = _context.enters.FirstOrDefault(_ent => _ent.Id == id);
        if (enterDb == null) return NotFound();
        if (file != null)
        {
            string FileName = @"/uploads/" + Guid.NewGuid() + file.FileName;
            using var stream = new FileStream(_env.WebRootPath + FileName, FileMode.Create);
            await file.CopyToAsync(stream);
            enterDb.PhotoUrl = FileName;
        }
        enterDb.SubTitle = enter.SubTitle;
        enterDb.Title = enter.Title;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public IActionResult Delete(int id)
    {
        var enter=_context.enters.FirstOrDefault(_ent => _ent.Id == id);
        if (enter == null) return NotFound();
        return View(enter);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(Enter enter )
    {
        _context.enters.Remove(enter);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}


