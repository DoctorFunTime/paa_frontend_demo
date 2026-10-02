var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();
var app = builder.Build();
if (app.Environment.IsDevelopment()) app.UseWebAssemblyDebugging();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.Run();
