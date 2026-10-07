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
// Rule cr-dotnet-0126: Heavy Coupling to Stateful Middleware
//   Replaced IIS in-process HttpSessionState (sticky-session) with Amazon ElastiCache for Redis
//   via RedisSessionHelper (IDistributedCache) to enable stateless horizontal scaling.
//   Session data now persists across pod restarts and scales horizontally without sticky routing.
// Removed: System.Web, System.Web.UI, System.Web.UI.WebControls (Web Forms namespaces not available in ASP.NET Core)
// Removed: System.Collections.Generic, System.Linq, System.Web.HttpContext (Web Forms dependencies)
// Replaced: System.Web.UI.Page base class with Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// Replaced: Page_Load event handler with OnGet() Razor Pages lifecycle method
// Replaced: Server-side TextBox controls (.Text) with [BindProperty] model binding
// Replaced: Session["key"] with RedisSessionHelper (IDistributedCache) backed by Amazon ElastiCache
// Replaced: Response.Redirect with RedirectToPage
// Replaced: Response.Write (inline script alerts) with TempData messages rendered in the view

namespace DBProject
{
    public class SignUpModel : PageModel
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SignUpModel(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        // Login form bound properties
        [BindProperty]
        public string LoginEmail { get; set; }

        [BindProperty]
        public string LoginPassword { get; set; }

        // Sign-up form bound properties
        [BindProperty]
        public string SName { get; set; }

        [BindProperty]
        public string SBirthDate { get; set; }

        [BindProperty]
        public string SEmail { get; set; }

        [BindProperty]
        public string SPassword { get; set; }

        [BindProperty]
        public string ScPassword { get; set; }

        [BindProperty]
        public string Phone { get; set; }

        [BindProperty]
        public string Gender { get; set; }

        [BindProperty]
        public string Address { get; set; }

        // Message to display to the user (replaces Response.Write inline script alerts)
        public string AlertMessage { get; set; }

        public void OnGet()
        {
            // cr-dotnet-0126: Use Redis-backed distributed session (Amazon ElastiCache)
            // instead of IIS in-process HttpSessionState to support horizontal scaling.
            // Original: Session["idoriginal"] = "";
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            // Clear session on page load (equivalent to Page_Load)
            session.SetString("idoriginal", "");
        }

        //-----------------------Function1--------------------------//
        public IActionResult OnPostLogin()
        {
            string email = LoginEmail;
            string password = LoginPassword;

            myDAL objmyDAl = new myDAL();

            int status = 0;
            int type = 0;
            int id = 0;

            status = objmyDAl.validateLogin(email, password, ref type, ref id);

            if (status == 0)
            {
                // cr-dotnet-0126: Use Redis-backed distributed session (Amazon ElastiCache)
                // instead of IIS in-process HttpSessionState to support horizontal scaling.
                // Original: Session["idoriginal"] = id;
                var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

                session.SetString("idoriginal", id.ToString());

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
                TempData["AlertMessage"] = "Email not found. Try Again !";
            }
            else if (status == 2)
            {
                TempData["AlertMessage"] = "Incorrect Password. Try Again !";
            }
            else if (status == -1)
            {
                TempData["AlertMessage"] = "There was some error. Try Again !";
            }

            return Page();
        }

        //-----------------------Function2--------------------------//
        public IActionResult OnPostSignup()
        {
            string Name = SName;
            string BirthDate = SBirthDate;
            string Email = SEmail;
            string Password = SPassword;
            string PhoneNo = Phone;
            string Addr = Address;
            string gender = Gender;

            myDAL objmyDAl = new myDAL();

            int id = 0;

            int status = objmyDAl.validateUser(Name, BirthDate, Email, Password, PhoneNo, gender, Addr, ref id);

            // status == 0 failure
            if (status == 0)
            {
                TempData["AlertMessage"] = "Email already exists. Please choose a different one.";
            }
            else if (status == 1)
            {
                // cr-dotnet-0126: Use Redis-backed distributed session (Amazon ElastiCache)
                // instead of IIS in-process HttpSessionState to support horizontal scaling.
                // Original: Session["idoriginal"] = id;
                var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

                session.SetString("idoriginal", id.ToString());
                return RedirectToPage("/Patient/PatientHome");
            }
            else if (status == -1)
            {
                TempData["AlertMessage"] = "There was some error. Try again !";
            }

            return Page();
        }

        //Enter new function here//
    }
}
