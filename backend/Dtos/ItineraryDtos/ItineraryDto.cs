using backend.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Dtos.ItineraryDtos
{
    public class ItineraryDto
    {
        public int? Id { get; set; }
        public string? Tour_Name { get; set; }
        public int? Sequence { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? HotelName { get; set; }
        public string? ResortName { get; set; }
        public string? RestaurantName { get; set; }
    }
}
