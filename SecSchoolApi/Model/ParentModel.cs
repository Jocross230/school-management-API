namespace SecSchoolApi.Model
{
    public class ParentModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;

        public ICollection<StudentModel> Children { get; set; } = new List<StudentModel>();
    }
}
