using Microsoft.EntityFrameworkCore;
using SecSchoolApi.Interface;
using SecSchoolApi.Services;
using SecSchoolApi.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using SecSchoolApi.Model;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ================= CORS =================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ================= CONTROLLERS =================
builder.Services.AddControllers(options =>
{
    options.Filters.Add<SecSchoolApi.Filters.ResponseEnvelopeFilter>();
})
.AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();

// ================= SWAGGER =================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SecSchoolApi",
        Version = "v1"
    });

    c.EnableAnnotations();
    c.ExampleFilters();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    // 🔥 IMPORTANT FOR SWAGGER ON RENDER
    c.AddServer(new OpenApiServer
    {
        Url = "https://school-management-api-h3ze.onrender.com"
    });
});

builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

// ================= DB =================
builder.Services.AddDbContext<SchoolDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("TestDB");

    if (string.IsNullOrWhiteSpace(conn))
        throw new Exception("Database connection string 'TestDB' is missing.");

    options.UseNpgsql(conn);
});

// ================= IDENTITY =================
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<SchoolDbContext>()
.AddDefaultTokenProviders();

// ================= JWT =================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretRaw = jwtSettings["Secret"];

if (string.IsNullOrWhiteSpace(secretRaw))
    throw new InvalidOperationException("JWT secret not configured.");

byte[] secretKeyBytes;
try
{
    secretKeyBytes = Convert.FromBase64String(secretRaw);
}
catch
{
    secretKeyBytes = Encoding.UTF8.GetBytes(secretRaw);
}

var key = new SymmetricSecurityKey(secretKeyBytes);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = key,

        ClockSkew = TimeSpan.Zero
    };
});

// ================= SERVICES =================
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IParentService, ParentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ISchoolService, SchoolService>();
builder.Services.AddScoped<IAccommodationService, AccommodationService>();
builder.Services.AddScoped<IAppControlService, AppControlService>();
builder.Services.AddScoped<IAcademicsService, AcademicsService>();
builder.Services.AddScoped<IReportsService, ReportsService>();
builder.Services.AddScoped<IBillingService, BillingService>();
builder.Services.AddScoped<IDisciplineService, DisciplineService>();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// ================= SAFE SEED (NO CRASH) =================
_ = Task.Run(async () =>
{
    try
    {
        await SeedDataAsync(app, builder.Configuration, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Seeding failed (ignored).");
    }
});

// ================= PIPELINE ORDER (IMPORTANT) =================

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

// 🔥 CORS MUST be before auth/controllers
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


// ================= SEED =================
static async Task SeedDataAsync(WebApplication app, IConfiguration configuration, ILogger logger)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<SchoolDbContext>();

    try
    {
        await db.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Migration failed.");
    }

    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        string[] roles = { "Admin", "Teacher", "Parent", "Student" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Role seeding failed.");
    }

    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var email = configuration["SeedAdmin:Email"];
        var password = configuration["SeedAdmin:Password"];

        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = "System Admin"
                };

                var result = await userManager.CreateAsync(admin, password);

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Admin seeding failed.");
    }

    try
    {
        if (!await db.Terms.AnyAsync())
        {
            var year = DateTime.UtcNow.Year;

            await db.Terms.AddRangeAsync(
                new AcademicTerm { Name = $"{year} Term 1", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(3), IsCurrent = true },
                new AcademicTerm { Name = $"{year} Term 2", StartDate = DateTime.UtcNow.AddMonths(3), EndDate = DateTime.UtcNow.AddMonths(6), IsCurrent = false }
            );

            await db.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Data seeding failed.");
    }
}
