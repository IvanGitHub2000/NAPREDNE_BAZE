//using NBP_I.Models;
//using Microsoft.AspNetCore.Mvc;
//using Neo4jClient;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using StackExchange.Redis;
//namespace NBP_I.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class EmployeeController : ControllerBase
//    {

//        private readonly IGraphClient _client;
//        private IDatabase database;
//        public EmployeeController([FromServices] IGraphClient client, [FromServices] IDatabase db) : base()
//        {
//            _client = client;
//            database = db;

//        }

    
//        [HttpGet("{eid}/assignemployee/{did}/")]
//        public async Task<IActionResult> AssignDepartment(int did, int eid)
//        {

//            await _client.Cypher.Match("(d:Department), (e:Employee)")
//                                .Where((Department d, Employee e) => d.id == did && e.id == eid)
//                                .Create("(d)-[r:hasEmployee]->(e)")
//                                .ExecuteWithoutResultsAsync();

//            return Ok();
//        }

//        [HttpPost]
//        [Route("CreateEmployee/{emp}")]
//        public async Task<IActionResult> CreateEmployee([FromBody] Employee emp)
//        {
//            await _client.Cypher.Create("(e:Employee $emp)")
//                                .WithParam("emp", emp)
//                                .ExecuteWithoutResultsAsync();

//            return Ok();
//        }
//        [HttpPost]
//        public async Task<IActionResult> Create(string name)
//        {
//            await database.StringSetAsync("key", name);



//            return Ok(name);
//        }
        
//        [HttpGet]
//        [Route("GetIme/{name}")]
//        public async Task<IActionResult> Get(string name)
//        {
//            string value = await database.StringGetAsync("name");



//            if (value != null)
//            {
//                return Ok(value);
//            }
//            else
//            {
//                return NotFound();
//            }



//        }
//        [HttpGet]
//        [Route("GetSve")]
//        public async Task<IActionResult> GetSve()
//        {
//            string value = await database.StringGetRangeAsync("key",0,50);



//            if (value != null)
//            {
//                return Ok(value);
//            }
//            else
//            {
//                return NotFound();
//            }



//        }
//    }
//}

