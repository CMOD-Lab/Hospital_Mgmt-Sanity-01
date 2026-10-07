using System;
using System.Web;

namespace DBProject
{
    /// <summary>
    /// Health check endpoint for containerization liveness/readiness probes.
    /// Accessible at /Health.aspx
    /// </summary>
    public partial class Health : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "application/json";
            Response.StatusCode = 200;
            Response.Write("{\"status\":\"Healthy\",\"application\":\"ClinicManagementSystem\",\"timestamp\":\"" + DateTime.UtcNow.ToString("o") + "\"}");
            Response.End();
        }
    }
}
