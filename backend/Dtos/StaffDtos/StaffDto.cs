using backend.Helper;

namespace backend.Dtos.StaffDtos
{
    public class StaffDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? PersonId { get; set; }
        public List<ImageDto> UrlImage { get; set; }

    }
}
