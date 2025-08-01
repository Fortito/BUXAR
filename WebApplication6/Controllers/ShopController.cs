using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Data;

namespace WebApplication6.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        public ShopController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string category = null)
        {
            var catagories = _context.catagories.ToList();
            ViewBag.catagories = catagories;
            ViewBag.selectedCategory = category;

            var games = _context.games
                .Include(g => g.GameCatagories)
                .ThenInclude(gc => gc.Catagory)
                .Include(g => g.GameImages)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category) && category != "all")
            {
                games = games.Where(g => g.GameCatagories
                    .Any(gc => gc.Catagory.Name.ToLower() == category.ToLower()));
            }

            return View(games.ToList());
        }
    }
}
