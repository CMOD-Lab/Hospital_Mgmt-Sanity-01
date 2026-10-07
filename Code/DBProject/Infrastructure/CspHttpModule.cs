using System;
using System.Web;

namespace DBProject.Infrastructure
{
    /// <summary>
    /// Custom HttpModule that adds Content-Security-Policy (CSP) headers to every HTTP response.
    /// Implements cloud security compliance requirements for SOC2 certification and penetration
    /// test compliance, preventing XSS attacks in multi-tenant cloud environments (AWS).
    /// Works in conjunction with AWS WAF managed rules for defence-in-depth.
    /// </summary>
    public class CspHttpModule : IHttpModule
    {
        // CSP policy value – tighten per environment via the CSP_POLICY environment variable.
        // The default policy below is a secure baseline suitable for this ASP.NET Web Forms app:
        //   default-src 'self'          – only same-origin resources by default
        //   script-src  'self' 'unsafe-inline' 'unsafe-eval' – required for Web Forms __doPostBack / inline scripts
        //   style-src   'self' 'unsafe-inline'               – required for inline styles used by Bootstrap / Web Forms
        //   img-src     'self' data:                         – allow data URIs for embedded images
        //   font-src    'self'                               – local fonts only
        //   connect-src 'self'                               – XHR / fetch to same origin only
        //   frame-ancestors 'none'                           – prevent clickjacking (replaces X-Frame-Options)
        //   form-action 'self'                               – form submissions to same origin only
        //   base-uri    'self'                               – restrict <base> tag
        //   object-src  'none'                               – block Flash / plugins
        private static readonly string DefaultCspPolicy =
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data:; " +
            "font-src 'self'; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none'; " +
            "form-action 'self'; " +
            "base-uri 'self'; " +
            "object-src 'none'";

        public void Init(HttpApplication context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if (application == null)
                return;

            HttpResponse response = application.Response;
            if (response == null)
                return;

            // Allow the CSP policy to be overridden at runtime via an environment variable,
            // supporting AWS ECS / Elastic Beanstalk environment variable injection.
            string cspPolicy = Environment.GetEnvironmentVariable("CSP_POLICY");
            if (string.IsNullOrWhiteSpace(cspPolicy))
            {
                cspPolicy = DefaultCspPolicy;
            }

            // Set Content-Security-Policy header (enforcing mode)
            if (!response.Headers.AllKeys.Contains("Content-Security-Policy"))
            {
                response.Headers.Set("Content-Security-Policy", cspPolicy);
            }

            // Additional security headers recommended for cloud/SOC2 compliance
            if (!response.Headers.AllKeys.Contains("X-Content-Type-Options"))
            {
                response.Headers.Set("X-Content-Type-Options", "nosniff");
            }

            if (!response.Headers.AllKeys.Contains("X-Frame-Options"))
            {
                response.Headers.Set("X-Frame-Options", "DENY");
            }

            if (!response.Headers.AllKeys.Contains("X-XSS-Protection"))
            {
                response.Headers.Set("X-XSS-Protection", "1; mode=block");
            }

            if (!response.Headers.AllKeys.Contains("Referrer-Policy"))
            {
                response.Headers.Set("Referrer-Policy", "strict-origin-when-cross-origin");
            }
        }

        public void Dispose()
        {
            // No unmanaged resources to release
        }
    }
}
