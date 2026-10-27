using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingExpenses.Models;

[Table("months")]
public class HousingMonth
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    [Column("month_name")]
    public string MonthName { get; set; } = string.Empty;

    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Column("is_archived")]
    public bool IsArchived { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public ICollection<MonthlyPayment> MonthlyPayments { get; set; }
        = new List<MonthlyPayment>();
}