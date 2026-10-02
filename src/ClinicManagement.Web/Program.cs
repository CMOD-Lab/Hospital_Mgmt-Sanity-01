using ClinicManagement.Application.Extensions;
using ClinicManagement.Infrastructure.Extensions;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/clinic-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog
    builder.Host.UseSerilog();

    // Add Razor Pages
    builder.Services.AddRazorPages();

    // Add session support
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    // Add HttpContextAccessor
    builder.Services.AddHttpContextAccessor();

    // Register Infrastructure services (DbContext + Repositories)
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // Register Application services
    builder.Services.AddApplicationServices();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseSession();

    app.UseAuthorization();

    app.MapRazorPages();

    // Default redirect to login page
    app.MapGet("/", context =>
    {
        context.Response.Redirect("/Index");
        return Task.CompletedTask;
    });

    Log.Information("Clinic Management System starting up...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
