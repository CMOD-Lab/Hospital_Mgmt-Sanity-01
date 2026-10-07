using System;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;
using DBProject.Infrastructure;

// Migrated from ASP.NET Web Forms (System.Web.UI.Page) to ASP.NET Core Razor Pages (PageModel)
// Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages
// Rule cr-dotnet-0045: Session State Provider
//   Replaced in-process HttpSessionState with Amazon ElastiCache for Redis via
//   RedisSessionHelper (IDistributedCache) to enable stateless horizontal scaling.

namespace doctor
{
    public class BillModel : PageModel
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BillModel(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public string BillAmount { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // cr-dotnet-0045: Use Redis-backed distributed session (ElastiCache)
            // instead of in-process HttpSessionState to support horizontal scaling.
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            myDAL objmyDAL = new myDAL();
            DataTable dt = new DataTable();
            int found;

            int did = session.GetInt32("idoriginal") ?? 0;

            found = objmyDAL.generate_bill_DAL(did, ref dt);

            if (found != 1)
            {
                ErrorMessage = "There was some error";
            }
            else
            {
                BillAmount = dt.Rows[0][0].ToString();
            }
        }

        public IActionResult OnPostBillPaid()
        {
            // cr-dotnet-0045: Use Redis-backed distributed session (ElastiCache)
            // instead of in-process HttpSessionState to support horizontal scaling.
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            myDAL objmyDAL = new myDAL();

            int did = session.GetInt32("idoriginal") ?? 0;
            int appoint = session.GetInt32("appointid") ?? 0;
            objmyDAL.paid_bill_DAL(did, appoint);

            return RedirectToPage("patienthistory");
        }

        public IActionResult OnPostBillUnpaid()
        {
            // cr-dotnet-0045: Use Redis-backed distributed session (ElastiCache)
            // instead of in-process HttpSessionState to support horizontal scaling.
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            myDAL objmyDAL = new myDAL();

            int did = session.GetInt32("idoriginal") ?? 0;
            int appoint = session.GetInt32("appointid") ?? 0;
            objmyDAL.Unpaid_bill_DAL(did, appoint);

            return RedirectToPage("patienthistory");
        }
    }
}
