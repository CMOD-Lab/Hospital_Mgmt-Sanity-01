using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Patient;

/// <summary>
/// Comprehensive unit tests for CurrentAppointmentModel page.
/// </summary>
public class CurrentAppointmentModelTests
{
    private readonly Mock<IPatientService> _mockPatientService;

    public CurrentAppointmentModelTests()
    {
        _mockPatientService = new Mock<IPatientService>();
    }

    private ClinicManagement.Web.Pages.Patient.CurrentAppointmentModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Patient.CurrentAppointmentModel(_mockPatientService.Object);
        SetupSession(sut, sessionUserId);
        return sut;
    }

    private static void SetupSession(PageModel model, int? userId)
    {
        var mockSession = new Mock<ISession>();
        if (userId.HasValue)
        {
            var bytes = BitConverter.GetBytes(userId.Value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            byte[] outBytes = bytes;
            mockSession.Setup(s => s.TryGetValue("UserId", out outBytes!)).Returns(true);
        }
        else
        {
            byte[]? nullBytes = null;
            mockSession.Setup(s => s.TryGetValue("UserId", out nullBytes!)).Returns(false);
        }
        mockSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()));
        var httpContext = new DefaultHttpContext();
        httpContext.Session = mockSession.Object;
        model.PageContext = new PageContext { HttpContext = httpContext };
    }

    [Fact]
    public void Constructor_WithValidService_CreatesInstance()
    {
        var model = new ClinicManagement.Web.Pages.Patient.CurrentAppointmentModel(_mockPatientService.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_CurrentAppointmentIsNull()
    {
        var sut = CreateSut(1);
        Assert.Null(sut.CurrentAppointment);
    }

    [Fact]
    public async Task OnGetAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnGetAsync(CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WithValidSession_ReturnsPage()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetCurrentAppointmentAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CurrentAppointmentData?)null);

        var result = await sut.OnGetAsync(CancellationToken.None);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WithAppointment_SetsCurrentAppointment()
    {
        var sut = CreateSut(1);
        var appointment = new CurrentAppointmentData("Dr. Smith", "10:00 AM - 11:00 AM");
        _mockPatientService
            .Setup(s => s.GetCurrentAppointmentAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        await sut.OnGetAsync(CancellationToken.None);

        Assert.NotNull(sut.CurrentAppointment);
        Assert.Equal("Dr. Smith", sut.CurrentAppointment!.DoctorName);
        Assert.Equal("10:00 AM - 11:00 AM", sut.CurrentAppointment.Timings);
    }

    [Fact]
    public async Task OnGetAsync_NoAppointment_CurrentAppointmentIsNull()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetCurrentAppointmentAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CurrentAppointmentData?)null);

        await sut.OnGetAsync(CancellationToken.None);

        Assert.Null(sut.CurrentAppointment);
    }

    [Fact]
    public async Task OnGetAsync_CallsServiceWithCorrectUserId()
    {
        var sut = CreateSut(7);
        _mockPatientService
            .Setup(s => s.GetCurrentAppointmentAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CurrentAppointmentData?)null);

        await sut.OnGetAsync(CancellationToken.None);

        _mockPatientService.Verify(s => s.GetCurrentAppointmentAsync(7, It.IsAny<CancellationToken>()), Times.Once);
    }
}
