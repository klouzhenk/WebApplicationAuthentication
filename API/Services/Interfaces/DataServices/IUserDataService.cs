using API.Entities;

namespace API.Services.Interfaces.DataServices
{
    public interface IUserDataService
    {
        Task<HttpResponseMessage> LoginUserAsync(string name, string password);
        Task<HttpResponseMessage> RegisterUserAsync(string name, string password, string role);
    }
}