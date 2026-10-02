using System;
using System.Web;
using System.Web.UI;

namespace DBProject
{
    /// <summary>
    /// Health check endpoint for container liveness/readiness probes.
    /// Accessible at GET /Health.aspx
    /// Returns HTTP 200 with JSON body: {"status":"healthy","application":"HospitalMgmt"}
    /// </summary>
    public partial class Health : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "application/json";
            Response.StatusCode = 200;
            Response.Write("{\"status\":\"healthy\",\"application\":\"HospitalMgmt\",\"timestamp\":\"" + DateTime.UtcNow.ToString("o") + "\"}");
            Response.End();
        }
    }
}
