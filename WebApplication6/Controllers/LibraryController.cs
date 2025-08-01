using Microsoft.AspNetCore.Mvc;
using WebApplication6.Data;
using System.Globalization;
namespace WebApplication6.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.EntityFrameworkCore;
    using System.Security.Claims;
    using WebApplication6.Models;

    [Authorize]
    public class LibraryController : Controller
    {
        private readonly AppDbContext _context;

        public LibraryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var libraries = _context.Libraries
               .Include(l => l.Items)
                   .ThenInclude(i => i.Game)
                       .ThenInclude(g => g.GameCatagories)
                           .ThenInclude(gc => gc.Catagory)
               .Include(l => l.Items)
                   .ThenInclude(i => i.Game)
                       .ThenInclude(g => g.GameImages)
               .Where(l => l.UserId == userId)
               .OrderByDescending(l => l.CreatedAt)
               .ToList();

            ;

            return View(libraries);




        }

        [HttpGet]
        public IActionResult GetOwnedGameIds()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Json(new { ids = new List<int>() });

            var ownedGameIds = _context.LibraryItems
                .Where(li => li.Library.UserId == userId)
                .Select(li => li.GameId)
                .ToList();

            return Json(new { ids = ownedGameIds });
        }

    }
}
