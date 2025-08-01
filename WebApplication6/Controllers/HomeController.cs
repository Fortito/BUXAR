using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Data;
using WebApplication6.Models;
using WebApplication6.ViewModels;

namespace WebApplication6.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var lastEnter = _context.enters.OrderByDescending(x => x.Id).FirstOrDefault();
            var catagories = _context.catagories.ToList();
            var gamecatagories = _context.GameCatagories.ToList();

            var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id;

            List<int> ownedGameIds = new();
            List<Game> recommendedGames = new();

            if (userId != null)
            {
                ownedGameIds = _context.LibraryItems
                    .Where(li => li.Library.UserId == userId)
                    .Select(li => li.GameId)
                    .Distinct()
                    .ToList();

                var ownedCategoryIds = _context.GameCatagories
                    .Where(gc => ownedGameIds.Contains(gc.GameId))
                    .Select(gc => gc.CatagoryId)
                    .Distinct()
                    .ToList();

                recommendedGames = _context.games
                    .Include(g => g.GameCatagories)
                        .ThenInclude(gc => gc.Catagory)
                    .Include(g => g.GameImages)
                    .Where(g =>
                        !ownedGameIds.Contains(g.Id) &&
                        g.GameCatagories.Any(gc => ownedCategoryIds.Contains(gc.CatagoryId)))
                    .OrderByDescending(g => g.ClickCount)
                    .Take(4)
                    .ToList();
            }

            var games = _context.games
                .Include(g => g.GameCatagories)
                    .ThenInclude(gc => gc.Catagory)
                .Include(g => g.GameImages)
                .Where(g => !ownedGameIds.Contains(g.Id))
                .OrderByDescending(x => x.Id)
                .Take(4)
                .ToList();

            var trendingGames = _context.games
                .Include(g => g.GameCatagories)
                    .ThenInclude(gc => gc.Catagory)
                .Include(g => g.GameImages)
                .Where(g => !ownedGameIds.Contains(g.Id))
                .OrderByDescending(g => g.ClickCount)
                .Take(4)
                .ToList();

            var mostOrderedGames = _context.OrderItems
                .Include(oi => oi.Game)
                    .ThenInclude(g => g.GameCatagories)
                        .ThenInclude(gc => gc.Catagory)
                .Include(oi => oi.Game.GameImages)
                .GroupBy(oi => oi.Game)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Where(g => !ownedGameIds.Contains(g.Id)) 
                .Take(4)
                .ToList();


            var featuredGames = _context.games
                .Include(g => g.GameImages)
                .Include(g => g.GameCatagories)
                    .ThenInclude(gc => gc.Catagory)
                .Where(g => !ownedGameIds.Contains(g.Id))
                .OrderByDescending(g => g.ClickCount)
                .Take(6)
                .ToList();

            HomeVm homeVm = new()
            {
                LastEnter = lastEnter,
                games = games,
                catagories = catagories,
                gamecatagories = gamecatagories,
                TrendingGames = trendingGames,
                MostOrderedGames = mostOrderedGames,
                FeaturedGames = featuredGames,
                RecommendedGames = recommendedGames,
                OwnedGameIds = ownedGameIds
            };

            return View(homeVm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
