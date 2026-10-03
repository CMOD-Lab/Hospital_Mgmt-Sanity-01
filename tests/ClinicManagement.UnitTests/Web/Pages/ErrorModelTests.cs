using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages;

/// <summary>
/// Unit tests for ErrorModel page.
/// </summary>
public class ErrorModelTests
{
    private ClinicManagement.Web.Pages.ErrorModel CreateSut()
    {
        var sut = new ClinicManagement.Web.Pages.ErrorModel();
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "test-trace-id";
        sut.PageContext = new PageContext { HttpContext = httpContext };
        return sut;
    }

    // ─── Constructor Tests ────────────────────────────────────────────────────

    [Fact]
    public void Constructor_CreatesInstance()
    {
        var model = new ClinicManagement.Web.Pages.ErrorModel();
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_RequestIdIsNull()
    {
        var sut = CreateSut();
        // Before OnGet is called, RequestId is null
        Assert.Null(sut.RequestId);
    }

    // ─── ShowRequestId Tests ──────────────────────────────────────────────────

    [Fact]
    public void ShowRequestId_WhenRequestIdIsNull_ReturnsFalse()
    {
        var sut = CreateSut();
        // RequestId is null initially
        Assert.False(sut.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsEmpty_ReturnsFalse()
    {
        var sut = CreateSut();
        sut.RequestId = "";
        Assert.False(sut.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdHasValue_ReturnsTrue()
    {
        var sut = CreateSut();
        sut.RequestId = "some-request-id";
        Assert.True(sut.ShowRequestId);
    }

    // ─── OnGet Tests ─────────────────────────────────────────────────────────

    [Fact]
    public void OnGet_WithTraceIdentifier_SetsRequestId()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        sut.OnGet();

        // Assert - TraceIdentifier is set to "test-trace-id" in CreateSut
        Assert.NotNull(sut.RequestId);
    }

    [Fact]
    public void OnGet_SetsRequestIdFromTraceIdentifier()
    {
        // Arrange
        var sut = new ClinicManagement.Web.Pages.ErrorModel();
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "my-trace-id-123";
        sut.PageContext = new PageContext { HttpContext = httpContext };

        // Act
        sut.OnGet();

        // Assert
        Assert.Equal("my-trace-id-123", sut.RequestId);
    }

    [Fact]
    public void OnGet_AfterCall_ShowRequestIdIsTrue()
    {
        // Arrange
        var sut = new ClinicManagement.Web.Pages.ErrorModel();
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "trace-123";
        sut.PageContext = new PageContext { HttpContext = httpContext };

        // Act
        sut.OnGet();

        // Assert
        Assert.True(sut.ShowRequestId);
    }
}
