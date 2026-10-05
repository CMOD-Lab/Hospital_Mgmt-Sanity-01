using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ClinicManagement.UnitTests.Services;

/// <summary>
/// Unit tests for AuthService.
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<ILoginRepository> _loginRepoMock;
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _loginRepoMock = new Mock<ILoginRepository>();
        _patientRepoMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<AuthService>>();
        _authService = new AuthService(_loginRepoMock.Object, _patientRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var login = new LoginTable { LoginId = 1, Email = "test@test.com", Password = "pass123", Type = 1 };
        _loginRepoMock.Setup(r => r.ValidateLoginAsync("test@test.com", "pass123", default))
            .ReturnsAsync(login);

        // Act
        var result = await _authService.ValidateLoginAsync(new LoginRequestDto
        {
            Email = "test@test.com",
            Password = "pass123"
        });

        // Assert
        result.Success.Should().BeTrue();
        result.UserId.Should().Be(1);
        result.UserType.Should().Be(1);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithInvalidEmail_ReturnsEmailNotFound()
    {
        // Arrange
        _loginRepoMock.Setup(r => r.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync((LoginTable?)null);
        _loginRepoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), default))
            .ReturnsAsync(false);

        // Act
        var result = await _authService.ValidateLoginAsync(new LoginRequestDto
        {
            Email = "notfound@test.com",
            Password = "pass123"
        });

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Email not found");
    }

    [Fact]
    public async Task ValidateLoginAsync_WithWrongPassword_ReturnsIncorrectPassword()
    {
        // Arrange
        _loginRepoMock.Setup(r => r.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync((LoginTable?)null);
        _loginRepoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), default))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.ValidateLoginAsync(new LoginRequestDto
        {
            Email = "test@test.com",
            Password = "wrongpass"
        });

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Incorrect Password");
    }

    [Fact]
    public async Task RegisterPatientAsync_WithExistingEmail_ReturnsFailure()
    {
        // Arrange
        _loginRepoMock.Setup(r => r.EmailExistsAsync("existing@test.com", default))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterPatientAsync(new PatientSignupDto
        {
            Email = "existing@test.com",
            Name = "Test",
            Password = "pass",
            BirthDate = "1990-01-01",
            Gender = "M"
        });

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Email already exists");
    }

    [Fact]
    public async Task RegisterPatientAsync_WithNewEmail_ReturnsSuccess()
    {
        // Arrange
        _loginRepoMock.Setup(r => r.EmailExistsAsync("new@test.com", default))
            .ReturnsAsync(false);
        _loginRepoMock.Setup(r => r.AddAsync(It.IsAny<LoginTable>(), default))
            .ReturnsAsync(42);
        _patientRepoMock.Setup(r => r.AddAsync(It.IsAny<Patient>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RegisterPatientAsync(new PatientSignupDto
        {
            Email = "new@test.com",
            Name = "New Patient",
            Password = "pass123",
            BirthDate = "1990-01-01",
            Gender = "M",
            PhoneNo = "12345678901",
            Address = "Test Address"
        });

        // Assert
        result.Success.Should().BeTrue();
        result.UserId.Should().Be(42);
        result.UserType.Should().Be(1);
    }
}
