csharp SecSchoolApi\Data\Configurations\SubjectConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecSchoolApi.Model;

namespace SecSchoolApi.Data.Configurations
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Code).HasMaxLength(32);
            builder.HasIndex(s => s.Name).IsUnique(false); // set to true if business requires unique names
        }
    }
}