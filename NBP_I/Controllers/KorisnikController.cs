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
    [ApiController]
    [Route("[controller]")]
    public class KorisnikController : Controller
    {
        private readonly IDriver _driver;
        private readonly IConnectionMultiplexer _redis;
        private readonly IHubContext<ObjaveHub> _hub;

        private static readonly object _lock = new();
        private static bool _firstTimeRun = true;

        public KorisnikController(IDriver driver, IHubContext<ObjaveHub> hub, IConnectionMultiplexer redis)
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
        [Route("Registracija")]
        public async Task<IActionResult> Registracija(string email, string korisnickoIme, string sifra, string fakultet, string smer, int godinaStudija)
        {

            if (HttpContext.Session.IsLoggedIn())
            {
                //Korisnik k = new Korisnik();
                //k.Email = email;
                //k.KorisnickoIme = korisnickoIme;
                //k.Sifra = sifra;
                //k.Fakultet = fakultet;
                //k.Smer = smer;
                //k.GodinaStudije = godinaStudija;

                return StatusCode(201, "Korisnik vec logovan!");
            }

            var session = _driver.AsyncSession();
            try
            {
                var result = await session.RunAsync($"CREATE (k:Korisnik {{email: '{email}', korisnickoIme: '{korisnickoIme}', sifra:  '{sifra}', fakultet: '{fakultet}', smer:  '{smer}', godinaStudija: {godinaStudija} }}) return id(k)");///*,rejting:'{null}'*/ mozda mada vraca rejting 0 kad idem get metodu
                var userId = await result.SingleAsync(record => record["id(k)"].As<int>());
                if (userId != -1)
                {
                    HttpContext.Session.SetString(SessionKeys.Username, korisnickoIme);
                    HttpContext.Session.SetInt32(SessionKeys.UserId, userId);
                    return new JsonResult(userId);
                }
            }
            finally
            {
                await session.CloseAsync();
            }

            return BadRequest("Greska!");
        }

        [HttpPost]
        [Route("Prijava")]
        public async Task<IActionResult> Prijava(string korisnickoIme, string sifra)
        {
            if (HttpContext.Session.IsLoggedIn())
            {
                return StatusCode(201, "Korisnik vec logovan!");
            }

            var session = _driver.AsyncSession();
            try
            {
                var result = await session.RunAsync($"MATCH (k:Korisnik {{korisnickoIme: '{korisnickoIme}', sifra: '{sifra}'}}) RETURN id(k)");

                var res = await result.ToListAsync();
                if (res.Count == 0)
                    return StatusCode(401);

                var userId = res[0]["id(k)"].As<int>();

                if (userId != -1)
                {
                    HttpContext.Session.SetString(SessionKeys.Username, korisnickoIme);
                    HttpContext.Session.SetInt32(SessionKeys.UserId, userId);
                    return new JsonResult(userId);
                }
            }
            finally
            {
                await session.CloseAsync();
            }

            return BadRequest("Greska!");
        }

        [HttpPost]
        [Route("Odjava")]
        public IActionResult Odjava()
        {
            if (!HttpContext.Session.IsLoggedIn())
                return BadRequest("Niste logovani");

            HttpContext.Session.Clear();

            return Ok("Uspesno ste se odjavili!");
        }

        [HttpPut]
        [Route("PromeniPodatke")]
        public async Task<IActionResult> PromeniPodatke(string id, string username,string email, string godinaStudija, string smer, string fakultet, /*string password1,*/ string password, string repassword)
        {
            //var newPass = password1;//nisam najsigurniji poentu ovoga ali verovatn
            var newPass = " ";
            if (!string.IsNullOrEmpty(password) && password.CompareTo(repassword) == 0)
                newPass = password;
            else
            {
                return BadRequest("Sifre nisu iste!");
            }
            var userId = int.Parse(id);
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                _ = await session.RunAsync(@$"MATCH (k:Korisnik) 
                                    WHERE id(k)={userId}
                                    SET k = {{ email: '{email}',fakultet: '{fakultet}', godinaStudija: '{godinaStudija}' ,korisnickoIme: '{username}', sifra: '{newPass}',  smer: '{smer}'}} ");
            }
            finally
            {
                await session.CloseAsync();
            }

            //return RedirectToAction("Profile", "Home");
            return Ok();//radi funkcija lepo menja

        }
        [HttpGet]
        [Route("PreuzmiPodatke")]
        public async Task<IActionResult> PreuzmiPodatke()//kod djuke je u homecontroller Profile funkcija
        {
            if (!HttpContext.Session.IsLoggedIn())
                return RedirectToAction("Login", "Home");

            var userId = HttpContext.Session.GetUserId();
            var session = _driver.AsyncSession();
            try
            {
               
                var result = await session.RunAsync($"MATCH (k:Korisnik) WHERE id(k) = {userId} RETURN k");
                var user = (await result.SingleAsync())["k"].As<INode>();
                string str_godinaStudija = user.Properties["godinaStudija"].ToString();
                return Ok(new Korisnik//djuka ovde ime View(), umesto OK()
                {
                    ID = userId,
                    KorisnickoIme = user.Properties["korisnickoIme"].ToString(),
                    Sifra = user.Properties["sifra"].ToString(),
                    Email = user.Properties["email"].ToString(),
                    Fakultet = user.Properties["fakultet"].ToString(),
                    Smer = user.Properties["smer"].ToString(),
                  GodinaStudije= Int32.Parse(str_godinaStudija)


                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        [HttpGet]
        [Route("ZapratiKorisnika")]
        public async Task<IActionResult> ZapratiKorisnika(int uId)
        {
            var userId = HttpContext.Session.GetUserId();
            if (!HttpContext.Session.IsLoggedIn())
                return RedirectToAction("Login", "Home");

            IResultCursor result;
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                result = await session.RunAsync(@$"MATCH (k1:Korisnik)-[r:PRATI]->(k2:Korisnik)
                                        WHERE id(k1) = {userId} AND id(k2) = {uId}
                                        RETURN k1, r, k2");//k1 zapracuje k2 i vraca k1 k2 i vezu izmedju(follow),provera da li ne prati vec tog korisnika
                var checkIfFollow = await result.ToListAsync();

                if (checkIfFollow.Count == 0)//da ga vec ne pratis
                {
                    session = _driver.AsyncSession();//ponovo se otvara asinhrona sesija
                    _ = await session.RunAsync(@$"MATCH(k1:Korisnik) WHERE id(k1)={userId} 
                                    MATCH (k2:Korisnik) WHERE id(k2)={uId} 
                                    CREATE (k1)-[:PRATI]->(k2)"); // AKO BUDE BILO VREMENA DA SE ISPITA DA LI POSTOJI KORISNIK KOG ZAPRACUJEMO

                    RedisManager<int>.Push($"korisnici:{uId}:pratioci", userId);
                }
                else
                {
                    return BadRequest("Vec pratite ovog korisnika!!!");
                }
            }
            finally
            {
                await session.CloseAsync();
            }

            //return RedirectToAction("Index", "Home");
            return Ok("Napravljena nova veza PRATI od korisnika sa ID-jem: " + userId + " ka korisniku sa ID-jem:" + uId);
        }

        [HttpGet]//POST i think or PUT smth like that
        [Route("OtpratiKorisnika")]
        public async Task<IActionResult> OtpratiKorisnika(int uId)
        {
            if (!HttpContext.Session.IsLoggedIn())
                return RedirectToAction("Login", "Home");

            var userId = HttpContext.Session.GetUserId();
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                _ = await session.RunAsync(@$"MATCH (k1:Korisnik)-[r:PRATI]->(k2:Korisnik)
                                        WHERE id(k1) = {userId} AND id(k2) = {uId}
                                        DELETE r");//nalazi vezu PRATI izmedu trenutno logovanog i onog sa uId i brise vezu PRATI
            }       // AKO BUDE BILO VREMENA DA SE ISPITA DA LI PRATIMO KORISNIKA KOG OTPRACUJEMO
            finally
            {
                await session.CloseAsync();
            }

            //return RedirectToAction("Index", "Home");
            return Ok("Veza PRATI gde korisnik sa ID-jem:" + userId + " prati korisnika sa ID-jem:" + uId + " je uspesno obrisana!!!");
        }

    }

}
