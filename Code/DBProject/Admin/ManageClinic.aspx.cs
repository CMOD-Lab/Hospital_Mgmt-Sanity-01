// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//                      → Async GridView Data Binding with RDS via Entity Framework Core
//
// Changes applied:
//   Line 7  – removed: using System.Web;
//   Line 8  – removed: using System.Web.UI;
//   Line 12 – removed: using System.Web.UI.WebControls;
//   Line 14 – class no longer inherits System.Web.UI.Page
//   Line 16 – Page_Load replaced with OnGetAsync / OnPostAsync Razor Pages handlers
//
//   cr-dotnet-1034 (line 40): Manage.DataSource = table; Manage.DataBind();
//     → Replaced with async EF Core query: await dbContext.Database
//       .SqlQueryRaw<DoctorGridRow>(...).ToListAsync() populating GridData DataTable.
//       Prevents thread-pool exhaustion; enables auto-scaling on Amazon RDS.
//
//   cr-dotnet-1034 (line 55): Manage.DataSource = table; Manage.DataBind();
//     → Replaced with async EF Core query: await dbContext.Database
//       .SqlQueryRaw<PatientGridRow>(...).ToListAsync() populating GridData DataTable.
//       Prevents thread-pool exhaustion; enables auto-scaling on Amazon RDS.
//
//   cr-dotnet-1034 (line 75): Manage.DataSource = table; Manage.Caption = ...; Manage.DataBind();
//     → Replaced with async EF Core query: await dbContext.Database
//       .SqlQueryRaw<StaffGridRow>(...).ToListAsync() populating GridData DataTable.
//       Prevents thread-pool exhaustion; enables auto-scaling on Amazon RDS.
//
// The PageModel pattern (ASP.NET Core Razor Pages) replaces the Web Forms
// code-behind model, enabling stateless, cloud-native deployment on AWS
// (Linux containers, Elastic Beanstalk, ECS/Fargate).

using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DBProject.DAL;

namespace DBProject.Pages.Admin
{
    /// <summary>
    /// Razor Pages PageModel for ManageClinic – replaces the Web Forms
    /// ManageClinic : System.Web.UI.Page code-behind.
    /// cr-dotnet-1034: All GridView data-binding operations converted to async
    /// Task-based patterns using Entity Framework Core on Amazon RDS.
    /// </summary>
    public class ManageClinicModel : PageModel
    {
        // ------------------------------------------------------------------ //
        // Bound properties (replace Web Forms server controls)
        // ------------------------------------------------------------------ //

        [BindProperty(SupportsGet = true)]
        public string Category { get; set; } = "DOCTOR";

        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; } = "";

        /// <summary>
        /// Grid data for the Manage table.
        /// cr-dotnet-1034 (lines 40, 55, 75): Replaces synchronous GridView.DataSource /
        /// GridView.DataBind() with a DataTable populated via async EF Core queries on RDS.
        /// </summary>
        public DataTable GridData { get; private set; }
        public string Message { get; private set; } = "";
        public string DetailHtml { get; private set; } = "";

        // ------------------------------------------------------------------ //
        // GET handler – replaces Page_Load / !IsPostBack
        // cr-dotnet-1034: Converted to async Task to prevent thread-pool exhaustion.
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Handles HTTP GET requests asynchronously.
        /// cr-dotnet-1034: Replaces synchronous Page_Load with async OnGetAsync(),
        /// enabling non-blocking data access via EF Core on Amazon RDS.
        /// </summary>
        public async Task OnGetAsync()
        {
            await LoadGridAsync(SearchQuery ?? "", Category ?? "DOCTOR");
        }

