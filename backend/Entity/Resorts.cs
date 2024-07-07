using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using webapi.Base;

namespace backend.Entity
{
  public class Resorts : BaseCreateDate
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public int? Rating { get; set; }

    [Column(TypeName = "ntext")]
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Price_range { get; set; }
    public string? PhoneNumber { get; set; }

    [ForeignKey(nameof(Location1.ID))]
    public int? LocationId { get; set; }
    public Location1? Location { get; set; }
    public string? Links { get; set; }
        public ICollection<Itinerary>? Itineraries { get; set; }
        public void AddImage(string imageName)
        {
            if (string.IsNullOrEmpty(Image))
            {
                Image = imageName;
            }
            else
            {
                Image += "," + imageName;
            }
        }
    }
}
