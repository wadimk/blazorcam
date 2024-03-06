using System;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorCam.Models;
using Microsoft.AspNetCore.SignalR;

namespace BlazorCam.Services
{
    public class WebRtcService : Hub
    {
        public async Task NewUser(string username)
        {
            var userInfo = new UserInfo() { userName = username, connectionId = Context.ConnectionId };
            await Clients.Others.SendAsync("NewUserArrived", JsonSerializer.Serialize(userInfo));
        }

        public async Task HelloUser(string userName, string user)
        {
            var userInfo = new UserInfo() { userName = userName, connectionId = Context.ConnectionId };
            await Clients.Client(user).SendAsync("UserSaidHello", JsonSerializer.Serialize(userInfo));
        }

        public async Task SendSignal(string signal, string user)
        {
            await Clients.Client(user).SendAsync("SendSignal", Context.ConnectionId, signal);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Others.SendAsync("UserConnect", Context.ConnectionId);
            Console.WriteLine($"{Context.ConnectionId} connected");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            await Clients.All.SendAsync("UserDisconnect", Context.ConnectionId);
            Console.WriteLine($"Disconnected {e?.Message} {Context.ConnectionId}");
            await base.OnDisconnectedAsync(e);
        }

        public async Task Send(string message)
        {
            Console.WriteLine(message);
            await Clients.Others.SendAsync("Receive", message);
        }
    }
}