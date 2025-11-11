using BusinessLogic;
using DataAccess;
using DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// Connection string
// ==========================
var connectionString = builder.Configuration.GetConnectionString("TripSyncDb");

// ==========================
// Register repositories (DAL)
// ==========================
builder.Services.AddScoped<RideRepository>(_ => new RideRepository(connectionString));
builder.Services.AddScoped<UserRepository>(_ => new UserRepository(connectionString));

// ==========================
// Register services (BLL)
// ==========================
builder.Services.AddScoped<RideService>();
builder.Services.AddScoped<UserService>();

// ==========================
// Add Razor Pages & session
// ==========================
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// ==========================
// Build the app
// ==========================
var app = builder.Build();

// ==========================
// Middleware pipeline
// ==========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession(); // Must come before MapRazorPages

app.MapRazorPages();

app.Run();
