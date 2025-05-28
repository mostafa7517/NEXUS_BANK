using BankSystem.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BankSystem.Controllers
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
            // pull the role out of Session
            var role = HttpContext.Session.GetString("Role");

            if (!string.IsNullOrEmpty(role))
            {
                // if they’re already logged in as an employee, go right to /Employee/Dashboard
                if (role == "Employee")
                    return RedirectToAction("Dashboard", "Employee");

                // (optionally) if you want customers to skip home as well:
                if (role == "Customer")
                    return RedirectToAction("Dashboard", "Customer");
            }

            // otherwise show the “Welcome to the Bank System” splash
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult Careers()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Security()
        {
            return View();
        }

        public IActionResult Faqs()
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
