using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using webapi.Base;

namespace backend.Entity
{
  public class Hotel : BaseCreateDate
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? Name { get; set; }
    //public double Price { get; set; }
    public string? Price_range { get; set; }
    public int? Rating { get; set; }

    public int? LocationId { get; set; }
        [ForeignKey("LocationId")]

        public Location1? location1 { get; set; }

    [Column(TypeName = "ntext")]
    public string? Description { get; set; }
    //public string? ImageDetail { get; set; }
    public string? Address { get; set; }
    public string? Image { get; set; }
    public string? PhoneNumber { get; set; }
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
