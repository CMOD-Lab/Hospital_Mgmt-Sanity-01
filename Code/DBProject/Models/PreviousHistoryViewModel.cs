// ViewModel for the Previous History page.
// Replaces the Web Forms code-behind data binding for PreviousHistory.aspx (cr-dotnet-0026).

using System.Data;

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the PreviousHistory Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:GridView ID="PHistoryGrid" with DataSource/DataBind and
    ///  asp:Label ID="PHistory" for error messages).
    /// </summary>
    public class PreviousHistoryViewModel
    {
        /// <summary>
        /// DataTable containing the history of treated patients for the logged-in doctor.
        /// Replaces PHistoryGrid.DataSource = DT; PHistoryGrid.DataBind();
        /// </summary>
        public DataTable HistoryData { get; set; }

        /// <summary>
        /// Error message displayed when data loading fails.
        /// Replaces asp:Label ID="PHistory" used for error text.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
