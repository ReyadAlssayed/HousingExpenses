using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingExpenses.Models;

[Table("people")]
public class Person
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    [Column("full_name")]
    public string FullName { get; set; } = string.Empty;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public ICollection<Expense> PaidExpenses { get; set; } = new List<Expense>();

    public ICollection<ExpenseParticipant> ExpenseParticipants { get; set; }
        = new List<ExpenseParticipant>();

    public ICollection<MonthlyPayment> MonthlyPayments { get; set; }
        = new List<MonthlyPayment>();
}