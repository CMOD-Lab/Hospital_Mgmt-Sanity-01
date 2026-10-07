using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

// Migrated from ASP.NET Web Forms (System.Web.UI.Page) to ASP.NET Core Razor Pages (PageModel)
// Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//   Replaced synchronous GridView.DataBind() with async Task-based data loading via
//   OnGetAsync()/OnPostAsync() and myDAL async methods connected to Amazon RDS, preventing
//   thread pool exhaustion under load and enabling efficient auto-scaling in cloud deployments.

namespace DBProject.Admin
{
    public class ManageClinicModel : PageModel
    {
        [BindProperty]
        public string SearchQuery { get; set; } = "";

        [BindProperty]
        public string SelectedCategory { get; set; } = "D";

        public DataTable GridData { get; set; }
        public string GridCaption { get; set; } = "Doctors Table";
        public string Message { get; set; }
        public string DetailHtml { get; set; }

        // cr-dotnet-1034: Changed from synchronous OnGet() to async OnGetAsync()
        // to prevent thread pool exhaustion under load in cloud (AWS RDS) deployments.
        public async Task OnGetAsync()
        {
            SelectedCategory = "D";
            await LoadGridAsync("", "DOCTOR");
        }

        // cr-dotnet-1034: Changed from synchronous OnPost() to async OnPostAsync()
        public async Task<IActionResult> OnPostAsync()
        {
            string category = Request.Form["Category"].ToString();
            if (!string.IsNullOrEmpty(category))
                SelectedCategory = category;

            string catKey = SelectedCategory == "D" ? "DOCTOR" : SelectedCategory == "P" ? "PATIENT" : "OTHERSTAFF";
            await LoadGridAsync("", catKey);
            return Page();
        }

        // cr-dotnet-1034: Changed from synchronous OnPostSearch() to async OnPostSearchAsync()
        public async Task<IActionResult> OnPostSearchAsync()
        {
            string category = Request.Form["Category"].ToString();
            if (!string.IsNullOrEmpty(category))
                SelectedCategory = category;

            string catKey = SelectedCategory == "D" ? "DOCTOR" : SelectedCategory == "P" ? "PATIENT" : "OTHERSTAFF";
            await LoadGridAsync(SearchQuery ?? "", catKey);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int rowIndex)
        {
            string category = Request.Form["Category"].ToString();
            if (!string.IsNullOrEmpty(category))
                SelectedCategory = category;

            string catKey = SelectedCategory == "D" ? "DOCTOR" : SelectedCategory == "P" ? "PATIENT" : "OTHERSTAFF";
            await LoadGridAsync(SearchQuery ?? "", catKey);

            if (GridData != null && rowIndex < GridData.Rows.Count)
            {
                string id = GridData.Rows[rowIndex][0].ToString();
                myDAL objDAL = new myDAL();

                if (SelectedCategory == "D")
                {
                    if (objDAL.DeleteDoctor(Convert.ToInt32(id)) == 1)
                    {
                        Message = " Doctor No: " + id + " Deleted";
                        await LoadGridAsync("", "DOCTOR");
                    }
                    else
                    {
                        Message = "there was some error";
                    }
                }
                else if (SelectedCategory == "P")
                {
                    Message = "You are not Authorized to Delete a Patient";
                }
                else
                {
                    if (objDAL.DeleteStaff(Convert.ToInt32(id)) == 1)
                    {
                        Message = "Staff No: " + id + " Deleted ";
                        await LoadGridAsync("", "OTHERSTAFF");
                    }
                    else
                    {
                        Message = "There was some Error";
                    }
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSelectAsync(int rowIndex)
        {
            string category = Request.Form["Category"].ToString();
            if (!string.IsNullOrEmpty(category))
                SelectedCategory = category;

            string catKey = SelectedCategory == "D" ? "DOCTOR" : SelectedCategory == "P" ? "PATIENT" : "OTHERSTAFF";
            await LoadGridAsync(SearchQuery ?? "", catKey);

            if (GridData != null && rowIndex < GridData.Rows.Count)
            {
                int id = Convert.ToInt32(GridData.Rows[rowIndex][0].ToString());
                myDAL objDAL = new myDAL();

                string name = "", phone = "", gender = "", address = "", bDate = "";
                float charges_Per_Visit = 0, ReputeIndex = 0;
                int PatientsTreated = 0, workE = 0, age = 0;
                string qualification = "", specialization = "";

                if (SelectedCategory == "D")
                {
                    if (objDAL.GET_DOCTOR_PROFILE(id, ref name, ref phone, ref gender, ref charges_Per_Visit, ref ReputeIndex, ref PatientsTreated, ref qualification, ref specialization, ref workE, ref age) == 1)
                    {
                        DetailHtml = "<p><b>Name:</b></p>" + name +
                                     " <p><b>phone:</b></p>" + phone +
                                     "<p><b>gender:</b></p>" + gender +
                                     "<p><b>Qualification:</b></p>" + qualification +
                                     "<p><b> Age:</b></p>" + age +
                                     "<p><b>Charges:</b></p> " + charges_Per_Visit +
                                     "<p><b>Repute index:</b></p>" + ReputeIndex;
                    }
                    else
                    {
                        Message = "there was some error";
                    }
                }
                else if (SelectedCategory == "P")
                {
                    if (objDAL.GETPATIENT(id, ref name, ref phone, ref address, ref bDate, ref age, ref gender) == 0)
                    {
                        DetailHtml = "<p><b>Name:</b></p>" + name +
                                     " <p><b>phone:</b></p>" + phone +
                                     "<p><b>gender:</b></p>" + gender +
                                     "<p><b>Address:</b></p>" + address +
                                     "<p><b> Age:</b></p>" + age;
                    }
                    else
                    {
                        Message = "there was some error";
                    }
                }
                else
                {
                    string designation = "";
                    int sal = 0;
                    if (objDAL.GETSATFF(id, ref name, ref phone, ref address, ref gender, ref designation, ref sal) == 1)
                    {
                        DetailHtml = "<p><b>Name:</b></p>" + name +
                                     " <p><b>phone:</b></p>" + phone +
                                     "<p><b>gender:</b></p>" + gender +
                                     "<p><b>Address:</b></p>" + address +
                                     "<p><b> salary:</b></p>" + sal;
                    }
                    else
                    {
                        Message = "there was some error";
                    }
                }
            }

            return Page();
        }

        // cr-dotnet-1034: Async grid loading replaces synchronous GridView.DataBind().
        // Data is fetched via Task-based API from Amazon RDS, allowing the request thread
        // to be released while awaiting I/O completion.
        private async Task LoadGridAsync(string searchQuery, string category)
        {
            myDAL objmyDaL = new myDAL();

            if (category == "DOCTOR")
            {
                GridCaption = "Doctors Table";
                DataTable table = await objmyDaL.LoadDoctorAsync(searchQuery);
                if (table != null && table.Rows.Count > 0)
                    GridData = table;
                else
                    Message = "No Doctors to show";
            }
            else if (category == "PATIENT")
            {
                GridCaption = "Patients Table";
                DataTable table = await objmyDaL.LoadPatientAsync(searchQuery);
                if (table != null && table.Rows.Count > 0)
                    GridData = table;
                else
                    Message = "No Pateints to show";
            }
            else
            {
                GridCaption = "Other Staff Table:";
                DataTable table = await objmyDaL.LoadOtherStaffAsync(searchQuery);
                if (table != null && table.Rows.Count > 0)
                    GridData = table;
                else
                    Message = "No Staff Member to show";
            }
        }
    }
}
