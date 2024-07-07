using backend.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Dtos.TourDtos
{
    public class TourDto
    {
        public int Id { get; set; } 
        public string? Name { get; set; }
        public double? Price { get; set; }
        public int? category_id { get; set; }
        public string? Description { get; set; }
        public int? quantity_limit { get; set; }
        public DateTime? Departure_Time { get; set; }
        public int? Rating { get; set; } = 0;
        public bool? Type { get; set; } = false;
        public int? Range_time { get; set; } = 4;
        public double? Discount { get; set; }
        public int? Transportation_ID { get; set; }
        public string? Departure_location { get; set; }

        public IFormFileCollection? fileCollection { get; set; }
    }
    public class Tour_Update_Dto : TourDto
    {
        public List<string>? path { get; set; }

    }
}
