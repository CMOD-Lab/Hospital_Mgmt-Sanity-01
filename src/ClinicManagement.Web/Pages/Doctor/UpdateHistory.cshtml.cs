using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>
/// Update patient history page model.
/// </summary>
public class UpdateHistoryModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<UpdateHistoryModel> _logger;

    public UpdateHistoryModel(IAppointmentService appointmentService, ILogger<UpdateHistoryModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public int AppointmentId { get; set; }

    [BindProperty]
    public UpdateHistoryInputModel Input { get; set; } = new();

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public class UpdateHistoryInputModel
    {
        [MaxLength(30)] public string? Disease { get; set; }
        [MaxLength(50)] public string? Progress { get; set; }
        [MaxLength(60)] public string? Prescription { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 1)
            return RedirectToPage("/Account/Login");

        AppointmentId = id;
        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment != null)
        {
            Input.Disease = appointment.Disease;
            Input.Progress = appointment.Progress;
            Input.Prescription = appointment.Prescription;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 1)
            return RedirectToPage("/Account/Login");

        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Manually map ViewModel to DTO
            var updateDto = new AppointmentUpdateDto
            {
                Disease = Input.Disease,
                Progress = Input.Progress,
                Prescription = Input.Prescription
            };

            await _appointmentService.UpdatePrescriptionAsync(AppointmentId, updateDto);
            SuccessMessage = "Patient history updated successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating history for appointment {AppointmentId}", AppointmentId);
            ErrorMessage = "An error occurred. Please try again.";
        }

        return Page();
    }
}
