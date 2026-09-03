using Taskify.Web.Components;
using Taskify.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Shared Aspire service defaults (health checks, OpenTelemetry, resilience, service discovery).
builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Per-circuit identity state (no-login phase 1) and the in-process real-time bus.
builder.Services.AddScoped<IdentityState>();
builder.Services.AddSingleton<RealtimeBus>();

// Typed client for the Taskify API, resolved via Aspire service discovery ("apiservice").
builder.Services.AddHttpClient<ApiClient>(client =>
    client.BaseAddress = new Uri("http://apiservice"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
