using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VideoAnalytics.Web.Services.Interfaces;

namespace VideoAnalytics.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomVisionAuthoringController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ICustomVisionAuthoringService _authoringService;
        private readonly ILogger<CustomVisionAuthoringController> _logger;

        public CustomVisionAuthoringController(
            ICustomVisionAuthoringService authoringService, 
            ILogger<CustomVisionAuthoringController> logger)
        {
            _authoringService = authoringService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var response = await _authoringService.GetOrCreateProject();

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
