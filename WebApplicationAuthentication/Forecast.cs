using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationAuthentication
{
    [Table("app_forecasts")]
    public class Forecast
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("id_town")]
        public int IdTown { get; set; }

        [Required]
        [Column("day")]
        public string Day { get; set; }
        
        [Required]
        [Column("time")]
        public string Time { get; set; }

        [Required]
        [Column("temperature")]
        public string Temperature { get; set; }

        [Required]
        [Column("state")]
        public string State { get; set; }
    }
}
