using System.ComponentModel.DataAnnotations;

namespace SecSchoolApi.Model
{
    public class AppSetting
    {
        public Guid Id { get; set; }
        [MaxLength(100)]
        public string Key { get; set; } = default!;
        [MaxLength(2000)]
        public string Value { get; set; } = string.Empty;
    }
}
