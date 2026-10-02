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

    public async Task<LoginTable?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        try
        {
            return await _context.LoginTable.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting login by email {Email}", email);
            return null;
        }
    }

    public async Task<LoginTable?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _context.LoginTable.AsNoTracking()
                .FirstOrDefaultAsync(x => x.LoginId == id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting login by id {Id}", id);
            return null;
        }
    }

    public async Task<LoginTable> AddAsync(LoginTable login, CancellationToken ct = default)
    {
        _context.LoginTable.Add(login);
        await _context.SaveChangesAsync(ct);
        return login;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        return await _context.LoginTable.AnyAsync(x => x.Email == email, ct);
    }
}

/// <summary>EF Core implementation of IPatientRepository.</summary>
public class PatientRepository : IPatientRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<PatientRepository> _logger;

    public PatientRepository(ClinicDbContext context, ILogger<PatientRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Patient?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Patients.AsNoTracking()
            .FirstOrDefaultAsync(x => x.PatientId == id, ct);
    }

    public async Task<IEnumerable<Patient>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Patients.AsNoTracking().ToListAsync(ct);
    }

    public async Task<IEnumerable<Patient>> SearchAsync(string query, CancellationToken ct = default)
    {
        return await _context.Patients.AsNoTracking()
            .Where(x => x.Name.Contains(query))
            .Select(x => new Patient { PatientId = x.PatientId, Name = x.Name, Phone = x.Phone })
            .ToListAsync(ct);
    }

    public async Task<Patient> AddAsync(Patient patient, CancellationToken ct = default)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync(ct);
        return patient;
    }

    public async Task UpdateAsync(Patient patient, CancellationToken ct = default)
    {
        _context.Patients.Update(patient);
        await _context.SaveChangesAsync(ct);
    }
}

/// <summary>EF Core implementation of IDoctorRepository.</summary>
public class DoctorRepository : IDoctorRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<DoctorRepository> _logger;

    public DoctorRepository(ClinicDbContext context, ILogger<DoctorRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Doctor?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Doctors.AsNoTracking()
            .Include(x => x.Department)
            .FirstOrDefaultAsync(x => x.DoctorId == id, ct);
    }

    public async Task<IEnumerable<Doctor>> GetAllActiveAsync(CancellationToken ct = default)
    {
        return await _context.Doctors.AsNoTracking()
            .Include(x => x.Department)
            .Where(x => x.Status == Domain.Enums.DoctorStatus.Present)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Doctor>> SearchAsync(string query, CancellationToken ct = default)
    {
        return await _context.Doctors.AsNoTracking()
            .Include(x => x.Department)
            .Where(x => x.Status == Domain.Enums.DoctorStatus.Present && x.Name.Contains(query))
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Doctor>> GetByDepartmentAsync(string deptName, CancellationToken ct = default)
    {
        return await _context.Doctors.AsNoTracking()
            .Include(x => x.Department)
            .Where(x => x.Status == Domain.Enums.DoctorStatus.Present && x.Department != null && x.Department.DeptName == deptName)
            .ToListAsync(ct);
    }

    public async Task<Doctor> AddAsync(Doctor doctor, CancellationToken ct = default)
    {
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync(ct);
        return doctor;
    }

    public async Task UpdateAsync(Doctor doctor, CancellationToken ct = default)
    {
        _context.Doctors.Update(doctor);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        return await _context.LoginTable.AnyAsync(x => x.Email == email, ct);
    }

    public async Task SoftDeleteAsync(int id, CancellationToken ct = default)
    {
        var doctor = await _context.Doctors.FindAsync(new object[] { id }, ct);
        if (doctor != null)
        {
            doctor.Status = Domain.Enums.DoctorStatus.Left;
            await _context.SaveChangesAsync(ct);
        }
    }
}

/// <summary>EF Core implementation of IDepartmentRepository.</summary>
public class DepartmentRepository : IDepartmentRepository
{
    private readonly ClinicDbContext _context;

    public DepartmentRepository(ClinicDbContext context) => _context = context;

    public async Task<IEnumerable<Department>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Departments.AsNoTracking()
            .Include(x => x.Doctors.Where(d => d.Status == Domain.Enums.DoctorStatus.Present))
            .ToListAsync(ct);
    }

    public async Task<Department?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Departments.AsNoTracking()
            .FirstOrDefaultAsync(x => x.DeptNo == id, ct);
    }

    public async Task<Department?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.Departments.AsNoTracking()
            .FirstOrDefaultAsync(x => x.DeptName == name, ct);
    }
}

/// <summary>EF Core implementation of IStaffRepository.</summary>
public class StaffRepository : IStaffRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<StaffRepository> _logger;

    public StaffRepository(ClinicDbContext context, ILogger<StaffRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OtherStaff?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.OtherStaff.AsNoTracking()
            .FirstOrDefaultAsync(x => x.StaffId == id, ct);
    }

    public async Task<IEnumerable<OtherStaff>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.OtherStaff.AsNoTracking().ToListAsync(ct);
    }

    public async Task<IEnumerable<OtherStaff>> SearchAsync(string query, CancellationToken ct = default)
    {
        return await _context.OtherStaff.AsNoTracking()
            .Where(x => x.Name.Contains(query))
            .ToListAsync(ct);
    }

    public async Task<OtherStaff> AddAsync(OtherStaff staff, CancellationToken ct = default)
    {
        _context.OtherStaff.Add(staff);
        await _context.SaveChangesAsync(ct);
        return staff;
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var staff = await _context.OtherStaff.FindAsync(new object[] { id }, ct);
        if (staff != null)
        {
            _context.OtherStaff.Remove(staff);
            await _context.SaveChangesAsync(ct);
        }
    }
}

