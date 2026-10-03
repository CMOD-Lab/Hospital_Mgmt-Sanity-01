using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Doctor;

/// <summary>
/// Comprehensive unit tests for PatientHistoryModel page.
/// </summary>
public class PatientHistoryModelTests
{
    private readonly Mock<IDoctorService> _mockDoctorService;

    public PatientHistoryModelTests()
    {
        _mockDoctorService = new Mock<IDoctorService>();
    }

    private ClinicManagement.Web.Pages.Doctor.PatientHistoryModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Doctor.PatientHistoryModel(_mockDoctorService.Object);
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
        var model = new ClinicManagement.Web.Pages.Doctor.PatientHistoryModel(_mockDoctorService.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_TodayAppointmentsIsEmpty()
    {
        var sut = CreateSut(1);
        Assert.Empty(sut.TodayAppointments);
    }

    [Fact]
    public void Constructor_InitialState_MessageIsNull()
    {
        var sut = CreateSut(1);
        Assert.Null(sut.Message);
    }

    [Fact]
    public void Constructor_InitialState_IsSuccessIsFalse()
    {
        var sut = CreateSut(1);
        Assert.False(sut.IsSuccess);
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
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.GetTodayAppointmentsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TodayAppointmentItem>());

        var result = await sut.OnGetAsync(CancellationToken.None);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WithAppointments_SetsAppointments()
    {
        var sut = CreateSut(2);
        var appointments = new List<TodayAppointmentItem>
        {
            new TodayAppointmentItem(1, "John Doe", DateTime.Today, "Flu", "Recovering", "Paracetamol"),
            new TodayAppointmentItem(2, "Jane Smith", DateTime.Today, "Cold", "Improving", "Vitamin C")
        };
        _mockDoctorService
            .Setup(s => s.GetTodayAppointmentsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        await sut.OnGetAsync(CancellationToken.None);

        Assert.Equal(2, sut.TodayAppointments.Count());
    }

    [Fact]
    public async Task OnPostUpdatePrescriptionAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnPostUpdatePrescriptionAsync(1, "Flu", "Recovering", "Paracetamol", CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnPostUpdatePrescriptionAsync_SuccessfulUpdate_SetsSuccessMessage()
    {
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.UpdatePrescriptionAsync(2, 5, "Flu", "Recovering", "Paracetamol", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockDoctorService
            .Setup(s => s.GetTodayAppointmentsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TodayAppointmentItem>());

        var result = await sut.OnPostUpdatePrescriptionAsync(5, "Flu", "Recovering", "Paracetamol", CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.True(sut.IsSuccess);
        Assert.Equal("Prescription updated successfully.", sut.Message);
    }

    [Fact]
    public async Task OnPostUpdatePrescriptionAsync_FailedUpdate_SetsFailureMessage()
    {
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.UpdatePrescriptionAsync(2, 5, "Flu", "Recovering", "Paracetamol", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockDoctorService
            .Setup(s => s.GetTodayAppointmentsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TodayAppointmentItem>());

        var result = await sut.OnPostUpdatePrescriptionAsync(5, "Flu", "Recovering", "Paracetamol", CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.False(sut.IsSuccess);
        Assert.Equal("Failed to update prescription.", sut.Message);
    }

    [Fact]
    public async Task OnPostUpdatePrescriptionAsync_ReloadsAppointmentsAfterUpdate()
    {
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.UpdatePrescriptionAsync(2, 5, "Flu", "Recovering", "Paracetamol", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockDoctorService
            .Setup(s => s.GetTodayAppointmentsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TodayAppointmentItem>());

        await sut.OnPostUpdatePrescriptionAsync(5, "Flu", "Recovering", "Paracetamol", CancellationToken.None);

        _mockDoctorService.Verify(s => s.GetTodayAppointmentsAsync(2, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnPostUpdatePrescriptionAsync_CallsServiceWithCorrectParameters()
    {
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.UpdatePrescriptionAsync(2, 10, "Diabetes", "Stable", "Metformin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockDoctorService
            .Setup(s => s.GetTodayAppointmentsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TodayAppointmentItem>());

        await sut.OnPostUpdatePrescriptionAsync(10, "Diabetes", "Stable", "Metformin", CancellationToken.None);

        _mockDoctorService.Verify(s => s.UpdatePrescriptionAsync(2, 10, "Diabetes", "Stable", "Metformin", It.IsAny<CancellationToken>()), Times.Once);
    }
}
