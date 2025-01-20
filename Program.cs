global using GstMagazin.Data;
global using GstMagazin.Models;
global using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<GstDbMagazin>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("StringChainCnx"),sqlServerOptionsAction=>sqlServerOptionsAction.EnableRetryOnFailure(maxRetryCount:3,maxRetryDelay:TimeSpan.FromSeconds(2),errorNumbersToAdd:null)));
builder.Services.AddSession(options =>{options.IdleTimeout = TimeSpan.FromMinutes(30); });// Set to 30 minutes (adjust as needed)

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
