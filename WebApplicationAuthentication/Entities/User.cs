using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationAuthentication.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public string Username { get; set; }

        [Required]
        [Column("password")]
        public string Password { get; set; }

        [Required]
        [Column("salt")]
        public string Salt { get; set; }

        [Required]
        [Column("refresh_token")]
        public string RefreshToken { get; set; }

        [Required]
        [Column("refresh_token_expiry_time")]
        public DateTime RefreshTokenExpiryTime { get; set; }

        [Required]
        [Column("role")]
        public string Role { get; set; }

        [Column("id_town")]
        public int IdTown { get; set; }
    }
}
