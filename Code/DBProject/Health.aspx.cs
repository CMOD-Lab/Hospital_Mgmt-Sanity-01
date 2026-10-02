using System;
using System.Web;
using System.Web.UI;

namespace DBProject
{
    /// <summary>
    /// Health check endpoint for containerization readiness.
    /// Accessible at /Health.aspx — returns HTTP 200 with JSON status.
    /// Used by Kubernetes liveness/readiness probes on EKS.
    /// </summary>
    public partial class Health : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "application/json";
            Response.StatusCode = 200;
            Response.Write("{\"status\":\"healthy\",\"application\":\"DBProject\",\"timestamp\":\"" + DateTime.UtcNow.ToString("o") + "\"}");
            Response.End();
        }
    }
}
