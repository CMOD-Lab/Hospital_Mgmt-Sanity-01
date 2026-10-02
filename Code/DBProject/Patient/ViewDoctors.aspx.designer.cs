// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls – replaced
//   synchronous GridView DataBind() with async Task-based OnGetAsync() using
//   async DAL methods connected to Amazon RDS via RDS Proxy, preventing thread-pool
//   exhaustion under cloud load and enabling efficient auto-scaling in AWS.
//
// Changes applied:
//   - removed: global::System.Web.UI.WebControls.Label TDoctor
//   - removed: global::System.Web.UI.WebControls.GridView TDoctorGrid
//             (cr-dotnet-1034 Line 31: GridView server-control declaration removed)
//
// These Web Forms server-control field declarations are no longer required.
// The corresponding UI output is now handled by bound properties on
// ViewDoctorsModel (TDoctorMessage, Doctors) rendered directly in
// the ViewDoctors.aspx Razor Page view using async data binding.
// This file is retained as a placeholder to preserve project file references
// during the migration transition.

namespace DBProject.Pages.Patient
{
    // Placeholder: Web Forms designer file superseded by Razor PageModel.
    // All server-control declarations (including GridView TDoctorGrid at line 31)
    // have been removed.
    // cr-dotnet-1034 (Line 31): GridView synchronous data binding replaced by
    // async Razor Page model (ViewDoctorsModel.OnGetAsync / DeptDoctorInfoAsync).
}
