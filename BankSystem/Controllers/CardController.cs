using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankSystem.Data;
using BankSystem.Models;        
using Microsoft.AspNetCore.Http; 

public class CardController : Controller
{
    private readonly BankContext _context;
    public CardController(BankContext context) => _context = context;

    // 1) INDEX: Customer sees only their cards; Employee sees all cards + customer info
    public async Task<IActionResult> Index()
    {
        var role = HttpContext.Session.GetString("Role");
        if (role == "Employee")
        {
            // admin: show every card, including link to deactivate
            var allCards = await _context.Cards
                .Include(c => c.Account)
                  .ThenInclude(a => a.Customer)
                .ToListAsync();
            return View(allCards);
        }
        else if (role == "Customer")
        {
            var username = HttpContext.Session.GetString("Username");
            var myCards = await _context.Cards
                .Include(c => c.Account)
                .Where(c => c.Account.Customer.Username == username)
                .ToListAsync();
            return View(myCards);
        }
        return RedirectToAction("Login", "Account");
    }

    // 2) GET: show form to choose Account & CardType
    public async Task<IActionResult> Create()
    {
        var username = HttpContext.Session.GetString("Username");
        if (username == null || HttpContext.Session.GetString("Role") != "Customer")
            return RedirectToAction("Login", "Account");

        // only customer's own accounts
        var accounts = await _context.Accounts
            .Where(a => a.Customer.Username == username)
            .ToListAsync();

        ViewBag.Accounts = accounts;
        ViewBag.CardTypes = Enum.GetValues(typeof(CardType))
                                .Cast<CardType>()
                                .ToList();
        return View();
    }

    // 2) POST: create card with chosen type
    [HttpPost]
    public async Task<IActionResult> Create(int accountId, CardType type)
    {
        var username = HttpContext.Session.GetString("Username");
        if (username == null || HttpContext.Session.GetString("Role") != "Customer")
            return RedirectToAction("Login", "Account");

        // ensure account belongs to customer
        var acct = await _context.Accounts
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Id == accountId && a.Customer.Username == username);
        if (acct == null) return Unauthorized();

        var card = new Card
        {
            AccountId = accountId,
            Type = type,
            CardNumber = GenerateCardNumber(),
            CVV = GenerateCvv(),
            ExpirationDate = DateTime.Now.AddYears(3),
            IsActive = true
        };
        _context.Cards.Add(card);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // 1) & 3) Reveal full card number after confirming password
    [HttpPost]
    public async Task<IActionResult> Reveal(int id, string password)
    {
        // only customer can reveal their own card
        var username = HttpContext.Session.GetString("Username");
        if (username == null || HttpContext.Session.GetString("Role") != "Customer")
            return RedirectToAction("Login", "Account");

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Username == username);

        if (customer == null || customer.Password != password)
        {
            TempData["Error"] = "Invalid password.";
            return RedirectToAction(nameof(Index));
        }

        var card = await _context.Cards
            .Include(c => c.Account)
              .ThenInclude(a => a.Customer)
            .FirstOrDefaultAsync(c => c.Id == id && c.Account.Customer.Username == username);

        if (card == null) return Unauthorized();

        TempData["CardId"] = id;
        TempData["FullCardNumber"] = card.CardNumber;
        return RedirectToAction(nameof(Index));
    }

    // 3) Deactivate (block) — allow both customer on their own cards and admin on any
    [HttpPost]
    public async Task<IActionResult> Deactivate(int id)
    {
        var card = await _context.Cards
            .Include(c => c.Account)
              .ThenInclude(a => a.Customer)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (card == null) return NotFound();

        var role = HttpContext.Session.GetString("Role");
        var username = HttpContext.Session.GetString("Username");

        bool isOwner = card.Account.Customer.Username == username;
        bool isAdmin = role == "Employee";

        if (!isOwner && !isAdmin)
            return Unauthorized();

        card.IsActive = false;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // Helpers
    private string GenerateCardNumber()
    {
        var rnd = new Random();
        return string.Concat(Enumerable.Range(0, 16)
            .Select(_ => rnd.Next(0, 10)));
    }
    private string GenerateCvv()
    {
        var rnd = new Random();
        return string.Concat(Enumerable.Range(0, 3)
            .Select(_ => rnd.Next(0, 10)));
    }
}
