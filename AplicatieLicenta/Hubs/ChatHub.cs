using Microsoft.AspNetCore.SignalR;
using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using System.Threading.Tasks;
using System;

namespace AplicatieLicenta.Hubs;

public class ChatHub : Hub
{
    private readonly AppDbContext _context;

    public ChatHub(AppDbContext context)
    {
        _context = context;
    }

    public async Task SendMessage(int idClub, int idUtilizator, string email, string mesaj)
    {
        var mesajNou = new MesajClub
        {
            IdClub = idClub,
            IdUtilizator = idUtilizator,
            Continut = mesaj,
            DataTrimiterii = DateTime.Now
        };

        _context.MesajClub.Add(mesajNou);
        await _context.SaveChangesAsync();

        await Clients.Group($"Club-{idClub}")
            .SendAsync("ReceiveMessage", email, mesaj, mesajNou.DataTrimiterii.ToString("HH:mm"));
    }
    public async Task SendVocal(int idClub, int userId, string email, string fileName)
    {
        var url = $"/vocale/{fileName}";
        var mesaj = $"<audio controls src='{url}'></audio>";

        var mesajNou = new MesajClub
        {
            IdClub = idClub,
            IdUtilizator = userId,
            Continut = mesaj,
            DataTrimiterii = DateTime.Now
        };

        _context.MesajClub.Add(mesajNou);
        await _context.SaveChangesAsync();

        await Clients.Group($"Club-{idClub}")
            .SendAsync("ReceiveMessage", email, mesaj, mesajNou.DataTrimiterii.ToString("HH:mm"));
    }

    public async Task JoinClub(int idClub)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Club-{idClub}");
    }
}