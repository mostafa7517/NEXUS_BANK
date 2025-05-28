namespace BankSystem.Data
{
    public class Transaction
    {
        public int Id { get; set; }

        public int? SenderAccountId { get; set; }
        public int? ReceiverAccountId { get; set; } // Nullable for withdrawal

        public decimal Amount { get; set; }
        public string Type { get; set; } // Deposit, Withdrawal, Transfer
        public DateTime Date { get; set; } = DateTime.Now;

        public virtual BankAccount SenderAccount { get; set; }
        public virtual BankAccount ReceiverAccount { get; set; }
    }
}