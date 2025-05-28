using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankSystem.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

public class EmployeeController : Controller
{
    private readonly BankContext _context;

    public EmployeeController(BankContext context) => _context = context;

    // GET: /Employee/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        // Summary stats
        ViewBag.TotalCustomers = await _context.Customers.CountAsync();
        ViewBag.TotalAccounts = await _context.Accounts.CountAsync();
        ViewBag.TotalCards = await _context.Cards.CountAsync();
        ViewBag.TotalBalance = await _context.Accounts.SumAsync(a => a.Balance);

        // — New accounts per month (last 12)
        var twelveMonthsAgo = DateTime.UtcNow.AddMonths(-11);
        var rawByMonth = await _context.Customers
            .Where(c => c.RegistrationDate >= twelveMonthsAgo)
            .GroupBy(c => new { c.RegistrationDate.Year, c.RegistrationDate.Month })
            .Select(g => new {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Count = g.Count()
            })
            .ToListAsync();   // bring Year/Month/Count into memory

        var accountsChartData = rawByMonth
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .Select(x => new {
                Label = $"{x.Month:00}/{x.Year % 100:D2}",
                Count = x.Count
            })
            .ToList();
        ViewBag.AccountsChartData = accountsChartData;

        // — Deposits vs Withdrawals
        var totalDeposits = await _context.Transactions
            .Where(t => t.Type == "Deposit")
            .SumAsync(t => t.Amount);
        var totalWithdrawals = await _context.Transactions
            .Where(t => t.Type == "Withdrawal")
            .SumAsync(t => t.Amount);
        ViewBag.TxnPieData = new[] {
            new { Label = "Deposits",    Value = totalDeposits    },
            new { Label = "Withdrawals", Value = totalWithdrawals }
        };

        // — Average account balance
        ViewBag.AverageBalance = await _context.Accounts.AverageAsync(a => a.Balance);

        return View();
    }

    // GET: /Employee/ManageAccounts
    public async Task<IActionResult> ManageAccounts()
    {
        var accounts = await _context.Accounts
            .Include(a => a.Customer)
            .ToListAsync();
        return View(accounts);
    }
}
