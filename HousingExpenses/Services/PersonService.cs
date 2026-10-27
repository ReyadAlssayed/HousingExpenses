using HousingExpenses.DbContext;
using HousingExpenses.Models;
using Microsoft.EntityFrameworkCore;

namespace HousingExpenses.Services;

public class PersonService
{
    private readonly AppDbContext _context;

    public PersonService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Person>> GetAllAsync()
    {
        try
        {
            return await _context.People
                .AsNoTracking()
                .OrderBy(p => p.FullName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    public async Task<List<Person>> GetActiveAsync()
    {
        return await _context.People
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.FullName)
            .ToListAsync();
    }

    public async Task<bool> AddAsync(string fullName)
    {
        try
        {
            fullName = fullName.Trim();

            if (string.IsNullOrWhiteSpace(fullName))
                return false;

            bool exists = await _context.People
                .AnyAsync(p => p.FullName.ToLower() == fullName.ToLower());

            if (exists)
                return false;

            _context.People.Add(new Person
            {
                FullName = fullName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    public async Task<bool> SetActiveAsync(int personId, bool isActive)
    {
        var person = await _context.People.FindAsync(personId);

        if (person is null)
            return false;

        person.IsActive = isActive;

        await _context.SaveChangesAsync();
        return true;
    }
}