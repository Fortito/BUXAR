using Microsoft.AspNetCore.Mvc;
using WebApplication6.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _context;

        public SearchController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string query)
        {
            var games = _context.games
                .Include(g => g.GameCatagories)
                    .ThenInclude(gc => gc.Catagory)
                .Where(g =>
                    g.Name.Contains(query) ||
                    g.GameCatagories.Any(gc => gc.Catagory.Name.Contains(query)))
                .ToList();

            var vm = new SearchResult
            {
                SearchTerm = query,
                Games = games
            };

            return View(vm);
        }
    }

}
