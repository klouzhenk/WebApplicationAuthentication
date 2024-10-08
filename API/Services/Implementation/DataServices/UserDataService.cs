using API.Services.Interfaces.DataServices;
using API.Services.Interfaces.HttpClients;

namespace API.Services.Implementation.DataServices;

public class UserDataService : IUserDataService
{
    private IUserAPIClient _client;
    public UserDataService(IUserAPIClient client)
    {
        _client = client;
    }
    public async Task<HttpResponseMessage> LoginUserAsync(string name, string password)
    {
        return await _client.LoginUserAsync(name, password);
    }

    public async Task<HttpResponseMessage> RegisterUserAsync(string name, string password, string role)
    {
        return await _client.RegisterUserAsync(name, password, role);
    }
}
