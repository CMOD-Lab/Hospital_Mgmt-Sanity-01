// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-0045: Session State Provider – replaced InProc HttpSessionState
//   with Amazon ElastiCache for Redis distributed session store.
//
// Changes applied (cr-dotnet-0026):
//   Line 5 – removed: using System.Web;
//   Line 5 – removed: using System.Web.UI;
//   Line 6 – removed: using System.Web.UI.WebControls;
//   class no longer inherits System.Web.UI.Page
//   Page_Load replaced with OnGet Razor Pages handler
//   Label controls replaced with bound properties on the PageModel
//   Response.Write alert replaced with ErrorMessage property
//
// Changes applied (cr-dotnet-0045):
//   Line 21 – Session["idoriginal"] (InProc HttpSessionState) replaced with
//             HttpContext.Session.GetInt32("idoriginal") backed by
//             Amazon ElastiCache for Redis distributed session, enabling
//             stateless horizontal scaling across multiple ECS tasks or pods.
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
    /// Razor Pages PageModel for DoctorHome – replaces the Web Forms
    /// doctorhome : System.Web.UI.Page code-behind.
    /// Session state is backed by Amazon ElastiCache for Redis via
    /// IDistributedCache (Microsoft.Extensions.Caching.StackExchangeRedis).
    /// </summary>
    public class DoctorHomeModel : PageModel
    {
        // ------------------------------------------------------------------ //
        // Properties bound to the Razor view (replace Label server controls)
        // ------------------------------------------------------------------ //
        public string DoctorName           { get; private set; } = string.Empty;
        public string Phone                { get; private set; } = string.Empty;
        public string Address              { get; private set; } = string.Empty;
        public string BirthDate            { get; private set; } = string.Empty;
        public string Gender               { get; private set; } = string.Empty;
        public string DepartmentNo         { get; private set; } = string.Empty;
        public string ChargesPerVisit      { get; private set; } = string.Empty;
        public string MonthlySalary        { get; private set; } = string.Empty;
        public string ReputeIndex          { get; private set; } = string.Empty;
        public string PatientsTreated      { get; private set; } = string.Empty;
        public string Qualification        { get; private set; } = string.Empty;
        public string Specialization       { get; private set; } = string.Empty;
        public string WorkExperience       { get; private set; } = string.Empty;
        public string Status               { get; private set; } = string.Empty;
        public string ErrorMessage         { get; private set; } = string.Empty;

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

            int found = objmyDAL.docinfo_DAL(did, ref dt);

            if (found != 1)
            {
                ErrorMessage = "There was some error";
            }
            else
            {
                DoctorName      = dt.Rows[0][1].ToString();
                Phone           = dt.Rows[0][2].ToString();
                Address         = dt.Rows[0][3].ToString();
                BirthDate       = dt.Rows[0][4].ToString();
                Gender          = dt.Rows[0][5].ToString();
                DepartmentNo    = dt.Rows[0][6].ToString();
                ChargesPerVisit = dt.Rows[0][7].ToString();
                MonthlySalary   = dt.Rows[0][8].ToString();
                ReputeIndex     = dt.Rows[0][9].ToString();
                PatientsTreated = dt.Rows[0][10].ToString();
                Qualification   = dt.Rows[0][11].ToString();
                Specialization  = dt.Rows[0][12].ToString();
                WorkExperience  = dt.Rows[0][13].ToString();
                Status          = dt.Rows[0][14].ToString();
            }
        }
    }
}
