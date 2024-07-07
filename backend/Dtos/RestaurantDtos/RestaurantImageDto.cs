using backend.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Dtos.RestaurantDtos
{
    public class RestaurantImageDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Price_range { get; set; }
        public int? Rating { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumbber { get; set; }
        public int? LocationId { get; set; }  
        public string? Links { get; set; }
        public IFormFileCollection? fileCollection { get; set; }
    }
    public class Restaurant_Update_Dto : RestaurantImageDto
    {
        public List<string>? path { get; set; }

    }
}
