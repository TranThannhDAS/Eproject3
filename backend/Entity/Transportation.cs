using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using webapi.Base;

namespace backend.Entity
{
    public class Transportation : BaseCreateDate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }

        [Column(TypeName = "ntext")]
        public string? Description { get; set; }
        public ICollection<Tour>? Tours { get; set; } 
    }
}
