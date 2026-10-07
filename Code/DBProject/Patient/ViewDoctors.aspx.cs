using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;
using DBProject.Infrastructure;

// Migrated from ASP.NET Web Forms (System.Web.UI.Page) to ASP.NET Core Razor Pages (PageModel)
// Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages
// Rule cr-dotnet-0126: Heavy Coupling to Stateful Middleware
//   Replaced IIS in-process HttpSessionState (sticky-session) with Amazon ElastiCache for Redis
//   via RedisSessionHelper (IDistributedCache) to enable stateless horizontal scaling.
//   Session data now persists across pod restarts and scales horizontally without sticky routing.
// Removed: System.Web, System.Web.UI, System.Web.UI.WebControls (Web Forms namespaces not available in ASP.NET Core)
// Replaced: System.Web.UI.Page base class with Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// Replaced: Page_Load event handler with OnGetAsync() Razor Pages lifecycle method
// Replaced: GridView server control with Razor Pages model-bound DataTable rendered in .cshtml view
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//   Replaced synchronous OnGet/OnPostSelectDoctor with async Task-based OnGetAsync/OnPostSelectDoctorAsync
//   using async DAL methods (getDeptDoctorInfoAsync) connected to Amazon RDS via Dapper async APIs,
//   preventing thread pool exhaustion under load and enabling efficient auto-scaling.

namespace DBProject.Patient
{
    public class ViewDoctorsModel : PageModel
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ViewDoctorsModel(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public string TDoctor { get; set; }
        public DataTable Doctors { get; set; }

        // cr-dotnet-1034: Converted from synchronous OnGet() to async Task OnGetAsync()
        // Uses async DAL method to prevent thread pool exhaustion under cloud load.
        public async Task OnGetAsync()
        {
            // cr-dotnet-0126: Use Redis-backed distributed session (Amazon ElastiCache)
            // instead of IIS in-process HttpSessionState to support horizontal scaling.
            // Original: Session["dID"] = "";
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            session.SetString("dID", "");
            await DeptDoctorInfoAsync();
        }

        //---------------Function Called whenever a Doctor is selected from the Grid View----//
        // cr-dotnet-1034: Converted from synchronous OnPostSelectDoctor to async Task<IActionResult> OnPostSelectDoctorAsync
        public async Task<IActionResult> OnPostSelectDoctorAsync(string dID)
        {
            // cr-dotnet-0126: Use Redis-backed distributed session (Amazon ElastiCache)
            // instead of IIS in-process HttpSessionState to support horizontal scaling.
            // Original: Session["dID"] = dID;
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            session.SetString("dID", dID ?? "");
            return RedirectToPage("/Patient/DoctorProfile");
        }

        //-----------------------Function1--------------------------//

        // cr-dotnet-1034: Converted from synchronous DeptDoctorInfo() to async Task DeptDoctorInfoAsync()
        // Calls getDeptDoctorInfoAsync on the DAL to avoid blocking the thread pool on I/O.
        private async Task DeptDoctorInfoAsync()
        {
            myDAL objmyDAl = new myDAL();

            // cr-dotnet-0126: Use Redis-backed distributed session (Amazon ElastiCache)
            // instead of IIS in-process HttpSessionState to support horizontal scaling.
            // Original: string deptName = (string)Session["deptOriginal"];
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            string deptName = session.GetString("deptOriginal");

            var (status, DT) = await objmyDAl.getDeptDoctorInfoAsync(deptName);

            if (status == -1)
            {
                TDoctor = "There was some error in retrieving the Doctors Information.";
            }
            else
            {
                TDoctor = "Following are our Specialized Doctors of " + session.GetString("deptOriginal") + " Department:";
                Doctors = DT;
            }
        }

        //-----------------------Add a new function here------------------//
    }
}
