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
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//   Replaced synchronous OnGet/OnPost with async Task-based OnGetAsync/OnPostSelectDeptAsync
//   using async DAL methods (getdeptInfoAsync) connected to Amazon RDS via Dapper async APIs,
//   preventing thread pool exhaustion under load and enabling efficient auto-scaling.

namespace DBProject.Patient
{
    public class TakeAppointmentModel : PageModel
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TakeAppointmentModel(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public string TDept { get; set; }
        public DataTable Departments { get; set; }

        // cr-dotnet-1034: Converted from synchronous OnGet() to async Task OnGetAsync()
        // Uses async DAL method to prevent thread pool exhaustion under cloud load.
        public async Task OnGetAsync()
        {
            // cr-dotnet-0126: Use Redis-backed distributed session (Amazon ElastiCache)
            // instead of IIS in-process HttpSessionState to support horizontal scaling.
            // Original: Session["deptOriginal"] = "";
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            session.SetString("deptOriginal", "");
            await DeptInfoAsync();
        }

        //---------------Function Called whenever a Department is selected from the Grid View----//
        // cr-dotnet-1034: Converted from synchronous OnPostSelectDept to async Task<IActionResult> OnPostSelectDeptAsync
        public async Task<IActionResult> OnPostSelectDeptAsync(string deptName)
        {
            // cr-dotnet-0126: Use Redis-backed distributed session (Amazon ElastiCache)
            // instead of IIS in-process HttpSessionState to support horizontal scaling.
            // Original: Session["deptOriginal"] = deptName;
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            session.SetString("deptOriginal", deptName ?? "");
            return RedirectToPage("/Patient/ViewDoctors");
        }

        //-----------------------Function1--------------------------//

        // cr-dotnet-1034: Converted from synchronous DeptInfo() to async Task DeptInfoAsync()
        // Calls getdeptInfoAsync on the DAL to avoid blocking the thread pool on I/O.
        private async Task DeptInfoAsync()
        {
            myDAL objmyDAl = new myDAL();

            var (status, DT) = await objmyDAl.getdeptInfoAsync();

            if (status == -1)
            {
                TDept = "There was some error in retrieving the Departments Information.";
            }
            else
            {
                TDept       = "Following are the departments available at our Clinic : ";
                Departments = DT;
            }
        }

        //-----------------------Add a new function here------------------//
    }
}
