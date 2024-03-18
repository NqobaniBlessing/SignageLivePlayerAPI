using SignageLivePlayerFrontEnd.Services;
using SignageLivePlayerFrontEnd.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAuthentication("Signage_Live")
    .AddCookie("Signage_Live", options =>
    {
        options.Cookie.Name = "Signage_Live";
        options.ExpireTimeSpan = TimeSpan.FromSeconds(200);
        options.Cookie.HttpOnly = true;
    });

builder.Services.AddSession();
builder.Services.AddHttpClient("sl_client", client =>
{
    client.BaseAddress = new Uri("https://localhost:7292");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddScoped<ISecurityContextService, SecurityContextService>();

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

app.UseSession();

// Add token to request header.
app.Use(async (context, next) =>
{
    var token = context.Session.GetString("access_token");
    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers.Add("Authorization", "Bearer " + token);
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
