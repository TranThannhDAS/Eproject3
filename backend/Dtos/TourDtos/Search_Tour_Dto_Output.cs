using backend.Entity;
using backend.Helper;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Dtos.TourDtos
{
    public class Search_Tour_Dto_Output
    {
        public int Id { get; set; }
        public string? Name { get; set; } 
        public double Price { get; set; }
        public int category_id { get; set; } = 1;
        public string? Description { get; set; } 
        public string? image { get; set; }
        public int quantity_limit { get; set; }
        public int Rating { get; set; } = 0;
        public bool? Type { get; set; } = false;
        public int? Range_time { get; set; } = 4;
        public double Discount { get; set; }
        public int Transportation_ID { get; set; }
        public List<ImageDto> UrlImage { get; set; }
    }
}
