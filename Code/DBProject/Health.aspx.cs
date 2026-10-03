using System;
using System.Web;

namespace DBProject
{
    /// <summary>
    /// Health check endpoint for containerized deployments on Amazon EKS.
    /// Accessible at GET /Health.aspx
    /// Returns HTTP 200 with JSON body: {"status":"healthy"} when the application is running.
    /// Used by Kubernetes liveness and readiness probes.
    /// </summary>
    public partial class Health : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "application/json";
            Response.StatusCode = 200;
            Response.Write("{\"status\":\"healthy\",\"application\":\"HospitalMgmtApp\",\"timestamp\":\"" + DateTime.UtcNow.ToString("o") + "\"}");
            Response.End();
        }
    }
}
