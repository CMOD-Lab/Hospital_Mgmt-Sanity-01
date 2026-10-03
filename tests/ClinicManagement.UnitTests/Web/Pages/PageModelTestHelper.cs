using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages;

/// <summary>
/// Shared test helpers for setting up PageModel HttpContext with session mocking.
/// </summary>
public static class PageModelTestHelper
{
    /// <summary>
    /// Creates a mock ISession that returns the given userId for "UserId" key.
    /// </summary>
    public static ISession CreateSessionWithUserId(int? userId)
    {
        var mockSession = new Mock<ISession>();

        if (userId.HasValue)
        {
            var bytes = BitConverter.GetBytes(userId.Value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            byte[] outBytes = bytes;
            mockSession
                .Setup(s => s.TryGetValue("UserId", out outBytes!))
                .Returns(true);
        }
        else
        {
            byte[]? nullBytes = null;
            mockSession
                .Setup(s => s.TryGetValue("UserId", out nullBytes!))
                .Returns(false);
        }

        mockSession.Setup(s => s.Clear());
        mockSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()));

        return mockSession.Object;
    }

    /// <summary>
    /// Attaches a mock HttpContext with session to the given PageModel.
    /// </summary>
    public static void SetupPageModelContext(PageModel pageModel, int? userId)
    {
        var session = CreateSessionWithUserId(userId);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = session;

        var pageContext = new PageContext
        {
            HttpContext = httpContext
        };
        pageModel.PageContext = pageContext;
    }
}
