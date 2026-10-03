// Program.cs – ASP.NET Core entry point for Hospital Management System.
//
// cr-dotnet-0045 – Session State Provider (Amazon ElastiCache for Redis):
//   Replaces in-process (InProc) HttpSessionState with a distributed session
//   store backed by Amazon ElastiCache for Redis.
//
// cr-dotnet-0126 – Heavy Coupling to Stateful Middleware (Amazon ElastiCache for Redis):
//   Replaces IIS application pool sticky sessions and in-process session state
//   with Amazon ElastiCache for Redis distributed cache session store.
//   Session data persists across pod/container restarts and scales horizontally
//   without sticky session routing. Eliminates server affinity requirements.
//
//   Affected files and session keys:
//     Doctor/PatientHistory.aspx.cs  (Line 46)  – Session["appointid"]
//     Patient/AppointmentTaker.aspx.cs (Lines 17, 33) – Session["freeSlot"], Session["dID"]
//     Patient/PatientFeedback.aspx.cs (Lines 21, 56)  – Session["aID"]
//     Patient/TakeAppointment.aspx.cs (Lines 17, 31)  – Session["deptOriginal"]
//     Patient/ViewDoctors.aspx.cs (Lines 17, 30)      – Session["dID"]
//     SignUp.aspx.cs (Line 17)                         – Session["idoriginal"]
//
//   Configuration:
//     - Redis connection string is read from the REDIS_CONNECTION_STRING
//       environment variable (set in ECS task definition, EKS secret, or
//       AWS Systems Manager Parameter Store).
//     - Session idle timeout defaults to 30 minutes (configurable via
//       SESSION_TIMEOUT_MINUTES environment variable).
//     - Secure, HttpOnly session cookies are enforced.
//
//   Required NuGet packages:
//     - Microsoft.Extensions.Caching.StackExchangeRedis 7.0.0
//     - StackExchange.Redis 2.6.122
//     - Microsoft.AspNetCore.Session (included in ASP.NET Core framework)
//
//   This enables stateless horizontal scaling across multiple ECS tasks or
//   Kubernetes pods without server affinity (sticky sessions).

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────────────
// cr-dotnet-0045 / cr-dotnet-0126:
//   Distributed session store using Amazon ElastiCache for Redis.
//   Replaces InProc HttpSessionState (IIS sticky sessions) which creates server
//   affinity and prevents horizontal scaling across multiple ECS tasks or pods.
//   Session data persists across pod restarts; no sticky session routing needed.
// ─────────────────────────────────────────────────────────────────────────────

// Read Redis connection string from environment variable (cloud-native pattern).
// Set REDIS_CONNECTION_STRING in ECS task definition, EKS secret, or
// AWS Systems Manager Parameter Store / Secrets Manager.
// Format: <elasticache-endpoint>:<port>,password=<auth-token>,ssl=True,abortConnect=False
string redisConnectionString =
    Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("Redis")
    ?? "localhost:6379";

// Read session timeout from environment variable (default: 30 minutes).
int sessionTimeoutMinutes = 30;
string sessionTimeoutEnv = Environment.GetEnvironmentVariable("SESSION_TIMEOUT_MINUTES");
if (!string.IsNullOrEmpty(sessionTimeoutEnv) && int.TryParse(sessionTimeoutEnv, out int parsedTimeout))
{
    sessionTimeoutMinutes = parsedTimeout;
}

// cr-dotnet-0045 / cr-dotnet-0126:
// Register Amazon ElastiCache for Redis as the distributed cache backing store.
// This replaces the default in-memory (InProc) session provider and eliminates
// IIS application pool sticky session dependency.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName  = "HospitalMgmt:";
});

// cr-dotnet-0045 / cr-dotnet-0126:
// Configure ASP.NET Core session middleware backed by the Redis distributed cache.
// Secure, HttpOnly cookies prevent client-side session hijacking.
// No server affinity required – any pod can serve any request.
builder.Services.AddSession(options =>
{
    options.IdleTimeout         = TimeSpan.FromMinutes(sessionTimeoutMinutes);
    options.Cookie.HttpOnly     = true;
    options.Cookie.IsEssential  = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite     = SameSiteMode.Strict;
    options.Cookie.Name         = ".HospitalMgmt.Session";
});

// ─────────────────────────────────────────────────────────────────────────────
// Standard ASP.NET Core MVC services
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// cr-dotnet-0045 / cr-dotnet-0126:
// Enable session middleware (must be placed before MapControllerRoute).
// Session data is stored in Amazon ElastiCache for Redis, not in-process memory.
// Eliminates IIS sticky session routing; enables stateless horizontal scaling.
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
