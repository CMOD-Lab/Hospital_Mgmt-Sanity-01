// ASP.NET Core MVC Controller for Treatment History (cr-dotnet-0026, cr-dotnet-1034).
// Migrated from TreatmentHistory.aspx.cs (Web Forms code-behind) to
// TreatmentHistoryController.cs.
//
// Web Forms patterns replaced:
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 14 – public partial class TreatmentHistory : System.Web.UI.Page
//             → TreatmentHistoryController (MVC Controller)
//   Line 16 – Page_Load → treatmentHistory()   → IndexAsync() GET action
//   THistory.Text = "..."                      → TreatmentHistoryViewModel.StatusMessage
//   THistoryGrid.DataSource / DataBind         → TreatmentHistoryViewModel.TreatmentData (async)
//   Session["idoriginal"]                      → HttpContext.Session.GetInt32
//
// cr-dotnet-1034 – Async GridView Data Binding fix:
//   Line 51 – THistoryGrid.DataSource = DT; THistoryGrid.DataBind() (synchronous) →
//             var (status, dt) = await _dal.getTreatmentHistory_Async(id);
//             vm.TreatmentData = dt;
//             Async Task-based data loading via getTreatmentHistory_Async() in myDAL,
//             connected to Amazon RDS via RDS Proxy (RDS_PROXY_CONNECTION_STRING env var).
//             Prevents thread pool exhaustion under cloud load; enables auto-scaling.
//
// All business logic from the original code-behind is preserved.

using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DBProject.DAL;
using DBProject.Models;

namespace DBProject.Controllers
{
    /// <summary>
    /// Handles the Treatment History page for patients.
    /// Replaces TreatmentHistory.aspx + TreatmentHistory.aspx.cs (Web Forms).
    /// Data access uses async Task-based patterns via Amazon RDS Proxy
    /// (cr-dotnet-1034 – prevents thread pool exhaustion, enables auto-scaling).
    /// </summary>
    public class TreatmentHistoryController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /TreatmentHistory
        // Replaces Page_Load → treatmentHistory() in TreatmentHistory.aspx.cs
        //
        // cr-dotnet-1034 fix (Line 51):
        //   BEFORE: THistoryGrid.DataSource = DT; THistoryGrid.DataBind();
        //           Synchronous GridView data binding blocks request threads.
        //   AFTER:  var (status, dt) = await _dal.getTreatmentHistory_Async(id);
        //           Async Task-based data loading via Amazon RDS Proxy.
        //           Prevents thread pool exhaustion; enables auto-scaling.
        // -----------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new TreatmentHistoryViewModel();
            await LoadTreatmentHistoryAsync(vm);
            return View(vm);
        }

        // -----------------------------------------------------------------------
        // Helper: loads treatment history from DAL into the view model asynchronously.
        // Replaces treatmentHistory() method in TreatmentHistory.aspx.cs.
        //
        // cr-dotnet-1034 fix (Line 51):
        //   BEFORE: int status = objmyDAl.getTreatmentHistory(id, ref DT);
        //           THistoryGrid.DataSource = DT; THistoryGrid.DataBind();
        //           Synchronous GridView data binding blocks request threads.
        //   AFTER:  var (status, dt) = await objmyDAl.getTreatmentHistory_Async(id);
        //           vm.TreatmentData = dt;
        //           Async Task-based data loading via Amazon RDS Proxy.
        //           Prevents thread pool exhaustion; enables auto-scaling.
        // -----------------------------------------------------------------------
        private async Task LoadTreatmentHistoryAsync(TreatmentHistoryViewModel vm)
        {
            var objmyDAl = new myDAL();

            // Replaces: int id = (int)Session["idoriginal"];
            int id = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            // cr-dotnet-1034 fix:
            // Replaced synchronous: int status = objmyDAl.getTreatmentHistory(id, ref DT);
            //                       THistoryGrid.DataSource = DT; THistoryGrid.DataBind();
            // With async Task-based: var (status, dt) = await objmyDAl.getTreatmentHistory_Async(id);
            // Connected to Amazon RDS via RDS Proxy (RDS_PROXY_CONNECTION_STRING env var).
            var (status, dt) = await objmyDAl.getTreatmentHistory_Async(id);

            if (status == -1)
            {
                // Replaces: THistory.Text = "There was some error in retrieving the Patient's Treatment History.";
                vm.StatusMessage = "There was some error in retrieving the Patient's Treatment History.";
            }
            else if (status == 0)
            {
                // Replaces: THistory.Text = "There is currently no treatment history of yours.";
                vm.StatusMessage = "There is currently no treatment history of yours.";
            }
            else
            {
                // Replaces: THistory.Text = "Treatment History of " + status + " Appointment(s) is found: ";
                //           THistoryGrid.DataSource = DT; THistoryGrid.DataBind();
                vm.StatusMessage = "Treatment History of " + status + " Appointment(s) is found: ";
                vm.TreatmentData = dt;
                vm.TreatmentCount = status;
            }
        }
    }
}
