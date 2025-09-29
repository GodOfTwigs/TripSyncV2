var builder = WebApplication.CreateBuilder(args);

//Add Razor Pages
builder.Services.AddRazorPages();

//Register configuration for dependency injection
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

//Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2); // Keep session alive for 2 hours
    options.Cookie.HttpOnly = true; // Prevent JavaScript from accessing session cookie
    options.Cookie.IsEssential = true; // Always store session cookie
});

//Optional: Add caching for better session performance
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

//Enable session before routing
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//No authentication middleware yet, but we prepare for it later
app.UseAuthorization();

app.MapRazorPages();

app.Run();
