using BankSystem.Data;
using BankSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

public class TransactionController : Controller
{
    private readonly BankContext _context;
    public TransactionController(BankContext context) => _context = context;

    // ─── DEPOSIT ──────────────────────────────────────────────────────
    [HttpGet]
    public IActionResult Deposit() => View();

    [HttpPost]
    public IActionResult Deposit(decimal amount)
    {
        var username = HttpContext.Session.GetString("Username");
        var customer = _context.Customers
            .Include(c => c.Account)
            .FirstOrDefault(c => c.Username == username);

        if (customer != null)
        {
            customer.Account.Balance += amount;
            _context.Transactions.Add(new Transaction
            {
                Amount = amount,
                Type = "Deposit",
                ReceiverAccountId = customer.Account.Id,
                Date = DateTime.Now
            });
            _context.SaveChanges();
            return RedirectToAction("Dashboard", "Customer");
        }

        ViewBag.ErrorMessage = "Deposit failed.";
        return View();
    }

    // ─── WITHDRAW ────────────────────────────────────────────────────
    [HttpGet]
    public IActionResult Withdraw() => View();

    [HttpPost]
    public IActionResult Withdraw(decimal amount)
    {
        var username = HttpContext.Session.GetString("Username");
        var customer = _context.Customers
            .Include(c => c.Account)
            .FirstOrDefault(c => c.Username == username);

        if (customer != null && customer.Account.Balance >= amount)
        {
            customer.Account.Balance -= amount;
            _context.Transactions.Add(new Transaction
            {
                Amount = amount,
                Type = "Withdrawal",
                SenderAccountId = customer.Account.Id,
                Date = DateTime.Now
            });
            _context.SaveChanges();
            return RedirectToAction("Dashboard", "Customer");
        }

        ViewBag.ErrorMessage = "Withdrawal failed or insufficient funds.";
        return View();
    }

    // ─── TRANSFER (by SSN) ───────────────────────────────────────────
    [HttpGet]
    public IActionResult Transfer() => View();

    [HttpPost]
    public IActionResult Transfer(string ssn, decimal amount)
    {
        var username = HttpContext.Session.GetString("Username");
        var sender = _context.Customers
            .Include(c => c.Account)
            .FirstOrDefault(c => c.Username == username);

        if (sender == null)
            return RedirectToAction("Login", "Account");

        var receiver = _context.Customers
            .Include(c => c.Account)
            .FirstOrDefault(c => c.SSN == ssn);

        if (receiver == null)
        {
            ViewBag.ErrorMessage = "No customer found with that SSN.";
            return View();
        }

        if (receiver.Account.Id == sender.Account.Id)
        {
            ViewBag.ErrorMessage = "Cannot transfer to your own account.";
            return View();
        }

        if (sender.Account.Balance < amount)
        {
            ViewBag.ErrorMessage = "Insufficient funds.";
            return View();
        }

        // all checks passed—do the transfer
        sender.Account.Balance -= amount;
        receiver.Account.Balance += amount;

        _context.Transactions.Add(new Transaction
        {
            Amount = amount,
            Type = "Transfer",
            SenderAccountId = sender.Account.Id,
            ReceiverAccountId = receiver.Account.Id,
            Date = DateTime.Now
        });

        _context.SaveChanges();
        return RedirectToAction("Dashboard", "Customer");
    }

    // ─── HISTORY ─────────────────────────────────────────────────────
    public IActionResult History()
    {
        var username = HttpContext.Session.GetString("Username");
        if (username == null) return RedirectToAction("Login", "Account");

        var customer = _context.Customers
            .Include(c => c.Account)
            .FirstOrDefault(c => c.Username == username);
        if (customer == null) return RedirectToAction("Login", "Account");

        var accountId = customer.Account.Id;
        var txns = _context.Transactions
            .Include(t => t.SenderAccount).ThenInclude(a => a.Customer)
            .Include(t => t.ReceiverAccount).ThenInclude(a => a.Customer)
            .Where(t => t.SenderAccountId == accountId
                     || t.ReceiverAccountId == accountId)
            .OrderByDescending(t => t.Date)
            .ToList();

        ViewBag.AccountId = accountId;
        return View(txns);
    }
}
