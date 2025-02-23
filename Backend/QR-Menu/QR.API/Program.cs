using AspNetCoreRateLimit;
using HR.API.Middlewares;
using HR.Datalayer.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QR.Application.Helpers;
using QR.Application.Services;
using QR.DataLayer.Repositories;
using QR_Domain.Entities;
using QR_Domain.Interfaces;
using QR_Domain.Validations;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Load Configuration Sections
ConfigureDatabase(builder);
ConfigureIdentity(builder);
ConfigureCachingAndRateLimiting(builder);
ConfigureAuthentication(builder);
ConfigureCors(builder);
ConfigureCompression(builder);
ConfigureDependencyInjection(builder);
ConfigureSwagger(builder);

var app = builder.Build();

// Seed Data on Startup
await SeedDataAsync(app);

// Configure Middleware
ConfigureMiddleware(app);

app.Run();


#region Private Methods
void ConfigureDatabase(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<QRContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
}

void ConfigureIdentity(WebApplicationBuilder builder)
{
    builder.Services.AddIdentity<User, Role>(options =>
    {
        options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
    })
    .AddEntityFrameworkStores<QRContext>()
    .AddDefaultTokenProviders();
}

void ConfigureCachingAndRateLimiting(WebApplicationBuilder builder)
{
    builder.Services.AddMemoryCache();
    builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
    builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
    builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
}

void ConfigureAuthentication(WebApplicationBuilder builder)
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
        };
    });
}

void ConfigureCors(WebApplicationBuilder builder)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
}

void ConfigureCompression(WebApplicationBuilder builder)
{
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
    });
}

void ConfigureDependencyInjection(WebApplicationBuilder builder)
{
    // Repositories
    builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Services
    builder.Services.AddScoped<AuthService>();
    builder.Services.AddScoped<UserService>();
    builder.Services.AddSingleton<LogManager>();
    builder.Services.AddSingleton<TokenProvideService>();
    builder.Services.AddScoped<UserValidations>();
    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
}

void ConfigureSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Description = "Standard Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        options.OperationFilter<SecurityRequirementsOperationFilter>();
    });
}

async Task SeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var staticFileRequestPath = app.Configuration["StaticFiles:Path"];

    URLHelper.StaticFileRequestPath = staticFileRequestPath;
}

void ConfigureMiddleware(WebApplication app)
{
    app.UseResponseCompression();
    app.UseCors("AllowAll");
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    //app.UseIpRateLimiting();
    app.UseMiddleware<GlobalExceptionHandler>();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers();
    // Configure memory cache settings
    app.UseResponseCaching();
    ConfigureStaticFiles(app);
}

void ConfigureStaticFiles(WebApplication app)
{
    if (app.Environment.IsProduction())
    {
        var requestPath = app.Configuration["StaticFiles:RequestPath"];
        var path = app.Configuration["StaticFiles:Path"];

        app.UseStaticFiles(new StaticFileOptions
        {
            // Enable compression
            OnPrepareResponse = ctx =>
            {
                ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=86400, immutable");  
                ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                ctx.Context.Response.Headers.AccessControlAllowMethods = "GET, OPTIONS";
                ctx.Context.Response.Headers.AcceptRanges = "bytes";

            },
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), path)),
            RequestPath = requestPath,
            HttpsCompression = HttpsCompressionMode.Compress,
            DefaultContentType = "application/octet-stream",
        });
    }
}
#endregion