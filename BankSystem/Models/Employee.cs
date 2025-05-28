using System.ComponentModel.DataAnnotations;
namespace BankSystem.Data
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string Department { get; set; }

        [Required]
        [Display(Name = "Position")]
        public string Position { get; set; }

        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; } = DateTime.Now;

        [Display(Name = "Last Login")]
        public DateTime? LastLogin { get; set; }

        public string Role { get; set; } = "Employee"; // Employee, Manager, Admin

        [Display(Name = "Employment Status")]
        public string EmploymentStatus { get; set; } = "Active"; // Active, OnLeave, Terminated
    }
}