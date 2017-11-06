using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AVISTED.Models;
using Microsoft.AspNetCore.Identity;

namespace AVISTED.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("AdminUserCollection", "Home");
            }
            return View();
        }
        public IActionResult AdminUserCollection()
        {
            return View();
        }
        public IActionResult Samples()
        {
            ViewData["Message"] = "Samples will be here soon.";

            return View();
        }

        public IActionResult FAQ()
        {
            ViewData["Message"] = "FAQ page.";

            return View();
        }
       
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
