using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PesoClaro.API.Data;
using PesoClaro.API.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Base de datos ──────────────────────────────────────
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (databaseUrl != null)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]}";
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
}

builder.Services.AddDbContext<PesoClaroContext>(options =>
    options.UseNpgsql(connectionString)
);

// ── CORS ───────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("PesoClaro", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://pesoclaro-production-436b.up.railway.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ── Servicios ──────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CuentaService>();
builder.Services.AddScoped<PatrimonioService>();
builder.Services.AddScoped<MovimientoService>();

// ── JWT ────────────────────────────────────────────────
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? Environment.GetEnvironmentVariable("JWT__Secret")!;
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

// ── Construcción de la app ─────────────────────────────
var app = builder.Build();

// ── Middlewares ────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("PesoClaro");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();