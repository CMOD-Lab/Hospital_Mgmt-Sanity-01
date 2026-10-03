using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IDepartmentRepository.
/// </summary>
public class DepartmentRepository : IDepartmentRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<DepartmentRepository> _logger;

    public DepartmentRepository(ClinicDbContext context, ILogger<DepartmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Departments
                .AsNoTracking()
                .Include(d => d.Doctors.Where(doc => doc.IsActive))
                .Where(d => d.IsActive)
                .OrderBy(d => d.DeptName)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all departments");
            throw;
        }
    }

    public async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Departments
                .AsNoTracking()
                .Include(d => d.Doctors.Where(doc => doc.IsActive))
                .FirstOrDefaultAsync(d => d.DepartmentId == id && d.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department with ID {DepartmentId}", id);
            throw;
        }
    }

    public async Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DeptName == name && d.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department with name {DepartmentName}", name);
            throw;
        }
    }

    public async Task<Department> AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync(cancellationToken);
            return department;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding department");
            throw;
        }
    }

    public async Task UpdateAsync(Department department, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating department with ID {DepartmentId}", department.DepartmentId);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var department = await _context.Departments.FindAsync(new object[] { id }, cancellationToken);
            if (department != null)
            {
                department.IsActive = false;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting department with ID {DepartmentId}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments.AnyAsync(d => d.DepartmentId == id && d.IsActive, cancellationToken);
    }
}

/// <summary>
/// EF Core implementation of IStaffRepository.
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
        try
        {
            return await _context.Staff
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all staff");
            throw;
        }
    }

    public async Task<Staff?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Staff
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StaffId == id && s.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff with ID {StaffId}", id);
            throw;
        }
    }

    public async Task<Staff> AddAsync(Staff staff, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Staff.Add(staff);
            await _context.SaveChangesAsync(cancellationToken);
            return staff;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff");
            throw;
        }
    }

    public async Task UpdateAsync(Staff staff, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Staff.Update(staff);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating staff with ID {StaffId}", staff.StaffId);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var staff = await _context.Staff.FindAsync(new object[] { id }, cancellationToken);
            if (staff != null)
            {
                staff.IsActive = false;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff with ID {StaffId}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Staff.AnyAsync(s => s.StaffId == id && s.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Staff>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Staff
                .AsNoTracking()
                .Where(s => s.IsActive && s.Name.Contains(searchQuery))
                .OrderBy(s => s.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching staff with query '{SearchQuery}'", searchQuery);
            throw;
        }
    }
}

/// <summary>
/// EF Core implementation of IBillRepository.
/// </summary>
public class BillRepository : IBillRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<BillRepository> _logger;

    public BillRepository(ClinicDbContext context, ILogger<BillRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Bill>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bills
                .AsNoTracking()
                .Include(b => b.Patient)
                .Include(b => b.Doctor)
                .Include(b => b.Appointment)
                .OrderByDescending(b => b.BillDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all bills");
            throw;
        }
    }

    public async Task<Bill?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bills
                .AsNoTracking()
                .Include(b => b.Patient)
                .Include(b => b.Doctor)
                .Include(b => b.Appointment)
                .FirstOrDefaultAsync(b => b.BillId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bill with ID {BillId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Bill>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bills
                .AsNoTracking()
                .Include(b => b.Doctor)
                .Include(b => b.Appointment)
                    .ThenInclude(a => a!.TimeSlot)
                .Where(b => b.PatientId == patientId)
                .OrderByDescending(b => b.BillDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bills for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<IEnumerable<Bill>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bills
                .AsNoTracking()
                .Include(b => b.Patient)
                .Include(b => b.Appointment)
                    .ThenInclude(a => a!.TimeSlot)
                .Where(b => b.DoctorId == doctorId)
                .OrderByDescending(b => b.BillDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bills for doctor {DoctorId}", doctorId);
            throw;
        }
    }

    public async Task<Bill> AddAsync(Bill bill, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync(cancellationToken);
            return bill;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding bill");
            throw;
        }
    }

    public async Task UpdateAsync(Bill bill, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Bills.Update(bill);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating bill with ID {BillId}", bill.BillId);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Bills.AnyAsync(b => b.BillId == id, cancellationToken);
    }

    public async Task<decimal> GetTotalIncomeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bills
                .Where(b => b.IsPaid)
                .SumAsync(b => b.Amount, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total income");
            throw;
        }
    }
}
