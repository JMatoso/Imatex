using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace Imatex.Web.Logging;

public static class LoggingConfiguration
{
    public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger =>
        (hostingContext, loggerConfiguration) =>
        {
            var env = hostingContext.HostingEnvironment;

            string? sentryDsn = hostingContext.Configuration.GetConnectionString("SentryDsn"); ;

            loggerConfiguration
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", env.ApplicationName)
                .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
                .Enrich.WithExceptionDetails()
                .Enrich.With()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information);

            if (!string.IsNullOrEmpty(sentryDsn))
            {
                loggerConfiguration.WriteTo.Sentry(o =>
                {
                    o.MinimumBreadcrumbLevel = LogEventLevel.Warning;
                    o.MinimumEventLevel = LogEventLevel.Warning;
                    o.AttachStacktrace = true;
                    o.Dsn = sentryDsn;
                    o.Debug = false;
                    o.TracesSampleRate = 1.0;
                    o.AttachStacktrace = true;
                    o.SendDefaultPii = true;
                    o.DiagnosticLevel = SentryLevel.Warning;
                });
            }

            loggerConfiguration.WriteTo.Console();

            if (hostingContext.HostingEnvironment.IsDevelopment())
            {
                loggerConfiguration.MinimumLevel.Override(env.ApplicationName, LogEventLevel.Debug);
            }
        };
}