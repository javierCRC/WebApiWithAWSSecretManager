using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiHandsOn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KralizekAWSSecretManagerController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public KralizekAWSSecretManagerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("mysql-connection")]
        public IActionResult GetMySqlConnection()
        {
            var connString = _configuration["ConnectionString:MySQLDBCloud"];
            if (string.IsNullOrEmpty(connString))
                return NotFound("Connection string not found.");

            return Ok(new { MySQLDBConnectionString = connString });
        }

        [HttpGet("say-myname")]
        public IActionResult GetMyName()
        {
            var value = _configuration["Say:MyName"];
            if (string.IsNullOrEmpty(value))
                return NotFound("Say my name not found.");

            return Ok(new { SayMyName = value });
        }

        [HttpGet("testmyname")]
        public IActionResult GetMyNameTwo()
        {
            var value = _configuration["TestMyName"];
            if (string.IsNullOrEmpty(value))
                return NotFound("TestMyName not found.");

            return Ok(new { TestMyName = value });
        }

        [HttpGet("mysqlConnection")]
        public IActionResult GetMySqlConnectionTwo()
        {
            var value = _configuration["ConnectionToMySQLDBCloud"];
            if (string.IsNullOrEmpty(value))
                return NotFound("ConnectionToMySQLDBCloud not found.");

            return Ok(new { ConnectionToMySQLDBCloud = value });
        }
    }
}
