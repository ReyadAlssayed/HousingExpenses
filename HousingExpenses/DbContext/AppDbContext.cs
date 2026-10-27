using HousingExpenses.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace HousingExpenses.DbContext
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> People => Set<Person>();
        public DbSet<HousingMonth> Months => Set<HousingMonth>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<ExpenseParticipant> ExpenseParticipants => Set<ExpenseParticipant>();
        public DbSet<MonthlyPayment> MonthlyPayments => Set<MonthlyPayment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExpenseParticipant>()
                .HasKey(ep => new { ep.ExpenseId, ep.PersonId });

            modelBuilder.Entity<ExpenseParticipant>()
                .HasOne(ep => ep.Expense)
                .WithMany(e => e.Participants)
                .HasForeignKey(ep => ep.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExpenseParticipant>()
                .HasOne(ep => ep.Person)
                .WithMany(p => p.ExpenseParticipants)
                .HasForeignKey(ep => ep.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Payer)
                .WithMany(p => p.PaidExpenses)
                .HasForeignKey(e => e.PaidBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Month)
                .WithMany(m => m.Expenses)
                .HasForeignKey(e => e.MonthId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MonthlyPayment>()
                .HasOne(mp => mp.Person)
                .WithMany(p => p.MonthlyPayments)
                .HasForeignKey(mp => mp.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MonthlyPayment>()
                .HasOne(mp => mp.Month)
                .WithMany(m => m.MonthlyPayments)
                .HasForeignKey(mp => mp.MonthId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(12, 2);

            modelBuilder.Entity<ExpenseParticipant>()
                .Property(ep => ep.ShareAmount)
                .HasPrecision(12, 2);

            modelBuilder.Entity<MonthlyPayment>()
                .Property(mp => mp.Amount)
                .HasPrecision(12, 2);
        }
    }
}