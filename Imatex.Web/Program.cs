using Blazor.Analytics;
using Imatex.Web.Helpers.Interop;
using Imatex.Web.Options;
using Imatex.Web.Services.Compression;
using Imatex.Web.Services.Extractor;
using Imatex.Web.Services.Ocr;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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
builder.Services.AddGoogleAnalytics(builder.Configuration["GoogleTagId"]);

builder.Services.AddScoped<IApplicationInterop, ApplicationInterop>();
builder.Services.AddScoped<IZipCompressorService, ZipCompressorService>();
builder.Services.AddScoped<ITextConverterService, TextConverterService>();
builder.Services.AddScoped<IImageExtractorService, ImageExtractorService>();
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

app.Run();
