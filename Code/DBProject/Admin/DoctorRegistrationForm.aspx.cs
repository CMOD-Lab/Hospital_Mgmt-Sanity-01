// DoctorRegistrationForm.aspx.cs — ASP.NET Core Razor PageModel
// Rule cr-dotnet-0026: Web Forms Usage → Migrate to ASP.NET Core MVC/Razor Pages
//
// Migration notes:
//   - Occurrence 7 (line 2): System.Web.UI removed; replaced with
//     Microsoft.AspNetCore.Mvc.RazorPages and Microsoft.AspNetCore.Mvc.
//   - Occurrence 8 (line 3): System.Web.UI.WebControls removed; DataAnnotations
//     used for validation instead of server-side validator controls.
//   - Occurrence 9 (line 8): System.Web.UI.Page base class replaced with PageModel.
//   - Occurrence 10 (line 10): Page_Load event handler replaced with OnGet() / OnPost()
//     Razor Pages lifecycle methods.
//   - ValidateDoctorEmail (CustomValidator) → moved into OnPost() as a manual
//     ModelState check, preserving the same DAL call.
//   - DepartmentValidate (CustomValidator) → moved into OnPost() as a manual
//     ModelState check, preserving the same logic.
//   - DoctorRegister (Button click handler) → replaced by OnPost() handler.
//   - flushInformation() → replaced by re-initialising Input after a successful POST.
//   - Response.BufferOutput (Web Forms API) removed; not applicable in ASP.NET Core.
//   - All DAL calls (myDAL.DoctorEmailAlreadyExist, myDAL.AddDoctor) are preserved.

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject.Pages.Admin
{
    /// <summary>
    /// Razor PageModel for the Doctor Registration Form.
    /// Replaces the Web Forms DoctorRegistrationForm : System.Web.UI.Page class.
    /// </summary>
    public class DoctorRegistrationFormModel : PageModel
    {
        // ---------------------------------------------------------------------------
        // Success message — replaces <asp:Label ID="Msg">
        // ---------------------------------------------------------------------------

        /// <summary>Set after a successful doctor registration to display a confirmation.</summary>
        public string? SuccessMessage { get; private set; }

        // ---------------------------------------------------------------------------
        // Input model — replaces individual <asp:TextBox> / <asp:DropDownList> controls
        // ---------------------------------------------------------------------------

        [BindProperty]
        public DoctorInputModel Input { get; set; } = new DoctorInputModel();

        // ---------------------------------------------------------------------------
        // OnGet — replaces Page_Load (no work needed on GET for this form)
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Handles HTTP GET requests.  Renders the empty registration form.
        /// Replaces: protected void Page_Load(object sender, EventArgs e) { }
        /// </summary>
        public void OnGet()
        {
            // No initialisation required — form is rendered empty on GET.
        }

        // ---------------------------------------------------------------------------
        // OnPost — replaces DoctorRegister button click handler
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Handles HTTP POST requests (form submission).
        /// Replaces: protected void DoctorRegister(object sender, EventArgs e)
        /// Incorporates ValidateDoctorEmail and DepartmentValidate custom validators.
        /// </summary>
        public IActionResult OnPost()
        {
            // --- Custom validation: duplicate e-mail check (replaces CustomValidator DoctorValidate) ---
            if (!string.IsNullOrWhiteSpace(Input.Email))
            {
                myDAL objmyDAL = new myDAL();
                if (objmyDAL.DoctorEmailAlreadyExist(Input.Email) == 1)
                {
                    ModelState.AddModelError(
                        nameof(Input.Email),
                        "This Email already exists, kindly choose a different one!");
                }
            }

            // --- Custom validation: department selection (replaces CustomValidator DV) ---
            if (Input.Department == 0)
            {
                ModelState.AddModelError(
                    nameof(Input.Department),
                    "Please Select Department");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // --- Business logic: add doctor (replaces DoctorRegister body) ---
            myDAL dal = new myDAL();

            int exp             = Convert.ToInt32(Input.Experience);
            int salary          = Convert.ToInt32(Input.Salary);
            int chargesPerVisit = Convert.ToInt32(Input.ChargesPerVisit);
            int dept            = Input.Department;
            char gender         = string.IsNullOrEmpty(Input.Gender) ? 'M' : Input.Gender[0];

            dal.AddDoctor(
                Input.Name,
                Input.Email,
                Input.Password,
                Input.BirthDate,
                dept,
                Input.Phone,
                gender,
                Input.Address,
                exp,
                salary,
                chargesPerVisit,
                Input.Specialization,
                Input.Qualification);

            // --- Show success message and reset form (replaces Msg.Text + flushInformation) ---
            SuccessMessage = "Doctor Added Successfully";
            Input = new DoctorInputModel();   // replaces flushInformation()

            return Page();
        }

        // ---------------------------------------------------------------------------
        // Input model definition
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Strongly-typed input model for the doctor registration form.
        /// DataAnnotations replace Web Forms validator controls.
        /// </summary>
        public class DoctorInputModel
        {
            // Replaces: <asp:TextBox ID="Name"> + RequiredFieldValidator
            [Required(ErrorMessage = "* Required")]
            public string Name { get; set; } = string.Empty;

            // Replaces: <asp:TextBox ID="BirthDate"> + RequiredFieldValidator + RegularExpressionValidator
            [Required(ErrorMessage = "*Required")]
            [RegularExpression(
                @"((?:0[1-9])|(?:1[0-2]))\/((?:0[0-9])|(?:[1-2][0-9])|(?:3[0-1]))\/(\d{4})",
                ErrorMessage = "Birth Date Format Not Correct")]
            public string BirthDate { get; set; } = string.Empty;

            // Replaces: <asp:TextBox ID="Email"> + RequiredFieldValidator + RegularExpressionValidator
            [Required(ErrorMessage = "*Required")]
            [EmailAddress(ErrorMessage = "Incorrect Email Format")]
            public string Email { get; set; } = string.Empty;

            // Replaces: <asp:TextBox ID="Password"> + RequiredFieldValidator + CompareValidator
            [Required(ErrorMessage = "*Required")]
            [Compare(nameof(ConfirmPassword), ErrorMessage = "Passwords Do not Match")]
            public string Password { get; set; } = string.Empty;

            // Replaces: <asp:TextBox ID="cPassword">
            [Required(ErrorMessage = "*Required")]
            public string ConfirmPassword { get; set; } = string.Empty;

            // Replaces: <asp:TextBox ID="Phone"> + RegularExpressionValidator (numbers only)
            [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
            public string Phone { get; set; } = string.Empty;

            // Replaces: <asp:TextBox ID="Salary"> + RegularExpressionValidator (numbers only)
            [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
            public string Salary { get; set; } = string.Empty;

            // Replaces: <asp:TextBox ID="Charges_per_visit"> + RegularExpressionValidator
            [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
            public string ChargesPerVisit { get; set; } = string.Empty;

            // Replaces: <asp:TextBox ID="Exp"> + RangeValidator (0-5)
            [Range(0, 5, ErrorMessage = "Experience Range should be (0-5)")]
            public int Experience { get; set; }

            // Replaces: <asp:DropDownList ID="Department"> + CustomValidator DV
            public int Department { get; set; }

            // Replaces: <asp:TextBox ID="Qualification">
            public string Qualification { get; set; } = string.Empty;

            // Replaces: <asp:TextBox ID="spec">
            public string Specialization { get; set; } = string.Empty;

            // Replaces: <asp:TextBox ID="Address">
            public string Address { get; set; } = string.Empty;

            // Replaces: <asp:RadioButton> Gender group (Male/Female)
            public string Gender { get; set; } = "M";
        }
    }
}
