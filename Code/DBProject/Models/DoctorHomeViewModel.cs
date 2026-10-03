// ViewModel for the Doctor Home page.
// Replaces the Web Forms code-behind data binding for DoctorHome.aspx (cr-dotnet-0026).

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the DoctorHome Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:Label Label1 through Label14).
    /// </summary>
    public class DoctorHomeViewModel
    {
        /// <summary>Doctor's full name (replaces asp:Label Label1).</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Doctor's phone number (replaces asp:Label Label2).</summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>Doctor's address (replaces asp:Label Label3).</summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>Doctor's birth date (replaces asp:Label Label4).</summary>
        public string BirthDate { get; set; } = string.Empty;

        /// <summary>Doctor's gender (replaces asp:Label Label5).</summary>
        public string Gender { get; set; } = string.Empty;

        /// <summary>Department number (replaces asp:Label Label6).</summary>
        public string DepartmentNo { get; set; } = string.Empty;

        /// <summary>Charges per visit (replaces asp:Label Label7).</summary>
        public string ChargesPerVisit { get; set; } = string.Empty;

        /// <summary>Monthly salary (replaces asp:Label Label8).</summary>
        public string MonthlySalary { get; set; } = string.Empty;

        /// <summary>Repute index (replaces asp:Label Label9).</summary>
        public string ReputeIndex { get; set; } = string.Empty;

        /// <summary>Number of patients treated (replaces asp:Label Label10).</summary>
        public string PatientsTreated { get; set; } = string.Empty;

        /// <summary>Qualification (replaces asp:Label Label11).</summary>
        public string Qualification { get; set; } = string.Empty;

        /// <summary>Specialization (replaces asp:Label Label12).</summary>
        public string Specialization { get; set; } = string.Empty;

        /// <summary>Work experience (replaces asp:Label Label13).</summary>
        public string WorkExperience { get; set; } = string.Empty;

        /// <summary>Employment status (replaces asp:Label Label14).</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Error message displayed when data loading fails.</summary>
        public string ErrorMessage { get; set; }
    }
}
