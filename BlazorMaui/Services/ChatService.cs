﻿using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorMaui.Services
{
    public class ChatService : IAsyncDisposable
    {
        private readonly HubConnection _hubConnection;
        private Timer _heartbeatTimer;
        private const int HeartbeatInterval = 10000; // 10 секунд

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
                StopHeartbeat();
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += connectionId =>
            {
                ConnectionState = "Connected";
                StartHeartbeat();
                return Task.CompletedTask;
            };

            _hubConnection.Closed += error =>
            {
                ConnectionState = "Disconnected";
                StopHeartbeat();
                return Task.CompletedTask;
            };
        }

        public async Task StartAsync()
        {
            try
            {
                await _hubConnection.StartAsync();
                ConnectionState = "Connected";
                StartHeartbeat();
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

        private void StartHeartbeat()
        {
            _heartbeatTimer = new Timer(async _ =>
            {
                if (_hubConnection.State == HubConnectionState.Connected)
                {
                    try
                    {
                        // Після того як зв'язок встановлено, перевірити активність
                        await _hubConnection.SendAsync("Ping"); 
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error during heartbeat: {ex.Message}");
                        ConnectionState = "Disconnected";
                        await _hubConnection.StopAsync();
                        await StartAsync(); // Перепідключення
                    }
                }
                else
                {
                    ConnectionState = "Disconnected";
                }
            }, null, HeartbeatInterval, HeartbeatInterval);
        }

        private void StopHeartbeat()
        {
            _heartbeatTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            _heartbeatTimer?.Dispose();
            _heartbeatTimer = null;
        }

        public event Action<string, string> MessageReceived;

        public async ValueTask DisposeAsync()
        {
            StopHeartbeat();
            await _hubConnection.DisposeAsync();
        }
    }
}
