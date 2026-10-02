// cr-dotnet-0045: Session State Provider
// Distributed session configuration helper for Amazon ElastiCache for Redis.
//
// This file provides the ASP.NET Core service-registration extension that
// replaces the legacy InProc HttpSessionState with a Redis-backed distributed
// session store, enabling stateless horizontal scaling across multiple ECS
// tasks or Kubernetes pods on AWS.
//
// Required NuGet packages (add to project):
//   Microsoft.Extensions.Caching.StackExchangeRedis  (>= 7.0.0)
//   Microsoft.AspNetCore.Session                      (>= 2.2.0)
//
// Environment variables consumed at runtime:
//   REDIS_CONNECTION_STRING  – ElastiCache Redis primary endpoint, e.g.
//                              "my-cluster.abc123.ng.0001.use1.cache.amazonaws.com:6379"
//   SESSION_TIMEOUT_MINUTES  – idle session timeout in minutes (default: 30)
//
// Usage in Program.cs / Startup.cs:
//   builder.Services.AddRedisDistributedSession(builder.Configuration);
//   app.UseSession();

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DBProject.Session
{
    /// <summary>
    /// Extension methods that register Amazon ElastiCache for Redis as the
    /// distributed session backing store, replacing the legacy InProc
    /// HttpSessionState provider (cr-dotnet-0045).
    /// </summary>
    public static class RedisSessionConfiguration
    {
        /// <summary>
        /// Registers Redis-backed distributed session services.
        /// Call this from Program.cs before <c>app.UseSession()</c>.
        /// </summary>
        /// <param name="services">The application service collection.</param>
        /// <param name="configuration">Application configuration (reads environment variables).</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddRedisDistributedSession(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Read the ElastiCache Redis connection string from environment variable.
            // Set REDIS_CONNECTION_STRING in your ECS task definition, Elastic Beanstalk
            // environment properties, or AWS Systems Manager Parameter Store / Secrets Manager.
            string redisConnectionString =
                Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING")
                ?? configuration["Redis:ConnectionString"]
                ?? throw new InvalidOperationException(
                    "Redis connection string not configured. " +
                    "Set the REDIS_CONNECTION_STRING environment variable to the " +
                    "Amazon ElastiCache for Redis primary endpoint.");

            // Read optional session timeout (default 30 minutes).
            int timeoutMinutes = 30;
            string? timeoutEnv = Environment.GetEnvironmentVariable("SESSION_TIMEOUT_MINUTES");
            if (!string.IsNullOrEmpty(timeoutEnv) && int.TryParse(timeoutEnv, out int parsed))
            {
                timeoutMinutes = parsed;
            }

            // Register StackExchange.Redis as the IDistributedCache implementation.
            // This replaces the InProc session provider with Amazon ElastiCache for Redis.
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName  = "HospitalMgmt_";   // key prefix to avoid collisions
            });

            // Register ASP.NET Core session middleware backed by the Redis IDistributedCache.
            services.AddSession(options =>
            {
                options.IdleTimeout        = TimeSpan.FromMinutes(timeoutMinutes);
                options.Cookie.HttpOnly    = true;
                options.Cookie.IsEssential = true;   // required for GDPR compliance
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
            });

            return services;
        }
    }
}
