// HospitalDbContext.cs — Entity Framework Core DbContext for Hospital Management System
//
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
// Remediation: Async GridView Data Binding with RDS via Entity Framework Core
//
// This DbContext provides the EF Core infrastructure required to replace synchronous
// GridView DataBind() calls with async Task-based patterns using SqlQueryRaw<T>
// connected to Amazon RDS, preventing thread-pool exhaustion under cloud load.
//
// Connection-string resolution order (12-factor / cloud-native):
//   1. Environment variable  RDS_PROXY_CONNECTION_STRING  (Amazon RDS Proxy endpoint)
//   2. Environment variable  DB_CONNECTION_STRING
//   3. Web.config / App.config  ConnectionStrings["sqlCon1"]  (local development fallback)

using System;
using Microsoft.EntityFrameworkCore;

namespace DBProject.DAL
{
    /// <summary>
    /// Entity Framework Core DbContext for the Hospital Management System.
    /// cr-dotnet-1034: Provides async data access via SqlQueryRaw&lt;T&gt;().ToListAsync()
    /// connected to Amazon RDS, replacing synchronous GridView DataBind() patterns.
    /// </summary>
    public class HospitalDbContext : DbContext
    {
        /// <summary>
        /// Initialises a new HospitalDbContext using the cloud-native connection string.
        /// Connection string is resolved from environment variables (RDS Proxy) with
        /// fallback to Web.config for local development.
        /// </summary>
        public HospitalDbContext() : base(BuildOptions())
        {
        }

        /// <summary>
        /// Initialises a new HospitalDbContext with explicit options (for DI / testing).
        /// </summary>
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Builds DbContextOptions using the cloud-native connection string.
        /// Resolves the connection string from environment variables first (12-factor),
        /// then falls back to Web.config for local development.
        /// </summary>
        private static DbContextOptions BuildOptions()
        {
            string connectionString = ResolveConnectionString();
            var optionsBuilder = new DbContextOptionsBuilder<HospitalDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            return optionsBuilder.Options;
        }

        /// <summary>
        /// Resolves the database connection string using cloud-native 12-factor ordering:
        /// 1. RDS_PROXY_CONNECTION_STRING environment variable (Amazon RDS Proxy)
        /// 2. DB_CONNECTION_STRING environment variable
        /// 3. Web.config ConnectionStrings["sqlCon1"] (local development fallback)
        /// </summary>
        private static string ResolveConnectionString()
        {
            // Prefer the RDS Proxy endpoint injected as an environment variable.
            string cs = Environment.GetEnvironmentVariable("RDS_PROXY_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(cs))
                return cs;

            cs = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(cs))
                return cs;

            // Fallback: read from Web.config / App.config (local development).
            return System.Configuration.ConfigurationManager
                         .ConnectionStrings["sqlCon1"]?.ConnectionString
                   ?? throw new InvalidOperationException(
                       "No database connection string found. Set the RDS_PROXY_CONNECTION_STRING " +
                       "or DB_CONNECTION_STRING environment variable, or configure sqlCon1 in Web.config.");
        }

        /// <summary>
        /// Configures the EF Core model.
        /// cr-dotnet-1034: No entity sets are required for SqlQueryRaw&lt;T&gt; usage;
        /// this context is used exclusively for raw async SQL queries against Amazon RDS.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Register keyless entity types used by SqlQueryRaw<T> async queries.
            // cr-dotnet-1034: These replace synchronous GridView DataBind() calls.

            // AdminHome projections (lines 44, 48)
            modelBuilder.Entity<ScalarIntResult>().HasNoKey();
            modelBuilder.Entity<ScalarStringResult>().HasNoKey();
            modelBuilder.Entity<DepartmentViewRow>().HasNoKey();
            modelBuilder.Entity<AppointmentViewRow>().HasNoKey();

            // ManageClinic projections (lines 40, 55, 75)
            modelBuilder.Entity<DoctorGridRow>().HasNoKey();
            modelBuilder.Entity<PatientGridRow>().HasNoKey();
            modelBuilder.Entity<StaffGridRow>().HasNoKey();
        }
    }
}
