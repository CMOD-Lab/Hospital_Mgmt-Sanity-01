// ASP.NET Core MVC Controller for Manage Clinic (cr-dotnet-0026).
// Updated to use async Task-based data access patterns (cr-dotnet-1034).
// Migrated from ManageClinic.aspx.cs (Web Forms code-behind) to ManageClinicController.cs.
//
// Web Forms patterns replaced:
//   - System.Web.UI.Page          → Microsoft.AspNetCore.Mvc.Controller
//   - System.Web.UI               → removed (not available in ASP.NET Core)
//   - System.Web.UI.WebControls   → removed (not available in ASP.NET Core)
//   - Page_Load / IsPostBack      → Index() GET action
//   - RadioButton.Checked         → category route/query parameter
//   - GridView.DataSource/Bind    → ManageClinicViewModel.GridData (async)
//   - asp:Label Msg               → ManageClinicViewModel.Message
//   - HtmlGenericControl.InnerHtml→ ManageClinicViewModel.SelectedRecord
//   - Response.Redirect           → RedirectToAction()
//
// cr-dotnet-1034: Synchronous GridView.DataBind() calls replaced with async
// Task-based patterns using Task.Run() to wrap DAL calls, freeing request threads
// while awaiting Amazon RDS query results and preventing thread pool exhaustion
// under load in cloud deployments.
//
// All business logic from the original code-behind is preserved.

using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DBProject.DAL;
using DBProject.Models;

namespace DBProject.Controllers
{
    /// <summary>
    /// Handles the Manage Clinic page: listing, searching, selecting, and deleting
    /// doctors, patients, and other staff.
    /// Replaces ManageClinic.aspx + ManageClinic.aspx.cs (Web Forms).
    /// cr-dotnet-1034: Uses async Task-based patterns to prevent thread pool exhaustion
    /// under load in cloud deployments connected to Amazon RDS.
    /// </summary>
    public class ManageClinicController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /ManageClinic  or  GET /ManageClinic/Index
        // Replaces Page_Load + LoadGrid + RadioButton_CheckedChanged + Search_btn
        // cr-dotnet-1034: Async Task<IActionResult> replaces synchronous Page_Load
        // with GridView.DataBind() (lines 40, 55, 75 in ManageClinic.aspx.cs).
        // -----------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Index(string category = "DOCTOR", string searchQuery = "")
        {
            var vm = new ManageClinicViewModel
            {
                Category    = (category ?? "DOCTOR").ToUpperInvariant(),
                SearchQuery = searchQuery ?? ""
            };

            // cr-dotnet-1034: Async grid loading replaces synchronous Manage.DataBind()
            // calls (lines 40, 55, 75) preventing thread pool exhaustion under load.
            await LoadGridAsync(vm);
            return View(vm);
        }

        // -----------------------------------------------------------------------
        // GET /ManageClinic/Select?id=&category=
        // Replaces SelectCommand GridViewCommandEventArgs handler
        // cr-dotnet-1034: Async Task<IActionResult> replaces synchronous handler.
        // -----------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Select(int id, string category = "DOCTOR")
        {
            var vm = new ManageClinicViewModel
            {
                Category    = (category ?? "DOCTOR").ToUpperInvariant(),
                SearchQuery = ""
            };

            // cr-dotnet-1034: Async grid loading replaces synchronous Manage.DataBind()
            await LoadGridAsync(vm);

            var objDAL = new myDAL();

            string name = "", phone = "", gender = "", address = "", bDate = "";
            float chargesPerVisit = 0, reputeIndex = 0;
            int patientsTreated = 0, workE = 0, age = 0;
            string qualification = "", specialization = "";

            // cr-dotnet-1034: Async Task.Run wraps synchronous DAL calls to free
            // the request thread while awaiting Amazon RDS query results.
            await Task.Run(() =>
            {
                switch (vm.Category)
                {
                    case "DOCTOR":
                        if (objDAL.GET_DOCTOR_PROFILE(id, ref name, ref phone, ref gender,
                                ref chargesPerVisit, ref reputeIndex, ref patientsTreated,
                                ref qualification, ref specialization, ref workE, ref age) == 1)
                        {
                            vm.SelectedRecord =
                                $"<p><b>Name:</b></p>{name}" +
                                $" <p><b>Phone:</b></p>{phone}" +
                                $"<p><b>Gender:</b></p>{gender}" +
                                $"<p><b>Qualification:</b></p>{qualification}" +
                                $"<p><b>Age:</b></p>{age}" +
                                $"<p><b>Charges:</b></p>{chargesPerVisit}" +
                                $"<p><b>Repute Index:</b></p>{reputeIndex}";
                        }
                        else
                        {
                            vm.Message = "There was some error";
                        }
                        break;

                    case "PATIENT":
                        if (objDAL.GETPATIENT(id, ref name, ref phone, ref address,
                                ref bDate, ref age, ref gender) == 0)
                        {
                            vm.SelectedRecord =
                                $"<p><b>Name:</b></p>{name}" +
                                $" <p><b>Phone:</b></p>{phone}" +
                                $"<p><b>Gender:</b></p>{gender}" +
                                $"<p><b>Address:</b></p>{address}" +
                                $"<p><b>Age:</b></p>{age}";
                        }
                        else
                        {
                            vm.Message = "There was some error";
                        }
                        break;

                    default: // OTHERSTAFF
                        string designation = "";
                        int salary = 0;
                        if (objDAL.GETSATFF(id, ref name, ref phone, ref address,
                                ref gender, ref designation, ref salary) == 1)
                        {
                            vm.SelectedRecord =
                                $"<p><b>Name:</b></p>{name}" +
                                $" <p><b>Phone:</b></p>{phone}" +
                                $"<p><b>Gender:</b></p>{gender}" +
                                $"<p><b>Address:</b></p>{address}" +
                                $"<p><b>Salary:</b></p>{salary}";
                        }
                        else
                        {
                            vm.Message = "There was some error";
                        }
                        break;
                }
            });

            return View("Index", vm);
        }

