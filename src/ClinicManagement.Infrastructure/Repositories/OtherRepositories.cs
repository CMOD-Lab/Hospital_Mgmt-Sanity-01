using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the Staff repository.
/// </summary>
public class StaffRepository : IStaffRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<StaffRepository> _logger;

    public StaffRepository(ClinicDbContext context, ILogger<StaffRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Staff>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .AsNoTracking()
            .Where(s => s.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Staff?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StaffId == id && s.IsActive, cancellationToken);
    }

    public async Task<Staff> AddAsync(Staff staff, CancellationToken cancellationToken = default)
    {
        _context.Staff.Add(staff);
        await _context.SaveChangesAsync(cancellationToken);
        return staff;
    }

    public async Task UpdateAsync(Staff staff, CancellationToken cancellationToken = default)
    {
        _context.Staff.Update(staff);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var staff = await _context.Staff.FindAsync(new object[] { id }, cancellationToken);
        if (staff != null)
        {
            staff.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Staff.AnyAsync(s => s.StaffId == id && s.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Staff>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .AsNoTracking()
            .Where(s => s.IsActive && s.Name.Contains(searchQuery))
            .ToListAsync(cancellationToken);
    }
}

/// <summary>
/// EF Core implementation of the Department repository.
/// </summary>
public class DepartmentRepository : IDepartmentRepository
{
    private readonly ClinicDbContext _context;

    public DepartmentRepository(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
            .Where(d => d.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DepartmentId == id && d.IsActive, cancellationToken);
    }

    public async Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DeptName == name && d.IsActive, cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments.AnyAsync(d => d.DepartmentId == id && d.IsActive, cancellationToken);
    }
}

/// <summary>
/// EF Core implementation of the Bill repository.
/// </summary>
public class BillRepository : IBillRepository
{
    private readonly ClinicDbContext _context;

    public BillRepository(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Bill>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Bills
            .AsNoTracking()
            .Include(b => b.Patient)
            .Include(b => b.Doctor)
            .ToListAsync(cancellationToken);
    }

    public async Task<Bill?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Bills
            .AsNoTracking()
            .Include(b => b.Patient)
            .Include(b => b.Doctor)
            .FirstOrDefaultAsync(b => b.BillId == id, cancellationToken);
    }

    public async Task<Bill> AddAsync(Bill bill, CancellationToken cancellationToken = default)
    {
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync(cancellationToken);
        return bill;
    }

    public async Task UpdateAsync(Bill bill, CancellationToken cancellationToken = default)
    {
        _context.Bills.Update(bill);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Bill>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await _context.Bills
            .AsNoTracking()
            .Include(b => b.Doctor)
            .Where(b => b.PatientId == patientId)
            .OrderByDescending(b => b.BillDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Bill>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        return await _context.Bills
            .AsNoTracking()
            .Include(b => b.Patient)
            .Where(b => b.DoctorId == doctorId)
            .OrderByDescending(b => b.BillDate)
            .ToListAsync(cancellationToken);
    }
}
