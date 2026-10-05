using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.UnitTests.Helpers;
using ClinicManagement.Web.Pages.Patient;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using DoctorEntity = ClinicManagement.Domain.Entities.Doctor;
using DepartmentEntity = ClinicManagement.Domain.Entities.Department;

namespace ClinicManagement.UnitTests.Pages.Patient;

/// <summary>
/// Unit tests for TakeAppointmentModel page model.
/// </summary>
public class TakeAppointmentModelTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IDoctorRepository> _doctorRepoMock;
    private readonly Mock<IDepartmentRepository> _departmentRepoMock;
    private readonly Mock<ILogger<AppointmentService>> _appointmentLoggerMock;
    private readonly Mock<ILogger<DoctorService>> _doctorLoggerMock;
    private readonly Mock<ILoginRepository> _loginRepoMock;
    private readonly AppointmentService _appointmentService;
    private readonly DoctorService _doctorService;

    public TakeAppointmentModelTests()
    {
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _doctorRepoMock = new Mock<IDoctorRepository>();
        _departmentRepoMock = new Mock<IDepartmentRepository>();
        _appointmentLoggerMock = new Mock<ILogger<AppointmentService>>();
        _doctorLoggerMock = new Mock<ILogger<DoctorService>>();
        _loginRepoMock = new Mock<ILoginRepository>();
        _appointmentService = new AppointmentService(
            _appointmentRepoMock.Object,
            _doctorRepoMock.Object,
            _departmentRepoMock.Object,
            _appointmentLoggerMock.Object);
        _doctorService = new DoctorService(
            _doctorRepoMock.Object,
            _appointmentRepoMock.Object,
            _departmentRepoMock.Object,
            _loginRepoMock.Object,
            _doctorLoggerMock.Object);
    }

    private TakeAppointmentModel CreatePageModel(int? sessionUserId = 1)
    {
        var model = new TakeAppointmentModel(_appointmentService, _doctorService);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockSession();
        if (sessionUserId.HasValue)
        {
            ((MockSession)httpContext.Session).SetInt32("UserId", sessionUserId.Value);
        }
        model.PageContext = new PageContext { HttpContext = httpContext };
        return model;
    }

    [Fact]
    public async Task OnGetAsync_WhenUserNotLoggedIn_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(null);

        // Act
        var result = await model.OnGetAsync(null, CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnGetAsync_WithNoDeptName_ReturnsDepartmentsOnly()
    {
        // Arrange
        var departments = new List<DepartmentEntity>
        {
            new() { DeptNo = 1, DeptName = "Cardiology" },
            new() { DeptNo = 2, DeptName = "Neurology" }
        };
        _departmentRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(departments);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(null, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Departments.Should().HaveCount(2);
        model.Doctors.Should().BeEmpty();
        model.SelectedDept.Should().BeEmpty();
    }

    [Fact]
    public async Task OnGetAsync_WithDeptName_ReturnsDepartmentsAndDoctors()
    {
        // Arrange
        var departments = new List<DepartmentEntity>
        {
            new() { DeptNo = 1, DeptName = "Cardiology" }
        };
        var doctors = new List<DoctorEntity>
        {
            new() { DoctorId = 1, Name = "Dr. Heart", Status = 1 }
        };
        _departmentRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(departments);
        _doctorRepoMock.Setup(r => r.GetByDepartmentAsync("Cardiology", It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctors);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync("Cardiology", CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Departments.Should().HaveCount(1);
        model.Doctors.Should().HaveCount(1);
        model.SelectedDept.Should().Be("Cardiology");
    }

    [Fact]
    public void TakeAppointmentModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(1);

        // Assert
        model.Departments.Should().NotBeNull();
        model.Departments.Should().BeEmpty();
        model.Doctors.Should().NotBeNull();
        model.Doctors.Should().BeEmpty();
        model.SelectedDept.Should().BeEmpty();
    }
}
