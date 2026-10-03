// ViewModel for the History Update page.
// Replaces the Web Forms code-behind data binding for HistoryUpdate.aspx (cr-dotnet-0026).

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the HistoryUpdate Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:TextBox Disease, asp:TextBox progress, asp:TextBox Prescription,
    ///  asp:Button submit, asp:Button Bill).
    /// </summary>
    public class HistoryUpdateViewModel
    {
        /// <summary>
        /// Disease text entered by the doctor
        /// (replaces asp:TextBox ID="Disease").
        /// </summary>
        public string Disease { get; set; } = string.Empty;

        /// <summary>
        /// Progress text entered by the doctor
        /// (replaces asp:TextBox ID="progress").
        /// </summary>
        public string Progress { get; set; } = string.Empty;

        /// <summary>
        /// Prescription text entered by the doctor
        /// (replaces asp:TextBox ID="Prescription").
        /// </summary>
        public string Prescription { get; set; } = string.Empty;

        /// <summary>Error message displayed when the save operation fails.</summary>
        public string ErrorMessage { get; set; }

        /// <summary>Success message displayed when the save operation succeeds.</summary>
        public string SuccessMessage { get; set; }
    }
}
