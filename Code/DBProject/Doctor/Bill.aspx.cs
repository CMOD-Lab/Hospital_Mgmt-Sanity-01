// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-0045: Session State Provider – replaced InProc HttpSessionState
//   with Amazon ElastiCache for Redis distributed session store.
//
// Changes applied (cr-dotnet-0026):
//   Line 5 – removed: using System.Web.UI;
//   Line 6 – removed: using System.Web.UI.WebControls;
//   class no longer inherits System.Web.UI.Page
//   Page_Load replaced with OnGet Razor Pages handler
//   bill_paid / bill_Unpaid event handlers replaced with OnPostBillPaid / OnPostBillUnpaid
//   Response.Redirect replaced with RedirectToPage
//
// Changes applied (cr-dotnet-0045):
//   Line 21 – Session["idoriginal"] (InProc HttpSessionState) replaced with
//             IDistributedSessionService.GetInt32("idoriginal") backed by
//             Amazon ElastiCache for Redis, enabling stateless horizontal scaling
//             across multiple ECS tasks or Kubernetes pods.
//   Line 38 – Session["idoriginal"] replaced with distributed session read.
//   Line 39 – Session["appointid"] replaced with distributed session read.
//   Line 51 – Session["idoriginal"] replaced with distributed session read.
//   Line 52 – Session["appointid"] replaced with distributed session read.
//
// The PageModel pattern (ASP.NET Core Razor Pages) + Redis distributed session
// replaces the Web Forms / InProc session model, enabling stateless, cloud-native
// deployment on AWS (Linux containers, Elastic Beanstalk, ECS/Fargate).

using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;
using DBProject.Session;

namespace DBProject.Pages.Doctor
{
    /// <summary>
    /// Razor Pages PageModel for Bill – replaces the Web Forms
    /// bill : System.Web.UI.Page code-behind.
    /// Session state is backed by Amazon ElastiCache for Redis via
    /// IDistributedCache (Microsoft.Extensions.Caching.StackExchangeRedis).
    /// </summary>
    public class BillModel : PageModel
    {
        // ------------------------------------------------------------------ //
        // Properties bound to the Razor view
        // ------------------------------------------------------------------ //
        public string BillAmount { get; private set; } = string.Empty;
        public string ErrorMessage { get; private set; } = string.Empty;

        // ------------------------------------------------------------------ //
        // GET handler – replaces Page_Load
        // ------------------------------------------------------------------ //
        public void OnGet()
        {
            myDAL objmyDAL = new myDAL();
            DataTable dt = new DataTable();

            // cr-dotnet-0045 (Line 21): Read "idoriginal" from Redis-backed
            // distributed session instead of InProc HttpSessionState.
            int did = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            int found = objmyDAL.generate_bill_DAL(did, ref dt);

            if (found != 1)
            {
                ErrorMessage = "There was some error";
            }
            else
            {
                BillAmount = dt.Rows[0][0].ToString();
            }
        }

        // ------------------------------------------------------------------ //
        // POST handler – Bill Paid (replaces bill_paid event handler)
        // ------------------------------------------------------------------ //
        public IActionResult OnPostBillPaid()
        {
            myDAL objmyDAL = new myDAL();

            // cr-dotnet-0045 (Lines 38-39): Read session values from Redis-backed
            // distributed session instead of InProc HttpSessionState.
            int did     = HttpContext.Session.GetInt32("idoriginal") ?? 0;
            int appoint = HttpContext.Session.GetInt32("appointid") ?? 0;

            objmyDAL.paid_bill_DAL(did, appoint);

            // RedirectToPage replaces Response.Redirect("patienthistory.aspx")
            return RedirectToPage("/Doctor/PatientHistory");
        }

        // ------------------------------------------------------------------ //
        // POST handler – Bill Unpaid (replaces bill_Unpaid event handler)
        // ------------------------------------------------------------------ //
        public IActionResult OnPostBillUnpaid()
        {
            myDAL objmyDAL = new myDAL();

            // cr-dotnet-0045 (Lines 51-52): Read session values from Redis-backed
            // distributed session instead of InProc HttpSessionState.
            int did     = HttpContext.Session.GetInt32("idoriginal") ?? 0;
            int appoint = HttpContext.Session.GetInt32("appointid") ?? 0;

            objmyDAL.Unpaid_bill_DAL(did, appoint);

            // RedirectToPage replaces Response.Redirect("patienthistory.aspx")
            return RedirectToPage("/Doctor/PatientHistory");
        }
    }
}
