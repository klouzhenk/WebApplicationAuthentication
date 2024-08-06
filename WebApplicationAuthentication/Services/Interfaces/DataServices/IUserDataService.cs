using WebApplicationAuthentication.Entities;
using WebApplicationAuthentication.Models.DTO;

namespace WebApplicationAuthentication.Services.Interfaces.DataServices
{
    public interface IUserDataService
    {
        Task<HttpResponseMessage> LoginUserAsync(string name, string password);
        Task<HttpResponseMessage> RegisterUserAsync(string name, string password, string role);
    }
}