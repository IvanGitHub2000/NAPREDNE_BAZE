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
                var result = await session.RunAsync($"CREATE (k:Korisnik {{email: '{email}', korisnickoIme: '{korisnickoIme}', sifra:  '{sifra}', fakultet: '{fakultet}', smer:  '{smer}', godinaStudija: {godinaStudija} }}) return id(k)");
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
























        //[HttpPost]
        //[Route("Register/{username}/{email}/{password}")]
        //public async Task<ActionResult> Register(string username, string email, string password)
        //{
        //    // Connect to the Neo4j database

        //    using IAsyncSession session = driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Write));
        //    //var session = driver.AsyncSession(); // mozes i ovako

        //    // Create a query to create a new User node with the specified name, password, and email properties
        //    //string createUserQuery = "CREATE (u:User {username: $username, password: $password, email: $email})";

        //    //// Execute the query, passing in the parameter values
        //    //var result=await session.RunAsync(createUserQuery, new { username = username, password = password, email = email });
        //    var result = await session.RunAsync($"CREATE (user:User {{username: '{username}', password:  '{password}', email:  '{email}' }}) return id(user)");
        //    var userId = await result.SingleAsync(record => record["id(user)"].As<int>());
        //    if (userId != -1)
        //    {
        //        HttpContext.Session.SetString(SessionKeys.Username, username);
        //        HttpContext.Session.SetInt32(SessionKeys.UserId, userId);
        //        //return RedirectToAction("Index", "Home");
        //    }
        //    // Close the session and driver
        //    await session.CloseAsync();
        //    //driver.Dispose();

        //    // Return a value indicating that the operation was successful
        //    return Ok("Korisnik uspesno registrovan!");
        //}
        //[HttpPost]
        //[Route("Login/{username}/{password}")]
        //public async Task<ActionResult> Login(string username,  string password)
        //{
        //    // Connect to the Neo4j database

        //    using IAsyncSession session = driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Write));

        //    //var session = driver.AsyncSession();


        //        var result = await session.RunAsync($"MATCH (u:User {{username: '{username}', password: '{password}'}}) RETURN id(u)");

        //        var res = await result.ToListAsync();
        //    //if (res.Count == 0)
        //    //    return RedirectToAction("Login", "Home");

        //    var userId = res[0]["id(u)"].As<int>();

        //    if (userId != -1)//ovde mu dozvoli da moz da radi sta hoce
        //    {
        //        HttpContext.Session.SetString(SessionKeys.Username, username);//ovde problem sa sesijom kaze nije nastelovana
        //        HttpContext.Session.SetInt32(SessionKeys.UserId, userId);
        //      //  return RedirectToAction("Index", "Home");//ovde kao se redirektuje dobije dozvolu
        //    }

        //    // Return a value indicating that the operation was successful
        //    await session.CloseAsync();
        //    //if(userId!=-1)
        //    //    return Ok("Korisnik je uspesno logovan!");
        //    //return Ok("Korisnik nije ulogovan!");
        //    return Ok("Korisnik je uspesno logovan!");
        /*chatgpt
            IDriver driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "password"));

using IAsyncSession session = driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Write));

// Create a query to find the User node with the specified username and password
string findUserQuery = "MATCH (u:User) WHERE u.username = $username AND u.password = $password RETURN u";

// Execute the query, passing in the parameter values
IResultCursor cursor = await session.RunAsync(findUserQuery, new { username = username, password = password });

// Check if a User node was found
if (cursor.SingleAsync().Result == null)
{
    // No User node was found, so return an error
    return StatusCode(StatusCodes.Status401Unauthorized);
}
else
{
    // A User node was found, so return a success result
    return Ok();
}

// Close the session and driver
await session.CloseAsync();*/

    }

}
