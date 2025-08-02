using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApplication6.Models;
using WebApplication6.Data;

namespace WebApplication6.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public DashboardController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewBag.GameCount = _context.games.Count();
            ViewBag.OrderCount = _context.Orders.Count();
            ViewBag.UserCount = _userManager.Users.Count();
            ViewBag.MostClickedGame = _context.games
                .OrderByDescending(g => g.ClickCount)
                .FirstOrDefault()?.Name ?? "None";
            ViewBag.LatestOrderDate = _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault()?.CreatedAt.ToString("MMMM dd, yyyy") ?? "Never";

            ViewBag.LastOrderDate = _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault()?.CreatedAt.ToString("MMMM dd, yyyy") ?? "Never";

            return View();
        }
    }
}
