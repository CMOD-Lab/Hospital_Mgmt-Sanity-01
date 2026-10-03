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
/// Unit tests for DoctorProfileModel page.
/// </summary>
public class DoctorProfileModelTests
{
    private readonly Mock<IPatientService> _mockPatientService;

    public DoctorProfileModelTests()
    {
        _mockPatientService = new Mock<IPatientService>();
    }

    private ClinicManagement.Web.Pages.Patient.DoctorProfileModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Patient.DoctorProfileModel(_mockPatientService.Object);
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
        var model = new ClinicManagement.Web.Pages.Patient.DoctorProfileModel(_mockPatientService.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_ProfileIsNull()
    {
        var sut = CreateSut(1);
        Assert.Null(sut.Profile);
    }

    [Fact]
    public async Task OnGetAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnGetAsync(1, CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WithValidSession_ReturnsPage()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetDoctorProfileAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DoctorProfileData?)null);

        var result = await sut.OnGetAsync(5, CancellationToken.None);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WithDoctorProfile_SetsProfile()
    {
        var sut = CreateSut(1);
        var profile = new DoctorProfileData(5, "Dr. Smith", "1234567890", "Male", 500.0, 4.5, 100, "MBBS", "Cardiology", 10, 45, "Cardiology");
        _mockPatientService
            .Setup(s => s.GetDoctorProfileAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        await sut.OnGetAsync(5, CancellationToken.None);

        Assert.NotNull(sut.Profile);
        Assert.Equal("Dr. Smith", sut.Profile!.Name);
        Assert.Equal(500.0, sut.Profile.ChargesPerVisit);
    }

    [Fact]
    public async Task OnGetAsync_DoctorNotFound_ProfileIsNull()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetDoctorProfileAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DoctorProfileData?)null);

        await sut.OnGetAsync(999, CancellationToken.None);

        Assert.Null(sut.Profile);
    }

    [Fact]
    public async Task OnGetAsync_CallsServiceWithCorrectDoctorId()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetDoctorProfileAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DoctorProfileData?)null);

        await sut.OnGetAsync(10, CancellationToken.None);

        _mockPatientService.Verify(s => s.GetDoctorProfileAsync(10, It.IsAny<CancellationToken>()), Times.Once);
    }
}
