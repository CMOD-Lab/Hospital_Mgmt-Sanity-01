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

namespace ClinicManagement.UnitTests.Web.Pages.Patient;

/// <summary>
/// Unit tests for TakeAppointmentModel page.
/// </summary>
public class TakeAppointmentModelTests
{
    private readonly Mock<IPatientService> _mockPatientService;

    public TakeAppointmentModelTests()
    {
        _mockPatientService = new Mock<IPatientService>();
    }

    private ClinicManagement.Web.Pages.Patient.TakeAppointmentModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Patient.TakeAppointmentModel(_mockPatientService.Object);
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

    // ─── Constructor Tests ────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidService_CreatesInstance()
    {
        var model = new ClinicManagement.Web.Pages.Patient.TakeAppointmentModel(_mockPatientService.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_DepartmentsIsEmpty()
    {
        var sut = CreateSut(1);
        Assert.Empty(sut.Departments);
    }

    [Fact]
    public void Constructor_InitialState_DoctorsIsEmpty()
    {
        var sut = CreateSut(1);
        Assert.Empty(sut.Doctors);
    }

    [Fact]
    public void Constructor_InitialState_FreeSlotsIsEmpty()
    {
        var sut = CreateSut(1);
        Assert.Empty(sut.FreeSlots);
    }

    [Fact]
    public void Constructor_InitialState_MessageIsNull()
    {
        var sut = CreateSut(1);
        Assert.Null(sut.Message);
    }

    // ─── OnGetAsync Tests ─────────────────────────────────────────────────────

    [Fact]
    public async Task OnGetAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnGetAsync(null, CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WithValidSession_NoDoctorId_LoadsDepartmentsOnly()
    {
        var sut = CreateSut(1);
        var depts = new List<DeptInfo> { new DeptInfo(1, "Cardiology", null) };
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(depts);

        var result = await sut.OnGetAsync(null, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.Single(sut.Departments);
        Assert.Empty(sut.FreeSlots);
    }

    [Fact]
    public async Task OnGetAsync_WithDoctorId_LoadsFreeSlotsAndDepartments()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeptInfo>());
        _mockPatientService
            .Setup(s => s.GetFreeSlotsAsync(5, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AppointmentSlot>
            {
                new AppointmentSlot(1, "10:00 AM", DateTime.Today),
                new AppointmentSlot(2, "11:00 AM", DateTime.Today)
            });

        var result = await sut.OnGetAsync(5, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.Equal(5, sut.SelectedDoctorId);
        Assert.Equal(2, sut.FreeSlots.Count());
    }

    [Fact]
    public async Task OnGetAsync_WithZeroDoctorId_DoesNotLoadSlots()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeptInfo>());

        await sut.OnGetAsync(0, CancellationToken.None);

        _mockPatientService.Verify(s => s.GetFreeSlotsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── OnPostAsync Tests ────────────────────────────────────────────────────

    [Fact]
    public async Task OnPostAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnPostAsync(null, 0, CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnPostAsync_WithDeptName_LoadsDoctors()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeptInfo>());
        _mockPatientService
            .Setup(s => s.GetDoctorsByDepartmentAsync("Cardiology", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorListItem> { new DoctorListItem(1, "Dr. Smith", "Cardiology") });

        var result = await sut.OnPostAsync("Cardiology", 0, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.Single(sut.Doctors);
    }

    [Fact]
    public async Task OnPostAsync_WithDoctorId_LoadsFreeSlots()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeptInfo>());
        _mockPatientService
            .Setup(s => s.GetFreeSlotsAsync(3, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AppointmentSlot> { new AppointmentSlot(1, "10:00 AM", DateTime.Today) });

        var result = await sut.OnPostAsync(null, 3, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.Single(sut.FreeSlots);
    }

    // ─── OnPostBookAsync Tests ────────────────────────────────────────────────

    [Fact]
    public async Task OnPostBookAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnPostBookAsync(1, 1, CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnPostBookAsync_SuccessfulBooking_SetsSuccessMessage()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.BookAppointmentAsync(5, 1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeptInfo>());

        var result = await sut.OnPostBookAsync(5, 2, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.True(sut.IsSuccess);
        Assert.Equal("Appointment request sent successfully!", sut.Message);
    }

    [Fact]
    public async Task OnPostBookAsync_FailedBooking_SetsFailureMessage()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.BookAppointmentAsync(5, 1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeptInfo>());

        var result = await sut.OnPostBookAsync(5, 2, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.False(sut.IsSuccess);
        Assert.Equal("Failed to book appointment. Please try again.", sut.Message);
    }
}
