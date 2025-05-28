namespace BankSystem.Data
{
    public class Appointment
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Purpose { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Completed, Cancelled
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}