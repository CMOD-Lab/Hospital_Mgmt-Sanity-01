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
using DepartmentEntity = ClinicManagement.Domain.Entities.Department;
using LoginTableEntity = ClinicManagement.Domain.Entities.LoginTable;

namespace ClinicManagement.UnitTests.Pages.Admin;

/// <summary>
/// Unit tests for DoctorRegistrationFormModel page model.
/// </summary>
public class DoctorRegistrationFormModelTests
{
    private readonly Mock<IDoctorRepository> _doctorRepoMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IDepartmentRepository> _departmentRepoMock;
    private readonly Mock<ILoginRepository> _loginRepoMock;
    private readonly Mock<ILogger<DoctorService>> _doctorLoggerMock;
    private readonly Mock<ILogger<AppointmentService>> _appointmentLoggerMock;
    private readonly DoctorService _doctorService;
    private readonly AppointmentService _appointmentService;

    public DoctorRegistrationFormModelTests()
    {
        _doctorRepoMock = new Mock<IDoctorRepository>();
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _departmentRepoMock = new Mock<IDepartmentRepository>();
        _loginRepoMock = new Mock<ILoginRepository>();
        _doctorLoggerMock = new Mock<ILogger<DoctorService>>();
        _appointmentLoggerMock = new Mock<ILogger<AppointmentService>>();

        _doctorService = new DoctorService(
            _doctorRepoMock.Object,
            _appointmentRepoMock.Object,
            _departmentRepoMock.Object,
            _loginRepoMock.Object,
            _doctorLoggerMock.Object);

        _appointmentService = new AppointmentService(
            _appointmentRepoMock.Object,
            _doctorRepoMock.Object,
            _departmentRepoMock.Object,
            _appointmentLoggerMock.Object);
    }

    private DoctorRegistrationFormModel CreatePageModel(int? sessionUserType = 3)
    {
        var model = new DoctorRegistrationFormModel(_doctorService, _appointmentService);
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
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnGetAsync_WhenAdminLoggedIn_ReturnsDepartments()
    {
        // Arrange
        var departments = new List<DepartmentEntity>
        {
            new() { DeptNo = 1, DeptName = "Cardiology" },
            new() { DeptNo = 2, DeptName = "Neurology" }
        };
        _departmentRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(departments);

        var model = CreatePageModel(3);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Departments.Should().HaveCount(2);
    }

    [Fact]
    public async Task OnPostAsync_WhenUserIsNotAdmin_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(1);

        // Act
        var result = await model.OnPostAsync(
            "Dr. Smith", "smith@test.com", "pass", "1975-01-01",
            1, "1234567890", "M", "Address", 10, 80000, 200, "Cardiology", "MBBS",
            CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnPostAsync_WhenAddDoctorSucceeds_SetsSuccessMessage()
    {
        // Arrange
        _doctorRepoMock.Setup(r => r.EmailExistsAsync("newdoc@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _loginRepoMock.Setup(r => r.AddAsync(It.IsAny<LoginTableEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);
        _doctorRepoMock.Setup(r => r.AddAsync(It.IsAny<DoctorEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _departmentRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DepartmentEntity>());

        var model = CreatePageModel(3);

        // Act
        var result = await model.OnPostAsync(
            "Dr. New", "newdoc@test.com", "pass123", "1975-06-15",
            1, "9876543210", "M", "123 Medical St", 5, 90000, 250, "General Medicine", "MBBS MD",
            CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeTrue();
        model.Message.Should().Contain("successfully");
    }

    [Fact]
    public async Task OnPostAsync_WhenEmailAlreadyExists_SetsFailureMessage()
    {
        // Arrange
        _doctorRepoMock.Setup(r => r.EmailExistsAsync("existing@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _departmentRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DepartmentEntity>());

        var model = CreatePageModel(3);

        // Act
        var result = await model.OnPostAsync(
            "Dr. Existing", "existing@test.com", "pass", "1975-01-01",
            1, "1234567890", "M", "Address", 5, 80000, 200, "Cardiology", "MBBS",
            CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeFalse();
        model.Message.Should().Contain("Failed");
    }

    [Fact]
    public async Task OnPostAsync_WithEmptyGender_UsesDefaultMaleGender()
    {
        // Arrange
        _doctorRepoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _loginRepoMock.Setup(r => r.AddAsync(It.IsAny<LoginTableEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(11);
        _doctorRepoMock.Setup(r => r.AddAsync(It.IsAny<DoctorEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _departmentRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DepartmentEntity>());

        var model = CreatePageModel(3);

        // Act
        var result = await model.OnPostAsync(
            "Dr. Alex", "alex@test.com", "pass", "1980-01-01",
            1, "1234567890", "", "Address", 3, 70000, 150, "Neurology", "MBBS",
            CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeTrue();
    }

    [Fact]
    public void DoctorRegistrationFormModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(3);

        // Assert
        model.Departments.Should().NotBeNull();
        model.Departments.Should().BeEmpty();
        model.Message.Should().BeEmpty();
        model.Success.Should().BeFalse();
    }
}
