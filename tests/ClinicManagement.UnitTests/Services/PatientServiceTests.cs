using Xunit;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ClinicManagement.UnitTests.Services;

/// <summary>
/// Unit tests for PatientService.
/// </summary>
public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<ILogger<PatientService>> _loggerMock;
    private readonly PatientService _patientService;

    public PatientServiceTests()
    {
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<PatientService>>();
        _patientService = new PatientService(_patientRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPatients()
    {
        // Arrange
        var patients = new List<Patient>
        {
            new() { PatientId = 1, Name = "John Doe", Email = "john@test.com", IsActive = true },
            new() { PatientId = 2, Name = "Jane Smith", Email = "jane@test.com", IsActive = true }
        };
        _patientRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(patients);

        // Act
        var result = await _patientService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "John Doe");
        result.Should().Contain(p => p.Name == "Jane Smith");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnPatient()
    {
        // Arrange
        var patient = new Patient { PatientId = 1, Name = "John Doe", Email = "john@test.com", IsActive = true };
        _patientRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        // Act
        var result = await _patientService.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.PatientId.Should().Be(1);
        result.Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _patientRepositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient?)null);

        // Act
        var result = await _patientService.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreatePatient()
    {
        // Arrange
        var dto = new PatientCreateDto
        {
            Name = "New Patient",
            Email = "new@test.com",
            Password = "password123",
            Phone = "1234567890",
            BirthDate = new DateTime(1990, 1, 1),
            Gender = "M",
            Address = "123 Test St"
        };

        _patientRepositoryMock.Setup(r => r.EmailExistsAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _patientRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient p, CancellationToken _) => { p.PatientId = 1; return p; });

        // Act
        var result = await _patientService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Patient");
        result.Email.Should().Be("new@test.com");
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateEmail_ShouldThrowException()
    {
        // Arrange
        var dto = new PatientCreateDto { Email = "existing@test.com", Name = "Test", Password = "pass" };
        _patientRepositoryMock.Setup(r => r.EmailExistsAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await _patientService.Invoking(s => s.CreateAsync(dto))
            .Should().ThrowAsync<Exception>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task ValidateLoginAsync_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var patient = new Patient { PatientId = 1, Name = "John Doe", Email = "john@test.com" };
        _patientRepositoryMock.Setup(r => r.ValidateLoginAsync("john@test.com", "password", It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        // Act
        var result = await _patientService.ValidateLoginAsync("john@test.com", "password");

        // Assert
        result.Success.Should().BeTrue();
        result.UserId.Should().Be(1);
        result.UserType.Should().Be("Patient");
    }

    [Fact]
    public async Task ValidateLoginAsync_WithInvalidCredentials_ShouldReturnFailure()
    {
        // Arrange
        _patientRepositoryMock.Setup(r => r.ValidateLoginAsync("wrong@test.com", "wrong", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient?)null);

        // Act
        var result = await _patientService.ValidateLoginAsync("wrong@test.com", "wrong");

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }
}
