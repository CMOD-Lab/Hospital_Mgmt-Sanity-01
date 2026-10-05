using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.UnitTests.Helpers;
using ClinicManagement.Web.Pages;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using LoginTableEntity = ClinicManagement.Domain.Entities.LoginTable;
using PatientEntity = ClinicManagement.Domain.Entities.Patient;

namespace ClinicManagement.UnitTests.Pages;

/// <summary>
/// Unit tests for SignUpModel page model.
/// </summary>
public class SignUpModelTests
{
    private readonly Mock<ILoginRepository> _loginRepoMock;
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<ILogger<AuthService>> _authLoggerMock;
    private readonly Mock<ILogger<SignUpModel>> _loggerMock;
    private readonly AuthService _authService;

    public SignUpModelTests()
    {
        _loginRepoMock = new Mock<ILoginRepository>();
        _patientRepoMock = new Mock<IPatientRepository>();
        _authLoggerMock = new Mock<ILogger<AuthService>>();
        _loggerMock = new Mock<ILogger<SignUpModel>>();
        _authService = new AuthService(_loginRepoMock.Object, _patientRepoMock.Object, _authLoggerMock.Object);
    }

    private SignUpModel CreatePageModel()
    {
        var model = new SignUpModel(_authService, _loggerMock.Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockSession();
        model.PageContext = new PageContext { HttpContext = httpContext };
        return model;
    }

    [Fact]
    public void OnGet_ClearsSessionData()
    {
        // Arrange
        var model = CreatePageModel();
        ((MockSession)model.HttpContext.Session).SetInt32("UserId", 1);
        ((MockSession)model.HttpContext.Session).SetInt32("UserType", 1);

        // Act
        model.OnGet();

        // Assert
        model.HttpContext.Session.TryGetValue("UserId", out _).Should().BeFalse();
        model.HttpContext.Session.TryGetValue("UserType", out _).Should().BeFalse();
    }

    [Fact]
    public async Task OnPostLoginAsync_WithEmptyEmail_ReturnsPageWithMessage()
    {
        // Arrange
        var model = CreatePageModel();

        // Act
        var result = await model.OnPostLoginAsync("", "password", CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.LoginMessage.Should().Contain("Please enter");
    }

    [Fact]
    public async Task OnPostLoginAsync_WithEmptyPassword_ReturnsPageWithMessage()
    {
        // Arrange
        var model = CreatePageModel();

        // Act
        var result = await model.OnPostLoginAsync("test@test.com", "", CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.LoginMessage.Should().Contain("Please enter");
    }

    [Fact]
    public async Task OnPostLoginAsync_WithValidPatientCredentials_RedirectsToPatientHome()
    {
        // Arrange
        var login = new LoginTableEntity { LoginId = 1, Email = "patient@test.com", Password = "pass", Type = 1 };
        _loginRepoMock.Setup(r => r.ValidateLoginAsync("patient@test.com", "pass", It.IsAny<CancellationToken>()))
            .ReturnsAsync(login);

        var model = CreatePageModel();

        // Act
        var result = await model.OnPostLoginAsync("patient@test.com", "pass", CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/Patient/PatientHome");
    }

    [Fact]
    public async Task OnPostLoginAsync_WithValidDoctorCredentials_RedirectsToDoctorHome()
    {
        // Arrange
        var login = new LoginTableEntity { LoginId = 2, Email = "doctor@test.com", Password = "pass", Type = 2 };
        _loginRepoMock.Setup(r => r.ValidateLoginAsync("doctor@test.com", "pass", It.IsAny<CancellationToken>()))
            .ReturnsAsync(login);

        var model = CreatePageModel();

        // Act
        var result = await model.OnPostLoginAsync("doctor@test.com", "pass", CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/Doctor/DoctorHome");
    }

    [Fact]
    public async Task OnPostLoginAsync_WithValidAdminCredentials_RedirectsToAdminHome()
    {
        // Arrange
        var login = new LoginTableEntity { LoginId = 3, Email = "admin@test.com", Password = "pass", Type = 3 };
        _loginRepoMock.Setup(r => r.ValidateLoginAsync("admin@test.com", "pass", It.IsAny<CancellationToken>()))
            .ReturnsAsync(login);

        var model = CreatePageModel();

        // Act
        var result = await model.OnPostLoginAsync("admin@test.com", "pass", CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/Admin/AdminHome");
    }

    [Fact]
    public async Task OnPostLoginAsync_WithInvalidCredentials_ReturnsPageWithMessage()
    {
        // Arrange
        _loginRepoMock.Setup(r => r.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LoginTableEntity?)null);
        _loginRepoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var model = CreatePageModel();

        // Act
        var result = await model.OnPostLoginAsync("wrong@test.com", "wrongpass", CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.LoginMessage.Should().NotBeEmpty();
    }

    [Fact]
    public async Task OnPostSignupAsync_WithExistingEmail_ReturnsPageWithMessage()
    {
        // Arrange
        _loginRepoMock.Setup(r => r.EmailExistsAsync("existing@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var model = CreatePageModel();

        // Act
        var result = await model.OnPostSignupAsync(
            "John", "1990-01-01", "existing@test.com", "pass", "1234567890", "M", "Address",
            CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.SignupMessage.Should().Contain("Email already exists");
    }

    [Fact]
    public async Task OnPostSignupAsync_WithNewEmail_RedirectsToPatientHome()
    {
        // Arrange
        _loginRepoMock.Setup(r => r.EmailExistsAsync("new@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _loginRepoMock.Setup(r => r.AddAsync(It.IsAny<LoginTableEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);
        _patientRepoMock.Setup(r => r.AddAsync(It.IsAny<PatientEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var model = CreatePageModel();

        // Act
        var result = await model.OnPostSignupAsync(
            "New Patient", "1990-01-01", "new@test.com", "pass123", "1234567890", "M", "123 Main St",
            CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/Patient/PatientHome");
    }

    [Fact]
    public void SignUpModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel();

        // Assert
        model.LoginMessage.Should().BeEmpty();
        model.SignupMessage.Should().BeEmpty();
        model.SignupSuccess.Should().BeFalse();
    }
}
