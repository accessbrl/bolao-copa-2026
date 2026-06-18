using System.Text;
using BolaoCopa.Api.Data;
using BolaoCopa.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var connectionString = DatabaseUrlParser.GetConnectionString(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<ScoringService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bolão Copa 2026 API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite: Bearer {seu_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var jwtSecret = builder.Configuration["JWT_SECRET"]
    ?? builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("JWT secret não configurado.");

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "BolaoCopa2026",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "BolaoCopa2026",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddAuthorization();

var allowedOrigins = new List<string>
{
    "http://localhost:5173",
    "http://127.0.0.1:5173",
    "https://accessbrl.github.io"
};

var frontendUrl = builder.Configuration["FRONTEND_URL"];
if (!string.IsNullOrWhiteSpace(frontendUrl))
{
    allowedOrigins.Add(frontendUrl.TrimEnd('/'));
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins.Distinct(StringComparer.OrdinalIgnoreCase).ToArray())
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    app = "Bolão Copa do Mundo 2026",
    status = "online",
    swagger = "/swagger"
}));

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    timestampUtc = DateTime.UtcNow
}));

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var autoCreate = builder.Configuration["AUTO_CREATE_DATABASE"] ?? "true";

    if (autoCreate.Equals("true", StringComparison.OrdinalIgnoreCase))
    {
        db.Database.EnsureCreated();
        DatabaseCompatibilityPatches.Apply(db);
    }

    if (!db.ScoringRules.Any())
    {
        db.ScoringRules.Add(new BolaoCopa.Api.Models.ScoringRule());
        db.SaveChanges();
    }
}

app.Run();

public static class DatabaseUrlParser
{
    public static string GetConnectionString(IConfiguration configuration)
    {
        var databaseUrl = configuration["DATABASE_URL"];

        if (string.IsNullOrWhiteSpace(databaseUrl))
        {
            return configuration.GetConnectionString("DefaultConnection")
                   ?? throw new InvalidOperationException("Connection string não configurada.");
        }

        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':', 2);
        var username = Uri.UnescapeDataString(userInfo[0]);
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
        var database = uri.AbsolutePath.TrimStart('/');

        var useSsl = !uri.Host.Contains("localhost", StringComparison.OrdinalIgnoreCase)
                     && !uri.Host.Contains("127.0.0.1", StringComparison.OrdinalIgnoreCase);

        var sslPart = useSsl ? ";SSL Mode=Require;Trust Server Certificate=true" : "";

        var port = uri.Port > 0 ? uri.Port : 5432;

        return $"Host={uri.Host};Port={port};Database={database};Username={username};Password={password}{sslPart}";
    }
}

public static class DatabaseCompatibilityPatches
{
    public static void Apply(AppDbContext db)
    {
        try
        {
            db.Database.ExecuteSqlRaw("ALTER TABLE users ALTER COLUMN email DROP NOT NULL;");
        }
        catch
        {
            // Banco novo ou tabela ainda inexistente: ignora patch de compatibilidade.
        }

        try
        {
            db.Database.ExecuteSqlRaw("UPDATE users SET email = NULL WHERE email IS NOT NULL AND btrim(email) = '';");
        }
        catch
        {
            // Banco novo ou tabela ainda inexistente: ignora patch de compatibilidade.
        }

        try
        {
            db.Database.ExecuteSqlRaw("CREATE UNIQUE INDEX IF NOT EXISTS ux_users_name_lower ON users (lower(name));");
        }
        catch
        {
            // Se já houver nomes duplicados, o controle da API ainda impede novos duplicados.
        }
    }
}
