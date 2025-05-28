using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankSystem.Data;
using BankSystem.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BankSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly BankContext _context;

        public AppointmentController(BankContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Account");

            if (role == "Employee")
                return RedirectToAction("ManageAppointments");

            // For customers
            var customer = _context.Customers.FirstOrDefault(c => c.Username == username);
            if (customer == null)
                return RedirectToAction("Login", "Account");

            var appointments = _context.Appointments
                .Where(a => a.CustomerId == customer.Id)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();

            return View(appointments);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Account");

            return View(new AppointmentViewModel { AppointmentDate = DateTime.Now.AddDays(1) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AppointmentViewModel model)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Account");

            // for business hours
            if (model.AppointmentDate.Hour < 9 || model.AppointmentDate.Hour >= 15)
            {
                ModelState.AddModelError("AppointmentDate", "Appointment time must be between 9 AM and 3 PM");
            }

            if (ModelState.IsValid)
            {
                var customer = _context.Customers.FirstOrDefault(c => c.Username == username);
                if (customer == null)
                    return RedirectToAction("Login", "Account");

                var appointment = new Appointment
                {
                    CustomerId = customer.Id,
                    AppointmentDate = model.AppointmentDate,
                    Purpose = model.Purpose,
                    Notes = model.Notes,
                    Status = "Pending"
                };

                _context.Appointments.Add(appointment);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Appointment scheduled successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult Cancel(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Account");

            var customer = _context.Customers.FirstOrDefault(c => c.Username == username);
            if (customer == null)
                return RedirectToAction("Login", "Account");

            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id && a.CustomerId == customer.Id);
            if (appointment == null)
                return NotFound();

            appointment.Status = "Cancelled";
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Appointment cancelled successfully.";
            return RedirectToAction("Index");
        }

        // Employee actions
        [HttpGet]
        public IActionResult ManageAppointments()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username) || role != "Employee")
                return RedirectToAction("Login", "Account");

            var appointments = _context.Appointments
                .Include(a => a.Customer)
                .OrderBy(a => a.Status)
                .ThenBy(a => a.AppointmentDate)
                .ToList();

            return View(appointments);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username) || role != "Employee")
                return RedirectToAction("Login", "Account");

            var appointment = _context.Appointments.Find(id);
            if (appointment == null)
                return NotFound();

            appointment.Status = status;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Appointment status updated successfully.";
            return RedirectToAction("ManageAppointments");
        }
    }
}