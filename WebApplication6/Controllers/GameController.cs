using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Data;
using WebApplication6.Models;
using WebApplication6.ViewModels;

namespace WebApplication6.Controllers
{
    public class GameController : Controller
    {
        private readonly AppDbContext _context;

        public GameController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int id)
        {
            var game = _context.games
                .Include(g => g.GameCatagories)
                    .ThenInclude(gc => gc.Catagory)
                .Include(g => g.GameImages)
                .Include(g => g.Comments).ThenInclude(c => c.User)
                .FirstOrDefault(g => g.Id == id);

            if (game == null) return NotFound();

            game.ClickCount += 1;
            _context.SaveChanges();

            string userId = User.Identity.IsAuthenticated
                ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value
                : null;

            DetailVm detailVm = new()
            {
                Game = game,
                RelatedGames = GetRelatedGames(game),
                Comments = game.Comments.OrderByDescending(c => c.CreatedAt).ToList(),
                AverageRating = CalculateAverageRating(game.Id),
                UserRating = GetUserRating(game.Id, userId),
                ImageUrls = game.GameImages.Select(img => img.ImageUrl).ToList()
            };

            return View(detailVm);
        }

        [HttpPost]
        public IActionResult AddComment(int GameId, string Text)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account", new { area = "Identity" });

            if (string.IsNullOrWhiteSpace(Text))
            {
                TempData["Error"] = "Comment cannot be empty";
                return RedirectToAction("Detail", new { id = GameId });
            }

            var comment = new Comment
            {
                GameId = GameId,
                Text = Text,
                CreatedAt = DateTime.Now,
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("Detail", new { id = GameId });
        }
        [HttpPost]
        public IActionResult DeleteComment(int commentId, int gameId)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;

            var comment = _context.Comments.FirstOrDefault(c => c.Id == commentId && c.UserId == userId);

            if (comment == null)
                return NotFound();

            _context.Comments.Remove(comment);
            _context.SaveChanges();

            return RedirectToAction("Detail", new { id = gameId });
        }

        public IActionResult AddRating()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddRating(int gameId, int score)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            if (score < 1 || score > 5)
                return BadRequest("Invalid rating score.");

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;

            var existingRating = _context.Ratings.FirstOrDefault(r => r.GameId == gameId && r.UserId == userId);

            if (existingRating != null)
            {
                existingRating.Score = score;
            }
            else
            {
                var rating = new Rating
                {
                    GameId = gameId,
                    UserId = userId,
                    Score = score
                };
                _context.Ratings.Add(rating);
            }

            _context.SaveChanges();

            return RedirectToAction("Detail", new { id = gameId });
        }




        private List<Game> GetRelatedGames(Game game)
        {
            return _context.GameCatagories
                .Where(gc => game.GameCatagories.Select(c => c.CatagoryId).Contains(gc.CatagoryId)
                             && gc.GameId != game.Id)
                .Select(gc => gc.Game)
                .Distinct()
                .Take(3)
                .ToList();
        }

        private double CalculateAverageRating(int gameId)
        {
            var ratings = _context.Ratings.Where(r => r.GameId == gameId).ToList();
            return ratings.Any() ? ratings.Average(r => r.Score) : 0;
        }

        private int GetUserRating(int gameId, string userId)
        {
            if (string.IsNullOrEmpty(userId)) return 0;

            var rating = _context.Ratings.FirstOrDefault(r => r.GameId == gameId && r.UserId == userId);
            return rating?.Score ?? 0;
        }
    }
}
