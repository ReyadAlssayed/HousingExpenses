using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingExpenses.Models;

[Table("expenses")]
public class Expense
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("month_id")]
    public int MonthId { get; set; }

    [Required, MaxLength(200)]
    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Column("amount", TypeName = "numeric(12,2)")]
    public decimal Amount { get; set; }

    [Column("paid_by")]
    public int PaidBy { get; set; }

    [Column("expense_date")]
    public DateTime ExpenseDate { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(MonthId))]
    public HousingMonth Month { get; set; } = null!;

    [ForeignKey(nameof(PaidBy))]
    public Person Payer { get; set; } = null!;

    public ICollection<ExpenseParticipant> Participants { get; set; }
        = new List<ExpenseParticipant>();
}