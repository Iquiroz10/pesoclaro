using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PesoClaro.API.Data;
using PesoClaro.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// ── Base de datos ──────────────────────────────────────
builder.Services.AddDbContext<PesoClaroContext>(options => 
options.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

// ── Servicios ──────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CuentaService>();
builder.Services.AddScoped<PatrimonioService>();


// ── JWT ────────────────────────────────────────────────
var jwtSecret = builder.Configuration["Jwt:Secret"]!;
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });




// -- Construccion de la app
var app = builder.Build();

// -- Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();