using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using StackExchange.Redis;
using Neo4jClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Neo4j.Driver;
using NBP_I.Session;
using NBP_I.Hubs;
using NBP_I.Extensions;
using NBP_I.Models;

namespace NBP_I.Controllers
{
    public class ObjavaController:Controller
    {
        private readonly IDriver _driver;
        private readonly IConnectionMultiplexer _redis;
        private readonly IHubContext<ObjaveHub> _hub;

        private static readonly object _lock = new();
        private static bool _firstTimeRun = true;

        public ObjavaController(IDriver driver, IHubContext<ObjaveHub> hub, IConnectionMultiplexer redis)
        {
            _driver = driver;
            _hub = hub;
            _redis = redis;

            //lock (_lock)
            //{
            //    if (!_firstTimeRun)
            //        return;
            //    _firstTimeRun = false;

            //    RedisManager<AdNotification>.Subscribe("AdPostedNotification", async (channel, data) => {
            //        data.followers.ToList().ForEach(x => RedisManager<string>
            //            .Push($"users:{x}:notifications", $"{data.userName} je postavio novi oglas - {data.adName}|{data.adId}"));
            //        await hub.Clients.All.SendAsync("NotificationReceived", data);
            //    });
            //}
        }
        [HttpPost]
        [Route("DodajObjavu")]
        public async Task<IActionResult> DodajObjavu(string naziv, string sadrzaj, string tag)//ovo radi xd
        {

            if (!HttpContext.Session.IsLoggedIn())
            { 

                return StatusCode(401, "Niste logovani!");
            }
            IResultCursor result;

            var session = _driver.AsyncSession();
            try
            {
                var res = await (await session.RunAsync($"MATCH (t:Tag {{ naziv: '{tag}' }}) RETURN id(t)")).ToListAsync();
                int tagID = -1;
                if(res.Count==0)//ako ne postoji taj tag kreiraj ga mafijo
                {
                    result = await session.RunAsync($"CREATE (tag:Tag {{ naziv: '{tag}' }}) return id(tag)");
                    tagID = await result.SingleAsync(record => record["id(tag)"].As<int>());
                }
                else
                {
                    tagID = res[0]["id[t]"].As<int>();
                }
                var idUser = HttpContext.Session.GetUserId();
                result = await session.RunAsync($"CREATE (o:Objava {{ naziv: '{naziv}', sadrzaj: '{sadrzaj}', tag: '{tag}', datumkreiranja: '{DateTime.Now}',idAutora:'{idUser}',likes:'{null}',rejting:'{0}' }}) return id(o)");
               var objavaID= await result.SingleAsync(record => record["id(o)"].As<int>());
                if (objavaID == -1)
                    //return RedirectToAction("Index", "Home");
                    return BadRequest("Nije kreirana objava");

                session = _driver.AsyncSession();
                result = await session.WriteTransactionAsync(tx => tx.RunAsync(@$"MATCH(t:Tag) WHERE id(t)={tagID} 
                                    MATCH (o:Objava) WHERE id(o)={objavaID} 
                                    CREATE (t)-[:HAS]->(o)"));

                var userId = HttpContext.Session.GetUserId();
                session = _driver.AsyncSession();
                result = await session.WriteTransactionAsync(tx => tx.RunAsync(@$"MATCH(k:Korisnik) WHERE id(k)={userId} 
                                    MATCH (o:Objava) WHERE id(o)={objavaID} 
                                    CREATE (k)-[:POSTED]->(o)"));

                //try //to je pubsub mehanizam to cemo posle
                //{
                //    result = await session.RunAsync($"MATCH p=(k1:Korisnik)-[r:PRATI]->(k2:Korisnik) WHERE id(k2)={userId} RETURN id(k1)");
                //    var idList = await result.ToListAsync();
                //    var ids = idList?.Select(x => x["id(u1)"].As<string>()).ToArray() ?? Array.Empty<string>();

                //    //var userName = HttpContext.Session.GetUsername();
                //    //RedisManager<AdNotification>.Publish("AdPostedNotification", new AdNotification(ids, adId, name, userId, userName));
                //}
                //finally { await session.CloseAsync(); }
            }
            finally
            {
                await session.CloseAsync();
            }

            return Ok("Uspesno je postavljena objava!");
        }

        [HttpDelete]
        [Route("ObrisiObjavu")]
        public async Task<IActionResult> ObrisiObjavu(int objavaID)
        {

            if (!HttpContext.Session.IsLoggedIn())
            {

                return StatusCode(401, "Niste logovani!");
            }
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                _ = await session.RunAsync($"MATCH (n) WHERE id(n) = {objavaID} DETACH DELETE n");
            }
            finally
            {
                await session.CloseAsync();
            }
            //return RedirectToAction("MineAds", "Home");
            return Ok("Uspesno obrisano!!!");
        }
        [HttpPost]
        [Route("SacuvajObjavu")]
        public async Task<IActionResult> SacuvajObjavu(int objavaID)
        {
            if (!HttpContext.Session.IsLoggedIn())
                //return RedirectToAction("Login", "Home");
                return StatusCode(401,"Niste logovani!!!");

            IAsyncSession session = _driver.AsyncSession();
            try
            {
                _ = await session.WriteTransactionAsync(tx => tx.RunAsync(@$"MATCH (k:Korisnik), (o:Objava) 
                                        WHERE id(k)={HttpContext.Session.GetUserId()} AND id(o)={objavaID}                                      
                                        CREATE (k)-[s:SACUVAO]->(o)
                                        RETURN type(s)"));
            }
            finally
            {
                await session.CloseAsync();
            }

            //return RedirectToAction("Index", "Home");
            return Ok("Uspesno sacuvano!!!");

        }
    }
}
