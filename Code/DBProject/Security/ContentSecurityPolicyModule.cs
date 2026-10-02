using System;
using System.Web;

namespace DBProject.Security
{
    /// <summary>
    /// cr-dotnet-1041: Content Security Policy HttpModule
    /// 
    /// Implements CSP headers for ASP.NET Web Forms to satisfy cloud security compliance
    /// requirements (SOC2 certification and penetration test compliance) in AWS environments.
    /// 
    /// This HttpModule is registered in Web.config under both:
    ///   - system.web/httpModules  (IIS Classic pipeline mode)
    ///   - system.webServer/modules (IIS Integrated pipeline mode / AWS Elastic Beanstalk)
    /// 
    /// The CSP policy is read from the following environment variables at runtime,
    /// allowing per-environment overrides without code changes:
    ///
    ///   CSP_DEFAULT_SRC   – default-src directive  (default: 'self')
    ///   CSP_SCRIPT_SRC    – script-src directive   (default: 'self' 'unsafe-inline')
    ///   CSP_STYLE_SRC     – style-src directive    (default: 'self' 'unsafe-inline')
    ///   CSP_IMG_SRC       – img-src directive      (default: 'self' data:)
    ///   CSP_FONT_SRC      – font-src directive     (default: 'self')
    ///   CSP_CONNECT_SRC   – connect-src directive  (default: 'self')
    ///   CSP_FRAME_SRC     – frame-src directive    (default: 'none')
    ///   CSP_OBJECT_SRC    – object-src directive   (default: 'none')
    ///   CSP_BASE_URI      – base-uri directive     (default: 'self')
    ///   CSP_FORM_ACTION   – form-action directive  (default: 'self')
    ///   CSP_REPORT_ONLY   – set to "true" to use Content-Security-Policy-Report-Only
    ///                        header instead of enforcing (default: false)
    ///
    /// Set these variables in your ECS task definition, Elastic Beanstalk environment
    /// properties, or AWS Systems Manager Parameter Store / Secrets Manager.
    ///
    /// AWS WAF managed rules (AWSManagedRulesCommonRuleSet) should be enabled on the
    /// associated CloudFront distribution or Application Load Balancer to provide an
    /// additional layer of XSS protection in multi-tenant cloud environments.
    /// </summary>
    public class ContentSecurityPolicyModule : IHttpModule
    {
        // Header names
        private const string CspHeaderName             = "Content-Security-Policy";
        private const string CspReportOnlyHeaderName   = "Content-Security-Policy-Report-Only";

        // Additional security headers added alongside CSP for defence-in-depth
        private const string XContentTypeOptionsHeader = "X-Content-Type-Options";
        private const string XFrameOptionsHeader       = "X-Frame-Options";
        private const string XssProtectionHeader       = "X-XSS-Protection";
        private const string ReferrerPolicyHeader      = "Referrer-Policy";

        /// <summary>
        /// Initialises the module and subscribes to the PostReleaseRequestState event
        /// so that headers are added after the response status code is determined but
        /// before the response is flushed to the client.
        /// </summary>
        public void Init(HttpApplication context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.PostReleaseRequestState += OnPostReleaseRequestState;
        }

        /// <summary>
        /// Adds Content-Security-Policy and complementary security headers to every
        /// HTTP response served by this application.
        /// </summary>
        private void OnPostReleaseRequestState(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            if (app == null)
                return;

            HttpResponse response = app.Response;
            if (response == null)
                return;

            // Build the CSP directive string from environment variables (with safe defaults)
            string cspValue = BuildCspHeaderValue();

            // Determine whether to enforce or report-only
            bool reportOnly = string.Equals(
                GetEnvOrDefault("CSP_REPORT_ONLY", "false"),
                "true",
                StringComparison.OrdinalIgnoreCase);

            string headerName = reportOnly ? CspReportOnlyHeaderName : CspHeaderName;

            // Remove any existing CSP header before setting ours to avoid duplicates
            response.Headers.Remove(CspHeaderName);
            response.Headers.Remove(CspReportOnlyHeaderName);
            response.Headers.Set(headerName, cspValue);

            // Defence-in-depth: additional security headers
            response.Headers.Set(XContentTypeOptionsHeader, "nosniff");
            response.Headers.Set(XFrameOptionsHeader, "SAMEORIGIN");
            response.Headers.Set(XssProtectionHeader, "1; mode=block");
            response.Headers.Set(ReferrerPolicyHeader, "strict-origin-when-cross-origin");
        }

        /// <summary>
        /// Constructs the CSP header value from environment variables, falling back to
        /// secure defaults when variables are not set.
        /// </summary>
        private static string BuildCspHeaderValue()
        {
            string defaultSrc  = GetEnvOrDefault("CSP_DEFAULT_SRC",  "'self'");
            string scriptSrc   = GetEnvOrDefault("CSP_SCRIPT_SRC",   "'self' 'unsafe-inline'");
            string styleSrc    = GetEnvOrDefault("CSP_STYLE_SRC",    "'self' 'unsafe-inline'");
            string imgSrc      = GetEnvOrDefault("CSP_IMG_SRC",      "'self' data:");
            string fontSrc     = GetEnvOrDefault("CSP_FONT_SRC",     "'self'");
            string connectSrc  = GetEnvOrDefault("CSP_CONNECT_SRC",  "'self'");
            string frameSrc    = GetEnvOrDefault("CSP_FRAME_SRC",    "'none'");
            string objectSrc   = GetEnvOrDefault("CSP_OBJECT_SRC",   "'none'");
            string baseUri     = GetEnvOrDefault("CSP_BASE_URI",     "'self'");
            string formAction  = GetEnvOrDefault("CSP_FORM_ACTION",  "'self'");

            return string.Format(
                "default-src {0}; script-src {1}; style-src {2}; img-src {3}; " +
                "font-src {4}; connect-src {5}; frame-src {6}; object-src {7}; " +
                "base-uri {8}; form-action {9}",
                defaultSrc, scriptSrc, styleSrc, imgSrc,
                fontSrc, connectSrc, frameSrc, objectSrc,
                baseUri, formAction);
        }

        /// <summary>
        /// Returns the value of an environment variable, or the specified default when
        /// the variable is absent or empty.
        /// </summary>
        private static string GetEnvOrDefault(string variableName, string defaultValue)
        {
            string value = Environment.GetEnvironmentVariable(variableName);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // No unmanaged resources to release.
        }
    }
}
