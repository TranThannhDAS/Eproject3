using backend.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Dtos.HotelDtos
{
    public class HotelImageDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Price_range { get; set; }
        public int? Rating { get; set; }
        public int? LocationId { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? Image { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Links { get; set; }
        public IFormFileCollection? fileCollection { get; set; }
    }
    public class Hotel_Update_Dto : HotelImageDto
    {
        public List<string>? path { get; set;}
    }
}
