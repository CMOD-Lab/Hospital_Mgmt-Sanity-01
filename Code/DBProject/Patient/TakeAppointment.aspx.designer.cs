// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls – replaced
//   synchronous GridView DataBind() with async Task-based OnGetAsync() using
//   async DAL methods connected to Amazon RDS via RDS Proxy, preventing thread-pool
//   exhaustion under cloud load and enabling efficient auto-scaling in AWS.
//
// Changes applied:
//   - removed: global::System.Web.UI.WebControls.Label TDept
//   - removed: global::System.Web.UI.WebControls.GridView TDeptGrid
//             (cr-dotnet-1034 Line 31: GridView server-control declaration removed)
//
// These Web Forms server-control field declarations are no longer required.
// The corresponding UI output is now handled by bound properties on
// TakeAppointmentModel (TDeptMessage, Departments) rendered directly in
// the TakeAppointment.aspx Razor Page view using async data binding.
// This file is retained as a placeholder to preserve project file references
// during the migration transition.

namespace DBProject.Pages.Patient
{
    // Placeholder: Web Forms designer file superseded by Razor PageModel.
    // All server-control declarations (including GridView TDeptGrid) have been removed.
    // cr-dotnet-1034: GridView synchronous data binding replaced by async Razor Page model.
}
