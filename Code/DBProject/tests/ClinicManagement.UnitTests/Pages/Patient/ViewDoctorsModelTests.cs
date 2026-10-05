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

namespace ClinicManagement.UnitTests.Pages.Patient;

/// <summary>
/// Unit tests for ViewDoctorsModel page model.
/// </summary>
public class ViewDoctorsModelTests
{
    private readonly Mock<IDoctorRepository> _doctorRepoMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IDepartmentRepository> _departmentRepoMock;
    private readonly Mock<ILoginRepository> _loginRepoMock;
    private readonly Mock<ILogger<DoctorService>> _loggerMock;
    private readonly DoctorService _doctorService;

    public ViewDoctorsModelTests()
    {
        _doctorRepoMock = new Mock<IDoctorRepository>();
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _departmentRepoMock = new Mock<IDepartmentRepository>();
        _loginRepoMock = new Mock<ILoginRepository>();
        _loggerMock = new Mock<ILogger<DoctorService>>();
        _doctorService = new DoctorService(
            _doctorRepoMock.Object,
            _appointmentRepoMock.Object,
            _departmentRepoMock.Object,
            _loginRepoMock.Object,
            _loggerMock.Object);
    }

    private ViewDoctorsModel CreatePageModel(int? sessionUserId = 1)
    {
        var model = new ViewDoctorsModel(_doctorService);
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
    public async Task OnGetAsync_WithNoSearch_ReturnsAllDoctors()
    {
        // Arrange
        var doctors = new List<DoctorEntity>
        {
            new() { DoctorId = 1, Name = "Dr. Smith", Status = 1 },
            new() { DoctorId = 2, Name = "Dr. Jones", Status = 1 }
        };
        _doctorRepoMock.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctors);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(null, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Doctors.Should().HaveCount(2);
        model.SearchQuery.Should().BeEmpty();
    }

    [Fact]
    public async Task OnGetAsync_WithSearchQuery_ReturnsFilteredDoctors()
    {
        // Arrange
        var doctors = new List<DoctorEntity>
        {
            new() { DoctorId = 1, Name = "Dr. Smith", Status = 1 }
        };
        _doctorRepoMock.Setup(r => r.SearchAsync("Smith", It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctors);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync("Smith", CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Doctors.Should().HaveCount(1);
        model.SearchQuery.Should().Be("Smith");
    }

    [Fact]
    public async Task OnGetAsync_WithEmptySearch_ReturnsAllDoctors()
    {
        // Arrange
        var doctors = new List<DoctorEntity>
        {
            new() { DoctorId = 1, Name = "Dr. Smith", Status = 1 }
        };
        _doctorRepoMock.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctors);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync("", CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.SearchQuery.Should().BeEmpty();
    }

    [Fact]
    public void ViewDoctorsModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(1);

        // Assert
        model.Doctors.Should().NotBeNull();
        model.Doctors.Should().BeEmpty();
        model.SearchQuery.Should().BeEmpty();
    }
}
