using System;
using System.Web;

namespace DBProject
{
    /// <summary>
    /// HTTP Handler that provides a health check endpoint for container liveness/readiness probes.
    /// Accessible at GET /health
    /// Returns HTTP 200 with JSON body {"status":"healthy"} when the application is running.
    /// </summary>
    public class HealthCheckHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;
            context.Response.Write("{\"status\":\"healthy\",\"application\":\"HospitalManagementSystem\",\"timestamp\":\"" + DateTime.UtcNow.ToString("o") + "\"}");
        }
    }
}
