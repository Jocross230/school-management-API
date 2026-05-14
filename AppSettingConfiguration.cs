using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecSchoolApi.Model;

namespace SecSchoolApi.Data.Configurations
{
    public class AppSettingConfiguration : IEntityTypeConfiguration<AppSetting>
    {
        public void Configure(EntityTypeBuilder<AppSetting> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Key).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Value).HasMaxLength(1024);
            builder.HasIndex(a => a.Key).IsUnique(true);
        }
    }
}