using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>
/// Take appointment page model for patients.
/// </summary>
public class TakeAppointmentModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly IDoctorService _doctorService;
    private readonly ILogger<TakeAppointmentModel> _logger;

    public TakeAppointmentModel(IAppointmentService appointmentService, IDoctorService doctorService, ILogger<TakeAppointmentModel> logger)
    {
        _appointmentService = appointmentService;
        _doctorService = doctorService;
        _logger = logger;
    }

    [BindProperty]
    public TakeAppointmentInputModel Input { get; set; } = new();

    public List<SelectListItem> DoctorOptions { get; set; } = new();
    public IEnumerable<TimeSlotDto> FreeSlots { get; set; } = new List<TimeSlotDto>();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public class TakeAppointmentInputModel
    {
        public int DoctorId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a time slot")]
        public int TimeSlotId { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int? doctorId = null)
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 2)
            return RedirectToPage("/Account/Login");

        await LoadDoctorsAsync();

        if (doctorId.HasValue)
        {
            Input.DoctorId = doctorId.Value;
            await LoadFreeSlotsAsync(doctorId.Value);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 2)
            return RedirectToPage("/Account/Login");

        await LoadDoctorsAsync();
        if (Input.DoctorId > 0)
            await LoadFreeSlotsAsync(Input.DoctorId);

        return Page();
    }

    public async Task<IActionResult> OnPostBookAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 2)
            return RedirectToPage("/Account/Login");

        if (!ModelState.IsValid || Input.DoctorId <= 0 || Input.TimeSlotId <= 0)
        {
            await LoadDoctorsAsync();
            if (Input.DoctorId > 0)
                await LoadFreeSlotsAsync(Input.DoctorId);
            return Page();
        }

        var patientId = HttpContext.Session.GetInt32("UserId")!.Value;
        try
        {
            // Manually map ViewModel to DTO
            var createDto = new AppointmentCreateDto
            {
                DoctorId = Input.DoctorId,
                PatientId = patientId,
                TimeSlotId = Input.TimeSlotId
            };

            await _appointmentService.CreateAsync(createDto);
            SuccessMessage = "Appointment request sent successfully! Please wait for doctor approval.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment");
            ErrorMessage = "An error occurred. Please try again.";
        }

        await LoadDoctorsAsync();
        return Page();
    }

    private async Task LoadDoctorsAsync()
    {
        var doctors = await _doctorService.GetAllAsync();
        DoctorOptions = doctors.Select(d => new SelectListItem
        {
            Value = d.DoctorId.ToString(),
            Text = $"Dr. {d.Name} - {d.DepartmentName}",
            Selected = d.DoctorId == Input.DoctorId
        }).ToList();
    }

    private async Task LoadFreeSlotsAsync(int doctorId)
    {
        var patientId = HttpContext.Session.GetInt32("UserId")!.Value;
        FreeSlots = await _appointmentService.GetFreeSlotsAsync(doctorId, patientId);
    }
}
