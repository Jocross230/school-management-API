using System.Text.Json.Serialization;

namespace SecSchoolApi.Model
{
    public class StudentModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public string Class { get; set; } = default!;
        public string? HealthIssue { get; set; }

        public Guid ParentId { get; set; }
        [JsonIgnore] // prevent cycles Parent.Children[].Parent
        public ParentModel? Parent { get; set; }
    }
}
