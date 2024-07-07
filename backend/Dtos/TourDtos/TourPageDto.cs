using backend.Helper;

namespace backend.Dtos.TourDtos
{
    public class TourPageDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public string? category_Name { get; set; }
        public string? Description { get; set; }
        public int? quantity_limit { get; set; }
        public DateTime? Departure_Time { get; set; }
        public int? Rating { get; set; } = 0;
        public bool? Type { get; set; } = false;
        public int? Range_time { get; set; } = 4;
        public double? Discount { get; set; }
        public string? Transportation_Name { get; set; }
        public string? Departure_location { get; set; }
        public List<ImageDto> UrlImage { get; set; }
    }
}
