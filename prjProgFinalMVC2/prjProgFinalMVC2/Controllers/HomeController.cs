using Microsoft.AspNetCore.Mvc;
using prjProgFinalMVC2.Models;
using System.Diagnostics;

namespace prjProgFinalMVC2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Lecturer"))
                {
                    return RedirectToAction("Index", "Claims");
                }
                else if (User.IsInRole("HR") || User.IsInRole("Coordinator"))
                {
                    return RedirectToAction("Index", "Claims");
                }
            }
            return View();
        }
    }

}
