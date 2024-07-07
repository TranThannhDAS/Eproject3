using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using webapi.Base;

namespace backend.Entity
{
    public class Staff:BaseCreateDate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? PersonId { get; set; }
        public ICollection<TourDetail>? TourDetails { get; set; }
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
