using BankSystem.Data;
using BankSystem.Models;
using System.Collections.Generic;

namespace BankSystem.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public Customer Customer { get; set; }
        public BankAccount Account { get; set; }
        public List<Transaction> Transactions { get; set; }
        public int CardCount { get; set; }
        public List<Card> Cards { get; set; } = new List<Card>();
    }
}
