using backend.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Dtos.TourDetailDtos
{
    public class TourDetailDto
    {
        public int? Id { get; set; }
        public string? Tour_Name { get; set; }    
        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public int? Quantity { get; set; }
    }
}
