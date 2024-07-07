using backend.Helper;

namespace backend.Dtos.ResortDtos
{
    public class ResortDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Price_range { get; set; }
        public int? Rating { get; set; }
        public int? LocationId { get; set; }

        public string? Location { get; set; }
        public string? PhoneNumber { get; set; }
        public List<ImageDto> UrlImage { get; set; }


    }
}
