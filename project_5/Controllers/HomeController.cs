using Microsoft.AspNetCore.Mvc;
using project_5.Models;
using System.Diagnostics;

namespace project_5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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

        public IActionResult About()
        {
            return View();
        }



        IActionResult Login()
        {
            return View("~/Views/User/Login.cshtml");
        }

        public IActionResult CarList()
        {
            return View("~/Views/CarList/Cars.cshtml"); 
        }


        [Route("details/{id:int}")] // Route: /products/details/5
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
