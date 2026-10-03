using System;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Web.Pages.Account;
using ClinicManagement.UnitTests.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Account;

public class LoginModelTests
{
    private readonly Mock<IPatientService> _mockPatientService;
    private readonly Mock<ILogger<LoginModel>> _mockLogger;
    private readonly LoginModel _loginModel;
    private readonly TestSession _testSession;
    private readonly DefaultHttpContext _httpContext;

    public LoginModelTests()
    {
        _mockPatientService = new Mock<IPatientService>();
        _mockLogger = new Mock<ILogger<LoginModel>>();
        _loginModel = new LoginModel(_mockPatientService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        _httpContext = new DefaultHttpContext();
        _httpContext.Session = _testSession;

        var pageContext = new PageContext
        {
            HttpContext = _httpContext
        };
        _loginModel.PageContext = pageContext;
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Arrange & Act
        var model = new LoginModel(_mockPatientService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitializesInputProperty()
    {
        // Assert
        Assert.NotNull(_loginModel.Input);
    }

    [Fact]
    public void OnGet_WhenUserNotLoggedIn_ReturnsPage()
    {
        // Arrange - no session values set

        // Act
        var result = _loginModel.OnGet();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public void OnGet_WhenUserAlreadyLoggedInAsAdmin_RedirectsToDashboard()
    {
        // Arrange
        _testSession.SetInt32("UserId", 1);
        _testSession.SetInt32("UserType", 0); // Admin

        // Act
        var result = _loginModel.OnGet();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        var redirect = (RedirectToPageResult)result;
        Assert.Equal("/Admin/Dashboard", redirect.PageName);
    }

    [Fact]
    public void OnGet_WhenUserAlreadyLoggedInAsDoctor_RedirectsToDoctorHome()
    {
        // Arrange
        _testSession.SetInt32("UserId", 2);
        _testSession.SetInt32("UserType", 1); // Doctor

        // Act
        var result = _loginModel.OnGet();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        var redirect = (RedirectToPageResult)result;
        Assert.Equal("/Doctor/Home", redirect.PageName);
    }

    [Fact]
    public void OnGet_WhenUserAlreadyLoggedInAsPatient_RedirectsToPatientHome()
    {
        // Arrange
        _testSession.SetInt32("UserId", 3);
        _testSession.SetInt32("UserType", 2); // Patient

        // Act
        var result = _loginModel.OnGet();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        var redirect = (RedirectToPageResult)result;
        Assert.Equal("/Patient/Home", redirect.PageName);
    }

    [Fact]
    public async Task OnPostAsync_WhenModelStateInvalid_ReturnsPage()
    {
        // Arrange
        _loginModel.ModelState.AddModelError("Email", "Email is required");

        // Act
        var result = await _loginModel.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WhenLoginFails_ReturnsPageWithErrorMessage()
    {
        // Arrange
        _loginModel.Input = new LoginModel.LoginInputModel
        {
            Email = "test@test.com",
            Password = "wrongpassword"
        };

        _mockPatientService.Setup(s => s.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync(new LoginResultDto { Success = false });

        // Act
        var result = await _loginModel.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Invalid email or password. Please try again.", _loginModel.ErrorMessage);
    }

    [Fact]
    public async Task OnPostAsync_WhenLoginSucceedsAsAdmin_RedirectsToDashboard()
    {
        // Arrange
        _loginModel.Input = new LoginModel.LoginInputModel
        {
            Email = "admin@test.com",
            Password = "password"
        };

        _mockPatientService.Setup(s => s.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync(new LoginResultDto { Success = true, UserId = 1, UserType = 0 });

        // Act
        var result = await _loginModel.OnPostAsync();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        var redirect = (RedirectToPageResult)result;
        Assert.Equal("/Admin/Dashboard", redirect.PageName);
    }

    [Fact]
    public async Task OnPostAsync_WhenLoginSucceedsAsDoctor_RedirectsToDoctorHome()
    {
        // Arrange
        _loginModel.Input = new LoginModel.LoginInputModel
        {
            Email = "doctor@test.com",
            Password = "password"
        };

        _mockPatientService.Setup(s => s.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync(new LoginResultDto { Success = true, UserId = 2, UserType = 1 });

        // Act
        var result = await _loginModel.OnPostAsync();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        var redirect = (RedirectToPageResult)result;
        Assert.Equal("/Doctor/Home", redirect.PageName);
    }

    [Fact]
    public async Task OnPostAsync_WhenLoginSucceedsAsPatient_RedirectsToPatientHome()
    {
        // Arrange
        _loginModel.Input = new LoginModel.LoginInputModel
        {
            Email = "patient@test.com",
            Password = "password"
        };

        _mockPatientService.Setup(s => s.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync(new LoginResultDto { Success = true, UserId = 3, UserType = 2 });

        // Act
        var result = await _loginModel.OnPostAsync();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        var redirect = (RedirectToPageResult)result;
        Assert.Equal("/Patient/Home", redirect.PageName);
    }

    [Fact]
    public async Task OnPostAsync_WhenExceptionThrown_ReturnsPageWithErrorMessage()
    {
        // Arrange
        _loginModel.Input = new LoginModel.LoginInputModel
        {
            Email = "test@test.com",
            Password = "password"
        };

        _mockPatientService.Setup(s => s.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _loginModel.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("An error occurred during login. Please try again.", _loginModel.ErrorMessage);
    }

    [Fact]
    public void LoginInputModel_DefaultValues_AreEmpty()
    {
        // Arrange & Act
        var input = new LoginModel.LoginInputModel();

        // Assert
        Assert.Equal(string.Empty, input.Email);
        Assert.Equal(string.Empty, input.Password);
    }

    [Fact]
    public void LoginInputModel_CanSetProperties()
    {
        // Arrange
        var input = new LoginModel.LoginInputModel
        {
            Email = "test@example.com",
            Password = "securepassword"
        };

        // Assert
        Assert.Equal("test@example.com", input.Email);
        Assert.Equal("securepassword", input.Password);
    }
}
