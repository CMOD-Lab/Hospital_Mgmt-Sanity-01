using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.UnitTests.Helpers;
using ClinicManagement.Web.Pages.Admin;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using DoctorEntity = ClinicManagement.Domain.Entities.Doctor;
using PatientEntity = ClinicManagement.Domain.Entities.Patient;
using DepartmentEntity = ClinicManagement.Domain.Entities.Department;

namespace ClinicManagement.UnitTests.Pages.Admin;

/// <summary>
/// Unit tests for AdminHomeModel page model.
/// </summary>
public class AdminHomeModelTests
{
    private readonly Mock<IDoctorRepository> _doctorRepoMock;
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<IStaffRepository> _staffRepoMock;
    private readonly Mock<IDepartmentRepository> _departmentRepoMock;
    private readonly Mock<ILoginRepository> _loginRepoMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<ILogger<AdminService>> _loggerMock;
    private readonly AdminService _adminService;

    public AdminHomeModelTests()
    {
        _doctorRepoMock = new Mock<IDoctorRepository>();
        _patientRepoMock = new Mock<IPatientRepository>();
        _staffRepoMock = new Mock<IStaffRepository>();
        _departmentRepoMock = new Mock<IDepartmentRepository>();
        _loginRepoMock = new Mock<ILoginRepository>();
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _loggerMock = new Mock<ILogger<AdminService>>();
        _adminService = new AdminService(
            _doctorRepoMock.Object,
            _patientRepoMock.Object,
            _staffRepoMock.Object,
            _departmentRepoMock.Object,
            _loginRepoMock.Object,
            _appointmentRepoMock.Object,
            _loggerMock.Object);
    }

    private AdminHomeModel CreatePageModel(int? sessionUserId = 3, int? sessionUserType = 3)
    {
        var model = new AdminHomeModel(_adminService);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockSession();
        if (sessionUserId.HasValue)
        {
            ((MockSession)httpContext.Session).SetInt32("UserId", sessionUserId.Value);
        }
        if (sessionUserType.HasValue)
        {
            ((MockSession)httpContext.Session).SetInt32("UserType", sessionUserType.Value);
        }
        model.PageContext = new PageContext { HttpContext = httpContext };
        return model;
    }

    [Fact]
    public async Task OnGetAsync_WhenUserNotLoggedIn_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(null, null);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnGetAsync_WhenUserIsNotAdmin_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(1, 1); // Patient type

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnGetAsync_WhenAdminLoggedIn_ReturnsAdminStats()
    {
        // Arrange
        var patients = new List<PatientEntity> { new() { PatientId = 1, BirthDate = DateTime.Today, Gender = 'M' }, new() { PatientId = 2, BirthDate = DateTime.Today, Gender = 'F' } };
        var doctors = new List<DoctorEntity> { new() { DoctorId = 1, Status = 1, BirthDate = DateTime.Today, Gender = 'M', ChargesPerVisit = 100, Qualification = "MBBS" } };
        var departments = new List<DepartmentEntity> { new() { DeptNo = 1, DeptName = "Cardiology", Doctors = new List<DoctorEntity>() } };

        _patientRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(patients);
        _doctorRepoMock.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctors);
        _departmentRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(departments);

        var model = CreatePageModel(3, 3);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Stats.Should().NotBeNull();
        model.Stats.TotalPatients.Should().Be(2);
        model.Stats.TotalDoctors.Should().Be(1);
    }

    [Fact]
    public void AdminHomeModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(3, 3);

        // Assert
        model.Stats.Should().NotBeNull();
    }
}
