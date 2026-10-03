// ASP.NET Core MVC Controller for History Update (cr-dotnet-0026).
// Migrated from HistoryUpdate.aspx.cs (Web Forms code-behind) to HistoryUpdateController.cs.
//
// cr-dotnet-0045 – Session State Provider (Amazon ElastiCache for Redis):
//   Replaced in-process HttpSessionState (InProc) with distributed session
//   backed by Amazon ElastiCache for Redis.
//
//   Original Web Forms session access (HistoryUpdate.aspx.cs):
//     Line 23 – Session["idoriginal"]  (HttpSessionState, InProc)
//     Line 28 – Session["appointid"]   (HttpSessionState, InProc)
//
//   Replaced with:
//     HttpContext.Session.GetInt32("idoriginal")  – reads from Redis-backed
//     HttpContext.Session.GetInt32("appointid")   – distributed session store
//
//   Session is now stored in Amazon ElastiCache for Redis via
//   Microsoft.Extensions.Caching.StackExchangeRedis, configured in
//   Startup.cs / Program.cs with:
//       services.AddStackExchangeRedisCache(options => {
//           options.Configuration = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
//       });
//       services.AddSession(options => {
//           options.IdleTimeout = TimeSpan.FromMinutes(30);
//           options.Cookie.HttpOnly = true;
//           options.Cookie.IsEssential = true;
//       });
//
// Web Forms patterns replaced:
//   Line 5  – using System.Web.UI;              → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI.WebControls;  → removed (not available in ASP.NET Core)
//   Line 12 – System.Web.UI.Page base class     → Microsoft.AspNetCore.Mvc.Controller
//   Line 14 – public partial class Historyupdate : System.Web.UI.Page
//             → HistoryUpdateController (MVC Controller)
//   Page_Load (empty)                           → Index() GET action
//   Session["idoriginal"]                       → HttpContext.Session.GetInt32("idoriginal")
//   Session["appointid"]                        → HttpContext.Session.GetInt32("appointid")
//   Disease.Text / progress.Text / Prescription.Text → form POST parameters
//   Response.Write(<script>alert(...);</script>) → ViewModel.ErrorMessage / SuccessMessage
//   Response.Redirect("bill.aspx")              → RedirectToAction("Index", "Bill")
//
// All business logic from the original code-behind is preserved.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DBProject.DAL;
using DBProject.Models;

namespace DBProject.Controllers
{
    /// <summary>
    /// Handles the Update History page for doctors.
    /// Replaces HistoryUpdate.aspx + HistoryUpdate.aspx.cs (Web Forms).
    /// Session state is backed by Amazon ElastiCache for Redis (cr-dotnet-0045).
    /// </summary>
    public class HistoryUpdateController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /HistoryUpdate  or  GET /HistoryUpdate/Index
        // Replaces Page_Load (empty) in HistoryUpdate.aspx.cs
        // -----------------------------------------------------------------------
        [HttpGet]
        public IActionResult Index()
        {
            var vm = new HistoryUpdateViewModel();
            return View(vm);
        }

        // -----------------------------------------------------------------------
        // POST /HistoryUpdate/Save
        // Replaces saveindatabase() event handler in HistoryUpdate.aspx.cs
        // cr-dotnet-0045: Session["idoriginal"] (Line 23) and
        //                 Session["appointid"]  (Line 28) now read from
        //                 Redis-backed distributed session via GetInt32().
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(string Disease, string Progress, string Prescription)
        {
            var vm = new HistoryUpdateViewModel
            {
                Disease      = Disease      ?? string.Empty,
                Progress     = Progress     ?? string.Empty,
                Prescription = Prescription ?? string.Empty
            };

            // cr-dotnet-0045 fix (Lines 23, 28): replaced InProc Session access
            // with distributed Redis-backed session via HttpContext.Session.GetInt32()
            int did   = HttpContext.Session.GetInt32("idoriginal") ?? 0;
            int appid = HttpContext.Session.GetInt32("appointid")  ?? 0;

            var objmyDAL = new myDAL();
            int found = objmyDAL.update_prescription_DAL(did, appid, Disease, Progress, Prescription);

            if (found != 1)
            {
                // Replaces Response.Write("<script>alert('There was some error');</script>")
                vm.ErrorMessage = "There was some error saving the history.";
            }
            else
            {
                // Replaces Response.Write("<script>alert('Information Successfully Updated');</script>")
                vm.SuccessMessage = "Information Successfully Updated.";
            }

            return View("Index", vm);
        }

        // -----------------------------------------------------------------------
        // POST /HistoryUpdate/GenerateBill
        // Replaces generate_bill() event handler in HistoryUpdate.aspx.cs
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GenerateBill()
        {
            // Replaces Response.Redirect("bill.aspx")
            return RedirectToAction("Index", "Bill");
        }
    }
}
