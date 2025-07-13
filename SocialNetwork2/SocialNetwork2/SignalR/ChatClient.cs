//-
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;


namespace SocialNetwork2.SignalR;

public class ChatClient : IAsyncDisposable
{
    // подключение для взаимодействия с хабом
    HubConnection _hubConnection;

    public bool IsConnected =>
        _hubConnection?.State == HubConnectionState.Connected;

    public ChatClient(string connectionUrl)
    {
        // создаем подключение к хабу
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(connectionUrl)
            .WithAutomaticReconnect()
            .Build();

        // регистрируем функцию Receive для получения данных
        _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            Console.WriteLine($"SignalR message - {user}: {message}");
        });
    }

    public async ValueTask StartAsync()
    {
        if (_hubConnection is not null && _hubConnection.State == HubConnectionState.Disconnected)
        {
            await _hubConnection.StartAsync();
        }
    }

    public async ValueTask StopAsync()
    {
        if (_hubConnection is not null && _hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.StopAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }

    /*
    */
    public async Task SendMessage(string userInput, string messageInput)
    {
        if (_hubConnection is not null)
        {
            // The SendAsync method returns a Task which completes when the message
            //  has been sent to the server. No return value is provided
            //  since this Task doesn't wait until the server method completes
            await _hubConnection.SendAsync("SendMessage", userInput, messageInput);

            // The InvokeAsync method returns a Task which completes when the server method returns.
            //  The return value, if any, is provided as the result of the Task
            // await _hubConnection.InvokeAsync("SendMessage", userInput, messageInput);
        }
    }
}