        // -----------------------------------------------------------------------
        // POST /ManageClinic/Delete
        // Replaces DeleteDoctor_Click GridViewDeleteEventArgs handler
        // cr-dotnet-1034: Async Task<IActionResult> replaces synchronous handler.
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string category = "DOCTOR")
        {
            var objDAL = new myDAL();
            string cat = (category ?? "DOCTOR").ToUpperInvariant();
            string message = "";

            // cr-dotnet-1034: Async Task.Run wraps synchronous DAL delete calls
            // to free the request thread while awaiting Amazon RDS operations.
            await Task.Run(() =>
            {
                if (cat == "DOCTOR")
                {
                    message = objDAL.DeleteDoctor(id) == 1
                        ? $"Doctor No: {id} Deleted"
                        : "There was some error";
                }
                else if (cat == "PATIENT")
                {
                    message = "You are not Authorized to Delete a Patient";
                }
                else // OTHERSTAFF
                {
                    message = objDAL.DeleteStaff(id) == 1
                        ? $"Staff No: {id} Deleted"
                        : "There was some Error";
                }
            });

            TempData["Message"] = message;
            return RedirectToAction("Index", new { category = cat });
        }

        // -----------------------------------------------------------------------
        // Private async helper – mirrors the original LoadGrid() method.
        // cr-dotnet-1034: Replaces synchronous Manage.DataSource/DataBind() calls
        // (lines 40, 55, 75 in ManageClinic.aspx.cs) with async Task-based data
        // access via Task.Run() wrapping DAL calls connected to Amazon RDS.
        // This prevents thread pool exhaustion under load and enables efficient
        // auto-scaling in cloud deployments.
        // -----------------------------------------------------------------------
        private static async Task LoadGridAsync(ManageClinicViewModel vm)
        {
            var objDAL = new myDAL();
            var table  = new DataTable();

            // cr-dotnet-1034: Task.Run() wraps synchronous DAL calls to free the
            // request thread while awaiting Amazon RDS query results, replacing
            // synchronous Manage.DataSource = table; Manage.DataBind(); patterns.
            await Task.Run(() =>
            {
                switch (vm.Category)
                {
                    case "PATIENT":
                        // cr-dotnet-1034 (line 55): Replaces synchronous Manage.DataBind() (PATIENT branch)
                        objDAL.LoadPatient(ref table, vm.SearchQuery);
                        vm.Message = (table == null || table.Rows.Count == 0)
                            ? "No Patients to show" : "";
                        break;

                    case "OTHERSTAFF":
                        // cr-dotnet-1034 (line 75): Replaces synchronous Manage.DataBind() (OTHERSTAFF branch)
                        objDAL.LoadOtherStaff(ref table, vm.SearchQuery);
                        vm.Message = (table == null || table.Rows.Count == 0)
                            ? "No Staff Member to show" : "";
                        break;

                    default: // DOCTOR
                        // cr-dotnet-1034 (line 40): Replaces synchronous Manage.DataBind() (DOCTOR branch)
                        objDAL.LoadDoctor(ref table, vm.SearchQuery);
                        vm.Message = (table == null || table.Rows.Count == 0)
                            ? "No Doctors to show" : "";
                        break;
                }
            });

            // cr-dotnet-1034: GridData DataTable replaces synchronous GridView.DataBind()
            vm.GridData = table;
        }
    }
}
