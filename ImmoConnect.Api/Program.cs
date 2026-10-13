using ImmoConnect.Api.Data;
using ImmoConnect.Api.Mappings;
using ImmoConnect.Api.Models.Domain;
using ImmoConnect.Api.Repository;
using ImmoConnect.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ---------- CONTROLLERS ----------
// JsonStringEnumConverter : les enums sont écrits en texte ("Villa") et non en chiffre (2)
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// ---------- SWAGGER AVEC BOUTON "AUTHORIZE" ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Déclare à Swagger qu'on utilise un token de type Bearer (JWT)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Colle uniquement le token JWT (sans le mot 'Bearer')",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Dit à Swagger d'envoyer ce token automatiquement sur les requêtes
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
            new List<string>()
        }
    });
});

// ---------- BASE DE DONNÉES ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------- AUTOMAPPER ----------
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// ---------- REPOSITORIES ----------
builder.Services.AddScoped<IBienRepository, BienRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IVisiteRepository, VisiteRepository>();

// ---------- SERVICE TOKEN ----------
builder.Services.AddScoped<ITokenService, TokenService>();

// ---------- IDENTITY ----------
// AddIdentityCore : version légère pour une API (pas de cookies ni de pages de login)
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    // Règles du mot de passe
    options.Password.RequireDigit = true;             // au moins un chiffre
    options.Password.RequireLowercase = true;         // au moins une minuscule
    options.Password.RequireUppercase = true;         // au moins une majuscule
    options.Password.RequireNonAlphanumeric = false;  // symbole non obligatoire
    options.Password.RequiredLength = 6;              // 6 caractères minimum
})
    .AddRoles<IdentityRole>()                             // active RoleManager (Admin, Agent)
    .AddEntityFrameworkStores<AppDbContext>();            // stocke users et rôles dans SQL Server

// ---------- AUTHENTIFICATION JWT ----------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Comment le serveur vérifie chaque token reçu
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,                // l'émetteur doit correspondre à Jwt:Issuer
            ValidateAudience = true,              // le destinataire doit correspondre à Jwt:Audience
            ValidateLifetime = true,              // refuse les tokens expirés
            ValidateIssuerSigningKey = true,      // vérifie la signature avec notre clé secrète
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// ---------- CORS (front React Vite) ----------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ---------- SEED : rôles Admin/Agent + compte Admin ----------
// Un "scope" est nécessaire car UserManager et RoleManager sont des services Scoped
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}

// ---------- PIPELINE HTTP (l'ordre compte !) ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.UseAuthentication();   // 1. "Qui es-tu ?"      -> lit et valide le token
app.UseAuthorization();    // 2. "As-tu le droit ?" -> vérifie [Authorize] et les rôles

app.MapControllers();

app.Run();