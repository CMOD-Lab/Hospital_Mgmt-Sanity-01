using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Patient;

/// <summary>
/// Comprehensive unit tests for Patient HomeModel page.
/// </summary>
public class PatientHomeModelTests
{
    private readonly Mock<IPatientService> _mockPatientService;
    private readonly Mock<ILogger<ClinicManagement.Web.Pages.Patient.HomeModel>> _mockLogger;

    public PatientHomeModelTests()
    {
        _mockPatientService = new Mock<IPatientService>();
        _mockLogger = new Mock<ILogger<ClinicManagement.Web.Pages.Patient.HomeModel>>();
    }

    private ClinicManagement.Web.Pages.Patient.HomeModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Patient.HomeModel(
            _mockPatientService.Object, _mockLogger.Object);
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
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new ClinicManagement.Web.Pages.Patient.HomeModel(
            _mockPatientService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_ProfileIsNull()
    {
        var sut = CreateSut(1);
        Assert.Null(sut.Profile);
    }

    [Fact]
    public void PatientName_WhenProfileIsNull_ReturnsDefaultPatient()
    {
        var sut = CreateSut(1);
        Assert.Equal("Patient", sut.PatientName);
    }

    // ─── OnGetAsync Tests ─────────────────────────────────────────────────────

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
            .Setup(s => s.GetPatientProfileAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PatientProfileData(1, "John Doe", "1234567890", "123 Main St", "1990-01-01", 34, "Male"));

        var result = await sut.OnGetAsync(CancellationToken.None);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WithValidSession_SetsProfile()
    {
        var sut = CreateSut(1);
        var profile = new PatientProfileData(1, "John Doe", "1234567890", "123 Main St", "1990-01-01", 34, "Male");
        _mockPatientService
            .Setup(s => s.GetPatientProfileAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        await sut.OnGetAsync(CancellationToken.None);

        Assert.NotNull(sut.Profile);
        Assert.Equal("John Doe", sut.Profile!.Name);
    }

    [Fact]
    public async Task PatientName_WhenProfileLoaded_ReturnsProfileName()
    {
        var sut = CreateSut(1);
        var profile = new PatientProfileData(1, "Jane Smith", "1234567890", "456 Oak Ave", "1985-05-15", 39, "Female");
        _mockPatientService
            .Setup(s => s.GetPatientProfileAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        await sut.OnGetAsync(CancellationToken.None);

        Assert.Equal("Jane Smith", sut.PatientName);
    }

    [Fact]
    public async Task OnGetAsync_ServiceThrowsException_ReturnsPageWithNullProfile()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetPatientProfileAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await sut.OnGetAsync(CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.Null(sut.Profile);
    }

    [Fact]
    public async Task OnGetAsync_ServiceReturnsNull_ProfileRemainsNull()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetPatientProfileAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PatientProfileData?)null);

        var result = await sut.OnGetAsync(CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.Null(sut.Profile);
    }

    [Fact]
    public async Task OnGetAsync_CallsServiceWithCorrectUserId()
    {
        var sut = CreateSut(42);
        _mockPatientService
            .Setup(s => s.GetPatientProfileAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PatientProfileData?)null);

        await sut.OnGetAsync(CancellationToken.None);

        _mockPatientService.Verify(s => s.GetPatientProfileAsync(42, It.IsAny<CancellationToken>()), Times.Once);
    }
}
