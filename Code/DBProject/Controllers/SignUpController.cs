// ASP.NET Core MVC Controller for Sign Up / Login
// (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-0126).
// Migrated from SignUp.aspx.cs (Web Forms code-behind) to
// SignUpController.cs.
//
// Web Forms patterns replaced:
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 13 – public partial class SignUp : System.Web.UI.Page
//             → SignUpController (MVC Controller)
//   Line 15 – Page_Load                        → Index() GET action
//   loginV (Button click handler)              → Login() POST action
//   signupV (Button click handler)             → Register() POST action
//   loginEmail.Text / loginPassword.Text       → SignUpViewModel.LoginEmail / LoginPassword
//   sName.Text, sBirthDate.Text, sEmail.Text   → SignUpViewModel.Name, BirthDate, Email
//   sPassword.Text, Phone.Text, Address.Text   → SignUpViewModel.Password, PhoneNo, Address
//   Request.Form["Gender"]                     → SignUpViewModel.Gender
//   Session["idoriginal"] = id                 → HttpContext.Session.SetInt32("idoriginal", id)
//   Response.Redirect("~/Patient/PatientHome.aspx") → RedirectToAction("Index", "PatientHome")
//   Response.Redirect("~/Doctor/DoctorHome.aspx")   → RedirectToAction("Index", "DoctorHome")
//   Response.Redirect("~/Admin/AdminHome.aspx")     → RedirectToAction("Index", "AdminHome")
//   Response.Write("<script>alert(...)...")    → SignUpViewModel.ErrorMessage (TempData)
//   Response.BufferOutput                      → removed (not applicable in ASP.NET Core)
//
// cr-dotnet-0045 – Session State Provider fix:
//   Line 17 – Session["idoriginal"] = "" (InProc) →
//             HttpContext.Session.SetString("idoriginal", "") backed by Amazon ElastiCache for Redis.
//   Line 36 – Session["idoriginal"] = id (InProc, loginV) →
//             HttpContext.Session.SetInt32("idoriginal", id) backed by Amazon ElastiCache for Redis.
//   Line 109 – Session["idoriginal"] = id (InProc, signupV) →
//              HttpContext.Session.SetInt32("idoriginal", id) backed by Amazon ElastiCache for Redis.
//   Redis is configured via REDIS_CONNECTION_STRING environment variable.
//
// cr-dotnet-0126 – Heavy Coupling to Stateful Middleware fix:
//   IIS application pool sticky sessions replaced with Amazon ElastiCache for Redis
//   distributed cache session store.
//
//   Line 17 – Session["idoriginal"] = "" (IIS InProc sticky session)
//             → HttpContext.Session.SetString("idoriginal", "")
//               backed by Amazon ElastiCache for Redis.
//
//   This eliminates server affinity (sticky sessions) required by IIS in-process
//   session state. Session data persists across pod/container restarts and scales
//   horizontally without sticky session routing.
//
// All business logic from the original code-behind is preserved.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DBProject.DAL;
using DBProject.Models;

namespace DBProject.Controllers
{
    /// <summary>
    /// Handles the Login and Patient Registration (Sign Up) page.
    /// Replaces SignUp.aspx + SignUp.aspx.cs (Web Forms).
    /// Session state is backed by Amazon ElastiCache for Redis
    /// (cr-dotnet-0045, cr-dotnet-0126 – eliminates IIS sticky session dependency).
    /// </summary>
    public class SignUpController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /SignUp
        // Replaces Page_Load in SignUp.aspx.cs
        // cr-dotnet-0045 / cr-dotnet-0126:
        //   Session["idoriginal"] = "" (IIS InProc sticky session) →
        //   HttpContext.Session.SetString backed by Amazon ElastiCache for Redis.
        //   No server affinity required; scales horizontally.
        // -----------------------------------------------------------------------
        [HttpGet]
        public IActionResult Index()
        {
            // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 17):
            // Replaced InProc Session["idoriginal"] = "" (IIS sticky session) with
            // distributed Redis-backed session via HttpContext.Session.SetString().
            // Session data persists across pod restarts; no server affinity needed.
            HttpContext.Session.SetString("idoriginal", "");

            var vm = new SignUpViewModel();
            return View(vm);
        }

