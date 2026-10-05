using Microsoft.AspNetCore.Http;

namespace ClinicManagement.UnitTests.Helpers;

/// <summary>
/// A simple in-memory mock implementation of ISession for unit testing.
/// Supports SetInt32/GetInt32 via the standard ASP.NET Core session extension format.
/// </summary>
public class MockSession : ISession
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
        return _store.TryGetValue(key, out value!);
    }

    /// <summary>
    /// Sets an integer value using the same encoding as ASP.NET Core's SetInt32 extension.
    /// ASP.NET Core stores int as 4 bytes big-endian.
    /// </summary>
    public void SetInt32(string key, int value)
    {
        var bytes = new byte[]
        {
            (byte)(value >> 24),
            (byte)(0xFF & (value >> 16)),
            (byte)(0xFF & (value >> 8)),
            (byte)(0xFF & value)
        };
        _store[key] = bytes;
    }
}
