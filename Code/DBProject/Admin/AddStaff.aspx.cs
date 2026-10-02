// Migrated from ASP.NET Web Forms (AddStaff.aspx.cs) to ASP.NET Core Razor Pages PageModel.
// Rule cr-dotnet-0026: Web Forms Usage → Migrate to ASP.NET Core MVC/Razor Pages
//
// Changes applied (Occurrences 2–5):
//   Line 5  (Occurrence 2): Removed "using System.Web.UI;" — Web Forms namespace, not available in ASP.NET Core.
//   Line 6  (Occurrence 3): Removed "using System.Web.UI.WebControls;" — Web Forms controls namespace.
//   Line 11 (Occurrence 4): Replaced "System.Web.UI.Page" base class with "PageModel" (Microsoft.AspNetCore.Mvc.RazorPages).
//   Line 13 (Occurrence 5): Replaced Web Forms Page_Load / event-handler pattern with ASP.NET Core
//                            OnGet() / OnPostStaffRegisterAsync() Razor Page handler methods.
//   All Web Forms server-control properties (TextBox.Text, Label.Visible, etc.) replaced with
//   a bound InputModel and SuccessMessage property.
//   Request.Form["Gender"] replaced with model-bound Input.Gender property.
//   Response.BufferOutput (Web Forms) removed; not applicable in ASP.NET Core.

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject.Admin
{
    /// <summary>
    /// ASP.NET Core Razor Page model for the Add Staff page.
    /// Replaces the Web Forms code-behind class that previously inherited System.Web.UI.Page.
    /// </summary>
    public class AddStaffModel : PageModel
    {
        // -----------------------------------------------------------------------
        // Bound input model — replaces individual asp:TextBox / asp:RadioButton
        // server controls that were declared in the Web Forms .aspx markup.
        // -----------------------------------------------------------------------
        [BindProperty]
        public StaffInputModel Input { get; set; } = new StaffInputModel();

        /// <summary>
        /// Displayed after a successful registration (replaces asp:Label "Msg").
        /// </summary>
        public string? SuccessMessage { get; private set; }

        // -----------------------------------------------------------------------
        // GET handler — replaces Web Forms Page_Load (initial load branch).
        // -----------------------------------------------------------------------
        public void OnGet()
        {
            // Nothing to pre-populate on initial load.
        }

        // -----------------------------------------------------------------------
        // POST handler — replaces the Web Forms StaffRegister event handler.
        // Named "StaffRegister" so the Razor form can target it with
        // asp-page-handler="StaffRegister".
        // -----------------------------------------------------------------------
        public IActionResult OnPostStaffRegister()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            myDAL objmyDAL = new myDAL();

            int salary = Convert.ToInt32(Input.Salary);
            char gender = string.IsNullOrEmpty(Input.Gender) ? 'M' : Input.Gender[0];

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

        // -----------------------------------------------------------------------
        // Clears the form fields after a successful submission.
        // Replaces the Web Forms flushInformation() helper method.
        // -----------------------------------------------------------------------
        private void FlushInformation()
        {
            Input = new StaffInputModel();
        }
    }

    // ---------------------------------------------------------------------------
    // Input model — encapsulates all form fields with validation attributes.
    // Replaces the individual asp:TextBox / asp:RadioButton server controls and
    // their associated asp:RequiredFieldValidator / asp:RegularExpressionValidator
    // controls from the Web Forms .aspx markup.
    // ---------------------------------------------------------------------------
    public class StaffInputModel
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

        /// <summary>
        /// "M" or "F" — replaces the Web Forms RadioButton GroupName="Gender" pair.
        /// </summary>
        public string Gender { get; set; } = "M";
    }
}
