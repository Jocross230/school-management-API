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

// ================= CORS (FIX FOR SWAGGER "FAILED TO FETCH") =================
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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SecSchoolApi", Version = "v1" });
    c.EnableAnnotations();
    c.ExampleFilters();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using Bearer scheme"
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
            new string[] {}
        }
    });
});

builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

// ================= DB =================
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TestDB")));

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
    throw new InvalidOperationException("JWT secret is not configured.");

byte[] secretKeyBytes;

try
{
    secretKeyBytes = Convert.FromBase64String(secretRaw);
}
catch
{
    secretKeyBytes = Encoding.UTF8.GetBytes(secretRaw);
}

if (secretKeyBytes.Length * 8 < 256)
    throw new InvalidOperationException("JWT secret too weak.");

var key = new SymmetricSecurityKey(secretKeyBytes);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

// ================= SAFE BACKGROUND SEED =================
_ = Task.Run(async () =>
{
    try
    {
        await SeedDataAsync(app, builder.Configuration, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Background seeding failed.");
    }
});

// ================= PIPELINE =================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SecSchoolApi v1");
    c.RoutePrefix = "swagger";
});

// 🔥 ORDER IS IMPORTANT
app.UseHttpsRedirection();

app.UseCors("AllowAll"); // ✅ FIX FOR SWAGGER + FRONTEND

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();


// ================= SEED METHOD =================
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
        logger.LogError(ex, "Migration failed but continuing.");
    }

    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var roles = new[] { "Admin", "Teacher", "Parent", "Student" };

        foreach (var r in roles)
        {
            if (!await roleManager.RoleExistsAsync(r))
                await roleManager.CreateAsync(new IdentityRole<Guid>(r));
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Role seeding skipped.");
    }

    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var adminEmail = configuration["SeedAdmin:Email"];
        var adminPassword = configuration["SeedAdmin:Password"];

        if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword))
        {
            var existing = await userManager.FindByEmailAsync(adminEmail);

            if (existing == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Admin"
                };

                var create = await userManager.CreateAsync(admin, adminPassword);

                if (create.Succeeded)
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

            await db.Terms.AddRangeAsync(new[]
            {
                new AcademicTerm { Name = $"{year} Term 1", StartDate = new DateTime(year,1,1,0,0,0,DateTimeKind.Utc), EndDate = new DateTime(year,4,1,0,0,0,DateTimeKind.Utc), IsCurrent = true },
                new AcademicTerm { Name = $"{year} Term 2", StartDate = new DateTime(year,4,2,0,0,0,DateTimeKind.Utc), EndDate = new DateTime(year,7,1,0,0,0,DateTimeKind.Utc), IsCurrent = false },
                new AcademicTerm { Name = $"{year} Term 3", StartDate = new DateTime(year,7,2,0,0,0,DateTimeKind.Utc), EndDate = new DateTime(year,10,1,0,0,0,DateTimeKind.Utc), IsCurrent = false }
            });
        }

        if (!await db.Subjects.AnyAsync())
        {
            await db.Subjects.AddRangeAsync(new[]
            {
                new Subject { Name = "English" },
                new Subject { Name = "Mathematics" },
                new Subject { Name = "Science" }
            });
        }

        if (!await db.GradingSchemes.AnyAsync())
        {
            await db.GradingSchemes.AddAsync(new GradingScheme { Name = "Default" });
        }

        await db.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Data seeding failed.");
    }
}
