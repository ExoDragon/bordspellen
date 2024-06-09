using Core.Domain.Security;
using Core.Infrastructure.Context;
using Core.Infrastructure.Repository.Actors;
using Core.Infrastructure.Repository.Entities;
using Core.Infrastructure.Seeder;
using Core.Repositories.Data.Actors;
using Core.Repositories.Data.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Database configuration
builder.Services.AddDbContext<EntityDatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
});

builder.Services.AddDbContext<IdentityDatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnectionString"));
});

// Add services to the container.
builder.Services.AddScoped<IBoardGameRepository, BoardGameDatabaseRepository>();
builder.Services.AddScoped<IGameEventRepository, GameEventDatabaseRepository>();
builder.Services.AddScoped<IPersonRepository, PersonDatabaseRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewDatabaseRepository>();
builder.Services.AddScoped<IDietRepository, DietDatabaseRepository>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//Adding Authentication
builder.Services.AddIdentity<BoardGameUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<IdentityDatabaseContext>();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(_ =>
{
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

//Entities & Actors
PersonSeeder.Seed(app).Wait();
GameEventSeeder.Seed(app).Wait();
BoardGameSeeder.Seed(app).Wait();
ReviewSeeder.Seed(app).Wait();
DietSeeder.Seed(app).Wait();

//Relations
BoardGameEventSeeder.Seed(app).Wait();
GameEventDietsSeeder.Seed(app).Wait();
PersonDietsSeeder.Seed(app).Wait();
PersonGameEventsSeeder.Seed(app).Wait();

//Identity
IdentitySeeder.Seed(app).Wait();

app.Run();