using ClinicManagement.Domain.Enums;
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

namespace ClinicManagement.UnitTests.Web.Pages;

/// <summary>
/// Comprehensive unit tests for IndexModel (Login/Signup page).
/// </summary>
public class IndexModelTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<ILogger<ClinicManagement.Web.Pages.IndexModel>> _mockLogger;
    private readonly ClinicManagement.Web.Pages.IndexModel _sut;

    public IndexModelTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockLogger = new Mock<ILogger<ClinicManagement.Web.Pages.IndexModel>>();
        _sut = new ClinicManagement.Web.Pages.IndexModel(_mockAuthService.Object, _mockLogger.Object);
        SetupHttpContext(_sut, userId: 0, hasSession: true);
    }

    private static void SetupHttpContext(PageModel model, int userId, bool hasSession)
    {
        var mockSession = new Mock<ISession>();
        byte[]? outBytes = null;
        mockSession.Setup(s => s.TryGetValue(It.IsAny<string>(), out outBytes!)).Returns(false);
        mockSession.Setup(s => s.Clear());
        mockSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()));

        var httpContext = new DefaultHttpContext();
        httpContext.Session = mockSession.Object;
        model.PageContext = new PageContext { HttpContext = httpContext };
    }

    // ─── Constructor Tests ────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new ClinicManagement.Web.Pages.IndexModel(
            _mockAuthService.Object,
            _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_ErrorMessageIsNull()
    {
        Assert.Null(_sut.ErrorMessage);
    }

    [Fact]
    public void Constructor_InitialState_SuccessMessageIsNull()
    {
        Assert.Null(_sut.SuccessMessage);
    }

    // ─── OnGet Tests ─────────────────────────────────────────────────────────

    [Fact]
    public void OnGet_ClearsSession()
    {
        var mockSession = new Mock<ISession>();
        mockSession.Setup(s => s.Clear());
        byte[]? outBytes = null;
        mockSession.Setup(s => s.TryGetValue(It.IsAny<string>(), out outBytes!)).Returns(false);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = mockSession.Object;
        _sut.PageContext = new PageContext { HttpContext = httpContext };

        _sut.OnGet();

        mockSession.Verify(s => s.Clear(), Times.Once);
    }

    // ─── OnPostLoginAsync Tests ───────────────────────────────────────────────

    [Fact]
    public async Task OnPostLoginAsync_EmptyEmail_ReturnsPageWithError()
    {
        var result = await _sut.OnPostLoginAsync("", "password", CancellationToken.None);
        Assert.IsType<PageResult>(result);
        Assert.Equal("Please enter email and password.", _sut.ErrorMessage);
    }

    [Fact]
    public async Task OnPostLoginAsync_EmptyPassword_ReturnsPageWithError()
    {
        var result = await _sut.OnPostLoginAsync("user@test.com", "", CancellationToken.None);
        Assert.IsType<PageResult>(result);
        Assert.Equal("Please enter email and password.", _sut.ErrorMessage);
    }

    [Fact]
    public async Task OnPostLoginAsync_WhitespaceEmail_ReturnsPageWithError()
    {
        var result = await _sut.OnPostLoginAsync("   ", "password", CancellationToken.None);
        Assert.IsType<PageResult>(result);
        Assert.Equal("Please enter email and password.", _sut.ErrorMessage);
    }

    [Fact]
    public async Task OnPostLoginAsync_WhitespacePassword_ReturnsPageWithError()
    {
        var result = await _sut.OnPostLoginAsync("user@test.com", "   ", CancellationToken.None);
        Assert.IsType<PageResult>(result);
        Assert.Equal("Please enter email and password.", _sut.ErrorMessage);
    }

    [Fact]
    public async Task OnPostLoginAsync_FailedLogin_ReturnsPageWithServiceMessage()
    {
        _mockAuthService
            .Setup(s => s.LoginAsync("user@test.com", "wrongpass", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResult(false, 0, UserType.Patient, "Invalid credentials"));

        var result = await _sut.OnPostLoginAsync("user@test.com", "wrongpass", CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.Equal("Invalid credentials", _sut.ErrorMessage);
    }

    [Fact]
    public async Task OnPostLoginAsync_SuccessfulPatientLogin_RedirectsToPatientHome()
    {
        _mockAuthService
            .Setup(s => s.LoginAsync("patient@test.com", "pass", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResult(true, 1, UserType.Patient, "OK"));

        var result = await _sut.OnPostLoginAsync("patient@test.com", "pass", CancellationToken.None);

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Patient/Home", redirect.PageName);
    }

    [Fact]
    public async Task OnPostLoginAsync_SuccessfulDoctorLogin_RedirectsToDoctorHome()
    {
        _mockAuthService
            .Setup(s => s.LoginAsync("doctor@test.com", "pass", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResult(true, 2, UserType.Doctor, "OK"));

        var result = await _sut.OnPostLoginAsync("doctor@test.com", "pass", CancellationToken.None);

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Doctor/Home", redirect.PageName);
    }

    [Fact]
    public async Task OnPostLoginAsync_SuccessfulAdminLogin_RedirectsToAdminHome()
    {
        _mockAuthService
            .Setup(s => s.LoginAsync("admin@test.com", "pass", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResult(true, 3, UserType.Admin, "OK"));

        var result = await _sut.OnPostLoginAsync("admin@test.com", "pass", CancellationToken.None);

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Admin/Home", redirect.PageName);
    }

    [Fact]
    public async Task OnPostLoginAsync_NullEmailAndPassword_ReturnsPageWithError()
    {
        var result = await _sut.OnPostLoginAsync(null!, null!, CancellationToken.None);
        Assert.IsType<PageResult>(result);
        Assert.Equal("Please enter email and password.", _sut.ErrorMessage);
    }

    // ─── OnPostSignupAsync Tests ──────────────────────────────────────────────

    [Fact]
    public async Task OnPostSignupAsync_EmptyName_ReturnsPageWithError()
    {
        var result = await _sut.OnPostSignupAsync(
            "", "1990-01-01", "user@test.com", "pass", "1234567890", "M", "Address",
            CancellationToken.None);
        Assert.IsType<PageResult>(result);
        Assert.Equal("Please fill in all required fields.", _sut.ErrorMessage);
    }

    [Fact]
    public async Task OnPostSignupAsync_EmptyEmail_ReturnsPageWithError()
    {
        var result = await _sut.OnPostSignupAsync(
            "John", "1990-01-01", "", "pass", "1234567890", "M", "Address",
            CancellationToken.None);
        Assert.IsType<PageResult>(result);
        Assert.Equal("Please fill in all required fields.", _sut.ErrorMessage);
    }

    [Fact]
    public async Task OnPostSignupAsync_EmptyPassword_ReturnsPageWithError()
    {
        var result = await _sut.OnPostSignupAsync(
            "John", "1990-01-01", "user@test.com", "", "1234567890", "M", "Address",
            CancellationToken.None);
        Assert.IsType<PageResult>(result);
        Assert.Equal("Please fill in all required fields.", _sut.ErrorMessage);
    }

    [Fact]
    public async Task OnPostSignupAsync_FailedSignup_ReturnsPageWithServiceMessage()
    {
        _mockAuthService
            .Setup(s => s.SignupPatientAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SignupResult(false, 0, "Email already exists"));

        var result = await _sut.OnPostSignupAsync(
            "John", "1990-01-01", "user@test.com", "pass", "1234567890", "M", "Address",
            CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.Equal("Email already exists", _sut.ErrorMessage);
    }

    [Fact]
    public async Task OnPostSignupAsync_SuccessfulSignup_RedirectsToPatientHome()
    {
        _mockAuthService
            .Setup(s => s.SignupPatientAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SignupResult(true, 10, "Success"));

        var result = await _sut.OnPostSignupAsync(
            "John", "1990-01-01", "user@test.com", "pass", "1234567890", "M", "Address",
            CancellationToken.None);

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Patient/Home", redirect.PageName);
    }

    [Fact]
    public async Task OnPostSignupAsync_WhitespaceName_ReturnsPageWithError()
    {
        var result = await _sut.OnPostSignupAsync(
            "   ", "1990-01-01", "user@test.com", "pass", "1234567890", "M", "Address",
            CancellationToken.None);
        Assert.IsType<PageResult>(result);
        Assert.Equal("Please fill in all required fields.", _sut.ErrorMessage);
    }
}
