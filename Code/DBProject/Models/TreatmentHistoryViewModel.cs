// ViewModel for the Treatment History page.
// Replaces the Web Forms code-behind data binding for TreatmentHistory.aspx (cr-dotnet-0026).

using System.Data;

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the TreatmentHistory Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:GridView ID="THistoryGrid" with DataSource/DataBind and
    ///  asp:Label ID="THistory" for status messages).
    /// </summary>
    public class TreatmentHistoryViewModel
    {
        /// <summary>
        /// Status or error message displayed when loading treatment history.
        /// Replaces asp:Label ID="THistory" used for status text.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// DataTable containing the patient's treatment history records.
        /// Replaces THistoryGrid.DataSource = DT; THistoryGrid.DataBind();
        /// </summary>
        public DataTable TreatmentData { get; set; }

        /// <summary>
        /// Number of treatment history records returned by the DAL.
        /// Used to display the count in the status message.
        /// </summary>
        public int TreatmentCount { get; set; }
    }
}
