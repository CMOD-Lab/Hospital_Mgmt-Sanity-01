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
/// Unit tests for DoctorProfileModel page model.
/// </summary>
public class DoctorProfileModelTests
{
    private readonly Mock<IDoctorRepository> _doctorRepoMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IDepartmentRepository> _departmentRepoMock;
    private readonly Mock<ILoginRepository> _loginRepoMock;
    private readonly Mock<ILogger<DoctorService>> _loggerMock;
    private readonly DoctorService _doctorService;

    public DoctorProfileModelTests()
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

    private DoctorProfileModel CreatePageModel(int? sessionUserId = 1)
    {
        var model = new DoctorProfileModel(_doctorService);
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
        var result = await model.OnGetAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnGetAsync_WhenDoctorExists_ReturnsDoctorProfile()
    {
        // Arrange
        var doctor = new DoctorEntity
        {
            DoctorId = 5,
            Name = "Dr. Smith",
            BirthDate = new DateTime(1975, 3, 10),
            Gender = 'M',
            ChargesPerVisit = 100,
            Qualification = "MBBS",
            Status = 1
        };
        _doctorRepoMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctor);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(5, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Doctor.Should().NotBeNull();
        model.Doctor!.Name.Should().Be("Dr. Smith");
        model.Doctor.DoctorId.Should().Be(5);
    }

    [Fact]
    public async Task OnGetAsync_WhenDoctorNotFound_ReturnsPageWithNullDoctor()
    {
        // Arrange
        _doctorRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DoctorEntity?)null);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(99, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Doctor.Should().BeNull();
    }

    [Fact]
    public void DoctorProfileModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(1);

        // Assert
        model.Doctor.Should().BeNull();
    }
}
