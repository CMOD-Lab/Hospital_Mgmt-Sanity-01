using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ClinicManagement.UnitTests.Web.Helpers;

/// <summary>
/// A simple in-memory session implementation for testing.
/// Uses the same byte encoding as ASP.NET Core's ISession extension methods.
/// </summary>
public class TestSession : ISession
{
    private readonly Dictionary<string, byte[]> _store = new();

    public bool IsAvailable => true;
    public string Id => Guid.NewGuid().ToString();
    public IEnumerable<string> Keys => _store.Keys;

    public void Clear() => _store.Clear();

    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public void Remove(string key) => _store.Remove(key);

    public void Set(string key, byte[] value) => _store[key] = value;

    public bool TryGetValue(string key, out byte[] value)
    {
        if (_store.TryGetValue(key, out var stored))
        {
            value = stored;
            return true;
        }
        value = Array.Empty<byte>();
        return false;
    }

    /// <summary>
    /// Sets an integer value using the same big-endian encoding as ASP.NET Core's SetInt32 extension.
    /// </summary>
    public void SetInt32(string key, int value)
    {
        // ASP.NET Core uses big-endian byte order for session integers
        Set(key, new byte[]
        {
            (byte)(value >> 24),
            (byte)(0xFF & (value >> 16)),
            (byte)(0xFF & (value >> 8)),
            (byte)(0xFF & value)
        });
    }

    /// <summary>
    /// Sets a string value in the session.
    /// </summary>
    public void SetString(string key, string value)
    {
        Set(key, System.Text.Encoding.UTF8.GetBytes(value));
    }
}
