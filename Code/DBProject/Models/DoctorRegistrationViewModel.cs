// ViewModel for the Doctor Registration page.
// Replaces the Web Forms server controls and validators in DoctorRegistrationForm.aspx (cr-dotnet-0026).

using System.ComponentModel.DataAnnotations;

namespace DBProject.Models
{
    /// <summary>
    /// View model for the Doctor Registration form.
    /// DataAnnotations replace Web Forms validators:
    ///   - [Required]          replaces asp:RequiredFieldValidator
    ///   - [RegularExpression] replaces asp:RegularExpressionValidator
    ///   - [Compare]           replaces asp:CompareValidator
    ///   - [Range]             replaces asp:RangeValidator
    ///   - [EmailAddress]      replaces asp:RegularExpressionValidator for email format
    /// Custom server-side validation (email uniqueness, department selection) is handled
    /// in the DoctorRegistrationController.
    /// </summary>
    public class DoctorRegistrationViewModel
    {
        [Required(ErrorMessage = "* Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "*Required")]
        [RegularExpression(
            @"((?:0[1-9])|(?:1[0-2]))\/((?:0[0-9])|(?:[1-2][0-9])|(?:3[0-1]))\/(\d{4})",
            ErrorMessage = "Birth Date Format Not Correct")]
        public string BirthDate { get; set; }

        [Required(ErrorMessage = "*Required")]
        [EmailAddress(ErrorMessage = "Incorrect Email Format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Passwords Do not Match")]
        public string ConfirmPassword { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
        public string Phone { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
        public int Salary { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
        public int ChargesPerVisit { get; set; }

        [Range(0, 5, ErrorMessage = "Experience Range should be (0-5)")]
        public int Experience { get; set; }

        /// <summary>Department ID; 0 means "Select Department" (validated in controller).</summary>
        public int DepartmentId { get; set; }

        public string Qualification { get; set; }

        public string Specialization { get; set; }

        public string Address { get; set; }

        /// <summary>Gender: "M" or "F" – bound from radio button group in the form.</summary>
        public string Gender { get; set; } = "M";
    }
}
