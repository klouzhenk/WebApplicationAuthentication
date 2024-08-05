using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationAuthentication.Entities
{
    [Table("app_users")]
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
        [Column("role")]
        public string Role { get; set; }

        [Column("id_town")]
        public int IdTown { get; set; }
    }
}
