using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.UnitTests.Web.Helpers;
using ClinicManagement.Web.Pages.Patient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Patient;

public class TakeAppointmentModelTests
{
    private readonly Mock<IAppointmentService> _mockAppointmentService;
    private readonly Mock<IDoctorService> _mockDoctorService;
    private readonly Mock<ILogger<TakeAppointmentModel>> _mockLogger;
    private readonly TakeAppointmentModel _takeAppointmentModel;
    private readonly TestSession _testSession;

    public TakeAppointmentModelTests()
    {
        _mockAppointmentService = new Mock<IAppointmentService>();
        _mockDoctorService = new Mock<IDoctorService>();
        _mockLogger = new Mock<ILogger<TakeAppointmentModel>>();
        _takeAppointmentModel = new TakeAppointmentModel(_mockAppointmentService.Object, _mockDoctorService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _takeAppointmentModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };

        _mockDoctorService.Setup(s => s.GetAllAsync(default))
            .ReturnsAsync(new List<DoctorDto>
            {
                new DoctorDto { DoctorId = 1, Name = "Dr. Smith", DepartmentName = "Cardiology" },
                new DoctorDto { DoctorId = 2, Name = "Dr. Jones", DepartmentName = "Neurology" }
            });
    }

    private void SetupPatientSession(int patientId = 7) { _testSession.SetInt32("UserId", patientId); _testSession.SetInt32("UserType", 2); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new TakeAppointmentModel(_mockAppointmentService.Object, _mockDoctorService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitializesProperties()
    {
        Assert.NotNull(_takeAppointmentModel.Input);
        Assert.NotNull(_takeAppointmentModel.DoctorOptions);
        Assert.NotNull(_takeAppointmentModel.FreeSlots);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _takeAppointmentModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsPatient_LoadsDoctors()
    {
        SetupPatientSession(7);
        var result = await _takeAppointmentModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.NotEmpty(_takeAppointmentModel.DoctorOptions);
    }

    [Fact]
    public async Task OnGetAsync_WhenDoctorIdProvided_LoadsFreeSlotsForDoctor()
    {
        SetupPatientSession(7);
        var freeSlots = new List<TimeSlotDto>
        {
            new TimeSlotDto { TimeSlotId = 1, Timings = "9:00 AM - 10:00 AM", IsAvailable = true }
        };
        _mockAppointmentService.Setup(s => s.GetFreeSlotsAsync(1, 7, default)).ReturnsAsync(freeSlots);

        var result = await _takeAppointmentModel.OnGetAsync(1);

        Assert.IsType<PageResult>(result);
        Assert.Equal(1, _takeAppointmentModel.Input.DoctorId);
    }

    [Fact]
    public async Task OnPostAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _takeAppointmentModel.OnPostAsync();
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WhenLoggedInAsPatient_LoadsDoctorsAndReturnsPage()
    {
        SetupPatientSession(7);
        _takeAppointmentModel.Input = new TakeAppointmentModel.TakeAppointmentInputModel { DoctorId = 1, TimeSlotId = 0 };
        _mockAppointmentService.Setup(s => s.GetFreeSlotsAsync(1, 7, default)).ReturnsAsync(new List<TimeSlotDto>());

        var result = await _takeAppointmentModel.OnPostAsync();
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostBookAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _takeAppointmentModel.OnPostBookAsync();
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnPostBookAsync_WhenValidInput_BookAppointmentSuccessfully()
    {
        SetupPatientSession(7);
        _takeAppointmentModel.Input = new TakeAppointmentModel.TakeAppointmentInputModel { DoctorId = 1, TimeSlotId = 2 };

        _mockAppointmentService.Setup(s => s.CreateAsync(It.IsAny<AppointmentCreateDto>(), default))
            .ReturnsAsync(new AppointmentDto { AppointmentId = 10 });
        _mockAppointmentService.Setup(s => s.GetFreeSlotsAsync(1, 7, default)).ReturnsAsync(new List<TimeSlotDto>());

        var result = await _takeAppointmentModel.OnPostBookAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal("Appointment request sent successfully! Please wait for doctor approval.", _takeAppointmentModel.SuccessMessage);
    }

    [Fact]
    public async Task OnPostBookAsync_WhenInvalidInput_ReturnsPageWithoutBooking()
    {
        SetupPatientSession(7);
        _takeAppointmentModel.Input = new TakeAppointmentModel.TakeAppointmentInputModel { DoctorId = 0, TimeSlotId = 0 };

        var result = await _takeAppointmentModel.OnPostBookAsync();

        Assert.IsType<PageResult>(result);
        _mockAppointmentService.Verify(s => s.CreateAsync(It.IsAny<AppointmentCreateDto>(), default), Times.Never);
    }

    [Fact]
    public async Task OnPostBookAsync_WhenExceptionThrown_SetsErrorMessage()
    {
        SetupPatientSession(7);
        _takeAppointmentModel.Input = new TakeAppointmentModel.TakeAppointmentInputModel { DoctorId = 1, TimeSlotId = 2 };

        _mockAppointmentService.Setup(s => s.CreateAsync(It.IsAny<AppointmentCreateDto>(), default))
            .ThrowsAsync(new Exception("Booking error"));
        _mockAppointmentService.Setup(s => s.GetFreeSlotsAsync(1, 7, default)).ReturnsAsync(new List<TimeSlotDto>());

        var result = await _takeAppointmentModel.OnPostBookAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal("An error occurred. Please try again.", _takeAppointmentModel.ErrorMessage);
    }

    [Fact]
    public void TakeAppointmentInputModel_DefaultValues()
    {
        var input = new TakeAppointmentModel.TakeAppointmentInputModel();
        Assert.Equal(0, input.DoctorId);
        Assert.Equal(0, input.TimeSlotId);
    }
}
