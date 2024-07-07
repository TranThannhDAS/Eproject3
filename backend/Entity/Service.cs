using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using webapi.Base;

namespace backend.Entity
{
  public class Service : BaseCreateDate
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    public string? Name { get; set; }

    [Column(TypeName = "ntext")]
    public string? Description { get; set; }

    public int? Tour_ID { get; set; }
     [ForeignKey("Tour_ID")]
     public Tour? Tour { get; set; }
  }
}
