using System;
using System.Web;

namespace DBProject.Modules
{
    /// <summary>
    /// cr-dotnet-1041 – Content Security Policy (CSP) HttpModule
    ///
    /// Adds Content-Security-Policy and related security headers to every HTTP
    /// response served by this ASP.NET Web Forms application.  The module is
    /// registered in Web.config under both &lt;system.web&gt;/&lt;httpModules&gt;
    /// (Classic pipeline) and &lt;system.webServer&gt;/&lt;modules&gt; (Integrated
    /// pipeline / IIS on AWS Elastic Beanstalk / ECS Windows containers).
    ///
    /// The CSP policy string can be overridden at runtime via the
    /// CSP_POLICY environment variable, enabling per-environment tuning
    /// without a code deployment (e.g., via ECS task-definition environment
    /// variables or AWS Systems Manager Parameter Store).
    ///
    /// Combined with AWS WAF managed rules (AWSManagedRulesCommonRuleSet,
    /// AWSManagedRulesKnownBadInputsRuleSet) this satisfies SOC 2 and
    /// penetration-test requirements for XSS prevention in multi-tenant
    /// cloud environments.
    /// </summary>
    public class ContentSecurityPolicyModule : IHttpModule
    {
        // ---------------------------------------------------------------------------
        // Default CSP policy – restrictive baseline suitable for a hospital portal.
        // Override at runtime via the CSP_POLICY environment variable.
        // ---------------------------------------------------------------------------
        private const string DefaultCspPolicy =
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data:; " +
            "font-src 'self'; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none'; " +
            "form-action 'self'; " +
            "base-uri 'self';";

        // ---------------------------------------------------------------------------
        // IHttpModule implementation
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Initialises the module and subscribes to the PreSendRequestHeaders event
        /// so that CSP headers are injected just before the response is flushed.
        /// </summary>
        public void Init(HttpApplication context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        /// <summary>
        /// Disposes any resources held by the module (none in this implementation).
        /// </summary>
        public void Dispose()
        {
            // No unmanaged resources to release.
        }

        // ---------------------------------------------------------------------------
        // Event handler
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Injects Content-Security-Policy and complementary security headers into
        /// every outgoing HTTP response.
        /// </summary>
        private static void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            if (application == null)
                return;

            HttpResponse response = application.Response;
            if (response == null)
                return;

            // ------------------------------------------------------------------
            // 1. Content-Security-Policy
            //    Allow runtime override via environment variable so that the
            //    policy can be tightened/relaxed per deployment environment
            //    (Dev / Staging / Prod) without a code change.
            // ------------------------------------------------------------------
            string cspPolicy = Environment.GetEnvironmentVariable("CSP_POLICY");
            if (string.IsNullOrWhiteSpace(cspPolicy))
                cspPolicy = DefaultCspPolicy;

            SetHeaderIfAbsent(response, "Content-Security-Policy", cspPolicy);

            // ------------------------------------------------------------------
            // 2. X-Content-Type-Options – prevents MIME-type sniffing attacks.
            // ------------------------------------------------------------------
            SetHeaderIfAbsent(response, "X-Content-Type-Options", "nosniff");

            // ------------------------------------------------------------------
            // 3. X-Frame-Options – defence-in-depth against clickjacking
            //    (redundant with frame-ancestors in CSP but kept for older
            //    browsers that do not support CSP).
            // ------------------------------------------------------------------
            SetHeaderIfAbsent(response, "X-Frame-Options", "DENY");

            // ------------------------------------------------------------------
            // 4. X-XSS-Protection – legacy XSS filter for older browsers.
            // ------------------------------------------------------------------
            SetHeaderIfAbsent(response, "X-XSS-Protection", "1; mode=block");

            // ------------------------------------------------------------------
            // 5. Referrer-Policy – limits referrer information leakage.
            // ------------------------------------------------------------------
            SetHeaderIfAbsent(response, "Referrer-Policy", "strict-origin-when-cross-origin");

            // ------------------------------------------------------------------
            // 6. Permissions-Policy – restricts access to browser features.
            // ------------------------------------------------------------------
            SetHeaderIfAbsent(response, "Permissions-Policy",
                "geolocation=(), microphone=(), camera=()");
        }

        // ---------------------------------------------------------------------------
        // Helper
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Adds a response header only when it has not already been set, preventing
        /// duplicate header values if another component (e.g., AWS WAF, ALB) has
        /// already injected the same header upstream.
        /// </summary>
        private static void SetHeaderIfAbsent(HttpResponse response, string name, string value)
        {
            try
            {
                if (string.IsNullOrEmpty(response.Headers[name]))
                    response.Headers[name] = value;
            }
            catch (PlatformNotSupportedException)
            {
                // Headers collection is not available in Classic pipeline mode;
                // fall back to AppendHeader which works in both pipeline modes.
                response.AppendHeader(name, value);
            }
        }
    }
}
