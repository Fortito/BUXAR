using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication6.Areas.Admin.Controllers;
[Area("Admin")]

public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

