using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using webapi.Base;

namespace backend.Entity
{
    public class TourDetail:BaseCreateDate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
        public int TourId { get; set; }
        [ForeignKey("TourId")]
        public Tour? tour { get; set; }
        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public int? Quantity { get; set; }
        public int? Staff_Id { get; set; } = 1;
        [ForeignKey("Staff_Id")]
        public Staff? Staff { get; set; }

        [Column(TypeName = "ntext")]
        public string? Description { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
//Them 1 bảng nữa và chạy migratiom