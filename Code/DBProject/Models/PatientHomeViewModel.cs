// ViewModel for the Patient Home page.
// Replaces the Web Forms code-behind data binding for PatientHome.aspx (cr-dotnet-0026).

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the PatientHome Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:Label ID="PName", "PPhone", "PBirthDate", "PatientAge", "PAddress", "PGender").
    /// </summary>
    public class PatientHomeViewModel
    {
        /// <summary>
        /// Patient's full name.
        /// Replaces asp:Label ID="PName" Font-Bold="true" Font-Size="Medium".
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Patient's phone number.
        /// Replaces asp:Label ID="PPhone" Font-Bold="true" Font-Size="Medium".
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Patient's birth date.
        /// Replaces asp:Label ID="PBirthDate" Font-Bold="true" Font-Size="Medium".
        /// </summary>
        public string BirthDate { get; set; }

        /// <summary>
        /// Patient's age.
        /// Replaces asp:Label ID="PatientAge" Font-Bold="true" Font-Size="Medium".
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Patient's gender.
        /// Replaces asp:Label ID="PGender" Font-Bold="true" Font-Size="Medium".
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Patient's address.
        /// Replaces asp:Label ID="PAddress" Font-Bold="true" Font-Size="Medium".
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Error message to display when patient info retrieval fails.
        /// Replaces Response.Write("&lt;script&gt;alert('...');&lt;/script&gt;").
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
