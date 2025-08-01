using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public IActionResult CheckOut()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(Order order)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (!string.Equals(userEmail, order.Email, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("Email", "You must use your account's email address.");
                return View(order);
            }


            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return Unauthorized();

            var cartCookie = Request.Cookies["cart"];
            if (cartCookie == null)
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return View(order);
            }

            var cartData = JsonSerializer.Deserialize<List<CartCookieDto>>(cartCookie);
            if (cartData == null || !cartData.Any())
            {
                ModelState.AddModelError("", "No valid game found in your cart.");
                return View(order);
            }

            var cartGameIds = cartData.Select(x => x.Id).ToList();


            var userLibraryItems = await _context.LibraryItems
                .Where(li => li.Library.UserId == userId)
                .ToListAsync();

            var ownedGameIds = userLibraryItems.Select(x => x.GameId).ToList();
            var newGameIds = cartGameIds.Except(ownedGameIds).ToList();

            if (!newGameIds.Any())
            {
                ModelState.AddModelError("", "You already own all the games in your cart.");
                return View(order);
            }

            var gamesToBuy = await _context.games
                .Where(x => newGameIds.Contains(x.Id))
                .Include(x => x.GameImages)
                .Include(x => x.GameCatagories)
                    .ThenInclude(gc => gc.Catagory)
                .ToListAsync();


            order.UserId = userId;
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var orderItems = gamesToBuy.Select(game => new OrderItem
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

            var libraryItems = gamesToBuy.Select(game => new LibraryItem
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
