using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.UnitTests.Helpers;
using ClinicManagement.Web.Pages.Doctor;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Pages.Doctor;

/// <summary>
/// Unit tests for DoctorHomeModel page model.
/// </summary>
public class DoctorHomeModelTests
{
    private readonly Mock<IDoctorRepository> _doctorRepoMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IDepartmentRepository> _departmentRepoMock;
    private readonly Mock<ILoginRepository> _loginRepoMock;
    private readonly Mock<ILogger<DoctorService>> _loggerMock;
    private readonly DoctorService _doctorService;

    public DoctorHomeModelTests()
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

    private DoctorHomeModel CreatePageModel(int? sessionUserId = 2)
    {
        var model = new DoctorHomeModel(_doctorService);
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
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsDoctorProfile()
    {
        // Arrange
        var doctor = new ClinicManagement.Domain.Entities.Doctor
        {
            DoctorId = 2,
            Name = "Dr. Johnson",
            BirthDate = new DateTime(1970, 6, 15),
            Gender = 'M',
            ChargesPerVisit = 150,
            Qualification = "MD",
            Status = 1
        };
        _doctorRepoMock.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctor);

        var model = CreatePageModel(2);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Doctor.Should().NotBeNull();
        model.Doctor!.Name.Should().Be("Dr. Johnson");
    }

    [Fact]
    public async Task OnGetAsync_WhenDoctorNotFound_ReturnsPageWithNullDoctor()
    {
        // Arrange
        _doctorRepoMock.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClinicManagement.Domain.Entities.Doctor?)null);

        var model = CreatePageModel(2);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Doctor.Should().BeNull();
    }

    [Fact]
    public void DoctorName_WhenDoctorIsNull_ReturnsDefaultName()
    {
        // Arrange
        var model = CreatePageModel(2);

        // Act
        var name = model.DoctorName;

        // Assert
        name.Should().Be("Doctor");
    }

    [Fact]
    public async Task DoctorName_WhenDoctorLoaded_ReturnsDoctorName()
    {
        // Arrange
        var doctor = new ClinicManagement.Domain.Entities.Doctor
        {
            DoctorId = 2,
            Name = "Dr. Williams",
            BirthDate = new DateTime(1968, 9, 20),
            Gender = 'F',
            ChargesPerVisit = 200,
            Qualification = "MBBS",
            Status = 1
        };
        _doctorRepoMock.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctor);

        var model = CreatePageModel(2);
        await model.OnGetAsync(CancellationToken.None);

        // Act
        var name = model.DoctorName;

        // Assert
        name.Should().Be("Dr. Williams");
    }

    [Fact]
    public void DoctorHomeModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(2);

        // Assert
        model.Doctor.Should().BeNull();
        model.DoctorName.Should().Be("Doctor");
    }
}
