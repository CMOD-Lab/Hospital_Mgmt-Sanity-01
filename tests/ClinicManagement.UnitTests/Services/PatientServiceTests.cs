using Xunit;
using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Mappings;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Exceptions;
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
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<PatientService>> _loggerMock;
    private readonly PatientService _sut;

    public PatientServiceTests()
    {
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<PatientService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _sut = new PatientService(_patientRepositoryMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPatients()
    {
        // Arrange
        var patients = new List<Patient>
        {
            new() { PatientId = 1, Name = "John Doe", Email = "john@test.com", IsActive = true },
            new() { PatientId = 2, Name = "Jane Doe", Email = "jane@test.com", IsActive = true }
        };
        _patientRepositoryMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(patients);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "John Doe");
    }

    [Fact]
    public async Task GetByIdAsync_WhenPatientExists_ShouldReturnPatient()
    {
        // Arrange
        var patient = new Patient { PatientId = 1, Name = "John Doe", Email = "john@test.com", IsActive = true };
        _patientRepositoryMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(patient);

        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetByIdAsync_WhenPatientNotFound_ShouldReturnNull()
    {
        // Arrange
        _patientRepositoryMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((Patient?)null);

        // Act
        var result = await _sut.GetByIdAsync(99);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WhenEmailNotExists_ShouldCreatePatient()
    {
        // Arrange
        var createDto = new PatientCreateDto
        {
            Name = "New Patient",
            Email = "new@test.com",
            Password = "password",
            Phone = "1234567890",
            Address = "123 Main St",
            BirthDate = new DateTime(1990, 1, 1),
            Gender = "M"
        };

        _patientRepositoryMock.Setup(r => r.GetByEmailAsync(createDto.Email, default)).ReturnsAsync((Patient?)null);
        _patientRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Patient>(), default))
            .ReturnsAsync((Patient p, CancellationToken _) => { p.PatientId = 1; return p; });

        // Act
        var result = await _sut.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Patient");
        _patientRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Patient>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailExists_ShouldThrowDuplicateEntityException()
    {
        // Arrange
        var createDto = new PatientCreateDto { Email = "existing@test.com" };
        var existingPatient = new Patient { Email = "existing@test.com" };
        _patientRepositoryMock.Setup(r => r.GetByEmailAsync(createDto.Email, default)).ReturnsAsync(existingPatient);

        // Act & Assert
        await _sut.Invoking(s => s.CreateAsync(createDto))
            .Should().ThrowAsync<DuplicateEntityException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenPatientExists_ShouldDelete()
    {
        // Arrange
        _patientRepositoryMock.Setup(r => r.ExistsAsync(1, default)).ReturnsAsync(true);

        // Act
        await _sut.DeleteAsync(1);

        // Assert
        _patientRepositoryMock.Verify(r => r.DeleteAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenPatientNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _patientRepositoryMock.Setup(r => r.ExistsAsync(99, default)).ReturnsAsync(false);

        // Act & Assert
        await _sut.Invoking(s => s.DeleteAsync(99))
            .Should().ThrowAsync<NotFoundException>();
    }
}
