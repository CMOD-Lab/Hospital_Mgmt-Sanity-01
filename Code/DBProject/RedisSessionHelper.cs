using System;
using System.Web;
using System.Web.SessionState;
using System.Configuration;

namespace DBProject
{
    /// <summary>
    /// RedisSessionHelper provides a distributed, Redis-backed session abstraction
    /// for containerized deployments on Amazon EKS with ElastiCache.
    ///
    /// The Redis endpoint is configured via the REDIS_CONNECTION_STRING environment
    /// variable (or the "RedisConnectionString" appSetting in Web.config as a fallback).
    /// Session state is stored in the ASP.NET distributed session provider configured
    /// in Web.config using Microsoft.Web.RedisSessionStateProvider.
    ///
    /// Usage:
    ///   Read  : RedisSessionHelper.GetValue&lt;T&gt;(Session, "key")
    ///   Write : RedisSessionHelper.SetValue(Session, "key", value)
    ///   Remove: RedisSessionHelper.RemoveValue(Session, "key")
    /// </summary>
    public static class RedisSessionHelper
    {
        /// <summary>
        /// Retrieves a typed value from the distributed session store.
        /// Returns the default value of T if the key is not present.
        /// </summary>
        public static T GetValue<T>(HttpSessionState session, string key)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            object value = session[key];
            if (value == null)
                return default(T);

            try
            {
                return (T)value;
            }
            catch (InvalidCastException)
            {
                // Attempt string conversion for primitive types stored as strings
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        /// <summary>
        /// Stores a value in the distributed session store under the given key.
        /// </summary>
        public static void SetValue(HttpSessionState session, string key, object value)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            session[key] = value;
        }

        /// <summary>
        /// Removes a key from the distributed session store.
        /// </summary>
        public static void RemoveValue(HttpSessionState session, string key)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            session.Remove(key);
        }

        /// <summary>
        /// Returns the Redis connection string from environment variable
        /// REDIS_CONNECTION_STRING, falling back to the Web.config appSetting.
        /// </summary>
        public static string GetRedisConnectionString()
        {
            string connStr = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(connStr))
                return connStr;

            // Fallback to Web.config appSetting
            connStr = ConfigurationManager.AppSettings["RedisConnectionString"];
            if (!string.IsNullOrEmpty(connStr))
                return connStr;

            throw new InvalidOperationException(
                "Redis connection string is not configured. " +
                "Set the REDIS_CONNECTION_STRING environment variable or " +
                "add a 'RedisConnectionString' appSetting in Web.config.");
        }
    }
}
