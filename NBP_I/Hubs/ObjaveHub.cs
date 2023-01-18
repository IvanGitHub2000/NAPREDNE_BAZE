using Microsoft.AspNetCore.SignalR;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBP_I.Hubs
{
    public class ObjaveHub : BaseHub
    {
        public IDriver Driver => Startup.driver;

        public override async Task OnConnectedAsync()
        {
            if (!UserId.HasValue) return;

            IAsyncSession session = Driver.AsyncSession();
            try
            {
                var result = await session.RunAsync($"MATCH p=(k1:Korisnik)-[r:FOLLOW]->(k2:Korisnik) WHERE id(k1)={UserId} RETURN id(k2)");
                await Task.WhenAll((await result.ToListAsync()).Select(x => Subscribe(x["id(k2)"].As<string>())));
            }
            finally { await session.CloseAsync(); }
        }

        public async Task ObjavaPosted(string objavaName)
        {
            IAsyncSession session = Driver.AsyncSession();
            try
            {
                var result = await session.RunAsync($"MATCH p=(k1:Korisnik)-[r:FOLLOW]->(k2:Korisnik) WHERE id(k2)={UserId} RETURN id(k1)");
                (await result.ToListAsync()).ForEach(x => RedisManager<string>.Push($"korisnici:{x["id(k1)"].As<string>()}:notifikacije", $"{UserName} je postavio novi oglas - {objavaName}"));
            }
            finally { await session.CloseAsync(); }

            await Clients.Group(UserId.Value.ToString()).SendAsync("ObjavaPosted", objavaName, UserName);
        }

        public async Task Subscribe(string room) => await Groups.AddToGroupAsync(Context.ConnectionId, room);
    }
}