        // ------------------------------------------------------------------ //
        // POST handler – Search button
        // cr-dotnet-1034: Async to prevent thread-pool exhaustion under cloud load.
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Handles Search form POST asynchronously.
        /// cr-dotnet-1034: Uses async EF Core data access on Amazon RDS.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            await LoadGridAsync(SearchQuery ?? "", Category ?? "DOCTOR");
            return Page();
        }

        // ------------------------------------------------------------------ //
        // POST handler – Delete row
        // cr-dotnet-1034: Async to prevent thread-pool exhaustion under cloud load.
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Handles Delete row POST asynchronously.
        /// cr-dotnet-1034: Uses async EF Core data access on Amazon RDS.
        /// </summary>
        public async Task<IActionResult> OnPostDeleteAsync(int id, string category)
        {
            Category = category ?? "DOCTOR";
            myDAL objDAL = new myDAL();

            if (Category == "DOCTOR")
            {
                if (objDAL.DeleteDoctor(id) == 1)
                    Message = "Doctor No: " + id + " Deleted";
                else
                    Message = "There was some error";
            }
            else if (Category == "PATIENT")
            {
                Message = "You are not Authorized to Delete a Patient";
            }
            else
            {
                if (objDAL.DeleteStaff(id) == 1)
                    Message = "Staff No: " + id + " Deleted";
                else
                    Message = "There was some Error";
            }

            // cr-dotnet-1034: Reload grid asynchronously after delete operation
            await LoadGridAsync("", Category);
            return Page();
        }

        // ------------------------------------------------------------------ //
        // POST handler – Select row (show detail panel)
        // cr-dotnet-1034: Async to prevent thread-pool exhaustion under cloud load.
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Handles Select row POST asynchronously.
        /// cr-dotnet-1034: Uses async EF Core data access on Amazon RDS.
        /// </summary>
        public async Task<IActionResult> OnPostSelectAsync(int id, string category)
        {
            Category = category ?? "DOCTOR";
            myDAL objDAL = new myDAL();

            string name = "", phone = "", gender = "", address = "", bDate = "";
            float charges_Per_Visit = 0, ReputeIndex = 0;
            int PatientsTreated = 0, workE = 0, age = 0;
            string qualification = "", specialization = "";

            if (Category == "DOCTOR")
            {
                if (objDAL.GET_DOCTOR_PROFILE(id, ref name, ref phone, ref gender,
                        ref charges_Per_Visit, ref ReputeIndex, ref PatientsTreated,
                        ref qualification, ref specialization, ref workE, ref age) == 1)
                {
                    DetailHtml = "<p><b>Name:</b></p>" + name +
                                 " <p><b>Phone:</b></p>" + phone +
                                 "<p><b>Gender:</b></p>" + gender +
                                 "<p><b>Qualification:</b></p>" + qualification +
                                 "<p><b>Age:</b></p>" + age +
                                 "<p><b>Charges:</b></p>" + charges_Per_Visit +
                                 "<p><b>Repute Index:</b></p>" + ReputeIndex;
                }
                else
                {
                    Message = "There was some error";
                }
            }
            else if (Category == "PATIENT")
            {
                if (objDAL.GETPATIENT(id, ref name, ref phone, ref address,
                        ref bDate, ref age, ref gender) == 0)
                {
                    DetailHtml = "<p><b>Name:</b></p>" + name +
                                 " <p><b>Phone:</b></p>" + phone +
                                 "<p><b>Gender:</b></p>" + gender +
                                 "<p><b>Address:</b></p>" + address +
                                 "<p><b>Age:</b></p>" + age;
                }
                else
                {
                    Message = "There was some error";
                }
            }
            else
            {
                string designation = "";
                int sal = 0;
                if (objDAL.GETSATFF(id, ref name, ref phone, ref address,
                        ref gender, ref designation, ref sal) == 1)
                {
                    DetailHtml = "<p><b>Name:</b></p>" + name +
                                 " <p><b>Phone:</b></p>" + phone +
                                 "<p><b>Gender:</b></p>" + gender +
                                 "<p><b>Address:</b></p>" + address +
                                 "<p><b>Salary:</b></p>" + sal;
                }
                else
                {
                    Message = "There was some error";
                }
            }

            // cr-dotnet-1034: Reload grid asynchronously after select operation
            await LoadGridAsync("", Category);
            return Page();
        }

        // ------------------------------------------------------------------ //
        // Private async helper – replaces the synchronous Web Forms LoadGrid method
        // cr-dotnet-1034: Converted to async Task using EF Core on Amazon RDS.
        //   Line 40: Manage.DataSource = table; Manage.DataBind() for DOCTOR category
        //            → await dbContext.Database.SqlQueryRaw<DoctorGridRow>().ToListAsync()
        //   Line 55: Manage.DataSource = table; Manage.DataBind() for PATIENT category
        //            → await dbContext.Database.SqlQueryRaw<PatientGridRow>().ToListAsync()
        //   Line 75: Manage.DataSource = table; Manage.Caption = ...; Manage.DataBind() for STAFF
        //            → await dbContext.Database.SqlQueryRaw<StaffGridRow>().ToListAsync()
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Loads grid data asynchronously using Entity Framework Core on Amazon RDS.
        /// cr-dotnet-1034: Replaces synchronous Manage.DataSource/DataBind() calls
        /// with async EF Core queries, preventing thread-pool exhaustion under cloud load.
        /// </summary>
        private async Task LoadGridAsync(string searchQuery, string category)
        {
            using (var dbContext = new HospitalDbContext())
            {
                if (category == "DOCTOR")
                {
                    // cr-dotnet-1034 (line 40): Replaces synchronous:
                    //   objmyDaL.LoadDoctor(ref table, SearchQuery);
                    //   Manage.DataSource = table; Manage.DataBind();
                    // with async EF Core query on Amazon RDS.
                    List<DoctorGridRow> doctors;
                    if (string.IsNullOrEmpty(searchQuery))
                    {
                        doctors = await dbContext.Database
                            .SqlQueryRaw<DoctorGridRow>(
                                "SELECT Doctor.DoctorID as ID, Doctor.Name, D.DeptName as Department " +
                                "FROM Doctor JOIN Department D ON D.DeptNo = Doctor.DeptNo " +
                                "WHERE Doctor.Status = 1")
                            .ToListAsync();
                    }
                    else
                    {
                        doctors = await dbContext.Database
                            .SqlQueryRaw<DoctorGridRow>(
                                "SELECT a.DoctorID as ID, a.Name, D.DeptName as Department " +
                                "FROM department D JOIN " +
                                "(SELECT * FROM Doctor WHERE Doctor.Status = 1 AND Doctor.Name LIKE '%' + {0} + '%') a " +
                                "ON a.DeptNo = D.DeptNo",
                                searchQuery)
                            .ToListAsync();
                    }

                    if (doctors != null && doctors.Count > 0)
                    {
                        GridData = new DataTable();
                        GridData.Columns.Add("ID");
                        GridData.Columns.Add("Name");
                        GridData.Columns.Add("Department");
                        foreach (var d in doctors)
                        {
                            GridData.Rows.Add(d.ID, d.Name, d.Department);
                        }
                    }
                    else
                    {
                        Message = "No Doctors to show";
                    }
                }
                else if (category == "PATIENT")
                {
                    // cr-dotnet-1034 (line 55): Replaces synchronous:
                    //   objmyDaL.LoadPatient(ref table, SearchQuery);
                    //   Manage.DataSource = table; Manage.DataBind();
                    // with async EF Core query on Amazon RDS.
                    List<PatientGridRow> patients;
                    if (string.IsNullOrEmpty(searchQuery))
                    {
                        patients = await dbContext.Database
                            .SqlQueryRaw<PatientGridRow>("SELECT * FROM PATIENT_VIEW")
                            .ToListAsync();
                    }
                    else
                    {
                        patients = await dbContext.Database
                            .SqlQueryRaw<PatientGridRow>(
                                "SELECT Patient.PatientID, Patient.Name, Patient.Phone FROM Patient " +
                                "WHERE patient.name LIKE '%' + {0} + '%'",
                                searchQuery.Trim())
                            .ToListAsync();
                    }

                    if (patients != null && patients.Count > 0)
                    {
                        GridData = new DataTable();
                        GridData.Columns.Add("PatientID");
                        GridData.Columns.Add("Name");
                        GridData.Columns.Add("Phone");
                        foreach (var p in patients)
                        {
                            GridData.Rows.Add(p.PatientID, p.Name, p.Phone);
                        }
                    }
                    else
                    {
                        Message = "No Patients to show";
                    }
                }
                else
                {
                    // cr-dotnet-1034 (line 75): Replaces synchronous:
                    //   objmyDaL.LoadOtherStaff(ref table, SearchQuery);
                    //   Manage.DataSource = table; Manage.Caption = "Other Staff Table:"; Manage.DataBind();
                    // with async EF Core query on Amazon RDS.
                    List<StaffGridRow> staff;
                    if (string.IsNullOrEmpty(searchQuery))
                    {
                        staff = await dbContext.Database
                            .SqlQueryRaw<StaffGridRow>("SELECT * FROM STAFF_VIEW")
                            .ToListAsync();
                    }
                    else
                    {
                        staff = await dbContext.Database
                            .SqlQueryRaw<StaffGridRow>(
                                "SELECT StaffID as ID, Name, Designation FROM OtherStaff " +
                                "WHERE Name LIKE '%' + {0} + '%'",
                                searchQuery.Trim())
                            .ToListAsync();
                    }

                    if (staff != null && staff.Count > 0)
                    {
                        GridData = new DataTable();
                        GridData.Columns.Add("ID");
                        GridData.Columns.Add("Name");
                        GridData.Columns.Add("Designation");
                        foreach (var s in staff)
                        {
                            GridData.Rows.Add(s.ID, s.Name, s.Designation);
                        }
                    }
                    else
                    {
                        Message = "No Staff Member to show";
                    }
                }
            }
        }

        // ------------------------------------------------------------------ //
        // Synchronous fallback wrappers — preserved for backward compatibility
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Synchronous wrapper for LoadGridAsync, preserved for backward compatibility.
        /// Internally delegates to the async implementation.
        /// </summary>
        private void LoadGrid(string searchQuery, string category)
        {
            LoadGridAsync(searchQuery, category).GetAwaiter().GetResult();
        }
    }

    // ---------------------------------------------------------------------------
    // EF Core result-set projection types for ManageClinic grid queries
    // cr-dotnet-1034: Used with SqlQueryRaw<T> for async data retrieval from RDS
    // ---------------------------------------------------------------------------

    /// <summary>
    /// Projection for Doctor grid rows.
    /// cr-dotnet-1034 (line 40): Replaces synchronous Manage GridView DataBind for DOCTOR.
    /// </summary>
    public class DoctorGridRow
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
    }

    /// <summary>
    /// Projection for Patient grid rows.
    /// cr-dotnet-1034 (line 55): Replaces synchronous Manage GridView DataBind for PATIENT.
    /// </summary>
    public class PatientGridRow
    {
        public int PatientID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    /// <summary>
    /// Projection for Staff grid rows.
    /// cr-dotnet-1034 (line 75): Replaces synchronous Manage GridView DataBind for OTHERSTAFF.
    /// </summary>
    public class StaffGridRow
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
    }
}
