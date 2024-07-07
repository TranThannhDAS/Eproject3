using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Entity
{
    public class ForgotPasswordRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Code { get; set; }
        public DateTime? ExpirationTime { get; set; }

        [RegularExpression(
            "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$",
            ErrorMessage = "Email is invalid"
        )]
        public string Email { get; set; }
    }
}
