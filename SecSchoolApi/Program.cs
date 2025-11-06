using Microsoft.EntityFrameworkCore;
using SecSchoolApi.Interface;
using SecSchoolApi.Services;
using SecSchoolApi.Data;
using System;
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

builder.Services.AddControllers(options =>
{
    options.Filters.Add<SecSchoolApi.Filters.ResponseEnvelopeFilter>();
})
.AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    o.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SecSchoolApi", Version = "v1" });
    c.EnableAnnotations();
    c.ExampleFilters();
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TestDB")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<SchoolDbContext>()
.AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings["Secret"]!;
var key = Encoding.UTF8.GetBytes(secret);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

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
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<SchoolDbContext>();
    db.Database.Migrate();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var roles = new[] { "Admin", "Teacher", "Parent", "Student" };
    foreach (var r in roles)
    {
        if (!roleManager.RoleExistsAsync(r).Result)
        {
            roleManager.CreateAsync(new IdentityRole<Guid>(r)).Wait();
        }
    }

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var adminEmail = builder.Configuration["SeedAdmin:Email"];
    var adminPassword = builder.Configuration["SeedAdmin:Password"];
    if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword))
    {
        var existing = userManager.FindByEmailAsync(adminEmail).Result;
        if (existing == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin"
            };
            var create = userManager.CreateAsync(admin, adminPassword).Result;
            if (create.Succeeded)
            {
                userManager.AddToRoleAsync(admin, "Admin").Wait();
            }
        }
    }

    if (!db.Terms.Any())
    {
        var year = DateTime.UtcNow.Year;
        db.Terms.Add(new AcademicTerm { Name = $"{year} Term 1", StartDate = new DateTime(year,1,1), EndDate = new DateTime(year,4,1), IsCurrent = true });
        db.Terms.Add(new AcademicTerm { Name = $"{year} Term 2", StartDate = new DateTime(year,4,2), EndDate = new DateTime(year,7,1), IsCurrent = false });
        db.Terms.Add(new AcademicTerm { Name = $"{year} Term 3", StartDate = new DateTime(year,7,2), EndDate = new DateTime(year,10,1), IsCurrent = false });
    }
    if (!db.Subjects.Any())
    {
        db.Subjects.AddRange(new [] { new Subject { Name = "English" }, new Subject { Name = "Mathematics" }, new Subject { Name = "Science" } });
    }
    if (!db.GradingSchemes.Any())
    {
        db.GradingSchemes.Add(new GradingScheme { Name = "Default" });
    }
    db.SaveChanges();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<SecSchoolApi.Middleware.MaintenanceMiddleware>();
app.MapControllers();
app.Run();
