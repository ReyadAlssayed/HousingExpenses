using System.ComponentModel.DataAnnotations.Schema;

namespace HousingExpenses.Models;

[Table("expense_participants")]
public class ExpenseParticipant
{
    [Column("expense_id")]
    public long ExpenseId { get; set; }

    [Column("person_id")]
    public int PersonId { get; set; }

    [Column("share_amount", TypeName = "numeric(12,2)")]
    public decimal ShareAmount { get; set; }

    [ForeignKey(nameof(ExpenseId))]
    public Expense Expense { get; set; } = null!;

    [ForeignKey(nameof(PersonId))]
    public Person Person { get; set; } = null!;
}