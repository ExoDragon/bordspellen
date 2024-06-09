using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using BordSpellenApi.GraphQL;
using BordSpellenApi.GraphQL.Queries;
using Core.Domain.Security;
using Core.Domain.Security.Roles;
using Core.Infrastructure.Context;
using Core.Infrastructure.Repository.Actors;
using Core.Infrastructure.Repository.Entities;
using Core.Infrastructure.Seeder;
using Core.Repositories.Data.Actors;
using Core.Repositories.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddMvc().AddSessionStateTempDataProvider();
builder.Services.AddSession();

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

//Allow CORS 
builder.Services.AddCors(options =>
{
    options.AddPolicy("Cors",
        policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

//Setup GraphQL
builder.Services.AddGraphQLServer()
    .AddQueryType<BaseQuery>()
        .AddTypeExtension<GameEventQuery>()
        .AddTypeExtension<BoardGameQuery>()
    .AddFiltering().AddSorting();

//Adding Authentication.
//Setup Security Provider.
builder.Services.AddIdentity<BoardGameUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<IdentityDatabaseContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var claims = new List<Claim>
                {
                    new (ClaimTypes.Role, UserRoles.User),
                    new (ClaimTypes.Role, UserRoles.Organiser)
                };
                
                var appIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
                context.Principal?.AddIdentity(appIdentity);
                return Task.CompletedTask;
            }
        };
    });




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BordSpellen API",
        Version = "v1"
    });
    
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Autorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
                
            },
            new string[] {}
        }
    });
    
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseCors("Cors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGraphQL();

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