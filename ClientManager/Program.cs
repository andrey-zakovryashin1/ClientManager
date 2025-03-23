using Microsoft.EntityFrameworkCore;
using ClientManager.Models;
using ClientManager.Services;
using ClientManager.Middleware;

var builder = WebApplication.CreateBuilder(args);

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ClientsContext>(options => options.UseSqlServer(connection));

builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ClientQueryService>();
builder.Services.AddScoped<DataInitializer>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ClientsContext>();
    var dataInitializer = services.GetRequiredService<DataInitializer>();
    dataInitializer.Initialize();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
