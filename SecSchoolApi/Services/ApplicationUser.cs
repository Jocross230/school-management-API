using Microsoft.AspNetCore.Identity;

namespace SecSchoolApi.Services
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = default!;
        public Guid? SchoolId { get; set; }
    }
}
