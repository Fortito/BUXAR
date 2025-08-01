using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using WebApplication6.Data;
using WebApplication6.DTOs;
using WebApplication6.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication6.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult AddToBasket(int id)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {

                var ownedGame = _context.LibraryItems
                    .Any(li => li.Library.UserId == userId && li.GameId == id);

                if (ownedGame)
                {
                    return Json(new { success = false, message = "You already own this game." });
                }
            }
            var cartCookie = Request.Cookies["cart"];
            List<CartCookieDto> cartCookies = new();
            CartCookieDto cartCookieDto = new CartCookieDto
            {
                Id = id
            };

            if (cartCookie == null)
            {
                cartCookies.Add(cartCookieDto);
                var cookieJson = JsonSerializer.Serialize(cartCookies);
                Response.Cookies.Append("cart", cookieJson);
            }
            else
            {
                var data = JsonSerializer.Deserialize<List<CartCookieDto>>(cartCookie);
                var findData = data.FirstOrDefault(x => x.Id == id);
                if (findData == null)
                {
                    data.Add(cartCookieDto);
                }
                var cookieJson = JsonSerializer.Serialize(data);
                Response.Cookies.Append("cart", cookieJson);
            }

            return Json(new { success = true });
        }

        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult GetGames()
        {
            var cartCookie = Request.Cookies["cart"];

            if (string.IsNullOrEmpty(cartCookie))
            {
                return Json(new { games = new List<object>() });
            }

            var datas = JsonSerializer.Deserialize<List<CartCookieDto>>(cartCookie);
            var dataIds = datas.Select(x => x.Id).ToList();

            var games = _context.games
                .Include(g => g.GameImages)
                .Where(x => dataIds.Contains(x.Id))
                .ToList();

            var gamesData = new
            {
                games = games.Select(g => new {
                    g.Id,
                    Name = g.Name,  
                    Price = g.Price,
                    PhotoUrl = g.GameImages.FirstOrDefault() != null ? g.GameImages.FirstOrDefault().ImageUrl : "/images/default.jpg"
                })
            };

            return Json(gamesData);
        }

        [HttpGet]
        public IActionResult CheckOut()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(Order order)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            order.UserId = userId;

            var cartCookie = Request.Cookies["cart"];
            var data = JsonSerializer.Deserialize<List<CartCookieDto>>(cartCookie);
            var dataIds = data.Select(x => x.Id).ToList();

          
            var ownedGameIds = await _context.LibraryItems
                .Where(li => li.Library.UserId == userId)
                .Select(li => li.GameId)
                .ToListAsync();

            var newGameIds = dataIds.Except(ownedGameIds).ToList();

            if (!newGameIds.Any())
            {
                
                ModelState.AddModelError("", "You already own all the games in your cart.");
                return View(order);
            }

            var games = _context.games.Where(x => newGameIds.Contains(x.Id)).ToList();


            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var orderItems = games.Select(game => new OrderItem
            {
                OrderId = order.Id,
                Name = game.Name,
                Price = game.Price,
                GameId = game.Id
            }).ToList();
            await _context.OrderItems.AddRangeAsync(orderItems);

            var library = new Library
            {
                UserId = userId,
                OrderId = order.Id,
                CreatedAt = DateTime.Now
            };
            await _context.Libraries.AddAsync(library);
            await _context.SaveChangesAsync();

            var libraryItems = games.Select(game => new LibraryItem
            {
                LibraryId = library.Id,
                GameId = game.Id,
                Name = game.Name,
                Price = game.Price,
                PhotoUrl = game.GameImages.FirstOrDefault()?.ImageUrl ?? "/images/default.jpg",
                CatagoryName = string.Join(", ", game.GameCatagories.Select(gc => gc.Catagory.Name))
            }).ToList();

            await _context.LibraryItems.AddRangeAsync(libraryItems);
            await _context.SaveChangesAsync();

            Response.Cookies.Delete("cart");

            return RedirectToAction("Index", "Library");
        }



        public IActionResult DeleteFromCart()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteFromCart(int id)
        {
            var cartCookie = Request.Cookies["cart"];
            if (cartCookie == null)
            {
                return Json(new { success = false });
            }

            var data = JsonSerializer.Deserialize<List<CartCookieDto>>(cartCookie);
            var itemToRemove = data.FirstOrDefault(x => x.Id == id);
            if (itemToRemove != null)
            {
                data.Remove(itemToRemove);
                var updatedCookie = JsonSerializer.Serialize(data);
                Response.Cookies.Append("cart", updatedCookie);
            }

            return Json(new { success = true, count = data.Count });
        }
        [HttpGet]
        public IActionResult GetOwnedGameIds()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { ids = new List<int>() });
            }

            var ownedGameIds = _context.LibraryItems
                .Where(li => li.Library.UserId == userId)
                .Select(li => li.GameId)
                .ToList();

            return Json(new { ids = ownedGameIds });
        }



    }

}
