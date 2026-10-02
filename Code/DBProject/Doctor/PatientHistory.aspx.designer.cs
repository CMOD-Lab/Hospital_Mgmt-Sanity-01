// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//
// cr-dotnet-1034 (Line 22):
//   The auto-generated designer file previously declared a Web Forms server-control
//   field (System.Web.UI.WebControls.GridView patientsgrid).
//   This synchronous GridView control is no longer applicable in ASP.NET Core Razor Pages
//   where data is exposed via public properties on the PageModel class and rendered
//   asynchronously via async Task OnGetAsync() using Entity Framework Core connected
//   to Amazon RDS.
//
// The patients data table is now a public DataTable? property on PatientHistoryModel
// in PatientHistory.aspx.cs, populated asynchronously via
// await _dal.search_patient_DAL_Async() using EF Core / SqlQueryRaw<T>().ToListAsync()
// connected to Amazon RDS, preventing thread-pool exhaustion under cloud load.
//
// This file is retained for project structure compatibility only.

namespace DBProject.Pages.Doctor
{
    // cr-dotnet-1034: Designer stub – Web Forms GridView server control removed.
    // The synchronous patientsgrid (System.Web.UI.WebControls.GridView) is replaced
    // by an HTML table in PatientHistory.aspx bound to the async-loaded Patients
    // DataTable property on PatientHistoryModel (PatientHistory.aspx.cs).
    // Data is fetched asynchronously via EF Core connected to Amazon RDS.
}
