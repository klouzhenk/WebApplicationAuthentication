﻿namespace WebApplicationAuthentication.Models.DTO
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int IdTown { get; set; }
    }
}