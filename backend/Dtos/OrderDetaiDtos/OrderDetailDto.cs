using backend.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Dtos.OrderDetaiDtos
{
    public class OrderDetailDto
    {
        public int? Id { get; set; }
        public int? OrderID { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public string? User_Name { get; set; }
        public string? Description { get; set; }
        public int? Tour_Detail_ID { get; set; }
    }
}
