// ASP.NET Core MVC Controller for Doctor Home (cr-dotnet-0026).
// Migrated from DoctorHome.aspx.cs (Web Forms code-behind) to DoctorHomeController.cs.
//
// cr-dotnet-0045 – Session State Provider (Amazon ElastiCache for Redis):
//   Replaced in-process HttpSessionState (InProc) with distributed session
//   backed by Amazon ElastiCache for Redis.
//
//   Original Web Forms session access (DoctorHome.aspx.cs):
//     Line 21 – Session["idoriginal"]  (HttpSessionState, InProc)
//
//   Replaced with:
//     HttpContext.Session.GetInt32("idoriginal")  – reads from Redis-backed
//                                                   distributed session store
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
//   - System.Web.UI.Page          → Microsoft.AspNetCore.Mvc.Controller
//   - System.Web.UI               → removed (not available in ASP.NET Core)
//   - System.Web.UI.WebControls   → removed (not available in ASP.NET Core)
//   - Page_Load                   → Index() GET action
//   - Session["idoriginal"]       → HttpContext.Session.GetInt32("idoriginal")
//   - Label1..Label14.Text        → DoctorHomeViewModel properties
//   - Response.Write(<script>)    → DoctorHomeViewModel.ErrorMessage
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
    /// Handles the Doctor Home page displaying the doctor's profile information.
    /// Replaces DoctorHome.aspx + DoctorHome.aspx.cs (Web Forms).
    /// Session state is backed by Amazon ElastiCache for Redis (cr-dotnet-0045).
    /// </summary>
    public class DoctorHomeController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /DoctorHome  or  GET /DoctorHome/Index
        // Replaces Page_Load in DoctorHome.aspx.cs
        // cr-dotnet-0045: Session["idoriginal"] (Line 21) now reads from
        //                 Redis-backed distributed session via GetInt32().
        // -----------------------------------------------------------------------
        [HttpGet]
        public IActionResult Index()
        {
            var vm = new DoctorHomeViewModel();

            // cr-dotnet-0045 fix (Line 21): replaced InProc Session["idoriginal"]
            // with distributed Redis-backed session via HttpContext.Session.GetInt32()
            int did = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            var objmyDAL = new myDAL();
            var dt = new DataTable();
            int found = objmyDAL.docinfo_DAL(did, ref dt);

            if (found != 1)
            {
                // Replaces Response.Write("<script>alert('There was some error');</script>")
                vm.ErrorMessage = "There was some error loading doctor information.";
            }
            else
            {
                // Replaces Label1.Text .. Label14.Text assignments
                vm.Name            = dt.Rows[0][1].ToString();
                vm.Phone           = dt.Rows[0][2].ToString();
                vm.Address         = dt.Rows[0][3].ToString();
                vm.BirthDate       = dt.Rows[0][4].ToString();
                vm.Gender          = dt.Rows[0][5].ToString();
                vm.DepartmentNo    = dt.Rows[0][6].ToString();
                vm.ChargesPerVisit = dt.Rows[0][7].ToString();
                vm.MonthlySalary   = dt.Rows[0][8].ToString();
                vm.ReputeIndex     = dt.Rows[0][9].ToString();
                vm.PatientsTreated = dt.Rows[0][10].ToString();
                vm.Qualification   = dt.Rows[0][11].ToString();
                vm.Specialization  = dt.Rows[0][12].ToString();
                vm.WorkExperience  = dt.Rows[0][13].ToString();
                vm.Status          = dt.Rows[0][14].ToString();
            }

            return View(vm);
        }
    }
}
