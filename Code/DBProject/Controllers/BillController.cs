// ASP.NET Core MVC Controller for Bill / Generate Bill (cr-dotnet-0026).
// Migrated from Bill.aspx.cs (Web Forms code-behind) to BillController.cs.
//
// cr-dotnet-0045 – Session State Provider (Amazon ElastiCache for Redis):
//   Replaced in-process HttpSessionState (InProc) with distributed session
//   backed by Amazon ElastiCache for Redis.
//
//   Original Web Forms session access (Bill.aspx.cs):
//     Line 21 – Session["idoriginal"]  (HttpSessionState, InProc)
//     Line 38 – Session["idoriginal"]  (HttpSessionState, InProc)
//     Line 39 – Session["appointid"]   (HttpSessionState, InProc)
//     Line 51 – Session["idoriginal"]  (HttpSessionState, InProc)
//     Line 52 – Session["appointid"]   (HttpSessionState, InProc)
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
//   - System.Web.UI.Page        → Microsoft.AspNetCore.Mvc.Controller
//   - System.Web.UI             → removed (not available in ASP.NET Core)
//   - System.Web.UI.WebControls → removed (not available in ASP.NET Core)
//   - Page_Load                 → Index() GET action
//   - Session["idoriginal"]     → HttpContext.Session.GetInt32("idoriginal")
//   - Session["appointid"]      → HttpContext.Session.GetInt32("appointid")
//   - Response.Redirect()       → RedirectToAction()
//   - Response.Write(<script>)  → ViewModel.ErrorMessage
//   - asp:Label Label1.Text     → BillViewModel.BillAmount
//
// All business logic from the original code-behind is preserved.

using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DBProject.DAL;
using DBProject.Models;

namespace DBProject.Controllers
{
    /// <summary>
    /// Handles the Generate Bill page for doctors.
    /// Replaces Bill.aspx + Bill.aspx.cs (Web Forms).
    /// Session state is backed by Amazon ElastiCache for Redis (cr-dotnet-0045).
    /// </summary>
    public class BillController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /Bill  or  GET /Bill/Index
        // Replaces Page_Load in Bill.aspx.cs
        // cr-dotnet-0045: Session["idoriginal"] (Line 21) now reads from
        //                 Redis-backed distributed session via GetInt32().
        // -----------------------------------------------------------------------
        [HttpGet]
        public IActionResult Index()
        {
            var vm = new BillViewModel();

            // cr-dotnet-0045 fix (Line 21): replaced InProc Session["idoriginal"]
            // with distributed Redis-backed session via HttpContext.Session.GetInt32()
            int did = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            var objmyDAL = new myDAL();
            var dt = new DataTable();
            int found = objmyDAL.generate_bill_DAL(did, ref dt);

            if (found != 1)
            {
                vm.ErrorMessage = "There was some error generating the bill.";
            }
            else
            {
                vm.BillAmount = dt.Rows[0][0].ToString();
            }

            return View(vm);
        }

        // -----------------------------------------------------------------------
        // POST /Bill/BillPaid
        // Replaces bill_paid() event handler in Bill.aspx.cs
        // cr-dotnet-0045: Session["idoriginal"] (Line 38) and
        //                 Session["appointid"]  (Line 39) now read from
        //                 Redis-backed distributed session via GetInt32().
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BillPaid()
        {
            // cr-dotnet-0045 fix (Lines 38-39): replaced InProc Session access
            // with distributed Redis-backed session via HttpContext.Session.GetInt32()
            int did     = HttpContext.Session.GetInt32("idoriginal") ?? 0;
            int appoint = HttpContext.Session.GetInt32("appointid")  ?? 0;

            var objmyDAL = new myDAL();
            objmyDAL.paid_bill_DAL(did, appoint);

            // Replaces Response.Redirect("patienthistory.aspx")
            return RedirectToAction("Index", "PatientHistory");
        }

        // -----------------------------------------------------------------------
        // POST /Bill/BillUnpaid
        // Replaces bill_Unpaid() event handler in Bill.aspx.cs
        // cr-dotnet-0045: Session["idoriginal"] (Line 51) and
        //                 Session["appointid"]  (Line 52) now read from
        //                 Redis-backed distributed session via GetInt32().
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BillUnpaid()
        {
            // cr-dotnet-0045 fix (Lines 51-52): replaced InProc Session access
            // with distributed Redis-backed session via HttpContext.Session.GetInt32()
            int did     = HttpContext.Session.GetInt32("idoriginal") ?? 0;
            int appoint = HttpContext.Session.GetInt32("appointid")  ?? 0;

            var objmyDAL = new myDAL();
            objmyDAL.Unpaid_bill_DAL(did, appoint);

            // Replaces Response.Redirect("patienthistory.aspx")
            return RedirectToAction("Index", "PatientHistory");
        }
    }
}
