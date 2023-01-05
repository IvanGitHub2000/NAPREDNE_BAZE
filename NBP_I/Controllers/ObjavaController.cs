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
        public async Task<IActionResult> DodajObjavu(string naziv, string sadrzaj, string tag)
        {

            if (!HttpContext.Session.IsLoggedIn())
            {
                //Korisnik k = new Korisnik();
                //k.Email = email;
                //k.KorisnickoIme = korisnickoIme;
                //k.Sifra = sifra;
                //k.Fakultet = fakultet;
                //k.Smer = smer;
                //k.GodinaStudije = godinaStudija;

                return StatusCode(401, "Niste logovani!");
            }

            var session = _driver.AsyncSession();
            try
            {
                var idUser= HttpContext.Session.GetUserId();//ovde mi stoji njegov id
                var result = await session.RunAsync($"CREATE(o:Objava {{naziv: '{naziv}',sadrzaj: '{sadrzaj}',tag:'{tag}',idAutora:'{idUser}',datumkreiranja:'{DateTime.Now}',brojlajkova:'{0}',rejting:'{null}' }}) return id(o)");
                if(result!=null)
                {
                    return StatusCode(200, "Uspesno ste dodali novu objavu!");
                }
            }
            finally
            {
                await session.CloseAsync();
            }

            return BadRequest("Greska!");
        }
        [HttpDelete]
        [Route("ObrisiObjavu")]
        public async Task<IActionResult> ObrisiObjavu(int idObjave)
        {

            if (!HttpContext.Session.IsLoggedIn())
            {

                return StatusCode(401, "Niste logovani!");
            }

            var session = _driver.AsyncSession();
            try
            {
                var result = await session.RunAsync($"MATCH (n) WHERE id(n) = {idObjave} DETACH DELETE n");
                if (result != null)
                {
                    return StatusCode(200, "Uspesno ste obrisali objavu!");
                }
            }
            finally
            {
                await session.CloseAsync();
            }

            return BadRequest("Greska!");
        }
    }
}
