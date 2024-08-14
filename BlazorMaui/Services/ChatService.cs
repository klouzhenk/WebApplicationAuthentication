using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorMaui.Services
{
    public class ChatService
    {
        private readonly HubConnection _hubConnection;

        public ChatService()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7296/chatHub") // Замініть на правильний URL вашого API
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                MessageReceived?.Invoke(user, message);
            });
        }

        public async Task StartAsync()
        {
            try
            {
                await _hubConnection.StartAsync();
                Console.WriteLine("Connection started");
            }
            catch (Exception ex)
            {
                // Логування або обробка помилки
                Console.WriteLine($"Error starting connection: {ex.Message}");
            }
        }

        public async Task SendMessageAsync(string user, string message)
        {
            try
            {
                await _hubConnection.SendAsync("SendMessage", user, message);
            }
            catch (Exception ex)
            {
                // Логування або обробка помилки
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        public event Action<string, string> MessageReceived;
    }
}
