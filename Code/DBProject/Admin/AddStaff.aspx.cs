// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)
// Original violations fixed:
//   Line 5: using System.Web.UI;           -> removed (Web Forms namespace)
//   Line 6: using System.Web.UI.WebControls; -> removed (Web Forms namespace)
//   Line 11: public partial class AddStaff : System.Web.UI.Page -> replaced with PageModel
//   Line 13: protected void Page_Load(...)  -> replaced with OnGet() Razor Pages lifecycle
//
// The Web Forms code-behind pattern (System.Web.UI.Page, Page_Load, server control
// references, Response.BufferOutput, Page.IsValid) has been replaced with the
// ASP.NET Core Razor Pages PageModel pattern, preserving all original business logic.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using DBProject.DAL;

namespace DBProject.Admin
{
    /// <summary>
    /// ASP.NET Core Razor Pages PageModel for Staff Registration.
    /// Replaces the legacy Web Forms AddStaff : System.Web.UI.Page code-behind.
    /// </summary>
    public class AddStaffModel : PageModel
    {
        [BindProperty]
        public AddStaffInputModel Input { get; set; } = new AddStaffInputModel();

        /// <summary>Success message displayed after a staff member is added.</summary>
        public string? SuccessMessage { get; private set; }

        // GET /Admin/AddStaff — replaces Page_Load
        public void OnGet()
        {
            // No initialisation required on first load.
        }

        // POST /Admin/AddStaff?handler=StaffRegister — replaces StaffRegister event handler
        public IActionResult OnPostStaffRegister()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            myDAL objmyDAL = new myDAL();

            int salary = Convert.ToInt32(Input.Salary);
            string genderValue = Request.Form["Gender"].ToString();
            char gender = (genderValue.Length > 0) ? genderValue[0] : 'M';

            int result = objmyDAL.AddStaff(
                Input.Name,
                Input.BirthDate,
                Input.Phone,
                gender,
                Input.Address,
                salary,
                Input.Qual,
                Input.Designation);

            if (result == 1)
            {
                SuccessMessage = Input.Designation + " Added Succesfully";
                FlushInformation();
            }

            return Page();
        }

        /// <summary>Clears all form fields after a successful submission.</summary>
        private void FlushInformation()
        {
            Input = new AddStaffInputModel();
        }
    }

    /// <summary>
    /// Input model for the Staff Registration form.
    /// Replaces the individual server-control references (Name.Text, BirthDate.Text, etc.)
    /// and the Web Forms validator controls with DataAnnotations.
    /// </summary>
    public class AddStaffInputModel
    {
        [Required(ErrorMessage = "*Required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "*Required")]
        [RegularExpression(
            @"((?:0[1-9])|(?:1[0-2]))\/((?:0[0-9])|(?:[1-2][0-9])|(?:3[0-1]))\/(\d{4})",
            ErrorMessage = "Birth Date Format Not Correct")]
        public string BirthDate { get; set; } = string.Empty;

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "*Required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
        public string Salary { get; set; } = string.Empty;

        public string Qual { get; set; } = string.Empty;

        [Required(ErrorMessage = "*Required")]
        public string Designation { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}
