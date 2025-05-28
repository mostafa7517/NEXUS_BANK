using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BankSystem.ViewModels;

public class CustomerController : Controller
{
    private readonly BankContext _context;

    public CustomerController(BankContext context) => _context = context;


    public async Task<IActionResult> Dashboard()
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
            return RedirectToAction("Login", "Account");

        // load the logged-in customer's account
        var account = await _context.Accounts
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Customer.Username == username);

        // load last 5 transactions *with* sender & receiver customer info
        var recentTx = await _context.Transactions
            .Include(t => t.SenderAccount)
                .ThenInclude(a => a.Customer)
            .Include(t => t.ReceiverAccount)
                .ThenInclude(a => a.Customer)
            .Where(t => t.SenderAccountId == account.Id
                     || t.ReceiverAccountId == account.Id)
            .OrderByDescending(t => t.Date)
            .Take(5)
            .ToListAsync();

        // your existing cards load
        var cards = await _context.Cards
            .Where(c => c.AccountId == account.Id)
            .OrderBy(c => c.ExpirationDate)
            .ToListAsync();

        var vm = new CustomerDashboardViewModel
        {
            Customer = account.Customer,
            Account = account,
            Transactions = recentTx,
            CardCount = cards.Count,
            Cards = cards
        };

        return View(vm);
    }


    public async Task<IActionResult> AccountDetails()
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (accountId == null || accountId == 0)
            return RedirectToAction("Login", "Account");

        var account = await _context.Accounts
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Id == accountId);

        return View(account);
    }


    public async Task<IActionResult> TransactionHistory()
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (accountId == null || accountId == 0)
            return RedirectToAction("Login", "Account");

        ViewBag.AccountId = accountId;

        var transactions = await _context.Transactions
            .Include(t => t.SenderAccount)
                .ThenInclude(a => a.Customer)
            .Include(t => t.ReceiverAccount)
                .ThenInclude(a => a.Customer)
            .Where(t => t.SenderAccountId == accountId || t.ReceiverAccountId == accountId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();

        return View(transactions);
    }

}