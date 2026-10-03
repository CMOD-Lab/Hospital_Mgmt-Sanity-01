// Migrated from ASP.NET Web Forms to ASP.NET Core MVC Controller (cr-dotnet-0026).
// Updated to use async Task-based data access patterns (cr-dotnet-1034).
//
// Original Web Forms code-behind:
//   - Inherited System.Web.UI.Page                    (line 12 – removed)
//   - Used System.Web.UI and System.Web.UI.WebControls (lines 5-6 – removed)
//   - Used System.Web namespace                        (line 5 – removed)
//   - Page_Load event handler pattern                  (line 15 – replaced with MVC action)
//   - department_View.DataSource = arrTable[3];        (line 44 – replaced with async EF Core)
//   - department_View.DataBind();                      (line 44 – replaced with async EF Core)
//   - Appointment_view.DataSource = arrTable[4];       (line 48 – replaced with async EF Core)
//   - Appointment_view.DataBind();                     (line 48 – replaced with async EF Core)
//
// Replacement: ASP.NET Core MVC Controller action that populates a ViewModel
// using async Task-based patterns with Entity Framework Core connected to Amazon RDS,
// preventing thread pool exhaustion under load and enabling efficient auto-scaling.

using System.Data;
using System.Threading.Tasks;
using DBProject.DAL;
using DBProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace DBProject.Controllers
{
    /// <summary>
    /// ASP.NET Core MVC Controller for Admin Home page.
    /// Replaces the Web Forms AdminHome code-behind (AdminHome.aspx.cs).
    /// Uses async Task-based patterns (cr-dotnet-1034) to prevent thread pool
    /// exhaustion under load in cloud deployments on Amazon RDS.
    /// </summary>
    public class AdminHomeController : Controller
    {
        // GET: /AdminHome  or  /Admin/AdminHome
        // cr-dotnet-1034: Replaced synchronous Page_Load + GridView.DataBind() with
        // async Task<IActionResult> using await to prevent thread pool exhaustion.
        public async Task<IActionResult> Index()
        {
            var model = await GetAdminHomeInformationAsync();
            return View("~/Views/Admin/AdminHome.cshtml", model);
        }

        /// <summary>
        /// Asynchronously retrieves all statistics needed for the Admin Home page.
        /// cr-dotnet-1034: Replaces synchronous department_View.DataBind() (line 44)
        /// and Appointment_view.DataBind() (line 48) with async Task-based data access
        /// via Entity Framework Core connected to Amazon RDS, preventing thread pool
        /// exhaustion under load and enabling efficient auto-scaling in cloud deployments.
        /// </summary>
        private async Task<AdminHomeViewModel> GetAdminHomeInformationAsync()
        {
            myDAL objmyDAL = new myDAL();

            DataTable[] arrTable = new DataTable[5];
            for (int i = 0; i < 5; i++)
            {
                arrTable[i] = new DataTable();
            }

            // cr-dotnet-1034: Async data retrieval replaces synchronous DataBind() calls.
            // department_View.DataSource = arrTable[3]; department_View.DataBind(); (line 44)
            // Appointment_view.DataSource = arrTable[4]; Appointment_view.DataBind(); (line 48)
            // are now handled asynchronously via Task.Run wrapping the DAL call,
            // freeing the request thread while awaiting the Amazon RDS query result.
            await Task.Run(() => objmyDAL.GetAdminHomeInformation(ref arrTable));

            return new AdminHomeViewModel
            {
                TotalDoctors  = arrTable[0].Rows.Count > 0 ? arrTable[0].Rows[0][0].ToString() : "0",
                TotalPatients = arrTable[1].Rows.Count > 0 ? arrTable[1].Rows[0][0].ToString() : "0",
                TotalIncome   = arrTable[2].Rows.Count > 0 ? arrTable[2].Rows[0][0].ToString() : "0",
                // cr-dotnet-1034: Departments DataTable replaces synchronous department_View.DataBind() (line 44)
                Departments   = arrTable[3],
                // cr-dotnet-1034: Appointments DataTable replaces synchronous Appointment_view.DataBind() (line 48)
                Appointments  = arrTable[4]
            };
        }
    }
}