/// <summary>EF Core implementation of IAppointmentRepository.</summary>
public class AppointmentRepository : IAppointmentRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<AppointmentRepository> _logger;

    public AppointmentRepository(ClinicDbContext context, ILogger<AppointmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Appointment?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Appointments
            .Include(x => x.Doctor)
            .Include(x => x.Patient)
            .FirstOrDefaultAsync(x => x.AppointId == id, ct);
    }

    public async Task<IEnumerable<Appointment>> GetPendingByDoctorAsync(int doctorId, CancellationToken ct = default)
    {
        var query = _context.Appointments.AsNoTracking()
            .Include(x => x.Patient)
            .Include(x => x.Doctor)
            .Where(x => x.AppointmentStatus == Domain.Enums.AppointmentStatus.Pending);

        if (doctorId > 0)
            query = query.Where(x => x.DoctorId == doctorId);

        return await query.ToListAsync(ct);
    }

    public async Task<IEnumerable<Appointment>> GetTodaysByDoctorAsync(int doctorId, CancellationToken ct = default)
    {
        var today = DateTime.Today;
        return await _context.Appointments.AsNoTracking()
            .Include(x => x.Patient)
            .Where(x => x.DoctorId == doctorId
                && x.AppointmentStatus == Domain.Enums.AppointmentStatus.Approved
                && x.Date.HasValue && x.Date.Value.Date == today)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Appointment>> GetBillsByDoctorAsync(int doctorId, CancellationToken ct = default)
    {
        return await _context.Appointments.AsNoTracking()
            .Include(x => x.Patient)
            .Where(x => x.DoctorId == doctorId
                && x.AppointmentStatus == Domain.Enums.AppointmentStatus.Approved)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Appointment>> GetHistoryByPatientAsync(int patientId, CancellationToken ct = default)
    {
        return await _context.Appointments.AsNoTracking()
            .Include(x => x.Doctor)
            .Where(x => x.PatientId == patientId
                && x.AppointmentStatus == Domain.Enums.AppointmentStatus.Completed)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Appointment>> GetBillHistoryByPatientAsync(int patientId, CancellationToken ct = default)
    {
        return await _context.Appointments.AsNoTracking()
            .Include(x => x.Doctor)
            .Where(x => x.PatientId == patientId && x.BillAmount.HasValue)
            .ToListAsync(ct);
    }

    public async Task<Appointment?> GetCurrentByPatientAsync(int patientId, CancellationToken ct = default)
    {
        var today = DateTime.Today;
        return await _context.Appointments.AsNoTracking()
            .Include(x => x.Doctor)
            .FirstOrDefaultAsync(x => x.PatientId == patientId
                && x.AppointmentStatus == Domain.Enums.AppointmentStatus.Approved
                && x.Date.HasValue && x.Date.Value.Date == today, ct);
    }

    public async Task<Appointment?> GetPendingFeedbackByPatientAsync(int patientId, CancellationToken ct = default)
    {
        return await _context.Appointments.AsNoTracking()
            .Include(x => x.Doctor)
            .FirstOrDefaultAsync(x => x.PatientId == patientId
                && x.FeedbackStatus == Domain.Enums.FeedbackStatus.Pending
                && x.AppointmentStatus == Domain.Enums.AppointmentStatus.Completed, ct);
    }

    public async Task<IEnumerable<Appointment>> GetNotificationsByPatientAsync(int patientId, CancellationToken ct = default)
    {
        return await _context.Appointments.AsNoTracking()
            .Include(x => x.Doctor)
            .Where(x => x.PatientId == patientId
                && x.PatientNotification == Domain.Enums.NotificationStatus.Unseen)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Appointment>> GetFreeSlotsByDoctorAsync(int doctorId, int patientId, CancellationToken ct = default)
    {
        // Return available appointment slots (pending appointments for this doctor that don't belong to this patient)
        return await _context.Appointments.AsNoTracking()
            .Where(x => x.DoctorId == doctorId
                && x.AppointmentStatus == Domain.Enums.AppointmentStatus.Pending
                && x.PatientId != patientId)
            .ToListAsync(ct);
    }

    public async Task<Appointment> AddAsync(Appointment appointment, CancellationToken ct = default)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync(ct);
        return appointment;
    }

    public async Task UpdateAsync(Appointment appointment, CancellationToken ct = default)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var appointment = await _context.Appointments.FindAsync(new object[] { id }, ct);
        if (appointment != null)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<int> GetTotalPatientsCountAsync(CancellationToken ct = default)
    {
        return await _context.Patients.CountAsync(ct);
    }

    public async Task<int> GetTotalDoctorsCountAsync(CancellationToken ct = default)
    {
        return await _context.Doctors.CountAsync(x => x.Status == Domain.Enums.DoctorStatus.Present, ct);
    }

    public async Task<double> GetTotalIncomeAsync(CancellationToken ct = default)
    {
        return await _context.Appointments
            .Where(x => x.BillStatus == Domain.Enums.BillStatus.Paid && x.BillAmount.HasValue)
            .SumAsync(x => x.BillAmount!.Value, ct);
    }
}
