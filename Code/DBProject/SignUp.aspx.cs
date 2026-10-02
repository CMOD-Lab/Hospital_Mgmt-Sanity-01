// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-0126: Heavy Coupling to Stateful Middleware – replaced IIS sticky
//   sessions / in-process session state with Redis distributed cache (Amazon ElastiCache),
//   enabling stateless horizontal scaling without sticky session routing.
//
// Changes applied:
//   - removed: using System.Web;
//   - removed: using System.Web.UI;
//   - removed: using System.Web.UI.WebControls;
//   - removed: public partial class SignUp : System.Web.UI.Page
//             replaced with: public class SignUpModel : PageModel
//   - removed: protected void Page_Load(object sender, EventArgs e)
//             replaced with: public void OnGet()
//   - removed: protected void loginV(object sender, EventArgs e)
//             replaced with: public IActionResult OnPostLogin()
//   - removed: protected void signupV(object sender, EventArgs e)
//             replaced with: public IActionResult OnPostSignup()
//   - removed: Session["idoriginal"] = id (System.Web.SessionState / IIS sticky session)
//             replaced with: HttpContext.Session.SetInt32("idoriginal", id)
//   - removed: Response.Redirect("~/Patient/PatientHome.aspx")
//             replaced with: return RedirectToPage("/Patient/PatientHome")
//   - removed: Response.Write("<script>alert(...);</script>")
//             replaced with: TempData["ErrorMessage"] = "..."
//
// Rule cr-dotnet-0045 / cr-dotnet-0126: Session State Provider
//   In-process (InProc) HttpSessionState / IIS sticky session replaced with Amazon
//   ElastiCache for Redis distributed session store via ASP.NET Core ISession
//   (IDistributedCache-backed). Session data persists across pod restarts and scales
//   horizontally without sticky session routing.
//   HttpContext.Session.SetString() / HttpContext.Session.SetInt32() use the ASP.NET Core
//   ISession extension methods (Microsoft.AspNetCore.Http) backed by Amazon ElastiCache for Redis.
//   Redis session is registered via AddRedisDistributedSession() in
//   Session/RedisSessionConfiguration.cs, reading REDIS_CONNECTION_STRING from the
//   ECS task definition / Elastic Beanstalk environment / AWS Systems Manager.
//   Replaces Session["idoriginal"] = "" and Session["idoriginal"] = id from the original
//   InProc / IIS sticky session.
//   Enables stateless horizontal scaling across multiple ECS tasks or Kubernetes pods.
//
// The Web Forms Page lifecycle (Page_Load, server controls, ViewState,
// Response.Write for alerts) has been replaced with an ASP.NET Core
// Razor PageModel with named handler methods (OnPostLogin / OnPostSignup).
// Session access uses the PageModel's HttpContext (ASP.NET Core).
// All business logic is preserved.
// Enables stateless, cloud-native deployment on AWS
// (Linux containers, Elastic Beanstalk, ECS/Fargate).

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject.Pages
{
    /// <summary>
    /// Razor Page model for SignUp – replaces the Web Forms SignUp : System.Web.UI.Page.
    ///
    /// cr-dotnet-0126: IIS sticky session / stateful middleware coupling removed.
    /// Session state is backed by Amazon ElastiCache for Redis, allowing the application
    /// to scale horizontally across multiple instances without sticky session routing.
    /// </summary>
    public class SignUpModel : PageModel
    {
        // ---- Login bound properties ----
        [BindProperty]
        public string LoginEmail { get; set; } = string.Empty;

        [BindProperty]
        public string LoginPassword { get; set; } = string.Empty;

        // ---- Sign-up bound properties ----
        [BindProperty]
        public string SName { get; set; } = string.Empty;

        [BindProperty]
        public string SBirthDate { get; set; } = string.Empty;

        [BindProperty]
        public string SEmail { get; set; } = string.Empty;

        [BindProperty]
        public string SPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ScPassword { get; set; } = string.Empty;

        [BindProperty]
        public string Phone { get; set; } = string.Empty;

        [BindProperty]
        public string Gender { get; set; } = string.Empty;

        [BindProperty]
        public string Address { get; set; } = string.Empty;

        // Replaces protected void Page_Load(object sender, EventArgs e)
        public void OnGet()
        {
            // cr-dotnet-0126 (Line 17): Session["idoriginal"] = "" (IIS sticky session
            // write / InProc HttpSessionState) replaced with
            // HttpContext.Session.SetString("idoriginal", "") backed by Amazon ElastiCache
            // for Redis distributed session store. Session data persists across pod restarts
            // and scales horizontally without sticky session routing at the load balancer.
            // Replaces: Session["idoriginal"] = "";
            HttpContext.Session.SetString("idoriginal", "");
        }

        //-----------------------Function1--------------------------//
        // Replaces protected void loginV(object sender, EventArgs e)
        public IActionResult OnPostLogin()
        {
            myDAL objmyDAl = new myDAL();

            int status = 0;
            int type = 0;
            int id = 0;

            status = objmyDAl.validateLogin(LoginEmail, LoginPassword, ref type, ref id);

            if (status == 0)
            {
                // cr-dotnet-0126: HttpContext.Session.SetInt32() uses the ASP.NET Core
                // ISession API backed by Amazon ElastiCache for Redis distributed session store.
                // Session data persists across pod restarts and scales horizontally without
                // sticky session routing at the load balancer.
                // Replaces: Session["idoriginal"] = id; (IIS sticky session write)
                HttpContext.Session.SetInt32("idoriginal", id);

                if (type == 1)
                {
                    return RedirectToPage("/Patient/PatientHome");
                }
                else if (type == 2)
                {
                    return RedirectToPage("/Doctor/DoctorHome");
                }
                else if (type == 3)
                {
                    return RedirectToPage("/Admin/AdminHome");
                }
            }
            else if (status == 1)
            {
                TempData["ErrorMessage"] = "Email not found. Try Again !";
            }
            else if (status == 2)
            {
                TempData["ErrorMessage"] = "Incorrect Password. Try Again !";
            }
            else if (status == -1)
            {
                TempData["ErrorMessage"] = "There was some error. Try Again !";
            }

            return Page();
        }

        //-----------------------Function2--------------------------//
        // Replaces protected void signupV(object sender, EventArgs e)
        public IActionResult OnPostSignup()
        {
            myDAL objmyDAl = new myDAL();

            int id = 0;

            int status = objmyDAl.validateUser(SName, SBirthDate, SEmail, SPassword, Phone, Gender, Address, ref id);

            // status == 0 failure
            if (status == 0)
            {
                TempData["ErrorMessage"] = "Email already exists. Please choose a different one.";
            }
            else if (status == 1)
            {
                // cr-dotnet-0126: HttpContext.Session.SetInt32() uses the ASP.NET Core
                // ISession API backed by Amazon ElastiCache for Redis distributed session store.
                // Session data persists across pod restarts and scales horizontally without
                // sticky session routing at the load balancer.
                // Replaces: Session["idoriginal"] = id; (IIS sticky session write)
                HttpContext.Session.SetInt32("idoriginal", id);

                return RedirectToPage("/Patient/PatientHome");
            }
            else if (status == -1)
            {
                TempData["ErrorMessage"] = "There was some error. Try again !";
            }

            return Page();
        }

        //Enter new function here//
    }
}
