using BankSystem.Data;
using BankSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
public class BankContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<BankAccount> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Card> Cards { get; set; }

    public BankContext(DbContextOptions<BankContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seeding example Employee - fixed by using HasData
        modelBuilder.Entity<Employee>().HasData(
            new Employee
            {
                Id = 1,
                Username = "admin",
                Password = "Admin2347531$",
                Name = "System Administrator",
                Email = "admin@bankingsystem.com",
                PhoneNumber = "555-123-4567",
                EmployeeID = "E0001",
                Department = "IT",
                Position = "System Administrator",
                HireDate = new DateTime(2023, 1, 1),
                Role = "Admin",
                EmploymentStatus = "Active"
            }
        );

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasOne(t => t.SenderAccount)
                  .WithMany()   
                  .HasForeignKey(t => t.SenderAccountId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(t => t.ReceiverAccount)
                  .WithMany()  
                  .HasForeignKey(t => t.ReceiverAccountId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}