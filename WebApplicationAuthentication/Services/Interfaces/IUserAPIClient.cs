﻿using WebApplicationAuthentication.Entities;

namespace WebApplicationAuthentication.Services.Interfaces
{
    public interface IUserAPIClient
    {
        Task<HttpResponseMessage> LoginUserAsync(string name, string password);
        Task<HttpResponseMessage> RegisterUserAsync(string name, string password, string role);
    }
}
