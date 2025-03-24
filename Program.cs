using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WineRecommendation.Data;
using WineRecommendation.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// EFCore with Sqlite
builder.Services.AddDbContext<WineDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

    options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddFilter((category, level) =>
        category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information ? false : true)));
});

// Register services
builder.Services.AddScoped<IWineService, WineService>();
builder.Services.AddScoped<IWinePredictionService, WinePredictionService>();
builder.Services.AddScoped<ModelTrainingService>();
builder.Services.AddScoped<DatabaseService>();
builder.Services.AddHostedService<SeedDataService>();
builder.Services.AddSingleton<BackgroundTrainingQueue>();
builder.Services.AddHostedService<BackgroundTrainingService>();

var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
