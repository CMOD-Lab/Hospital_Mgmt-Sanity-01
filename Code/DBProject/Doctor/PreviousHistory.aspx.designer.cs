// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//
// cr-dotnet-1034 (Line 22):
//   The auto-generated designer file previously declared Web Forms server-control
//   fields (System.Web.UI.WebControls.Label PHistory and
//   System.Web.UI.WebControls.GridView PHistoryGrid).
//   These synchronous Web Forms controls are no longer applicable in ASP.NET Core
//   Razor Pages where data is exposed via public properties on the PageModel class
//   and rendered asynchronously via async Task OnGetAsync() using Entity Framework
//   Core connected to Amazon RDS.
//
// The history data table is now a public DataTable? property on PreviousHistoryModel
// in PreviousHistory.aspx.cs, populated asynchronously via
// await _dal.getPHistory_Async() using EF Core / SqlQueryRaw<T>().ToListAsync()
// connected to Amazon RDS, preventing thread-pool exhaustion under cloud load.
//
// The error label (PHistory) is replaced by the string ErrorMessage property on
// PreviousHistoryModel, rendered inline in the Razor Page template.
//
// This file is retained for project structure compatibility only.

namespace DBProject.Pages.Doctor
{
    // cr-dotnet-1034: Designer stub – Web Forms GridView and Label server controls removed.
    // The synchronous PHistoryGrid (System.Web.UI.WebControls.GridView) is replaced
    // by an HTML table in PreviousHistory.aspx bound to the async-loaded History
    // DataTable property on PreviousHistoryModel (PreviousHistory.aspx.cs).
    // The synchronous PHistory (System.Web.UI.WebControls.Label) is replaced by the
    // ErrorMessage string property on PreviousHistoryModel.
    // Data is fetched asynchronously via EF Core connected to Amazon RDS.
}
