using FurniZone.BLL.Helpers;
using FurniZone.BLL.Mapping;
using FurniZone.BLL.Services.Abstractions;
using FurniZone.BLL.Services.Implementations;
using FurniZone.DAL.Database;
using FurniZone.DAL.Repositories.Abstractions;
using FurniZone.DAL.Repositories.Implementations;
using FurniZone.PL.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FurniZone E-Commerce API",
        Version = "v1",
        Description = "A complete E-Commerce REST API with JWT authentication"
    });

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

    // Group endpoints by access level tags
    options.TagActionsBy(api =>
    {
        if (api.ActionDescriptor is Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor descriptor)
        {
            var methodInfo = descriptor.MethodInfo;

            // Check for action-level attributes
            var authAttr = methodInfo.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
                .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>();
            var anonAttr = methodInfo.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute), true).Any();

            // Also check controller-level
            var controllerAuth = descriptor.ControllerTypeInfo.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
                .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>();
            var controllerAnon = descriptor.ControllerTypeInfo.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute), true).Any();

            var hasAuth = authAttr.Any() || controllerAuth.Any();
            var hasAnon = anonAttr || controllerAnon;

            if (hasAnon || !hasAuth)
                return new[] { "🔓 Public" };

            var allRoles = authAttr.Concat(controllerAuth).SelectMany(a => a.Roles?.Split(',') ?? Array.Empty<string>());
            if (allRoles.Any(r => r.Contains("Admin")))
                return new[] { "🔐 Admin Only" };
        }

        return new[] { "👤 User Access" };
    });

    options.DocInclusionPredicate((name, api) => true);

    // Add descriptions for each tag group
    options.DocumentFilter<TagDescriptionsDocumentFilter>();
});

// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Helpers
builder.Services.AddScoped<IJwtHelper, JwtHelper>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IFileUploadHelper, FileUploadHelper>();
builder.Services.AddScoped<IPaginationHelper, PaginationHelper>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Ensure wwwroot exists (with error handling for Azure)
try
{
    var webRootPath = app.Environment.WebRootPath;
    if (!string.IsNullOrEmpty(webRootPath) && !Directory.Exists(webRootPath))
    {
        Directory.CreateDirectory(webRootPath);
    }

    // Create images directory
    if (!string.IsNullOrEmpty(webRootPath))
    {
        var imagesPath = Path.Combine(webRootPath, "images", "products");
        if (!Directory.Exists(imagesPath))
        {
            Directory.CreateDirectory(imagesPath);
        }
    }
}
catch (Exception ex)
{
    // Log but don't fail startup - Azure may have different permissions
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Could not create wwwroot directories. Static files may not work correctly.");
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "FurniZone API v1");
    options.RoutePrefix = string.Empty; // Serve Swagger at root
});

// Custom middlewares
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

// Root endpoint - API info
app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();

app.Run();

// Document filter to add descriptions to Swagger tag groups
public class TagDescriptionsDocumentFilter : Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter
{
    public void Apply(Microsoft.OpenApi.Models.OpenApiDocument swaggerDoc, Swashbuckle.AspNetCore.SwaggerGen.DocumentFilterContext context)
    {
        swaggerDoc.Tags = new List<Microsoft.OpenApi.Models.OpenApiTag>
        {
            new Microsoft.OpenApi.Models.OpenApiTag
            {
                Name = "🔓 Public",
                Description = "No authentication required. Anyone can access these endpoints."
            },
            new Microsoft.OpenApi.Models.OpenApiTag
            {
                Name = "👤 User Access",
                Description = "Requires JWT token. Logged-in users can access their own data and perform user operations."
            },
            new Microsoft.OpenApi.Models.OpenApiTag
            {
                Name = "🔐 Admin Only",
                Description = "Requires JWT token with Admin role. Full system management access."
            }
        };
    }
}
