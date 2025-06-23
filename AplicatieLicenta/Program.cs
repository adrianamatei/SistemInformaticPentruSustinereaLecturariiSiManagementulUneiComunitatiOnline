using AplicatieLicenta.Data;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using AplicatieLicenta.Hubs;
using AplicatieLicenta.Models;
using AplicatieLicenta.Services;
using QuestPDF.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options=>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
}
    );
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; 
});
builder.Services.AddSignalR();
builder.Services.AddHttpClient<GoogleBookService>();
builder.Services.AddSingleton<GeminiService>();
QuestPDF.Settings.License = LicenseType.Community;

builder.Services.Configure<GoogleBooksSettings>(builder.Configuration.GetSection("GoogleBooks"));
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient<GoogleSearchServices>();


var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapHub<ChatHub>("/chatHub");
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
