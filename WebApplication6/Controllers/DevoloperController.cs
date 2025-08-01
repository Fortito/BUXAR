using Microsoft.AspNetCore.Mvc;
using WebApplication6.ViewModels;

namespace WebApplication6.Controllers;

public class DeveloperController : Controller
{

    [HttpGet]
    public IActionResult Apply()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Apply(DeveloperVm model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill all fields correctly.";
            return View(model);
        }



        TempData["Success"] = "Your application has been submitted successfully!";
        return RedirectToAction("Apply");
    }
}