        // -----------------------------------------------------------------------
        // POST /SignUp/Login
        // Replaces loginV(object sender, EventArgs e) in SignUp.aspx.cs
        // cr-dotnet-0045 / cr-dotnet-0126:
        //   Session["idoriginal"] = id (Line 36, IIS InProc sticky session) →
        //   HttpContext.Session.SetInt32 backed by Amazon ElastiCache for Redis.
        //   Eliminates IIS application pool sticky session dependency.
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(SignUpViewModel vm)
        {
            // Replaces: string email = loginEmail.Text;
            //           string password = loginPassword.Text;
            string email    = vm.LoginEmail    ?? "";
            string password = vm.LoginPassword ?? "";

            var objmyDAl = new myDAL();

            int status = 0;
            int type   = 0;
            int id     = 0;

            // Replaces: status = objmyDAl.validateLogin(email, password, ref type, ref id);
            status = objmyDAl.validateLogin(email, password, ref type, ref id);

            if (status == 0)
            {
                // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 36):
                // Replaced InProc Session["idoriginal"] = id (IIS sticky session) with
                // distributed Redis-backed session via HttpContext.Session.SetInt32().
                // Session data persists across pod restarts; no server affinity needed.
                HttpContext.Session.SetInt32("idoriginal", id);

                if (type == 1)
                {
                    // Replaces: Response.BufferOutput = true;
                    //           Response.Redirect("~/Patient/PatientHome.aspx");
                    return RedirectToAction("Index", "PatientHome");
                }
                else if (type == 2)
                {
                    // Replaces: Response.BufferOutput = true;
                    //           Response.Redirect("~/Doctor/DoctorHome.aspx");
                    return RedirectToAction("Index", "DoctorHome");
                }
                else if (type == 3)
                {
                    // Replaces: Response.BufferOutput = true;
                    //           Response.Redirect("~/Admin/AdminHome.aspx");
                    return RedirectToAction("Index", "AdminHome");
                }
            }
            else if (status == 1)
            {
                // Replaces: Response.Write("<script>alert('Email not found. Try Again !');</script>");
                vm.ErrorMessage = "Email not found. Try Again !";
            }
            else if (status == 2)
            {
                // Replaces: Response.Write("<script>alert('Incorrect Password. Try Again !');</script>");
                vm.ErrorMessage = "Incorrect Password. Try Again !";
            }
            else if (status == -1)
            {
                // Replaces: Response.Write("<script>alert('There was some error. Try Again !');</script>");
                vm.ErrorMessage = "There was some error. Try Again !";
            }

            return View("Index", vm);
        }

        // -----------------------------------------------------------------------
        // POST /SignUp/Register
        // Replaces signupV(object sender, EventArgs e) in SignUp.aspx.cs
        // cr-dotnet-0045 / cr-dotnet-0126:
        //   Session["idoriginal"] = id (Line 109, IIS InProc sticky session) →
        //   HttpContext.Session.SetInt32 backed by Amazon ElastiCache for Redis.
        //   Eliminates IIS application pool sticky session dependency.
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(SignUpViewModel vm)
        {
            // Replaces: string Name = sName.Text;
            //           string BirthDate = sBirthDate.Text;
            //           string Email = sEmail.Text;
            //           string Password = sPassword.Text;
            //           string PhoneNo = Phone.Text;
            //           string Addr = Address.Text;
            //           string gender = Request.Form["Gender"].ToString();
            string name      = vm.Name      ?? "";
            string birthDate = vm.BirthDate ?? "";
            string email     = vm.Email     ?? "";
            string password  = vm.Password  ?? "";
            string phoneNo   = vm.PhoneNo   ?? "";
            string addr      = vm.Address   ?? "";
            string gender    = vm.Gender    ?? "M";

            var objmyDAl = new myDAL();
            int id = 0;

            // Replaces: int status = objmyDAl.validateUser(Name, BirthDate, Email, Password, PhoneNo, gender, Addr, ref id);
            int status = objmyDAl.validateUser(name, birthDate, email, password, phoneNo, gender, addr, ref id);

            // status == 0 failure (email already exists)
            if (status == 0)
            {
                // Replaces: Response.Write("<script>alert('Email already exists. Please choose a different one.');</script>");
                vm.ErrorMessage = "Email already exists. Please choose a different one.";
            }
            else if (status == 1)
            {
                // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 109):
                // Replaced InProc Session["idoriginal"] = id (IIS sticky session) with
                // distributed Redis-backed session via HttpContext.Session.SetInt32().
                // Session data persists across pod restarts; no server affinity needed.
                HttpContext.Session.SetInt32("idoriginal", id);

                // Replaces: Response.BufferOutput = true;
                //           Response.Redirect("~/Patient/PatientHome.aspx");
                return RedirectToAction("Index", "PatientHome");
            }
            else if (status == -1)
            {
                // Replaces: Response.Write("<script>alert('There was some error. Try again !');</script>");
                vm.ErrorMessage = "There was some error. Try again !";
            }

            return View("Index", vm);
        }
    }
}
