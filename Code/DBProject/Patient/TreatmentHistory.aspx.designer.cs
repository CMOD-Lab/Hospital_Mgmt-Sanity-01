// This file is intentionally left minimal.
// The Web Forms auto-generated designer file has been superseded by the
// ASP.NET Core Razor Pages migration (TreatmentHistory.aspx / TreatmentHistoryModel).
// Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//   The GridView server control (THistoryGrid) has been replaced by async model-bound
//   DataTable rendering in the Razor view. OnGetAsync() uses async DAL method
//   (getTreatmentHistoryAsync) connected to Amazon RDS via Dapper async APIs,
//   preventing thread pool exhaustion under load.

namespace DBProject.Patient
{
    // Designer stub retained for project compatibility during migration.
    // All UI controls are now defined as properties on TreatmentHistoryModel (PageModel).
    // GridView THistoryGrid replaced by DataTable TreatmentData property with async data loading.
}
