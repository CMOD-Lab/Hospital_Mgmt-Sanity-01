using System;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Exceptions;
using ClinicManagement.UnitTests.Web.Helpers;
using ClinicManagement.Web.Pages.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Account;

public class SignUpModelTests
{
    private readonly Mock<IPatientService> _mockPatientService;
    private readonly Mock<ILogger<SignUpModel>> _mockLogger;
    private readonly SignUpModel _signUpModel;
    private readonly TestSession _testSession;

    public SignUpModelTests()
    {
        _mockPatientService = new Mock<IPatientService>();
        _mockLogger = new Mock<ILogger<SignUpModel>>();
        _signUpModel = new SignUpModel(_mockPatientService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);

        var pageContext = new PageContext { HttpContext = mockHttpContext.Object };
        _signUpModel.PageContext = pageContext;
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new SignUpModel(_mockPatientService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitializesInputProperty()
    {
        Assert.NotNull(_signUpModel.Input);
    }

    [Fact]
    public void OnGet_ReturnsPage()
    {
        var result = _signUpModel.OnGet();
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WhenModelStateInvalid_ReturnsPage()
    {
        _signUpModel.ModelState.AddModelError("Name", "Name is required");
        var result = await _signUpModel.OnPostAsync();
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WhenRegistrationSucceeds_RedirectsToPatientHome()
    {
        _signUpModel.Input = new SignUpModel.SignUpInputModel
        {
            Name = "John Doe", Email = "john@test.com", Password = "password123",
            Phone = "1234567890", Address = "123 Main St",
            BirthDate = new DateTime(1990, 1, 1), Gender = "M"
        };

        _mockPatientService.Setup(s => s.CreateAsync(It.IsAny<PatientCreateDto>(), default))
            .ReturnsAsync(new PatientDto { PatientId = 1, Name = "John Doe", Email = "john@test.com" });

        var result = await _signUpModel.OnPostAsync();

        Assert.IsType<RedirectToPageResult>(result);
        var redirect = (RedirectToPageResult)result;
        Assert.Equal("/Patient/Home", redirect.PageName);
    }

    [Fact]
    public async Task OnPostAsync_WhenDuplicateEmail_ReturnsPageWithErrorMessage()
    {
        _signUpModel.Input = new SignUpModel.SignUpInputModel
        {
            Name = "John Doe", Email = "existing@test.com", Password = "password123",
            Phone = "1234567890", Address = "123 Main St",
            BirthDate = new DateTime(1990, 1, 1), Gender = "M"
        };

        _mockPatientService.Setup(s => s.CreateAsync(It.IsAny<PatientCreateDto>(), default))
            .ThrowsAsync(new DuplicateEntityException("An account with this email already exists."));

        var result = await _signUpModel.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal("An account with this email already exists.", _signUpModel.ErrorMessage);
    }

    [Fact]
    public async Task OnPostAsync_WhenExceptionThrown_ReturnsPageWithErrorMessage()
    {
        _signUpModel.Input = new SignUpModel.SignUpInputModel
        {
            Name = "John Doe", Email = "john@test.com", Password = "password123",
            Phone = "1234567890", Address = "123 Main St",
            BirthDate = new DateTime(1990, 1, 1), Gender = "M"
        };

        _mockPatientService.Setup(s => s.CreateAsync(It.IsAny<PatientCreateDto>(), default))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _signUpModel.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal("An error occurred during registration. Please try again.", _signUpModel.ErrorMessage);
    }

    [Fact]
    public async Task OnPostAsync_WhenSuccessful_SetsSessionValues()
    {
        _signUpModel.Input = new SignUpModel.SignUpInputModel
        {
            Name = "Jane Doe", Email = "jane@test.com", Password = "password123",
            Phone = "9876543210", Address = "456 Oak Ave",
            BirthDate = new DateTime(1985, 6, 15), Gender = "F"
        };

        _mockPatientService.Setup(s => s.CreateAsync(It.IsAny<PatientCreateDto>(), default))
            .ReturnsAsync(new PatientDto { PatientId = 5, Name = "Jane Doe", Email = "jane@test.com" });

        await _signUpModel.OnPostAsync();

        // Verify session was set
        Assert.True(_testSession.TryGetValue("UserId", out _));
        Assert.True(_testSession.TryGetValue("UserType", out _));
    }

    [Fact]
    public void SignUpInputModel_DefaultValues_AreEmpty()
    {
        var input = new SignUpModel.SignUpInputModel();
        Assert.Equal(string.Empty, input.Name);
        Assert.Equal(string.Empty, input.Email);
        Assert.Equal(string.Empty, input.Password);
        Assert.Equal(string.Empty, input.Phone);
        Assert.Equal(string.Empty, input.Address);
        Assert.Equal(string.Empty, input.Gender);
    }

    [Fact]
    public void SignUpInputModel_CanSetAllProperties()
    {
        var birthDate = new DateTime(1990, 5, 20);
        var input = new SignUpModel.SignUpInputModel
        {
            Name = "Test User", Email = "test@example.com", Password = "password",
            Phone = "5551234567", Address = "789 Pine St", BirthDate = birthDate, Gender = "M"
        };

        Assert.Equal("Test User", input.Name);
        Assert.Equal("test@example.com", input.Email);
        Assert.Equal("password", input.Password);
        Assert.Equal("5551234567", input.Phone);
        Assert.Equal("789 Pine St", input.Address);
        Assert.Equal(birthDate, input.BirthDate);
        Assert.Equal("M", input.Gender);
    }
}
