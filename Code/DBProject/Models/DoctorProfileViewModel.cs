// ViewModel for the Doctor Profile page.
// Replaces the Web Forms code-behind data binding for DoctorProfile.aspx (cr-dotnet-0026).

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the DoctorProfile Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:Label ID="DName", "DPhone", "DQualification", "DSpecialization",
    ///  "DWork", "DAge", "DGender", "DDept", "DCharges", "DRI", "DPT").
    /// </summary>
    public class DoctorProfileViewModel
    {
        /// <summary>
        /// Error message when doctor info retrieval fails.
        /// Replaces Response.Write alert for status == -1.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Doctor's full name.
        /// Replaces asp:Label ID="DName".
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Doctor's phone number.
        /// Replaces asp:Label ID="DPhone".
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Doctor's qualification.
        /// Replaces asp:Label ID="DQualification".
        /// </summary>
        public string Qualification { get; set; }

        /// <summary>
        /// Doctor's specialization.
        /// Replaces asp:Label ID="DSpecialization".
        /// </summary>
        public string Specialization { get; set; }

        /// <summary>
        /// Doctor's work experience (years).
        /// Replaces asp:Label ID="DWork".
        /// </summary>
        public string WorkExperience { get; set; }

        /// <summary>
        /// Doctor's age.
        /// Replaces asp:Label ID="DAge".
        /// </summary>
        public string Age { get; set; }

        /// <summary>
        /// Doctor's gender.
        /// Replaces asp:Label ID="DGender".
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Doctor's department name.
        /// Replaces asp:Label ID="DDept".
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Doctor's charges per appointment visit.
        /// Replaces asp:Label ID="DCharges".
        /// </summary>
        public string ChargesPerVisit { get; set; }

        /// <summary>
        /// Doctor's repute index.
        /// Replaces asp:Label ID="DRI".
        /// </summary>
        public string ReputeIndex { get; set; }

        /// <summary>
        /// Number of patients treated by the doctor.
        /// Replaces asp:Label ID="DPT".
        /// </summary>
        public string PatientsTreated { get; set; }
    }
}
