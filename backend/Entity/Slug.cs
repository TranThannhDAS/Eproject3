using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Entity
{
    public class Slug
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "URI is required")]
        public string URI { get; set; }

        [Required(ErrorMessage = "URIName is required")]
        public string URIName { get; set; }

        [ForeignKey(nameof(Role.Id))]
        public int RoleId { get; set; }
        public virtual Role? Ro { get; set; }
    }
}
