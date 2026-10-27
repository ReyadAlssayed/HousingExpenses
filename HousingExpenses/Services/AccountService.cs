using HousingExpenses.DbContext;
using Microsoft.EntityFrameworkCore;

namespace HousingExpenses.Services;

public class AccountService
{
    private readonly AppDbContext _context;

    public AccountService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PersonAccount>> GetMonthlyAccountsAsync(int monthId)
    {
        var monthExists = await _context.Months
            .AnyAsync(m => m.Id == monthId);

        if (!monthExists)
            return new List<PersonAccount>();

        var people = await _context.People
            .AsNoTracking()
            .OrderBy(p => p.FullName)
            .ToListAsync();

        var shares = await _context.ExpenseParticipants
            .Where(ep => ep.Expense.MonthId == monthId)
            .GroupBy(ep => ep.PersonId)
            .Select(g => new
            {
                PersonId = g.Key,
                Total = g.Sum(ep => ep.ShareAmount)
            })
            .ToDictionaryAsync(x => x.PersonId, x => x.Total);

        var paid = await _context.Expenses
            .Where(e => e.MonthId == monthId)
            .GroupBy(e => e.PaidBy)
            .Select(g => new
            {
                PersonId = g.Key,
                Total = g.Sum(e => e.Amount)
            })
            .ToDictionaryAsync(x => x.PersonId, x => x.Total);

        return people.Select(person =>
        {
            decimal owed = shares.GetValueOrDefault(person.Id, 0m);
            decimal paidAmount = paid.GetValueOrDefault(person.Id, 0m);

            return new PersonAccount
            {
                PersonId = person.Id,
                FullName = person.FullName,
                TotalOwed = owed,
                TotalPaid = paidAmount,
                Balance = Math.Round(
                    paidAmount - owed, 2, MidpointRounding.AwayFromZero)
            };
        }).ToList();
    }
}

public class PersonAccount
{
    public int PersonId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public decimal TotalOwed { get; set; }

    public decimal TotalPaid { get; set; }

    public decimal Balance { get; set; }
}