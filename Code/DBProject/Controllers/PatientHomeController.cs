// ASP.NET Core MVC Controller for Patient Home (cr-dotnet-0026).
// Migrated from PatientHome.aspx.cs (Web Forms code-behind) to
// PatientHomeController.cs.
//
// Web Forms patterns replaced:
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 12 – public partial class PatientHome : System.Web.UI.Page
//             → PatientHomeController (MVC Controller)
//   Line 14 – Page_Load → patientInfo()        → Index() GET action
//   PName.Text = name                          → PatientHomeViewModel.Name
//   PPhone.Text = phone                        → PatientHomeViewModel.Phone
//   PBirthDate.Text = birthDate                → PatientHomeViewModel.BirthDate
//   PatientAge.Text = age.ToString()           → PatientHomeViewModel.Age
//   PAddress.Text = address                    → PatientHomeViewModel.Address
//   PGender.Text = gender                      → PatientHomeViewModel.Gender
//   Response.Write("<script>alert(...)...")    → PatientHomeViewModel.ErrorMessage
//   Session["idoriginal"]                      → HttpContext.Session.GetInt32
//
// All business logic from the original code-behind is preserved.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DBProject.DAL;
using DBProject.Models;

namespace DBProject.Controllers
{
    /// <summary>
    /// Handles the Patient Home page.
    /// Replaces PatientHome.aspx + PatientHome.aspx.cs (Web Forms).
    /// </summary>
    public class PatientHomeController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /PatientHome
        // Replaces Page_Load → patientInfo() in PatientHome.aspx.cs
        // -----------------------------------------------------------------------
        [HttpGet]
        public IActionResult Index()
        {
            var vm = new PatientHomeViewModel();
            LoadPatientInfo(vm);
            return View(vm);
        }

        // -----------------------------------------------------------------------
        // Helper: loads patient information from DAL into the view model.
        // Replaces patientInfo() method in PatientHome.aspx.cs.
        // -----------------------------------------------------------------------
        private void LoadPatientInfo(PatientHomeViewModel vm)
        {
            var objmyDAl = new myDAL();

            // Replaces: int pid = (int)Session["idoriginal"];
            int pid = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            string name      = "";
            string phone     = "";
            string address   = "";
            string birthDate = "";
            int    age       = 0;
            string gender    = "";

            // Replaces: int status = objmyDAl.patientInfoDisplayer(pid, ref name, ref phone,
            //               ref address, ref birthDate, ref age, ref gender);
            int status = objmyDAl.patientInfoDisplayer(pid, ref name, ref phone, ref address, ref birthDate, ref age, ref gender);

            if (status == -1)
            {
                // Replaces: Response.Write("<script>alert('There was some error in retrieving the Patient's Info.');</script>");
                vm.ErrorMessage = "There was some error in retrieving the Patient's Info.";
            }
            else if (status == 0)
            {
                // Replaces: PName.Text = name; PPhone.Text = phone; PBirthDate.Text = birthDate;
                //           PatientAge.Text = age.ToString(); PAddress.Text = address; PGender.Text = gender;
                vm.Name      = name;
                vm.Phone     = phone;
                vm.BirthDate = birthDate;
                vm.Age       = age;
                vm.Address   = address;
                vm.Gender    = gender;
            }
        }
    }
}
