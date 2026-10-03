// ViewModel for the Bills History page.
// Replaces the Web Forms code-behind data binding for BillsHistory.aspx (cr-dotnet-0026).

using System.Data;

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the BillsHistory Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:GridView ID="BHistoryGrid" with DataSource/DataBind and
    ///  asp:Label ID="BHistory" for status messages).
    /// </summary>
    public class BillsHistoryViewModel
    {
        /// <summary>
        /// Status or error message displayed when loading bill history.
        /// Replaces asp:Label ID="BHistory" used for status text.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// DataTable containing the patient's bill history records.
        /// Replaces BHistoryGrid.DataSource = DT; BHistoryGrid.DataBind();
        /// </summary>
        public DataTable BillHistoryData { get; set; }

        /// <summary>
        /// Number of bill records returned by the DAL.
        /// Used to display the count in the status message.
        /// </summary>
        public int BillCount { get; set; }
    }
}
