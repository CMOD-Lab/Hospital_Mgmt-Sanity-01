using System;
using Microsoft.AspNetCore.Http;
using Moq;

namespace ClinicManagement.UnitTests.Web.Helpers;

/// <summary>
/// Helper class for setting up mock sessions in tests.
/// </summary>
public static class SessionTestHelper
{
    /// <summary>
    /// Sets up a mock session to simulate a logged-in user.
    /// </summary>
    public static void SetupLoggedInUser(Mock<ISession> mockSession, int userId, int userType, string email = "test@test.com")
    {
        var userIdBytes = BitConverter.GetBytes(userId);
        var userTypeBytes = BitConverter.GetBytes(userType);
        var emailBytes = System.Text.Encoding.UTF8.GetBytes(email);

        mockSession.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(true);
        mockSession.Setup(s => s.TryGetValue("UserType", out userTypeBytes)).Returns(true);
        mockSession.Setup(s => s.TryGetValue("UserEmail", out emailBytes)).Returns(true);
    }

    /// <summary>
    /// Sets up a mock session to simulate a logged-out user.
    /// </summary>
    public static void SetupLoggedOutUser(Mock<ISession> mockSession)
    {
        byte[]? nullBytes = null;
        mockSession.Setup(s => s.TryGetValue("UserId", out nullBytes)).Returns(false);
        mockSession.Setup(s => s.TryGetValue("UserType", out nullBytes)).Returns(false);
    }
}
