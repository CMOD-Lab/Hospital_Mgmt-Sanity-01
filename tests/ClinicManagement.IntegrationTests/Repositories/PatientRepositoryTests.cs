using Xunit;
using Moq;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Data;
using ClinicManagement.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace ClinicManagement.IntegrationTests.Repositories;

/// <summary>
/// Integration tests for PatientRepository using in-memory database.
/// </summary>
public class PatientRepositoryTests : IDisposable
{
    private readonly ClinicDbContext _context;
    private readonly PatientRepository _sut;

    public PatientRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ClinicDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ClinicDbContext(options);
        var loggerMock = new Mock<ILogger<PatientRepository>>();
        _sut = new PatientRepository(_context, loggerMock.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldAddPatient()
    {
        // Arrange
        var patient = new Patient
        {
            Name = "Test Patient",
            Email = "test@test.com",
            Password = "password",
            Phone = "1234567890",
            Address = "123 Main St",
            BirthDate = new DateTime(1990, 1, 1),
            Gender = "M",
            IsActive = true
        };

        // Act
        var result = await _sut.AddAsync(patient);

        // Assert
        result.PatientId.Should().BeGreaterThan(0);
        result.Name.Should().Be("Test Patient");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActivePatients()
    {
        // Arrange
        _context.Patients.AddRange(
            new Patient { Name = "Active", Email = "active@test.com", Password = "p", IsActive = true },
            new Patient { Name = "Inactive", Email = "inactive@test.com", Password = "p", IsActive = false }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Active");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeactivatePatient()
    {
        // Arrange
        var patient = new Patient { Name = "To Delete", Email = "delete@test.com", Password = "p", IsActive = true };
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Act
        await _sut.DeleteAsync(patient.PatientId);

        // Assert
        var deleted = await _context.Patients.FindAsync(patient.PatientId);
        deleted!.IsActive.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
