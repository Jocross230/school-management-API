using SecSchoolApi.Interface;
using SecSchoolApi.Model;
using SecSchoolApi.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace SecSchoolApi.Services
{
    public class SchoolService : ISchoolService
    {
        private readonly SchoolDbContext _db;
        public SchoolService(SchoolDbContext db) => _db = db;

        public async Task<IEnumerable<School>> GetAllAsync() =>
            await _db.Set<School>().Include(s => s.Branding).ToListAsync();

        public async Task<School?> GetByIdAsync(Guid id) =>
            await _db.Set<School>().Include(s => s.Branding).FirstOrDefaultAsync(s => s.Id == id);

        public async Task<School> RegisterAsync(School school)
        {
            _db.Set<School>().Add(school);
            await _db.SaveChangesAsync();
            return school;
        }

        public async Task<School?> UpdateAsync(Guid id, School school)
        {
            var s = await _db.Set<School>().FindAsync(id);
            if (s == null) return null;
            s.Name = school.Name;
            s.Address = school.Address;
            s.ContactEmail = school.ContactEmail;
            await _db.SaveChangesAsync();
            return s;
        }

        public async Task<Branding?> GetBrandingAsync(Guid schoolId) =>
            await _db.Set<Branding>().FirstOrDefaultAsync(b => b.SchoolId == schoolId);

        public async Task<Branding> SetBrandingAsync(Guid schoolId, Branding branding, CancellationToken ct = default)
        {
            var existing = await _db.Set<Branding>().FirstOrDefaultAsync(b => b.SchoolId == schoolId, ct);
            if (existing == null)
            {
                branding.SchoolId = schoolId;
                _db.Set<Branding>().Add(branding);
                await _db.SaveChangesAsync(ct);
                return branding;
            }
            existing.LogoUrl = branding.LogoUrl;
            existing.PrimaryColor = branding.PrimaryColor;
            existing.SecondaryColor = branding.SecondaryColor;
            await _db.SaveChangesAsync(ct);
            return existing;
        }
    }
}
