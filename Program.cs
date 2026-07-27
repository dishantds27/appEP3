using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Data;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// Add database context - SQLite for cloud, SQL Server for local
var connectionString = builder.Configuration.GetConnectionString("MyDbConnection");
if (connectionString != null && connectionString.StartsWith("Data Source="))
{
    builder.Services.AddDbContext<MyDatabaseContext>(options =>
        options.UseSqlite(connectionString));
}
else
{
    builder.Services.AddDbContext<MyDatabaseContext>(options =>
        options.UseSqlServer(connectionString));
}
builder.Services.AddDistributedMemoryCache();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add App Service logging
builder.Logging.AddAzureWebAppDiagnostics();

var app = builder.Build();

// Auto-migrate database bij opstarten
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyDatabaseContext>();
    var connStr = builder.Configuration.GetConnectionString("MyDbConnection");
    if (connStr != null && connStr.StartsWith("Data Source="))
    {
        db.Database.EnsureCreated();
    }
    else
    {
        var retries = 5;
        while (retries > 0)
        {
            try
            {
                db.Database.Migrate();
                break;
            }
            catch
            {
                retries--;
                Thread.Sleep(5000);
            }
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Todos}/{action=Index}/{id?}");

app.Run();