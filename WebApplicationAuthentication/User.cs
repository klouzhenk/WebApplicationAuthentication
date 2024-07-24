﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationAuthentication
{
    [Table("app_users")]
    public class User
    {
        [Key]
        [Column("id")]
        public decimal Id { get; set; }

        [Column("name")]
        public string Username { get; set; }

        [Column("password")]
        public string Password { get; set; }
    }
}
