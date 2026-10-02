// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//
// cr-dotnet-1034 (Line 22):
//   The auto-generated designer file previously declared a Web Forms server-control
//   field (System.Web.UI.WebControls.GridView pendingappointments).
//   This synchronous GridView control is no longer applicable in ASP.NET Core Razor Pages
//   where data is exposed via public properties on the PageModel class and rendered
//   asynchronously via async Task OnGetAsync() using Entity Framework Core connected
//   to Amazon RDS.
//
// The appointments data table is now a public DataTable? property on PendingAppointmentModel
// in PendingAppointment.aspx.cs, populated asynchronously via
// await _dal.GetAllpendingappointments_DAL_Async() using EF Core / SqlQueryRaw<T>().ToListAsync()
// connected to Amazon RDS, preventing thread-pool exhaustion under cloud load.
//
// This file is retained for project structure compatibility only.

namespace DBProject.Pages.Doctor
{
    // cr-dotnet-1034: Designer stub – Web Forms GridView server control removed.
    // The synchronous pendingappointments (System.Web.UI.WebControls.GridView) is replaced
    // by an HTML table in PendingAppointment.aspx bound to the async-loaded Appointments
    // DataTable property on PendingAppointmentModel (PendingAppointment.aspx.cs).
    // Data is fetched asynchronously via EF Core connected to Amazon RDS.
}
