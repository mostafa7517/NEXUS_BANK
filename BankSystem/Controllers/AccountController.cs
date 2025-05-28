using BankSystem.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BankSystem.ViewModels;

public class AccountController : Controller
{
    private readonly BankContext _context;
    public AccountController(BankContext context) => _context = context;

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(Customer customer)
    {
        if (ModelState.IsValid)
        {
            // Check if username already exists
            if (_context.Customers.Any(c => c.Username == customer.Username))
            {
                ModelState.AddModelError("Username", "Username already exists. Please choose a different one.");
                return View(customer);
            }

            // Check if email already exists
            if (_context.Customers.Any(c => c.Email == customer.Email))
            {
                ModelState.AddModelError("Email", "Email already exists. Please use a different email address.");
                return View(customer);
            }

            customer.RegistrationDate = DateTime.Now;
            customer.AccountStatus = "Active";

            _context.Customers.Add(customer);
            _context.SaveChanges();

            var account = new BankAccount { CustomerId = customer.Id, Balance = 0 };
            _context.Accounts.Add(account);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful! Please login with your credentials.";
            return RedirectToAction("Login");
        }
        return View(customer);
    }
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var customer = _context.Customers.FirstOrDefault(c => c.Username == username && c.Password == password);
        var employee = _context.Employees.FirstOrDefault(e => e.Username == username && e.Password == password);

        if (customer != null)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.CustomerId == customer.Id);
            HttpContext.Session.SetString("Username", customer.Username);
            HttpContext.Session.SetString("Role", "Customer");
            HttpContext.Session.SetInt32("AccountId", account?.Id ?? 0); 

            return RedirectToAction("Dashboard", "Customer");
        }

        if (employee != null)
        {
            HttpContext.Session.SetString("Username", employee.Username);
            HttpContext.Session.SetString("Role", "Employee");
            return RedirectToAction("Dashboard", "Employee"); 
        }

        ViewBag.ErrorMessage = "Invalid credentials";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
    [HttpGet]
    public IActionResult ResetPassword()
    {
        var username = HttpContext.Session.GetString("Username");
        var role = HttpContext.Session.GetString("Role");

        if (!string.IsNullOrEmpty(username))
        {
            // Pre-fill username and user type if already logged in
            var model = new ResetPasswordViewModel
            {
                Username = username,
                UserType = role ?? "Customer"
            };
            return View(model);
        }

        return View(new ResetPasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ResetPassword(ResetPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.UserType == "Customer")
            {
                var customer = _context.Customers.FirstOrDefault(c =>
                    c.Username == model.Username &&
                    c.Password == model.CurrentPassword);

                if (customer != null)
                {
                    customer.Password = model.NewPassword;
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Password changed successfully!";
                    return RedirectToAction("Login");
                }
            }
            else if (model.UserType == "Employee")
            {
                var employee = _context.Employees.FirstOrDefault(e =>
                    e.Username == model.Username &&
                    e.Password == model.CurrentPassword);

                if (employee != null)
                {
                    employee.Password = model.NewPassword;
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Password changed successfully!";
                    return RedirectToAction("Login");
                }
            }

            ModelState.AddModelError("", "Invalid username or current password.");
        }

        return View(model);
    }

}
