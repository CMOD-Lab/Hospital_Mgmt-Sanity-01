// ASP.NET Core MVC Controller for Doctor Profile (cr-dotnet-0026).
// Migrated from DoctorProfile.aspx.cs (Web Forms code-behind) to
// DoctorProfileController.cs.
//
// Web Forms patterns replaced:
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 14 – public partial class DoctorProfile : System.Web.UI.Page
//             → DoctorProfileController (MVC Controller)
//   Line 16 – Page_Load → doctorInfo()         → Index() GET action
//   DName.Text = name                          → DoctorProfileViewModel.Name
//   DPhone.Text = phone                        → DoctorProfileViewModel.Phone
//   DQualification.Text = qualification        → DoctorProfileViewModel.Qualification
//   DSpecialization.Text = specialization      → DoctorProfileViewModel.Specialization
//   DWork.Text = workE.ToString()              → DoctorProfileViewModel.WorkExperience
//   DAge.Text = age.ToString()                 → DoctorProfileViewModel.Age
//   DGender.Text = gender                      → DoctorProfileViewModel.Gender
//   DDept.Text = deptName                      → DoctorProfileViewModel.Department
//   DCharges.Text = charges_Per_Visit.ToString() → DoctorProfileViewModel.ChargesPerVisit
//   DRI.Text = ReputeIndex.ToString()          → DoctorProfileViewModel.ReputeIndex
//   DPT.Text = PatientsTreated.ToString()      → DoctorProfileViewModel.PatientsTreated
//   Response.Redirect("AppointmentTaker.aspx") → RedirectToAction("Index","AppointmentTaker")
//   Session["dID"]                             → HttpContext.Session.GetString
//   Session["deptOriginal"]                    → HttpContext.Session.GetString
//
// All business logic from the original code-behind is preserved.

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DBProject.DAL;
using DBProject.Models;

namespace DBProject.Controllers
{
    /// <summary>
    /// Handles the Doctor Profile page for patients.
    /// Replaces DoctorProfile.aspx + DoctorProfile.aspx.cs (Web Forms).
    /// </summary>
    public class DoctorProfileController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /DoctorProfile
        // Replaces Page_Load → doctorInfo() in DoctorProfile.aspx.cs
        // -----------------------------------------------------------------------
        [HttpGet]
        public IActionResult Index()
        {
            var vm = new DoctorProfileViewModel();
            LoadDoctorProfile(vm);
            return View(vm);
        }

        // -----------------------------------------------------------------------
        // POST /DoctorProfile/TakeAppointment
        // Replaces RedirectToAppointmentTaker (Button OnClick) in DoctorProfile.aspx.cs
        // Original: Response.Redirect("AppointmentTaker.aspx")
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TakeAppointment()
        {
            // Replaces: Response.Redirect("AppointmentTaker.aspx")
            return RedirectToAction("Index", "AppointmentTaker");
        }

        // -----------------------------------------------------------------------
        // Helper: loads doctor profile data from DAL into the view model.
        // Replaces doctorInfo() method in DoctorProfile.aspx.cs.
        // -----------------------------------------------------------------------
        private void LoadDoctorProfile(DoctorProfileViewModel vm)
        {
            var objmyDAl = new myDAL();

            // Replaces: string dID1 = (string)Session["dID"];
            string dID1 = HttpContext.Session.GetString("dID");
            int dID = Convert.ToInt32(dID1);

            string name = "";
            string phone = "";
            string gender = "";

            float charges_Per_Visit = 0;
            float ReputeIndex = 0;
            int PatientsTreated = 0;
            string qualification = "";
            string specialization = "";
            int workE = 0;
            int age = 0;

            // Replaces: string deptName = (string)Session["deptOriginal"];
            string deptName = HttpContext.Session.GetString("deptOriginal") ?? "";

            // Replaces: int status = objmyDAl.doctorInfoDisplayer(dID, ref name, ref phone, ...)
            int status = objmyDAl.doctorInfoDisplayer(
                dID,
                ref name,
                ref phone,
                ref gender,
                ref charges_Per_Visit,
                ref ReputeIndex,
                ref PatientsTreated,
                ref qualification,
                ref specialization,
                ref workE,
                ref age);

            if (status == -1)
            {
                // Replaces: Response.Write("<script>alert('There was some error in retrieving the Doctor's Info.');</script>");
                vm.ErrorMessage = "There was some error in retrieving the Doctor's Info.";
            }
            else if (status == 0)
            {
                // Replaces: DName.Text = name; DPhone.Text = phone; etc.
                vm.Name = name;
                vm.Phone = phone;
                vm.Qualification = qualification;
                vm.Specialization = specialization;
                vm.WorkExperience = workE.ToString();
                vm.Age = age.ToString();
                vm.Gender = gender;
                vm.Department = deptName;
                vm.ChargesPerVisit = charges_Per_Visit.ToString();
                vm.ReputeIndex = ReputeIndex.ToString();
                vm.PatientsTreated = PatientsTreated.ToString();
            }
        }
    }
}
