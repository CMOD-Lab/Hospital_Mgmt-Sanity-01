using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Admin;

/// <summary>
/// Comprehensive unit tests for DoctorRegistrationModel page.
/// </summary>
public class DoctorRegistrationModelTests
{
    private readonly Mock<IAdminService> _mockAdminService;
    private readonly Mock<IPatientService> _mockPatientService;

    public DoctorRegistrationModelTests()
    {
        _mockAdminService = new Mock<IAdminService>();
        _mockPatientService = new Mock<IPatientService>();
    }

    private ClinicManagement.Web.Pages.Admin.DoctorRegistrationModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Admin.DoctorRegistrationModel(
            _mockAdminService.Object, _mockPatientService.Object);
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
    public void Constructor_WithValidServices_CreatesInstance()
    {
        var model = new ClinicManagement.Web.Pages.Admin.DoctorRegistrationModel(
            _mockAdminService.Object, _mockPatientService.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_DepartmentsIsEmpty()
    {
        var sut = CreateSut(1);
        Assert.Empty(sut.Departments);
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
    public async Task OnGetAsync_WithValidSession_LoadsDepartments()
    {
        var sut = CreateSut(3);
        var depts = new List<DeptInfo>
        {
            new DeptInfo(1, "Cardiology", "Heart"),
            new DeptInfo(2, "Neurology", "Brain")
        };
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(depts);

        var result = await sut.OnGetAsync(CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.Equal(2, System.Linq.Enumerable.Count(sut.Departments));
    }

    [Fact]
    public async Task OnPostAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnPostAsync(
            "Dr. Smith", "doctor@test.com", "pass", "1980-01-01",
            1, "M", "1234567890", "123 Main St",
            10, 80000, 500, "Cardiology", "MBBS",
            CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnPostAsync_EmailAlreadyExists_SetsEmailExistsMessage()
    {
        var sut = CreateSut(3);
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeptInfo>());
        _mockAdminService
            .Setup(s => s.DoctorEmailExistsAsync("doctor@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await sut.OnPostAsync(
            "Dr. Smith", "doctor@test.com", "pass", "1980-01-01",
            1, "M", "1234567890", "123 Main St",
            10, 80000, 500, "Cardiology", "MBBS",
            CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.False(sut.IsSuccess);
        Assert.Equal("This email already exists. Please choose a different one.", sut.Message);
    }

    [Fact]
    public async Task OnPostAsync_SuccessfulRegistration_SetsSuccessMessage()
    {
        var sut = CreateSut(3);
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeptInfo>());
        _mockAdminService
            .Setup(s => s.DoctorEmailExistsAsync("newdoctor@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockAdminService
            .Setup(s => s.RegisterDoctorAsync(It.IsAny<RegisterDoctorRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await sut.OnPostAsync(
            "Dr. Smith", "newdoctor@test.com", "pass", "1980-01-01",
            1, "M", "1234567890", "123 Main St",
            10, 80000, 500, "Cardiology", "MBBS",
            CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.True(sut.IsSuccess);
        Assert.Equal("Doctor registered successfully!", sut.Message);
    }

    [Fact]
    public async Task OnPostAsync_FailedRegistration_SetsFailureMessage()
    {
        var sut = CreateSut(3);
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeptInfo>());
        _mockAdminService
            .Setup(s => s.DoctorEmailExistsAsync("newdoctor@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockAdminService
            .Setup(s => s.RegisterDoctorAsync(It.IsAny<RegisterDoctorRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await sut.OnPostAsync(
            "Dr. Smith", "newdoctor@test.com", "pass", "1980-01-01",
            1, "M", "1234567890", "123 Main St",
            10, 80000, 500, "Cardiology", "MBBS",
            CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.False(sut.IsSuccess);
        Assert.Equal("Failed to register doctor. Please try again.", sut.Message);
    }

    [Fact]
    public async Task OnPostAsync_WithEmptyGender_UsesDefaultMale()
    {
        var sut = CreateSut(3);
        RegisterDoctorRequest? capturedRequest = null;
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeptInfo>());
        _mockAdminService
            .Setup(s => s.DoctorEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockAdminService
            .Setup(s => s.RegisterDoctorAsync(It.IsAny<RegisterDoctorRequest>(), It.IsAny<CancellationToken>()))
            .Callback<RegisterDoctorRequest, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(true);

        await sut.OnPostAsync(
            "Dr. Smith", "newdoctor@test.com", "pass", "1980-01-01",
            1, "", "1234567890", "123 Main St",
            10, 80000, 500, "Cardiology", "MBBS",
            CancellationToken.None);

        Assert.NotNull(capturedRequest);
        Assert.Equal('M', capturedRequest!.Gender);
    }

    [Fact]
    public async Task OnPostAsync_AlwaysLoadsDepartments()
    {
        var sut = CreateSut(3);
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeptInfo>());
        _mockAdminService
            .Setup(s => s.DoctorEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockAdminService
            .Setup(s => s.RegisterDoctorAsync(It.IsAny<RegisterDoctorRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await sut.OnPostAsync(
            "Dr. Smith", "newdoctor@test.com", "pass", "1980-01-01",
            1, "M", "1234567890", "123 Main St",
            10, 80000, 500, "Cardiology", "MBBS",
            CancellationToken.None);

        _mockPatientService.Verify(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
