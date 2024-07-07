    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using webapi.Base;

namespace backend.Entity
{
    public class Order : BaseCreateDate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double? Price { get; set; }
        public int? Number_people { get; set; }
        public int Tour_Detail_ID { get; set; }
        [ForeignKey("Tour_Detail_ID")]
        public TourDetail? tourDetail { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
