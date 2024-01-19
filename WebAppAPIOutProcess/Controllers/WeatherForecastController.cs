using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Text;

namespace WebAppAPIOutProcess.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        private const int ServerPort = 12345; // Change this to your actual server port

        [HttpPost(Name = "CallTcpSocket")]
        public IActionResult CallTcpSocket()
        {
            try
            {
                string requestData = "req";
                string response = CoreBankingInterface.ProcessRequest(requestData);

                // Your actual implementation logic goes here

                return Ok(new { Message = response });
            }
            catch (Exception ex)
            {
                // Handle exceptions here, log or return an appropriate response
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }
    }
    public static class CoreBankingInterface
    {
        public static string ProcessRequest(string requestData)
        {
            // Your logic to process the request and prepare data for TCP communication
            string dataToSend = SerializeRequestData(requestData);

            // Simulating TCP socket communication
            using (TcpClient tcpClient = new TcpClient("localhost", 80))
            {
                using (NetworkStream networkStream = tcpClient.GetStream())
                {
                    // Send data to the server
                    byte[] dataBytes = Encoding.UTF8.GetBytes(dataToSend);
                    networkStream.Write(dataBytes, 0, dataBytes.Length);

                    // Receive response from the server
                    using (StreamReader reader = new StreamReader(networkStream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        private static string SerializeRequestData(string requestData)
        {
            // Your logic to serialize the request data to a string
            // For example, you can use a JSON serializer
            // Replace this with your actual serialization logic
            return Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
        }
    }
}