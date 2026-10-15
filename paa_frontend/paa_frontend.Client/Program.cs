using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using paa_frontend.Client;
using paa_frontend.Client.Services;
using paa_frontend.Client.Services.Admin;
using paa_frontend.Client.Services.Auth;
using paa_frontend.Client.Services.StaffPortal;
using paa_frontend.Client.Services.StudentsPortal;
using paa_frontend.Client.Services.StudentPass;
using paa_frontend.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");

// The demo host forwards API requests to the same development gateway as the main frontend.
// Same-origin calls avoid changing the gateway's single-origin CORS configuration.
var apiBaseUrl = builder.HostEnvironment.BaseAddress;

// 2. Auth setup
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<UiActivityState>();
builder.Services.AddScoped<CustomAuthorizationMessageHandler>();
builder.Services.AddTransient<GlobalHttpErrorHandler>();

// Set a realistic timeout for cold starts (under 2 min for browser compliance)
void RegisterClient<T>(IServiceCollection services, TimeSpan? timeout = null) where T : class
{
    services.AddHttpClient<T>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = timeout ?? TimeSpan.FromSeconds(110); // just under browser limit
    })
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>()
    .AddHttpMessageHandler<GlobalHttpErrorHandler>();
}

// 4. Register all services
RegisterClient<StudentProfileServiceClient>(builder.Services, TimeSpan.FromSeconds(110));
RegisterClient<AssessmentServiceClient>(builder.Services);
RegisterClient<FinanceServiceClient>(builder.Services);
RegisterClient<TeacherProfileServiceClient>(builder.Services);
RegisterClient<BurserProfileServiceClient>(builder.Services);
RegisterClient<AdminProfileServiceClient>(builder.Services);
RegisterClient<ApplicationServiceClient>(builder.Services);
// Public application submission must not inherit a saved admin/student bearer token.
builder.Services.AddHttpClient<PublicApplicationServiceClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(40);
});
RegisterClient<GradesServiceClient>(builder.Services);
RegisterClient<StudentSubjectServiceClient>(builder.Services);
RegisterClient<AlevelClassServiceClient>(builder.Services);
RegisterClient<OlevelClassServiceClient>(builder.Services);
RegisterClient<StudentClassesServiceClient>(builder.Services);
RegisterClient<AdminGradesServiceClient>(builder.Services);
RegisterClient<AttendanceServiceClient>(builder.Services);
RegisterClient<FinancePassServiceClient>(builder.Services);
RegisterClient<TimetableServiceClient>(builder.Services);

// 5. Auth service (no interceptor needed)
builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(110);
})
.AddHttpMessageHandler<GlobalHttpErrorHandler>();

// 6. Notification service
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddAuthorizationCore();

var host = builder.Build();
await host.Services.GetRequiredService<IJSRuntime>().InvokeVoidAsync("setNimbusIdentity", "PrimeApp Demo");
await host.RunAsync();
