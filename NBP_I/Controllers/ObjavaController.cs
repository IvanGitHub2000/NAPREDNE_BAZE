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
     
        [HttpDelete]
        [Route("ObrisiNotifikaciju")]
        public IActionResult ObrisiNotifikaciju(string path, string item)
               => RedisManager<string>.DeleteItem(path, item) ? Ok() : BadRequest();

       
        [HttpPut]
        [Route("PromeniObjavu")]
        public async Task<IActionResult> PromeniObjavu(string id, string naziv, string sadrzaj, string tag)//radi ovo
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
            var objavaId = int.Parse(id);//samo mozda se pravi novi tag kad se menja objava***
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                _ = await session.RunAsync($"MATCH (o:Objava) WHERE id(o)={objavaId} SET o = {{ naziv: '{naziv}', sadrzaj: '{sadrzaj}', tag: '{tag}' }} ");//ne kreira novi tag




                /*     _ = await session.RunAsync(@$"MATCH (ad:Ad) 
                                    WHERE id(ad)={adId}
                                    SET ad = {{ name: '{name}', category: '{category}', price: '{price}', description: '{descritpion}' }} ");*/
            }
            finally
            {
                await session.CloseAsync();
            }
            // STA TREBA OVDE? \\
            //return RedirectToAction("MineAds", "Home");
            return Ok("Uspesna promena!");
        }

        [HttpGet]
        [Route("PrikaziSveObjaveKorisnika")]
        public async Task<IActionResult> PrikaziSveObjaveKorisnika(int userId)//radi
        {
            List<Objava> objavaList = null;
            IResultCursor result;
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                result = await session.RunAsync($"MATCH (k:Korisnik)-[r:POSTED]->(o:Objava) WHERE id(k) = {userId} RETURN o");
                var list = await result.ToListAsync();



                if (list.Count > 0)
                {
                    Korisnik k = new Korisnik { ID = userId };
                    objavaList = new List<Objava>();
                    list.ForEach(x =>
                    {
                        INode objava = x["o"].As<INode>();



                        objavaList.Add(new Objava
                        {
                            Id = (int)objava.Id,
                            Naziv = objava.Properties["naziv"].ToString(),
                            Sadrzaj = objava.Properties["sadrzaj"].ToString(),
                            Tag = objava.Properties["tag"].ToString(),
                            idAutora = userId
                        });
                    });
                }
            }
            finally
            {
                await session.CloseAsync();
            }



            return Ok(objavaList);
        }

        [HttpGet]
        [Route("PrikaziPojedinacneObjaveKorisnika")]
        public async Task<IActionResult> PrikaziPojedinacneObjaveKorisnika(int objavaId)//radi i ovo
        {
            if (!HttpContext.Session.IsLoggedIn())
            {
                return StatusCode(401, "Niste logovani!");
            }



            var userId = HttpContext.Session.GetUserId();
            Objava objava;
            IResultCursor result;
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                //Sta treba ovde?
                result = await session.RunAsync($"MATCH (k:Korisnik)-[r:POSTED]-(o:Objava) WHERE id(o) = {objavaId} RETURN k, o");
                var objava1 = await result.SingleAsync();



                INode objava2 = objava1["o"].As<INode>();
                INode k = objava1["k"].As<INode>();



                if (userId == (int)k.Id)
                {
                    objava = new()
                    {
                        Id = (int)objava2.Id,
                        Naziv = objava2.Properties["naziv"].ToString(),
                        Sadrzaj = objava2.Properties["sadrzaj"].ToString(),
                        Tag = objava2.Properties["tag"].ToString(),
                        idAutora = userId
                    };
                }
                else
                {
                    session = _driver.AsyncSession();
                    result = await session.RunAsync($"MATCH (k1:Korisnik)<-[r:PRATI]-(k2:Korisnik) WHERE id(k1) = {(int)k.Id} RETURN id(k2)");
                    var pratiociId = await result.ToListAsync();



                    Korisnik kor = new()
                    {
                        ID = (int)k.Id,
                        KorisnickoIme = k.Properties["username"].ToString(),
                        Pratioci = pratiociId.Select(x => x["id(k2)"].As<int>()).ToList()
                    };



                    objava = new()
                    {
                        Id = (int)objava2.Id,
                        Naziv = objava2.Properties["naziv"].ToString(),
                        Sadrzaj = objava2.Properties["sadrzaj"].ToString(),
                        Tag = objava2.Properties["tag"].ToString(),
                        idAutora = userId
                    };
                }
                session = _driver.AsyncSession();
                result = await session.RunAsync(@$"MATCH (k:Korisnik)-[r:VISITED]-(o:Objava) 
                                    WHERE id(k)={userId} 
                                    RETURN o, r, k");



                var poseceneObjave = await result.ToListAsync();



                var rel = poseceneObjave.Select(rec => rec["r"].As<IRelationship>()).ToList().Find(x => x.EndNodeId == objavaId);
                long count = 1;
                // Ovo ne znam sta je
                if (rel != null && rel.Properties.TryGetValue("visitedCount", out object val))
                {
                    count = (long)val + 1;
                }



                // Ovo ne znam sta je
                if (poseceneObjave.Count < 6)
                {
                    session = _driver.AsyncSession();
                    if (rel == null)
                    {
                        result = await session.RunAsync(@$"MATCH(k:Korisnik) WHERE id(k)={userId} 
                                                MATCH (o:Objava) WHERE id(o)={objavaId} 
                                                CREATE (k)-[:VISITED {{ timestamp: {DateTime.Now.Ticks}, visitedCount: {count} }}]->(o)");
                    }
                    else
                    {
                        result = await session.RunAsync(@$"MATCH(k:Korisnik)-[r:VISITED]-(o:Objava) 
                                                WHERE id(k)={userId} AND id(o)={objavaId} 
                                                SET r = {{ timestamp: {DateTime.Now.Ticks}, visitedCount: {count} }}");
                    }
                }
                else
                {
                    var min = poseceneObjave.Min(x => (long)x["r"].As<IRelationship>().Properties["timestamp"]);
                    session = _driver.AsyncSession();
                    result = await session.RunAsync(@$"MATCH(k:Korisnik)-[r:VISITED]->(o:Objava) 
                                            WHERE id(k)={userId} AND r.timestamp={min} 
                                            DELETE r");



                    session = _driver.AsyncSession();
                    if (rel == null)
                    {
                        await session.RunAsync(@$"MATCH(k:Korisnik) WHERE id(k)={userId} 
                                                MATCH (o:Objava) WHERE id(o)={objavaId} 
                                                CREATE (k)-[:VISITED {{ timestamp: {DateTime.Now.Ticks}, visitedCount: {count} }}]->(o)");
                    }
                    else
                    {
                        await session.RunAsync(@$"MATCH(k:Korisnik)-[r:VISITED]-(o:Objava) 
                                                WHERE id(k)={userId} AND id(o)={objavaId} 
                                                SET r = {{ timestamp: {DateTime.Now.Ticks}, visitedCount: {count} }}");
                    }
                }
            }
            finally
            {
                await session.CloseAsync();
            }



            //return View(objava);
            return Ok(objava);
        }


        [HttpGet]
        [Route("MojeObjave")]
        public async Task<IActionResult> MojeObjave()
        {
            if (!HttpContext.Session.IsLoggedIn())
                //return RedirectToAction("Login", "Home");
                return BadRequest("Niste ulogovani!");

            var userId = HttpContext.Session.GetUserId();
            List<Objava> objavaList = new();
            IResultCursor result;
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                result = await session.RunAsync($"MATCH (k:Korisnik)-[r:POSTED]-(o:Objava) WHERE id(k) = {userId} RETURN o");
                var objave = await result.ToListAsync();
                objave.ForEach(a => {
                    INode o1 = a["o"].As<INode>();
                    objavaList.Add(new()
                    {
                        Id = (int)o1.Id,
                        Naziv = o1.Properties["naziv"].ToString(),
                        Sadrzaj = o1.Properties["sadrzaj"].ToString(),
                        Tag = o1.Properties["tag"].ToString(),
                        //Description = o1.Properties["description"].ToString()
                    });
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return Ok(objavaList);
        }


        [HttpGet]
        [Route("Profil")]
        public async Task<IActionResult> Profil()
        {
            if (!HttpContext.Session.IsLoggedIn())
                //return RedirectToAction("Login", "Home");
                return StatusCode(401, "Niste ulogovani!");

            var userId = HttpContext.Session.GetUserId();
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.RunAsync($"MATCH (k:Korisnik) WHERE id(k) = {userId} RETURN k");
                var user = (await result.SingleAsync())["k"].As<INode>();
                string str_godinaStudija = user.Properties["godinaStudija"].ToString();

                return Ok(new Korisnik
                {
                    ID = userId,
                    KorisnickoIme = user.Properties["korisnickoIme"].ToString(),
                    Sifra = user.Properties["sifra"].ToString(),
                    Email = user.Properties["email"].ToString(),
                    Fakultet = user.Properties["fakultet"].ToString(),
                    Smer = user.Properties["smer"].ToString(),
                    GodinaStudije = Int32.Parse(str_godinaStudija)

                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        // TO DO A, B, C
        [HttpGet]
        [Route("PrikaziSveObjave")]
        public async Task<IActionResult> PrikaziSveObjave()
        {
            List<Tag> tagList;
            List<Objava> objavaList = null;
            List<Objava> objavaPreporukeList = null;



            IResultCursor result;
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                // Pokupi sve tagove
                result = await session.RunAsync($"MATCH (t:Tag) RETURN t");



                tagList = (await result.ToListAsync()).Select(t => {
                    INode tag = t["t"].As<INode>();
                    return new Tag
                    {
                        Id = (int)tag.Id,
                        Naziv = tag.Properties["naziv"].ToString()
                    };
                }).ToList();



                // Nadji sve tagove koji imaju objave i dodaj te objave u njihovu listu objava
                await Task.WhenAll(tagList.Select(async x => {
                    session = _driver.AsyncSession();
                    result = await session.RunAsync($"MATCH (t:Tag {{naziv: '{x.Naziv}'}})-[r]-(o:Objava) return o");
                    var veze = await result.ToListAsync();
                    veze.ForEach(v => {
                        INode v1 = v["o"].As<INode>();
                        var o = new Objava
                        {
                            Id = (int)v1.Id,
                            Naziv = v1.Properties["naziv"].ToString(),
                            Sadrzaj = v1.Properties["sadrzaj"].ToString(),
                            Tag = v1.Properties["tag"].ToString(),
                            idAutora = Int32.Parse((string)v1.Properties["idAutora"])
                        };
                        x.Objave.Add(o);
                    });
                }));



                var userId = HttpContext.Session.GetUserId();
                if (userId >= 0)
                {
                    // Sve objave koje je trenutni korisnik posetio, pamte se u objavaList
                    session = _driver.AsyncSession();
                    result = await session.RunAsync(@$"MATCH (k:Korisnik)-[r:VISITED]-(o:Objava) 
                                    WHERE id(k)={userId} 
                                    RETURN o, r, k");



                    objavaList = (await result.ToListAsync()).Select(o => {
                        INode oo = o["o"].As<INode>();
                        return new Objava
                        {
                            Id = (int)oo.Id,
                            Naziv = oo.Properties["naziv"].ToString(),
                            Sadrzaj = oo.Properties["sadrzaj"].ToString(),
                            Tag = oo.Properties["tag"].ToString(),
                            idAutora = (int)oo.Properties["idAutora"]
                        };
                    }).ToList();



                    // Gleda koji su najcesce trazeni tagovi
                    session = _driver.AsyncSession();
                    result = await session.RunAsync(@$"MATCH (k:Korisnik)-[r:VISITED]-(o:Objava) 
                                    WHERE id(k)={userId} 
                                    RETURN o.tag
                                    ORDER BY r.visitedCount DESC
                                    LIMIT 1");
                    var fav = await result.ToListAsync();
                    if (fav.Count > 0)
                    {
                        // Ovde ne znam sto je stavio 2 puta Values, probaj obe varijante 
                        var favTag = fav[0].Values.Values.FirstOrDefault().ToString();
                        //var favTag = fav[0].Values.FirstOrDefault().ToString();



                        // Sve objave omiljenog taga, pamte se u objavaPreporukeList
                        session = _driver.AsyncSession();
                        result = await session.RunAsync(@$"MATCH (t:Tag)-[r:HAS]-(o:Objava) 
                                    WHERE t.name = '{favTag}' 
                                    RETURN o");



                        objavaPreporukeList = (await result.ToListAsync()).Select(o => {
                            INode oo = o["o"].As<INode>();



                            Objava obj = new()
                            {
                                Id = (int)oo.Id,
                                Naziv = oo.Properties["naziv"].ToString(),
                                Sadrzaj = oo.Properties["sadrzaj"].ToString(),
                                Tag = oo.Properties["tag"].ToString(),
                                idAutora = userId
                            };



                            return obj;
                        }).Where(x => objavaList.Find(y => x.Id == y.Id) == null).ToList();
                    }
                }
            }
            finally
            {
                await session.CloseAsync();
            }
            //return View(new Ads { CategoryList = categoryList, AdList = adList, AdRecomendList = adRecomendList, Leaderboard = Leaderboard });
            return Ok('1');
        }

    }
}
