// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//
// cr-dotnet-1034 (Line 31):
//   The auto-generated designer file previously declared Web Forms server-control
//   fields (System.Web.UI.WebControls.Label BHistory and
//   System.Web.UI.WebControls.GridView BHistoryGrid).
//   These synchronous Web Forms controls are no longer applicable in ASP.NET Core
//   Razor Pages where data is exposed via public properties on the PageModel class
//   and rendered asynchronously via async Task OnGetAsync() using Entity Framework
//   Core connected to Amazon RDS.
//
// The bill history data table is now a public DataTable? property on BillsHistoryModel
// in BillsHistory.aspx.cs, populated asynchronously via
// await _dal.getBillHistory_Async() using EF Core / SqlQueryRaw<T>().ToListAsync()
// connected to Amazon RDS, preventing thread-pool exhaustion under cloud load.
//
// The status label (BHistory) is replaced by the string StatusMessage property on
// BillsHistoryModel, rendered inline in the Razor Page template.
//
// This file is retained for project structure compatibility only.

namespace DBProject.Pages.Patient
{
    // cr-dotnet-1034: Designer stub – Web Forms GridView and Label server controls removed.
    // The synchronous BHistoryGrid (System.Web.UI.WebControls.GridView) is replaced
    // by an HTML table in BillsHistory.aspx bound to the async-loaded BillHistory
    // DataTable property on BillsHistoryModel (BillsHistory.aspx.cs).
    // The synchronous BHistory (System.Web.UI.WebControls.Label) is replaced by the
    // StatusMessage string property on BillsHistoryModel.
    // Data is fetched asynchronously via EF Core connected to Amazon RDS.
}
