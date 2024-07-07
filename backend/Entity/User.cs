using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using webapi.Base;

namespace backend.Entity
{
    public class User : BaseCreateDate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(
            "(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])((?=.*\\W)|(?=.*_))^[^ ]+$",
            ErrorMessage = "Password must contains at least one lower case char, upper case char, digit and symbol"
        )]
        public string Password { get; set; }

        [RegularExpression(
            "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$",
            ErrorMessage = "Email is invalid"
        )]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }

        [ForeignKey(nameof(Role.Id))]
        [Required(ErrorMessage = "RoleId is required")]
        public int RoleId { get; set; }
        public virtual Role? Roles { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
