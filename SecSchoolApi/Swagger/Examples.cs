using Swashbuckle.AspNetCore.Filters;
using SecSchoolApi.Model;

namespace SecSchoolApi.Swagger
{
    public class AccommodationRequestDtoExample : IExamplesProvider<AccommodationRequestDto>
    {
        public AccommodationRequestDto GetExamples() => new AccommodationRequestDto
        {
            HostelPreference = "Hostel A",
            Remark = "Needs lower bunk"
        };
    }

    public class AccommodationRequestExample : IExamplesProvider<AccommodationRequest>
    {
        public AccommodationRequest GetExamples() => new AccommodationRequest
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            StudentId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Status = AccommodationStatus.Pending,
            RequestedAt = DateTime.UtcNow,
            Remark = "Needs lower bunk"
        };
    }

    public class RoomExample : IExamplesProvider<Room>
    {
        public Room GetExamples() => new Room
        {
            Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            Name = "A101",
            Hostel = "Hostel A",
            Capacity = 4,
            Occupied = 2
        };
    }
}
