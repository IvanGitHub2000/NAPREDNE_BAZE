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
    public class KonverzacijaController : Controller
    {
        private readonly IDriver _driver;

        public KonverzacijaController(IDriver dirver)
        {
            _driver = dirver;
        }

        [HttpGet]
        [Route("VratiPoruke")]
        public IActionResult VratiPoruke(string idKonverzacije)
        {
            Konverzacija k = RedisManager<Konverzacija>.GetString($"objave:{idKonverzacije}:konverzacija");
            Poruka[] poruke = k.VratiPoruke();

            return Ok(poruke);
        }

        [HttpGet]
        [Route("SaljiPoruku")]
        public IActionResult SaljiPoruku(string idKonverzacije, string sadrzaj)
        {
            int userId = HttpContext.Session.GetUserId();
            if (!HttpContext.Session.IsLoggedIn())
            {
                return BadRequest("Niste logovani");
            }
            Poruka poruka = new Poruka(userId, sadrzaj);
            Konverzacija k = RedisManager<Konverzacija>.GetString($"objave:{idKonverzacije}:konverzacija");
            k.SaljiPoruku(poruka);

            return Ok(poruka);
        }

        [HttpPost]
        [Route("NapraviKonverzaciju")]
        public IActionResult NapraviKonverzaciju(string idObjave, string nazivObjave)
        {
            if (!HttpContext.Session.IsLoggedIn())
            {
                return BadRequest("Niste logovani");
            }
            var k = new Konverzacija(idObjave, nazivObjave);
            RedisManager<Konverzacija>.SetString($"objave:{idObjave}:konverzacija", k);
            return Ok(k);
        }

    }
}
