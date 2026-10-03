using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Exceptions;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for staff-related operations.
/// </summary>
public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<StaffService> _logger;

    public StaffService(IStaffRepository staffRepository, IMapper mapper, ILogger<StaffService> logger)
    {
        _staffRepository = staffRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<StaffDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var staff = await _staffRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<StaffDto>>(staff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all staff");
            throw;
        }
    }

    public async Task<StaffDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var staff = await _staffRepository.GetByIdAsync(id, cancellationToken);
            return staff == null ? null : _mapper.Map<StaffDto>(staff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff with ID {StaffId}", id);
            throw;
        }
    }

    public async Task<StaffDto> CreateAsync(StaffCreateDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new staff member {Name}", createDto.Name);
            var staff = _mapper.Map<Staff>(createDto);
            var created = await _staffRepository.AddAsync(staff, cancellationToken);
            return _mapper.Map<StaffDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating staff member");
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting staff with ID {StaffId}", id);
            if (!await _staffRepository.ExistsAsync(id, cancellationToken))
                throw new NotFoundException(nameof(Staff), id);

            await _staffRepository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff with ID {StaffId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<StaffDto>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            var staff = await _staffRepository.SearchAsync(searchQuery, cancellationToken);
            return _mapper.Map<IEnumerable<StaffDto>>(staff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching staff");
            throw;
        }
    }
}

/// <summary>
/// Service for department-related operations.
/// </summary>
public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DepartmentService> _logger;

    public DepartmentService(IDepartmentRepository departmentRepository, IMapper mapper, ILogger<DepartmentService> logger)
    {
        _departmentRepository = departmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all departments");
            throw;
        }
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var dept = await _departmentRepository.GetByIdAsync(id, cancellationToken);
            return dept == null ? null : _mapper.Map<DepartmentDto>(dept);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department with ID {DepartmentId}", id);
            throw;
        }
    }
}

/// <summary>
/// Service for bill-related operations.
/// </summary>
public class BillService : IBillService
{
    private readonly IBillRepository _billRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<BillService> _logger;

    public BillService(IBillRepository billRepository, IAppointmentRepository appointmentRepository, IMapper mapper, ILogger<BillService> logger)
    {
        _billRepository = billRepository;
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<BillDto>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var bills = await _billRepository.GetByPatientIdAsync(patientId, cancellationToken);
            return _mapper.Map<IEnumerable<BillDto>>(bills);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bills for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<IEnumerable<BillDto>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var bills = await _billRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
            return _mapper.Map<IEnumerable<BillDto>>(bills);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bills for doctor {DoctorId}", doctorId);
            throw;
        }
    }

    public async Task MarkAsPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking appointment {AppointmentId} bill as paid", appointmentId);
            var bills = await _billRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
            var bill = bills.FirstOrDefault(b => b.AppointmentId == appointmentId);
            if (bill != null)
            {
                bill.Status = Domain.Enums.BillStatus.Paid;
                await _billRepository.UpdateAsync(bill, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as paid for appointment {AppointmentId}", appointmentId);
            throw;
        }
    }

    public async Task MarkAsUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking appointment {AppointmentId} bill as unpaid", appointmentId);
            var bills = await _billRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
            var bill = bills.FirstOrDefault(b => b.AppointmentId == appointmentId);
            if (bill != null)
            {
                bill.Status = Domain.Enums.BillStatus.Unpaid;
                await _billRepository.UpdateAsync(bill, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as unpaid for appointment {AppointmentId}", appointmentId);
            throw;
        }
    }
}

/// <summary>
/// Service for admin dashboard operations.
/// </summary>
public class AdminService : IAdminService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IBillRepository _billRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IBillRepository billRepository,
        IDepartmentRepository departmentRepository,
        IAppointmentRepository appointmentRepository,
        IMapper mapper,
        ILogger<AdminService> logger)
    {
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _billRepository = billRepository;
        _departmentRepository = departmentRepository;
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AdminDashboardDto> GetDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving admin dashboard data");
            var doctors = await _doctorRepository.GetAllAsync(cancellationToken);
            var patients = await _patientRepository.GetAllAsync(cancellationToken);
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            var appointments = await _appointmentRepository.GetAllAsync(cancellationToken);
            var bills = new List<Domain.Entities.Bill>();

            foreach (var doctor in doctors)
            {
                var doctorBills = await _billRepository.GetByDoctorIdAsync(doctor.DoctorId, cancellationToken);
                bills.AddRange(doctorBills);
            }

            return new AdminDashboardDto
            {
                TotalDoctors = doctors.Count(),
                TotalPatients = patients.Count(),
                TotalIncome = bills.Where(b => b.Status == Domain.Enums.BillStatus.Paid).Sum(b => b.Amount),
                Departments = _mapper.Map<IEnumerable<DepartmentDto>>(departments),
                RecentAppointments = _mapper.Map<IEnumerable<AppointmentDto>>(appointments.Take(10))
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving admin dashboard data");
            throw;
        }
    }
}
