
using HousingExpenses.DbContext;
using HousingExpenses.Models;
using Microsoft.EntityFrameworkCore;

namespace HousingExpenses.Services;

public class ExpenseService
{
    private readonly AppDbContext _context;

    public ExpenseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddAsync(
        int monthId,
        string description,
        decimal amount,
        int paidBy,
        List<int> participantIds,
        DateTime expenseDate)
    {
        description = description.Trim();

        participantIds = participantIds
            .Distinct()
            .ToList();

        if (string.IsNullOrWhiteSpace(description) ||
            amount <= 0 ||
            participantIds.Count == 0)
        {
            return false;
        }

        if (amount * 100m != decimal.Truncate(amount * 100m))
            return false;

        var month = await _context.Months
            .FirstOrDefaultAsync(m => m.Id == monthId);

        if (month is null || month.IsArchived)
            return false;

        bool payerExists = await _context.People
            .AnyAsync(p => p.Id == paidBy && p.IsActive);

        if (!payerExists)
            return false;

        int activeParticipants = await _context.People
            .CountAsync(p =>
                participantIds.Contains(p.Id) && p.IsActive);

        if (activeParticipants != participantIds.Count)
            return false;

        // حقل التاريخ في الصفحة يمثل تاريخ المصروف المحلي.
        // نحوله إلى UTC قبل حفظه في PostgreSQL.
        DateTime localDate = DateTime.SpecifyKind(
            expenseDate.Date,
            DateTimeKind.Local);

        DateTime expenseDateUtc = localDate.ToUniversalTime();

        long totalCents = (long)(amount * 100m);
        long baseShare = totalCents / participantIds.Count;
        long remainder = totalCents % participantIds.Count;

        var expense = new Expense
        {
            MonthId = monthId,
            Description = description,
            Amount = amount,
            PaidBy = paidBy,
            ExpenseDate = expenseDateUtc,
            CreatedAt = DateTime.UtcNow
        };

        for (int i = 0; i < participantIds.Count; i++)
        {
            long cents = baseShare + (i < remainder ? 1 : 0);

            expense.Participants.Add(new ExpenseParticipant
            {
                PersonId = participantIds[i],
                ShareAmount = cents / 100m
            });
        }

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<Expense>> GetByMonthAsync(int monthId)
    {
        return await _context.Expenses
            .AsNoTracking()
            .Include(e => e.Payer)
            .Include(e => e.Participants)
                .ThenInclude(p => p.Person)
            .Where(e => e.MonthId == monthId)
            .OrderByDescending(e => e.ExpenseDate)
            .ThenByDescending(e => e.Id)
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(long expenseId)
    {
        var expense = await _context.Expenses
            .Include(e => e.Month)
            .FirstOrDefaultAsync(e => e.Id == expenseId);

        if (expense is null || expense.Month.IsArchived)
            return false;

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<int> DeleteAllAsync()
    {
        var expenses = await _context.Expenses
            .Include(e => e.Participants)
            .ToListAsync();

        if (expenses.Count == 0)
            return 0;

        // حذف حصص المستفيدين أولًا
        _context.RemoveRange(
            expenses.SelectMany(e => e.Participants));

        // حذف جميع المصروفات من جميع الأشهر
        _context.Expenses.RemoveRange(expenses);

        await _context.SaveChangesAsync();

        return expenses.Count;
    }
}
