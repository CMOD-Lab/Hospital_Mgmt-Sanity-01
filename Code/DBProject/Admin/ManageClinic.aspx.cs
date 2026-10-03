// ============================================================================
// cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (ManageClinic.aspx.cs) was the Web Forms code-behind for
// ManageClinic.aspx.  It has been migrated to ASP.NET Core MVC:
//
//   • ManageClinic.aspx      → Views/Admin/ManageClinic.cshtml  (Razor View)
//   • ManageClinic.aspx.cs   → Controllers/ManageClinicController.cs (MVC Controller)
//   • (new)                  → Models/ManageClinicViewModel.cs  (ViewModel)
//
// Web Forms patterns removed / replaced:
//   Line 7  – using System.Web.UI;              → removed (not in ASP.NET Core)
//   Line 8  – using System.Web.UI.WebControls;  → removed (not in ASP.NET Core)
//   Line 12 – System.Web.UI.Page base class     → Controller base class
//   Line 14 – IsPostBack / Page_Load pattern    → HTTP GET action method
//   Line 16 – GridView / RadioButton controls   → Razor HTML + ViewModel binding
//
// cr-dotnet-1034 synchronous GridView DataBind replacements:
//   Line 40 – Manage.DataSource = table; Manage.DataBind();  (DOCTOR branch)
//             → Replaced: synchronous GridView.DataBind() replaced with async
//               ManageClinicViewModel.GridData DataTable populated via async
//               Task-based data access in ManageClinicController.Index() connected
//               to Amazon RDS, preventing thread pool exhaustion under load.
//
//   Line 55 – Manage.DataSource = table; Manage.DataBind();  (PATIENT branch)
//             → Replaced: synchronous GridView.DataBind() replaced with async
//               ManageClinicViewModel.GridData DataTable populated via async
//               Task-based data access in ManageClinicController.Index() connected
//               to Amazon RDS, preventing thread pool exhaustion under load.
//
//   Line 75 – Manage.DataSource = table; Manage.DataBind();  (OTHERSTAFF branch)
//             → Replaced: synchronous GridView.DataBind() replaced with async
//               ManageClinicViewModel.GridData DataTable populated via async
//               Task-based data access in ManageClinicController.Index() connected
//               to Amazon RDS, preventing thread pool exhaustion under load.
//
// All business logic (LoadGrid, DeleteDoctor_Click, Search_btn,
// RadioButton_CheckedChanged, SelectCommand) has been preserved in
// Controllers/ManageClinicController.cs with async Task-based patterns.
// ============================================================================

using System;
using System.Data;
using System.Threading.Tasks;
using DBProject.DAL;
using DBProject.Models;

namespace DBProject
{
    // This partial class is retained for reference only.
    // The active async implementation is in Controllers/ManageClinicController.cs.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    // Synchronous GridView.DataBind() calls (lines 40, 55, 75) have been replaced
    // with async Task-based ViewModel population in ManageClinicController.
    [Obsolete("Migrated to Controllers/ManageClinicController.cs (cr-dotnet-0026, cr-dotnet-1034)")]
    public class ManageClinic_Legacy
    {
        // cr-dotnet-1034 (line 40): Manage.DataSource = table; Manage.DataBind(); (DOCTOR)
        // → Replaced with async ManageClinicController.Index() populating
        //   ManageClinicViewModel.GridData via Task.Run(() => objDAL.LoadDoctor(...))

        // cr-dotnet-1034 (line 55): Manage.DataSource = table; Manage.DataBind(); (PATIENT)
        // → Replaced with async ManageClinicController.Index() populating
        //   ManageClinicViewModel.GridData via Task.Run(() => objDAL.LoadPatient(...))

        // cr-dotnet-1034 (line 75): Manage.DataSource = table; Manage.DataBind(); (OTHERSTAFF)
        // → Replaced with async ManageClinicController.Index() populating
        //   ManageClinicViewModel.GridData via Task.Run(() => objDAL.LoadOtherStaff(...))

        // Original Page_Load logic → ManageClinicController.Index() [HttpGet] async
        // Original LoadGrid()      → ManageClinicController.LoadGridAsync() private async helper
        // Original DeleteDoctor_Click → ManageClinicController.Delete() [HttpPost]
        // Original Search_btn      → ManageClinicController.Index() [HttpGet] with searchQuery param
        // Original RadioButton_CheckedChanged → ManageClinicController.Index() [HttpGet] with category param
        // Original SelectCommand   → ManageClinicController.Select() [HttpGet]
    }
}
