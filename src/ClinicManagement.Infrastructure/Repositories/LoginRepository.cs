using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>EF Core implementation of ILoginRepository.</summary>
public class LoginRepository : ILoginRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<LoginRepository> _logger;

    public LoginRepository(ClinicDbContext context, ILogger<LoginRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<LoginTable?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.LoginTable
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Email == email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving login by email: {Email}", email);
            throw;
        }
    }

    public async Task<LoginTable?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.LoginTable
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LoginID == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving login by ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.LoginTable
                .AsNoTracking()
                .AnyAsync(l => l.Email == email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email existence: {Email}", email);
            throw;
        }
    }

    public async Task<LoginTable> AddAsync(LoginTable login, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.LoginTable.Add(login);
            await _context.SaveChangesAsync(cancellationToken);
            return login;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding login entry for email: {Email}", login.Email);
            throw;
        }
    }

    public async Task<int> GetTotalPatientCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Patients.AsNoTracking().CountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total patient count");
            throw;
        }
    }

    public async Task<double> GetTotalIncomeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .AsNoTracking()
                .Where(a => a.BillStatus == "Paid" && a.BillAmount.HasValue)
                .SumAsync(a => a.BillAmount!.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total income");
            throw;
        }
    }
}
