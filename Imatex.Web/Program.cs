using Blazor.Analytics;
using Imatex.Web.Helpers.Interop;
using Imatex.Web.Logging;
using Imatex.Web.Options;
using Imatex.Web.Services.Compression;
using Imatex.Web.Services.Extractor;
using Imatex.Web.Services.Ocr;
using Imatex.Web.Services.SocialMedias;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.IO;
using MudBlazor;
using MudBlazor.Services;
using Serilog;
using Steeltoe.Extensions.Configuration;
using Steeltoe.Extensions.Configuration.Placeholder;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddPlaceholderResolver();
builder.Host.UseSerilog(LoggingConfiguration.ConfigureLogger);

// Add services to the container.
string? sentryDsn = builder.Configuration["SentryDsn"];

if (!string.IsNullOrWhiteSpace(sentryDsn))
{
    builder.WebHost.UseSentry(options =>
    {
        options.Debug = false;
        options.TracesSampleRate = 1.0;
        options.Dsn = sentryDsn;
        options.MaxRequestBodySize = Sentry.Extensibility.RequestSize.Always;
        options.SendDefaultPii = true;
        options.MinimumBreadcrumbLevel = LogLevel.Warning;
        options.MinimumEventLevel = LogLevel.Warning;
        options.DiagnosticLevel = SentryLevel.Warning;
        options.AttachStacktrace = true;

        options.SetBeforeSend(beforeSend =>
        {
            return beforeSend;
        });
    });
}

builder.Services.AddRazorPages();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 350;
    config.SnackbarConfiguration.ShowTransitionDuration = 100;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
});

builder.Services.AddServerSideBlazor();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddGoogleAnalytics(builder.Configuration["GoogleTagId"]);
builder.Services.AddHttpClient(nameof(Program)).AddStandardResilienceHandler();

builder.Services.AddScoped<ITikTokService, TikTokService>();
builder.Services.AddScoped<IYouTubeService, YouTubeService>();
builder.Services.AddSingleton<RecyclableMemoryStreamManager>();
builder.Services.AddScoped<IApplicationInterop, ApplicationInterop>();
builder.Services.AddScoped<IZipCompressorService, ZipCompressorService>();
builder.Services.AddScoped<ITextConverterService, TextConverterService>();
builder.Services.AddScoped<IImageExtractorService, ImageExtractorService>();

builder.Services.Configure<TikTokOptions>(builder.Configuration.GetSection(TikTokOptions.TikTokOptionsKey));
builder.Services.Configure<ExtensionsOptions>(builder.Configuration.GetSection(ExtensionsOptions.ExtensionOptionsKey));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

if (!string.IsNullOrWhiteSpace(sentryDsn))
{
    app.UseSentryTracing();
}

app.Run();
