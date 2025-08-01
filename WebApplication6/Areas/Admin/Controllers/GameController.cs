using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Data;
using WebApplication6.Helper;
using WebApplication6.Models;
using WebApplication6.ViewModels;

namespace WebApplication6.Areas.Admin.Controllers;
[Area(nameof(Admin))]
[Authorize(Roles = "Admin, Devoloper")]
public class GameController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public GameController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var games = await _context.games
            .Include(g => g.GameCatagories)
                .ThenInclude(gc => gc.Catagory)
            .Include(g => g.GameImages)
            .ToListAsync();

        return View(games);

    }
    public IActionResult Create()
    {
        ViewBag.Catagories = _context.catagories.ToList();
        ViewBag.GameUrls = GetGameFileList();
        return View(new Game());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Game game, List<IFormFile> files)
    {
        bool exists = _context.games.Any(c => c.Name.ToLower() == game.Name.ToLower());

        if (exists)
        {
            ModelState.AddModelError("Name", "This Game name already exists.");
            ViewBag.Catagories = _context.catagories.ToList();
            return View(game);
        }
        if (files == null || !files.Any())
        {
            ViewData["error"] = "At least one image is required.";
            ViewBag.Catagories = _context.catagories.ToList();
            return View(game);
        }

        foreach (var file in files)
        {
            var photoUrl = await file.SaveFileAsync(_env.WebRootPath, "games");
            game.GameImages.Add(new GameImage
            {
                ImageUrl = photoUrl,
                Game = game
            });

            if (game.PhotoUrl == null)
            {
                game.PhotoUrl = photoUrl;
            }
        }


        game.GameCatagories.Clear();

        if (game.SelectedCatagoryIds != null)
        {
            foreach (var catId in game.SelectedCatagoryIds)
            {
                game.GameCatagories.Add(new GameCatagory
                {
                    CatagoryId = catId,
                    Game = game
                });
            }
        }

        await _context.games.AddAsync(game);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.games
            .Include(g => g.GameImages)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (product == null) return NotFound();

        _context.GameImages.RemoveRange(product.GameImages);
        _context.games.Remove(product);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var game = _context.games
            .Include(g => g.GameCatagories)
            .Include(g => g.GameImages) 
            .FirstOrDefault(g => g.Id == id);
        if (game == null) return NotFound();

        ViewBag.Catagories = _context.catagories.ToList();
        ViewBag.SelectedCatagoryIds = game.GameCatagories.Select(gc => gc.CatagoryId).ToList();
        ViewBag.GameUrls = GetGameFileList();
        return View(game);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Game game, List<IFormFile> files)
    {
        bool exists = _context.games.Any(c => c.Name.ToLower() == game.Name.ToLower() && c.Id != game.Id);
        if (exists)
        {
            ModelState.AddModelError("Name", "This Game name already exists.");
            ViewBag.Catagories = _context.catagories.ToList();
            ViewBag.SelectedCatagoryIds = game.SelectedCatagoryIds ?? new List<int>();
            return View(game);
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Catagories = _context.catagories.ToList();
            ViewBag.SelectedCatagoryIds = game.SelectedCatagoryIds ?? new List<int>();
            return View(game);
        }

        var gameDb = await _context.games
            .Include(g => g.GameCatagories)
            .Include(g => g.GameImages)
            .FirstOrDefaultAsync(g => g.Id == game.Id);

        if (gameDb == null) return NotFound();

        gameDb.Name = game.Name;
        gameDb.Description = game.Description;
        gameDb.Price = game.Price;
        gameDb.Dimensionality = game.Dimensionality;
        gameDb.Discount = game.Discount;
        gameDb.GameUrl = game.GameUrl;

        if (files != null && files.Any())
        {
            _context.GameImages.RemoveRange(gameDb.GameImages);

            foreach (var file in files)
            {
                var photoUrl = await file.SaveFileAsync(_env.WebRootPath, "games");
                gameDb.GameImages.Add(new GameImage
                {
                    ImageUrl = photoUrl,
                    GameId = gameDb.Id
                });

                if (gameDb.PhotoUrl == null)
                {
                    gameDb.PhotoUrl = photoUrl;
                }
            }
        }

        if (game.SelectedCatagoryIds != null)
        {

            _context.GameCatagories.RemoveRange(gameDb.GameCatagories);

            foreach (var catId in game.SelectedCatagoryIds)
            {
                gameDb.GameCatagories.Add(new GameCatagory
                {
                    GameId = gameDb.Id,
                    CatagoryId = catId
                });
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    private IEnumerable<SelectListItem> GetGameFileList()
    {
        var gamesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "games");
        var dirs = Directory.GetDirectories(gamesPath)
            .Select(dir => new SelectListItem
            {
                Text = Path.GetFileName(dir),
                Value = $"/games/{Path.GetFileName(dir)}/index.html"
            }).ToList();

        return dirs;
    }





}
