using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using NBP_I.Models;

namespace NBP_I.Hubs
{
    public class ChatHub : BaseHub
    {
        public async Task SaljiPoruku(string zaObjavu, string sadrzaj)
        {
            Poruka poruka = new Poruka(UserId.Value, sadrzaj);
            await Clients.Group(zaObjavu).SendAsync("MessageReceived", JsonConvert.SerializeObject(poruka), zaObjavu);
            Konverzacija k = RedisManager<Konverzacija>.GetString($"objave:{zaObjavu}:konverzacija");
            k.SaljiPoruku(poruka);
        }

        public async Task Subscribe(string objava) => await Groups.AddToGroupAsync(Context.ConnectionId, objava);
    }
}
