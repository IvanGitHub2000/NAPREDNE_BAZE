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
namespace NBP_I.Controllers
{
    public class KorisnikController:Controller
    {
        private readonly IDriver driver;
        private IDatabase database;
        public KorisnikController([FromServices] IDriver _driver, [FromServices] IDatabase db) 
        {
            driver = _driver;
            database = db;

        }

        [HttpPost]
        [Route("Register/{username}/{email}/{password}")]
        public async Task<ActionResult> Register(string username, string email, string password)
        {
            // Connect to the Neo4j database
           
            using IAsyncSession session = driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Write));
            //var session = driver.AsyncSession(); // mozes i ovako

            // Create a query to create a new User node with the specified name, password, and email properties
            string createUserQuery = "CREATE (u:User {username: $username, password: $password, email: $email})";

            // Execute the query, passing in the parameter values
            await session.RunAsync(createUserQuery, new { username = username, password = password, email = email });

            // Close the session and driver
            await session.CloseAsync();
            //driver.Dispose();

            // Return a value indicating that the operation was successful
            return Ok();
        }
        [HttpPost]
        [Route("Login/{username}/{password}")]
        public async Task<ActionResult> Login(string username,  string password)
        {
            // Connect to the Neo4j database

            using IAsyncSession session = driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Write));

            //var session = driver.AsyncSession();

         
                var result = await session.RunAsync($"MATCH (u:User {{username: '{username}', password: '{password}'}}) RETURN id(u)");

                var res = await result.ToListAsync();
            //if (res.Count == 0)
            //    return RedirectToAction("Login", "Home");

            var userId = res[0]["id(u)"].As<int>();

            //if (userId != -1)//ovde mu dozvoli da moz da radi sta hoce
            //{
            //    HttpContext.Session.SetString(SessionKeys.Username, username);//ovde problem sa sesijom kaze nije nastelovana
            //    HttpContext.Session.SetInt32(SessionKeys.UserId, userId);
            //    return RedirectToAction("Index", "Home");//ovde kao se redirektuje dobije dozvolu
            //}

            // Return a value indicating that the operation was successful
            await session.CloseAsync();
            if(userId!=-1)
                return Ok("Korisnik je uspesno logovan!");
            return Ok("Korisnik nije ulogovan!");
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
}
