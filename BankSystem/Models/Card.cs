using BankSystem.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankSystem.Models
{
    public enum CardType
    {
        Debit,
        Credit
    }

    public class Card
    {
        public int Id { get; set; }

        [Required, StringLength(16, MinimumLength = 16)]
        public string CardNumber { get; set; }

        [Required, StringLength(3, MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string CVV { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public CardType Type { get; set; } = CardType.Debit;

        [ForeignKey(nameof(Account))]
        public int AccountId { get; set; }
        public BankAccount Account { get; set; }
    }
}
