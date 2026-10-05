using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

public class DoctorRegistrationFormModel : PageModel
{
    private readonly DoctorService _doctorService;
    private readonly AppointmentService _appointmentService;

    public IEnumerable<DepartmentDto> Departments { get; set; } = Enumerable.Empty<DepartmentDto>();
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public DoctorRegistrationFormModel(DoctorService doctorService, AppointmentService appointmentService)
    {
        _doctorService = doctorService;
        _appointmentService = appointmentService;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        Departments = await _appointmentService.GetDepartmentsAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string name, string email, string password, string birthDate,
        int deptNo, string phone, string gender, string address,
        int workExperience, int salary, int chargesPerVisit,
        string specialization, string qualification,
        CancellationToken cancellationToken)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        var dto = new AddDoctorDto
        {
            Name = name,
            Email = email,
            Password = password,
            BirthDate = birthDate,
            DeptNo = deptNo,
            Phone = phone,
            Gender = string.IsNullOrEmpty(gender) ? 'M' : gender[0],
            Address = address,
            WorkExperience = workExperience,
            Salary = salary,
            ChargesPerVisit = chargesPerVisit,
            Specialization = specialization,
            Qualification = qualification
        };

        var result = await _doctorService.AddDoctorAsync(dto, cancellationToken);
        Success = result;
        Message = result ? "Doctor registered successfully!" : "Failed to register doctor. Email may already exist.";

        Departments = await _appointmentService.GetDepartmentsAsync(cancellationToken);
        return Page();
    }
}
