namespace SecSchoolApi.Model
{
    public class CreateRoomDto
    {
        public string Name { get; set; } = default!;
        public string? Hostel { get; set; }
        public int Capacity { get; set; }
    }
}
