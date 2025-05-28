using System.ComponentModel.DataAnnotations;

namespace BankSystem.ViewModels
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Appointment Date")]
        [DataType(DataType.DateTime)]
        [FutureDate(ErrorMessage = "Appointment date must be in the future during business hours (9 AM - 3 PM)")]
        public DateTime AppointmentDate { get; set; }
        [Required]
        [StringLength(100)]
        public string Purpose { get; set; }
        [StringLength(500)]
        public string Notes { get; set; }
    }

    // Updated validation attribute to ensure date is in the future and within business hours
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime date = (DateTime)value;

            // Check if the date is in the future
            if (date <= DateTime.Now)
                return false;

            // Check if appointment is during business hours (9 AM - 3 PM)
            if (date.Hour < 9 || date.Hour >= 15)
                return false;

            return true;
        }
    }
}