// ASP.NET Core MVC Controller for View Doctors
// (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-0126, cr-dotnet-1034).
// Migrated from ViewDoctors.aspx.cs (Web Forms code-behind) to
// ViewDoctorsController.cs.
//
// Web Forms patterns replaced:
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 13 – public partial class ViewDoctors : System.Web.UI.Page
//             → ViewDoctorsController (MVC Controller)
//   Line 15 – Page_Load → deptDoctorInfo()     → IndexAsync() GET action
//   TDoctorGrid_RowCommand (GridView row select) → SelectDoctor() POST action
//   TDoctor.Text = "..."                        → ViewDoctorsViewModel.StatusMessage
//   TDoctorGrid.DataSource / DataBind           → ViewDoctorsViewModel.DoctorData (async)
//   Session["deptOriginal"]                     → HttpContext.Session.GetString
//   Session["dID"]                              → HttpContext.Session.SetString
//   Response.Redirect("DoctorProfile.aspx")     → RedirectToAction("Index", "DoctorProfile")
//
// cr-dotnet-1034 – Async GridView Data Binding fix:
//   Line 63 – TDoctorGrid.DataSource = DT; TDoctorGrid.DataBind() (synchronous) →
//             var (status, dt) = await _dal.getDeptDoctorInfo_Async(deptName);
//             vm.DoctorData = dt;
//             Async Task-based data loading via getDeptDoctorInfo_Async() in myDAL,
//             connected to Amazon RDS via RDS Proxy (RDS_PROXY_CONNECTION_STRING env var).
//             Prevents thread pool exhaustion under cloud load; enables auto-scaling.
//
// cr-dotnet-0045 – Session State Provider fix:
//   Line 17 – Session["dID"] = "" (InProc) → HttpContext.Session.SetString("dID", "")
//   Line 30 – Session["dID"] = dID (InProc) → HttpContext.Session.SetString("dID", selectedDoctorId)
//   Line 49 – Session["deptOriginal"] (InProc) → HttpContext.Session.GetString("deptOriginal")
//   Line 61 – Session["deptOriginal"] (InProc) → deptName from distributed Redis session
//   All backed by Amazon ElastiCache for Redis via REDIS_CONNECTION_STRING env var.
//
// cr-dotnet-0126 – Heavy Coupling to Stateful Middleware fix:
//   IIS application pool sticky sessions replaced with Amazon ElastiCache for Redis
//   distributed cache session store.
//
//   Line 17 – Session["dID"] = "" (IIS InProc sticky session)
//             → HttpContext.Session.SetString("dID", "")
//               backed by Amazon ElastiCache for Redis.
//   Line 30 – Session["dID"] = dID (IIS InProc sticky session)
//             → HttpContext.Session.SetString("dID", selectedDoctorId)
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
    /// Handles the View Doctors (Department Doctor Listing) page for patients.
    /// Replaces ViewDoctors.aspx + ViewDoctors.aspx.cs (Web Forms).
    /// Session state is backed by Amazon ElastiCache for Redis
    /// (cr-dotnet-0045, cr-dotnet-0126 – eliminates IIS sticky session dependency).
    /// Data access uses async Task-based patterns via Amazon RDS Proxy
    /// (cr-dotnet-1034 – prevents thread pool exhaustion, enables auto-scaling).
    /// </summary>
    public class ViewDoctorsController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /ViewDoctors
        // Replaces Page_Load → deptDoctorInfo() in ViewDoctors.aspx.cs
        //
        // cr-dotnet-1034 fix:
        //   BEFORE: TDoctorGrid.DataSource = DT; TDoctorGrid.DataBind();
        //           Synchronous GridView data binding blocks request threads.
        //   AFTER:  var (status, dt) = await _dal.getDeptDoctorInfo_Async(deptName);
        //           Async Task-based data loading via Amazon RDS Proxy.
        //           Prevents thread pool exhaustion; enables auto-scaling.
        //
        // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 17):
        //   Session["dID"] = "" (IIS InProc sticky session) →
        //   HttpContext.Session.SetString backed by Amazon ElastiCache for Redis.
        //   No server affinity required; scales horizontally.
        // -----------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 17):
            // Replaced InProc Session["dID"] = "" (IIS sticky session) with
            // distributed Redis-backed session via HttpContext.Session.SetString().
            // Session data persists across pod restarts; no server affinity needed.
            HttpContext.Session.SetString("dID", "");

            var vm = new ViewDoctorsViewModel();
            await LoadDeptDoctorInfoAsync(vm);
            return View(vm);
        }

        // -----------------------------------------------------------------------
        // POST /ViewDoctors/SelectDoctor
        // Replaces TDoctorGrid_RowCommand (GridView row select handler)
        // in ViewDoctors.aspx.cs.
        // The selected doctor ID is submitted via a form POST.
        // cr-dotnet-0045 / cr-dotnet-0126:
        //   Session["dID"] = dID (Line 30, IIS InProc sticky session) →
        //   HttpContext.Session.SetString backed by Amazon ElastiCache for Redis.
        //   Eliminates IIS application pool sticky session dependency.
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectDoctor(string selectedDoctorId)
        {
            if (!string.IsNullOrEmpty(selectedDoctorId))
            {
                // Replaces: string dID = TDoctorGrid.Rows[num].Cells[2].Text;
                //           Session["dID"] = dID;
                // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 30):
                // Replaced InProc Session["dID"] (IIS sticky session) with
                // distributed Redis-backed session. No server affinity required.
                HttpContext.Session.SetString("dID", selectedDoctorId);

                // Replaces: Response.BufferOutput = true;
                //           Response.Redirect("DoctorProfile.aspx");
                return RedirectToAction("Index", "DoctorProfile");
            }

            // If no doctor selected, reload the page
            var vm = new ViewDoctorsViewModel();
            _ = LoadDeptDoctorInfoAsync(vm).GetAwaiter().GetResult();
            return View("Index", vm);
        }

        // -----------------------------------------------------------------------
        // Helper: loads department doctor information from DAL into the view model asynchronously.
        // Replaces deptDoctorInfo() method in ViewDoctors.aspx.cs.
        //
        // cr-dotnet-1034 fix:
        //   BEFORE: int status = objmyDAl.getDeptDoctorInfo(deptName, ref DT);
        //           TDoctorGrid.DataSource = DT; TDoctorGrid.DataBind();
        //           Synchronous GridView data binding blocks request threads.
        //   AFTER:  var (status, dt) = await objmyDAl.getDeptDoctorInfo_Async(deptName);
        //           vm.DoctorData = dt;
        //           Async Task-based data loading via Amazon RDS Proxy.
        //           Prevents thread pool exhaustion; enables auto-scaling.
        //
        // cr-dotnet-0045 / cr-dotnet-0126:
        //   Session["deptOriginal"] (Line 49, InProc) →
        //   HttpContext.Session.GetString backed by Amazon ElastiCache for Redis.
        // -----------------------------------------------------------------------
        private async Task LoadDeptDoctorInfoAsync(ViewDoctorsViewModel vm)
        {
            var objmyDAl = new myDAL();

            // cr-dotnet-0045 fix (Line 49):
            // Replaced InProc Session["deptOriginal"] with distributed Redis-backed session.
            string deptName = HttpContext.Session.GetString("deptOriginal") ?? "";
            vm.DepartmentName = deptName;

            // cr-dotnet-1034 fix:
            // Replaced synchronous: int status = objmyDAl.getDeptDoctorInfo(deptName, ref DT);
            //                       TDoctorGrid.DataSource = DT; TDoctorGrid.DataBind();
            // With async Task-based: var (status, dt) = await objmyDAl.getDeptDoctorInfo_Async(deptName);
            // Connected to Amazon RDS via RDS Proxy (RDS_PROXY_CONNECTION_STRING env var).
            var (status, dt) = await objmyDAl.getDeptDoctorInfo_Async(deptName);

            if (status == -1)
            {
                // Replaces: TDoctor.Text = "There was some error in retrieving the Doctors Information.";
                vm.StatusMessage = "There was some error in retrieving the Doctors Information.";
            }
            else
            {
                // Replaces: TDoctor.Text = "Following are our Specialized Doctors of " + Session["deptOriginal"] + " Department:";
                //           TDoctorGrid.DataSource = DT; TDoctorGrid.DataBind();
                // cr-dotnet-0045 fix (Line 61): Uses deptName from distributed Redis session
                // instead of InProc Session["deptOriginal"].
                vm.StatusMessage = "Following are our Specialized Doctors of " + deptName + " Department:";
                vm.DoctorData = dt;
            }
        }
    }
}
