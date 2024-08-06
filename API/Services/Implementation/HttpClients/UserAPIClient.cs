using API.Services.Interfaces.HttpClients;
using API.Models.DTO;
using static System.Net.WebRequestMethods;

namespace API.Services.Implementation.HttpClients
{
    public class UserAPIClient : IUserAPIClient
    {
        private readonly HttpClient _client;

        public UserAPIClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> LoginUserAsync(string name, string password)
        {
            var loginRequest = new LoginRequest { Username = name, Password = password };
            return await _client.PostAsJsonAsync("/Auth/login", loginRequest);
        }

        public async Task<HttpResponseMessage> RegisterUserAsync(string name, string password, string role)
        {
            var registerRequest = new RegisterRequest { Username = name, Password = password, Role = role };
            return await _client.PostAsJsonAsync("/Auth/register", registerRequest);
        }
    }
}














//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Threading.Tasks;
//using Microsoft.Win32;
//using API.Entities;
//using API.Models.DTO;
//using API.Services.Interfaces;
//using static System.Net.WebRequestMethods;

//namespace API.Services.Implementation
//{
//    public class UserAPIClient : IUserAPIClient
//    {
//        private readonly HttpClient _client;
//        public UserAPIClient(HttpClient client)
//        {
//            _client = client;
//        }

//        public async Task<User> LoginUserAsync(string name, string password)
//        {
//            var loginRequest = new LoginRequest { Username = name, Password = password };
//            var response = await _client.PostAsJsonAsync("/Auth/login", loginRequest);
//            return await response.Content.ReadFromJsonAsync<User>();
//        }

//        public async Task<User> RegisterUserAsync(string name, string password, string role)
//        {
//            var registerRequest = new RegisterRequest { Username = name, Password = password, Role = role };
//            var response = await _client.PostAsJsonAsync("/Auth/register", registerRequest);
//            return await response.Content.ReadFromJsonAsync<User>();
//        }
//    }
//}
