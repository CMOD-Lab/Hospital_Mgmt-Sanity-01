using ClinicManagement.Application.Services;
using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Services;

/// <summary>Unit tests for AuthService.</summary>
public class AuthServiceTests
{
    private readonly Mock<ILoginRepository> _loginRepositoryMock;
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _loginRepositoryMock = new Mock<ILoginRepository>();
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<AuthService>>();
        _authService = new AuthService(
            _loginRepositoryMock.Object,
            _patientRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var login = new LoginTable { LoginID = 1, Email = email, Password = password, Type = 1 };

        _loginRepositoryMock
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(login);

        // Act
        var result = await _authService.ValidateLoginAsync(email, password);

        // Assert
        result.Status.Should().Be(0);
        result.UserId.Should().Be(1);
        result.UserType.Should().Be(1);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithNonExistentEmail_ReturnsEmailNotFound()
    {
        // Arrange
        _loginRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LoginTable?)null);

        // Act
        var result = await _authService.ValidateLoginAsync("notfound@example.com", "password");

        // Assert
        result.Status.Should().Be(1);
        result.ErrorMessage.Should().Contain("Email not found");
    }

    [Fact]
    public async Task ValidateLoginAsync_WithWrongPassword_ReturnsIncorrectPassword()
    {
        // Arrange
        var login = new LoginTable { LoginID = 1, Email = "test@example.com", Password = "correct", Type = 1 };

        _loginRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(login);

        // Act
        var result = await _authService.ValidateLoginAsync("test@example.com", "wrong");

        // Assert
        result.Status.Should().Be(2);
        result.ErrorMessage.Should().Contain("Incorrect Password");
    }

    [Fact]
    public async Task SignUpPatientAsync_WithExistingEmail_ReturnsEmailExists()
    {
        // Arrange
        _loginRepositoryMock
            .Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var dto = new PatientSignUpDto
        {
            Name = "Test Patient",
            Email = "existing@example.com",
            Password = "password",
            BirthDate = "1990-01-01",
            PhoneNo = "12345678901",
            Gender = "M",
            Address = "Test Address"
        };

        // Act
        var result = await _authService.SignUpPatientAsync(dto);

        // Assert
        result.Status.Should().Be(0);
        result.ErrorMessage.Should().Contain("Email already exists");
    }

    [Fact]
    public async Task SignUpPatientAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        _loginRepositoryMock
            .Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var createdLogin = new LoginTable { LoginID = 5, Email = "new@example.com", Password = "password", Type = 1 };
        _loginRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<LoginTable>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdLogin);

        _patientRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var dto = new PatientSignUpDto
        {
            Name = "New Patient",
            Email = "new@example.com",
            Password = "password",
            BirthDate = "1990-01-01",
            PhoneNo = "12345678901",
            Gender = "M",
            Address = "Test Address"
        };

        // Act
        var result = await _authService.SignUpPatientAsync(dto);

        // Assert
        result.Status.Should().Be(1);
        result.PatientId.Should().Be(5);
    }
}
