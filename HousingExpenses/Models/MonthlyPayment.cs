using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingExpenses.Models;

[Table("monthly_payments")]
public class MonthlyPayment
{
    [Key]
    public long Id { get; set; }

    [Column("month_id")]
    public int MonthId { get; set; }

    [Column("person_id")]
    public int PersonId { get; set; }

    [Column(TypeName = "numeric(12,2)")]
    public decimal Amount { get; set; }

    [Column("payment_date")]
    public DateTime PaymentDate { get; set; }

    [MaxLength(200)]
    public string? Notes { get; set; }

    [ForeignKey(nameof(MonthId))]
    public HousingMonth Month { get; set; } = null!;

    [ForeignKey(nameof(PersonId))]
    public Person Person { get; set; } = null!;
}