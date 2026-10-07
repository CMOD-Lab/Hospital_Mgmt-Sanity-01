using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

// cr-dotnet-0045: Session State Provider
// Distributed session helper backed by Amazon ElastiCache for Redis.
// Replaces in-process HttpSessionState with a Redis-backed IDistributedCache
// to enable stateless horizontal scaling across multiple ECS tasks or pods.

namespace DBProject.Infrastructure
{
    /// <summary>
    /// Provides strongly-typed, Redis-backed distributed session access using
    /// Amazon ElastiCache for Redis via IDistributedCache.
    /// Use this helper in place of HttpContext.Session to ensure all session
    /// data is stored in the external Redis cluster rather than in-process memory.
    /// </summary>
    public class RedisSessionHelper
    {
        private readonly IDistributedCache _cache;
        private readonly string _sessionId;

        private static readonly DistributedCacheEntryOptions DefaultOptions =
            new DistributedCacheEntryOptions
            {
                // Session sliding expiration: 20 minutes (matches ASP.NET Core default)
                SlidingExpiration = TimeSpan.FromMinutes(20)
            };

        public RedisSessionHelper(IDistributedCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            if (httpContextAccessor?.HttpContext == null)
                throw new ArgumentNullException(nameof(httpContextAccessor));

            // Use the ASP.NET Core session ID as the Redis key prefix so each
            // user's data is isolated in the shared ElastiCache cluster.
            _sessionId = httpContextAccessor.HttpContext.Session.Id;
        }

        // ------------------------------------------------------------------ //
        //  String helpers                                                      //
        // ------------------------------------------------------------------ //

        /// <summary>Sets a string value in the Redis-backed distributed session.</summary>
        public void SetString(string key, string value)
        {
            var redisKey = BuildKey(key);
            var bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
            _cache.Set(redisKey, bytes, DefaultOptions);
        }

        /// <summary>Gets a string value from the Redis-backed distributed session.</summary>
        public string GetString(string key)
        {
            var redisKey = BuildKey(key);
            var bytes = _cache.Get(redisKey);
            return bytes == null ? null : Encoding.UTF8.GetString(bytes);
        }

        // ------------------------------------------------------------------ //
        //  Integer helpers                                                     //
        // ------------------------------------------------------------------ //

        /// <summary>Sets an integer value in the Redis-backed distributed session.</summary>
        public void SetInt32(string key, int value)
        {
            SetString(key, value.ToString());
        }

        /// <summary>
        /// Gets an integer value from the Redis-backed distributed session.
        /// Returns null if the key does not exist or cannot be parsed.
        /// </summary>
        public int? GetInt32(string key)
        {
            var raw = GetString(key);
            if (raw == null) return null;
            return int.TryParse(raw, out var result) ? result : (int?)null;
        }

        // ------------------------------------------------------------------ //
        //  Remove / Clear                                                      //
        // ------------------------------------------------------------------ //

        /// <summary>Removes a single key from the Redis-backed distributed session.</summary>
        public void Remove(string key)
        {
            _cache.Remove(BuildKey(key));
        }

        // ------------------------------------------------------------------ //
        //  Private helpers                                                     //
        // ------------------------------------------------------------------ //

        private string BuildKey(string key) => $"session:{_sessionId}:{key}";
    }
}
