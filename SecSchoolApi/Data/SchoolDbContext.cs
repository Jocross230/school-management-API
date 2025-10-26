using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecSchoolApi.Model;
using SecSchoolApi.Services;
using System;

namespace SecSchoolApi.Data
{
    public class SchoolDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options) { }

        public DbSet<StudentModel> Students { get; set; } = default!;
        public DbSet<Admin> Admins { get; set; } = default!;
        public DbSet<ParentModel> Parents { get; set; } = default!;
        public DbSet<Teacher> Teachers { get; set; } = default!;
        public DbSet<FeePayment> Payments { get; set; } = default!;
        public DbSet<Result> Results { get; set; } = default!;
        public DbSet<AttendanceModel> Attendance { get; set; } = default!;
        public DbSet<AnnouncementModel> Announcements { get; set; } = default!;
        public DbSet<Assignment> Assignments { get; set; } = default!;
        public DbSet<DisciplineRecord> DisciplineRecords { get; set; } = default!;
        public DbSet<Timetable> Timetables { get; set; } = default!;
        public DbSet<Message> Messages { get; set; } = default!;
        public DbSet<Notification> Notifications { get; set; } = default!;
        public DbSet<School> Schools { get; set; } = default!;
        public DbSet<Branding> Brandings { get; set; } = default!;
        public DbSet<Class> Classes { get; set; } = default!;
        public DbSet<Room> Rooms { get; set; } = default!;
        public DbSet<AccommodationRequest> AccommodationRequests { get; set; } = default!;
        public DbSet<AppSetting> AppSettings { get; set; } = default!;
        public DbSet<AcademicTerm> Terms { get; set; } = default!;
        public DbSet<Subject> Subjects { get; set; } = default!;
        public DbSet<StudentClassEnrollment> Enrollments { get; set; } = default!;
        public DbSet<GradingScheme> GradingSchemes { get; set; } = default!;
        public DbSet<Invoice> Invoices { get; set; } = default!;
        public DbSet<DisciplineCase> DisciplineCases { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentModel>()
                .HasOne(s => s.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(s => s.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Branding>()
                .HasOne<School>()
                .WithOne(s => s.Branding)
                .HasForeignKey<Branding>(b => b.SchoolId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FeePayment>().HasIndex(p => p.Reference).IsUnique(true);
            modelBuilder.Entity<AppSetting>().HasIndex(s => s.Key).IsUnique();
        }
    }
}
