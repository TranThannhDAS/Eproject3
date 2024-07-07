using System.Configuration;
using System.Text;
using backend.Entity;
using backend.Extensions;
using backend.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using webapi.Dao.IServices;
using webapi.Dao.Services;
using webapi.Data;
using webapi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add data context
builder.Services.AddDbContext<DataContext>(
    (options) =>
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("SQLServerConnectionString")
        );
    }
);

// Jwt Config
builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        var Key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]);
        o.SaveToken = true;
        o.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Key),
            ClockSkew = TimeSpan.Zero
        };
        o.Events = new JwtBearerEvents()
        {
            OnAuthenticationFailed = (context) =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

// Dependency injection
builder.Services.AddScoped<IJWTManagerService, JWTManagerService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<DataContext>();
builder.Services.AddAppServices();
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
// Cors
builder.Services.AddCors(
    opt =>
        opt.AddPolicy(
            name: "AllowLocalHost",
            policy =>
            {
                policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
            }
        )
);

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

app.UseCors("AllowLocalHost");

app.UseAuthentication();

app.UseAuthorization();

//app.UseMiddleware<CheckSlugRole>();
app.UseMiddleware<GlobalErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
