using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
    
        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (User.IsInRole("Seller"))
            {
                string id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                return RedirectToAction("Index" , "Seller", new { sellerId = id });
            }
            else if (User.IsInRole("Buyer"))
            {
                string id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                return RedirectToAction("Index", "Buyer", new { buyerid = id });
            }
            else
            {
                return RedirectToAction("Login", "Acccount");
            }
         //   return View();
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