using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBP_I.Models;
namespace NBP_I.Controllers
{
    public class ChatController:Controller
    {

        private readonly IDriver driver;
        private IDatabase database;
        public ChatController(IDriver _driver,IDatabase d)
        {
            driver = _driver;
            database = d;
        }


        [HttpPost]
        public async Task<IActionResult> LikeUser(int userId)
        {
            IAsyncSession session = driver.AsyncSession();
            try
            {
                var result = await session.RunAsync($"MATCH (u:User) WHERE id(u) = {userId} RETURN u.username");
                var item = await result.SingleAsync();
                var userName = item.Values[item.Keys[0]].ToString();
                //RedisManager<UserInfo>.IncrementSortedSet($"leaderboard", new(userId, userName));
               
                database.SortedSetAdd("leaderboard", userName, 1000);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = e.Message });
            }
            finally
            {
                await session.CloseAsync();
            }
            /*using Microsoft.AspNetCore.Mvc; CHATGPT ZA LEADERBOARD
using StackExchange.Redis;

namespace MyWebApplication
{
    public class LeaderboardController : Controller
    {
        private readonly ConnectionMultiplexer redis;

        public LeaderboardController(ConnectionMultiplexer redis)
        {
            this.redis = redis;
        }

        [HttpGet]
        [Route("api/leaderboard")]
        public async Task<ActionResult> GetLeaderboard()
        {
            IDatabase database = redis.GetDatabase();

            // Get the top 10 members of the leaderboard
            SortedSetEntry[] topMembers = database.SortedSetRangeByRankWithScores("leaderboard", 0, 9, Order.Descending);

            // Return the top members as a JSON array
            return Json(topMembers);
        }

        [HttpPost]
        [Route("api/leaderboard")]
        public async Task<ActionResult> AddToLeaderboard(string member, double score)
        {
            IDatabase database = redis.GetDatabase();

            // Add the member to the leaderboard
            bool success = database.SortedSetAdd("leaderboard", member, score);

            // Return a success or error result
            if (success)
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}*/
        }
    }
}
