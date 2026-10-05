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
using StaffEntity = ClinicManagement.Domain.Entities.OtherStaff;

namespace ClinicManagement.UnitTests.Pages.Admin;

/// <summary>
/// Unit tests for ManageClinicModel page model.
/// </summary>
public class ManageClinicModelTests
{
    private readonly Mock<IDoctorRepository> _doctorRepoMock;
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<IStaffRepository> _staffRepoMock;
    private readonly Mock<IDepartmentRepository> _departmentRepoMock;
    private readonly Mock<ILoginRepository> _loginRepoMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<ILogger<AdminService>> _adminLoggerMock;
    private readonly Mock<ILogger<DoctorService>> _doctorLoggerMock;
    private readonly Mock<ILogger<PatientService>> _patientLoggerMock;
    private readonly AdminService _adminService;
    private readonly DoctorService _doctorService;
    private readonly PatientService _patientService;

    public ManageClinicModelTests()
    {
        _doctorRepoMock = new Mock<IDoctorRepository>();
        _patientRepoMock = new Mock<IPatientRepository>();
        _staffRepoMock = new Mock<IStaffRepository>();
        _departmentRepoMock = new Mock<IDepartmentRepository>();
        _loginRepoMock = new Mock<ILoginRepository>();
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _adminLoggerMock = new Mock<ILogger<AdminService>>();
        _doctorLoggerMock = new Mock<ILogger<DoctorService>>();
        _patientLoggerMock = new Mock<ILogger<PatientService>>();

        _adminService = new AdminService(
            _doctorRepoMock.Object,
            _patientRepoMock.Object,
            _staffRepoMock.Object,
            _departmentRepoMock.Object,
            _loginRepoMock.Object,
            _appointmentRepoMock.Object,
            _adminLoggerMock.Object);

        _doctorService = new DoctorService(
            _doctorRepoMock.Object,
            _appointmentRepoMock.Object,
            _departmentRepoMock.Object,
            _loginRepoMock.Object,
            _doctorLoggerMock.Object);

        _patientService = new PatientService(
            _patientRepoMock.Object,
            _appointmentRepoMock.Object,
            _patientLoggerMock.Object);
    }

    private ManageClinicModel CreatePageModel(int? sessionUserType = 3)
    {
        var model = new ManageClinicModel(_adminService, _doctorService, _patientService);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockSession();
        if (sessionUserType.HasValue)
        {
            ((MockSession)httpContext.Session).SetInt32("UserType", sessionUserType.Value);
        }
        model.PageContext = new PageContext { HttpContext = httpContext };
        return model;
    }

    [Fact]
    public async Task OnGetAsync_WhenUserIsNotAdmin_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(null, null, null, CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnGetAsync_WhenAdminLoggedIn_ReturnsAllData()
    {
        // Arrange
        var doctors = new List<DoctorEntity> { new() { DoctorId = 1, Name = "Dr. Smith", Status = 1, BirthDate = DateTime.Today, Gender = 'M', ChargesPerVisit = 100, Qualification = "MBBS" } };
        var patients = new List<PatientEntity> { new() { PatientId = 1, Name = "John Doe", BirthDate = DateTime.Today, Gender = 'M' } };
        var staff = new List<StaffEntity> { new() { StaffId = 1, Name = "Nurse Jane", Gender = 'F', Designation = "Nurse" } };

        _doctorRepoMock.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctors);
        _patientRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(patients);
        _staffRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(staff);

        var model = CreatePageModel(3);

        // Act
        var result = await model.OnGetAsync(null, null, null, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Doctors.Should().HaveCount(1);
        model.Patients.Should().HaveCount(1);
        model.Staff.Should().HaveCount(1);
    }

    [Fact]
    public async Task OnGetAsync_WithSearchQueries_SetsSearchProperties()
    {
        // Arrange
        _doctorRepoMock.Setup(r => r.SearchAsync("Smith", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorEntity>());
        _patientRepoMock.Setup(r => r.SearchAsync("John", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PatientEntity>());
        _staffRepoMock.Setup(r => r.SearchAsync("Nurse", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StaffEntity>());

        var model = CreatePageModel(3);

        // Act
        var result = await model.OnGetAsync("Smith", "John", "Nurse", CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.DoctorSearch.Should().Be("Smith");
        model.PatientSearch.Should().Be("John");
        model.StaffSearch.Should().Be("Nurse");
    }

    [Fact]
    public async Task OnPostDeleteDoctorAsync_WhenUserIsNotAdmin_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(1);

        // Act
        var result = await model.OnPostDeleteDoctorAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnPostDeleteDoctorAsync_WhenDeleteSucceeds_SetsSuccessMessage()
    {
        // Arrange
        _doctorRepoMock.Setup(r => r.SoftDeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _doctorRepoMock.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorEntity>());
        _patientRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PatientEntity>());
        _staffRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StaffEntity>());

        var model = CreatePageModel(3);

        // Act
        var result = await model.OnPostDeleteDoctorAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeTrue();
        model.Message.Should().Contain("removed");
    }

    [Fact]
    public async Task OnPostDeleteStaffAsync_WhenUserIsNotAdmin_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(1);

        // Act
        var result = await model.OnPostDeleteStaffAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnPostDeleteStaffAsync_WhenDeleteSucceeds_SetsSuccessMessage()
    {
        // Arrange
        _staffRepoMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _doctorRepoMock.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorEntity>());
        _patientRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PatientEntity>());
        _staffRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StaffEntity>());

        var model = CreatePageModel(3);

        // Act
        var result = await model.OnPostDeleteStaffAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeTrue();
        model.Message.Should().Contain("removed");
    }

    [Fact]
    public void ManageClinicModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(3);

        // Assert
        model.Doctors.Should().NotBeNull();
        model.Patients.Should().NotBeNull();
        model.Staff.Should().NotBeNull();
        model.DoctorSearch.Should().BeEmpty();
        model.PatientSearch.Should().BeEmpty();
        model.StaffSearch.Should().BeEmpty();
        model.Message.Should().BeEmpty();
        model.Success.Should().BeFalse();
    }
}
