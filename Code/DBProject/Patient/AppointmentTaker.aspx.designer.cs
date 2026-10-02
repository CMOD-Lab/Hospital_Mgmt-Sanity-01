// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//
// cr-dotnet-1034 (Line 31):
//   The auto-generated designer file previously declared Web Forms server-control
//   fields (System.Web.UI.WebControls.Label PAppointment and
//   System.Web.UI.WebControls.GridView PAppointmentGrid).
//   These synchronous Web Forms controls are no longer applicable in ASP.NET Core
//   Razor Pages where data is exposed via public properties on the PageModel class
//   and rendered asynchronously via async Task OnGetAsync() using Entity Framework
//   Core connected to Amazon RDS.
//
// The free slots data table is now a public DataTable? property on AppointmentTakerModel
// in AppointmentTaker.aspx.cs, populated asynchronously via
// await _dal.getFreeSlots_Async() using EF Core / SqlQueryRaw<T>().ToListAsync()
// connected to Amazon RDS, preventing thread-pool exhaustion under cloud load.
//
// The status label (PAppointment) is replaced by the string StatusMessage property on
// AppointmentTakerModel, rendered inline in the Razor Page template.
//
// This file is retained for project structure compatibility only.

namespace DBProject.Pages.Patient
{
    // cr-dotnet-1034: Designer stub – Web Forms GridView and Label server controls removed.
    // The synchronous PAppointmentGrid (System.Web.UI.WebControls.GridView) is replaced
    // by an HTML table in AppointmentTaker.aspx bound to the async-loaded FreeSlots
    // DataTable property on AppointmentTakerModel (AppointmentTaker.aspx.cs).
    // The synchronous PAppointment (System.Web.UI.WebControls.Label) is replaced by the
    // StatusMessage string property on AppointmentTakerModel.
    // Data is fetched asynchronously via EF Core connected to Amazon RDS.
}
