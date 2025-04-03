using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

namespace AplicatieLicenta.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(int idClub, int idUtilizator, string email, string mesaj)
        {
            var ora = DateTime.Now.ToString("HH:mm");

            await Clients.Group($"Club-{idClub}")
                .SendAsync("ReceiveMessage", email, mesaj, ora);
        }

        public async Task SendVocal(int idClub, int userId, string email, string fileUrl)
        {
            var mesaj = "[Audio]";
            var ora = DateTime.Now.ToString("HH:mm");

            await Clients.Group($"Club-{idClub}")
                .SendAsync("ReceiveMessage", email, mesaj, ora, fileUrl);
        }

        public async Task JoinClub(int idClub)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Club-{idClub}");
        }
    }
}
