using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Dtos.TransportationDtos
{
    public class TransportationDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public string? Description { get; set; }
    }
}
