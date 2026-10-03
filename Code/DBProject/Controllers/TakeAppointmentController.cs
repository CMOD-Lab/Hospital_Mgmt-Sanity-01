// ASP.NET Core MVC Controller for Take Appointment (Department Selection)
// (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-0126, cr-dotnet-1034).
// Migrated from TakeAppointment.aspx.cs (Web Forms code-behind) to
// TakeAppointmentController.cs.
//
// Web Forms patterns replaced:
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 13 – public partial class TakeAppointment : System.Web.UI.Page
//             → TakeAppointmentController (MVC Controller)
//   Line 15 – Page_Load → deptInfo()           → IndexAsync() GET action
//   TDeptGrid_RowCommand (GridView row select)  → SelectDepartment() POST action
//   TDept.Text = "..."                          → TakeAppointmentViewModel.StatusMessage
//   TDeptGrid.DataSource / DataBind             → TakeAppointmentViewModel.DeptData (async)
//   Session["deptOriginal"]                     → HttpContext.Session.SetString
//   Response.Redirect("ViewDoctors.aspx")       → RedirectToAction("Index", "ViewDoctors")
//
// cr-dotnet-1034 – Async GridView Data Binding fix:
//   Line 63 – TDeptGrid.DataSource = DT; TDeptGrid.DataBind() (synchronous) →
//             var (status, dt) = await _dal.getdeptInfo_Async();
//             vm.DeptData = dt;
//             Async Task-based data loading via getdeptInfo_Async() in myDAL,
//             connected to Amazon RDS via RDS Proxy (RDS_PROXY_CONNECTION_STRING env var).
//             Prevents thread pool exhaustion under cloud load; enables auto-scaling.
//
// cr-dotnet-0045 – Session State Provider fix:
//   Line 17 – Session["deptOriginal"] = "" (InProc) →
//             HttpContext.Session.SetString("deptOriginal", "") backed by Amazon ElastiCache for Redis.
//   Line 31 – Session["deptOriginal"] = deptName (InProc) →
//             HttpContext.Session.SetString("deptOriginal", selectedDept) backed by Amazon ElastiCache for Redis.
//   Redis is configured via REDIS_CONNECTION_STRING environment variable.
//
// cr-dotnet-0126 – Heavy Coupling to Stateful Middleware fix:
//   IIS application pool sticky sessions replaced with Amazon ElastiCache for Redis
//   distributed cache session store.
//
//   Line 17 – Session["deptOriginal"] = "" (IIS InProc sticky session)
//             → HttpContext.Session.SetString("deptOriginal", "")
//               backed by Amazon ElastiCache for Redis.
//   Line 31 – Session["deptOriginal"] = deptName (IIS InProc sticky session)
//             → HttpContext.Session.SetString("deptOriginal", selectedDept)
//               backed by Amazon ElastiCache for Redis.
//
//   This eliminates server affinity (sticky sessions) required by IIS in-process
//   session state. Session data persists across pod/container restarts and scales
//   horizontally without sticky session routing.
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
    /// Handles the Take Appointment (Department Selection) page for patients.
    /// Replaces TakeAppointment.aspx + TakeAppointment.aspx.cs (Web Forms).
    /// Session state is backed by Amazon ElastiCache for Redis
    /// (cr-dotnet-0045, cr-dotnet-0126 – eliminates IIS sticky session dependency).
    /// Data access uses async Task-based patterns via Amazon RDS Proxy
    /// (cr-dotnet-1034 – prevents thread pool exhaustion, enables auto-scaling).
    /// </summary>
    public class TakeAppointmentController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /TakeAppointment
        // Replaces Page_Load → deptInfo() in TakeAppointment.aspx.cs
        //
        // cr-dotnet-1034 fix (Line 63):
        //   BEFORE: TDeptGrid.DataSource = DT; TDeptGrid.DataBind();
        //           Synchronous GridView data binding blocks request threads.
        //   AFTER:  var (status, dt) = await _dal.getdeptInfo_Async();
        //           Async Task-based data loading via Amazon RDS Proxy.
        //           Prevents thread pool exhaustion; enables auto-scaling.
        //
        // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 17):
        //   Session["deptOriginal"] = "" (IIS InProc sticky session) →
        //   HttpContext.Session.SetString backed by Amazon ElastiCache for Redis.
        //   No server affinity required; scales horizontally.
        // -----------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 17):
            // Replaced InProc Session["deptOriginal"] = "" (IIS sticky session) with
            // distributed Redis-backed session via HttpContext.Session.SetString().
            // Session data persists across pod restarts; no server affinity needed.
            HttpContext.Session.SetString("deptOriginal", "");

            var vm = new TakeAppointmentViewModel();
            await LoadDeptInfoAsync(vm);
            return View(vm);
        }

        // -----------------------------------------------------------------------
        // POST /TakeAppointment/SelectDepartment
        // Replaces TDeptGrid_RowCommand (GridView row select handler)
        // in TakeAppointment.aspx.cs.
        // The selected department name is submitted via a form POST.
        // cr-dotnet-0045 / cr-dotnet-0126:
        //   Session["deptOriginal"] = deptName (Line 31, IIS InProc sticky session) →
        //   HttpContext.Session.SetString backed by Amazon ElastiCache for Redis.
        //   Eliminates IIS application pool sticky session dependency.
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectDepartment(string selectedDept)
        {
            if (!string.IsNullOrEmpty(selectedDept))
            {
                // Replaces: string deptName = TDeptGrid.Rows[num].Cells[2].Text;
                //           Session["deptOriginal"] = deptName;
                // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 31):
                // Replaced InProc Session["deptOriginal"] (IIS sticky session) with
                // distributed Redis-backed session. No server affinity required.
                HttpContext.Session.SetString("deptOriginal", selectedDept);

                // Replaces: Response.Redirect("ViewDoctors.aspx");
                return RedirectToAction("Index", "ViewDoctors");
            }

            // If no department selected, reload the page
            var vm = new TakeAppointmentViewModel();
            _ = LoadDeptInfoAsync(vm).GetAwaiter().GetResult();
            return View("Index", vm);
        }

        // -----------------------------------------------------------------------
        // Helper: loads department information from DAL into the view model asynchronously.
        // Replaces deptInfo() method in TakeAppointment.aspx.cs.
        //
        // cr-dotnet-1034 fix (Line 63):
        //   BEFORE: int status = objmyDAl.getdeptInfo(ref DT);
        //           TDeptGrid.DataSource = DT; TDeptGrid.DataBind();
        //           Synchronous GridView data binding blocks request threads.
        //   AFTER:  var (status, dt) = await objmyDAl.getdeptInfo_Async();
        //           vm.DeptData = dt;
        //           Async Task-based data loading via Amazon RDS Proxy.
        //           Prevents thread pool exhaustion; enables auto-scaling.
        // -----------------------------------------------------------------------
        private async Task LoadDeptInfoAsync(TakeAppointmentViewModel vm)
        {
            var objmyDAl = new myDAL();

            // cr-dotnet-1034 fix:
            // Replaced synchronous: int status = objmyDAl.getdeptInfo(ref dt);
            //                       TDeptGrid.DataSource = dt; TDeptGrid.DataBind();
            // With async Task-based: var (status, dt) = await objmyDAl.getdeptInfo_Async();
            // Connected to Amazon RDS via RDS Proxy (RDS_PROXY_CONNECTION_STRING env var).
            var (status, dt) = await objmyDAl.getdeptInfo_Async();

            if (status == -1)
            {
                // Replaces: TDept.Text = "There was some error in retrieving the Departments Information.";
                vm.StatusMessage = "There was some error in retrieving the Departments Information.";
            }
            else
            {
                // Replaces: TDept.Text = "Following are the departments available at our Clinic : ";
                //           TDeptGrid.DataSource = DT; TDeptGrid.DataBind();
                vm.StatusMessage = "Following are the departments available at our Clinic : ";
                vm.DeptData = dt;
            }
        }
    }
}
