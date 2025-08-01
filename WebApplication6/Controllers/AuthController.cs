using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using WebApplication6.Data;
using WebApplication6.DTOs;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            var lastEnter = _context.enters.OrderByDescending(e => e.Id).FirstOrDefault();
            ViewBag.Enter = lastEnter;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var findEmail = await _userManager.FindByEmailAsync(login.Email);
            if (findEmail == null)
            {
                ViewData["error"] = "Email or Password is not valid";
                    return View();
            }

            Microsoft.AspNetCore.Identity.SignInResult result 
                = await _signInManager.PasswordSignInAsync(findEmail, login.Password, login.Rememberme, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["error"] = "Email or Password is not valid ";
                return View();
            }
            
        }
        
        public IActionResult Register()

        {
            var lastEnter = _context.enters.OrderByDescending(e => e.Id).FirstOrDefault();
            ViewBag.Enter = lastEnter;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }
           
            AppUser NewUser = new AppUser
            {
                FirstName = register.FirstName,
                LastName = register.LastName,
                Email = register.Email,
                UserName = register.Email
            };
            IdentityResult result = await _userManager.CreateAsync(NewUser, register.Password);
           

            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("error", error.Description);

                }
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }

}
