// This file is intentionally left minimal.
// The Web Forms auto-generated designer file has been superseded by the
// ASP.NET Core Razor Pages migration (ViewDoctors.aspx / ViewDoctorsModel).
// Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//   The GridView server control (TDoctorGrid) has been replaced by async model-bound
//   DataTable rendering in the Razor view. OnGetAsync() / OnPostSelectDoctorAsync()
//   use async DAL methods (getDeptDoctorInfoAsync) connected to Amazon RDS via Dapper
//   async APIs, preventing thread pool exhaustion under load.

namespace DBProject.Patient
{
    // Designer stub retained for project compatibility during migration.
    // All UI controls are now defined as properties on ViewDoctorsModel (PageModel).
    // GridView TDoctorGrid replaced by DataTable Doctors property with async data loading.
}
