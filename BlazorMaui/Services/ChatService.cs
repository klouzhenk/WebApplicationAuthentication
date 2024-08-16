using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorMaui.Services
{
    public class ChatService
    {
        private readonly HubConnection _hubConnection;

        public string ConnectionState { get; private set; } = "Disconnected";

        public ChatService()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7267/chatHub") // Замініть на правильний URL вашого API
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                MessageReceived?.Invoke(user, message);
            });

            _hubConnection.Reconnecting += error =>
            {
                ConnectionState = "Reconnecting";
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += connectionId =>
            {
                ConnectionState = "Connected";
                return Task.CompletedTask;
            };

            _hubConnection.Closed += error =>
            {
                ConnectionState = "Disconnected";
                return Task.CompletedTask;
            };
        }

        public async Task StartAsync()
        {
            try
            {
                await _hubConnection.StartAsync();
                ConnectionState = "Connected";
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
