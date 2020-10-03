using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VideoAnalytics.Web.Services.Interfaces;

namespace VideoAnalytics.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomVisionPredictionController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICustomVisionPredictionService _predictionService;
        private readonly ILogger<CustomVisionPredictionController> _logger;

        public CustomVisionPredictionController(
            IWebHostEnvironment webHostEnvironment,
            ICustomVisionPredictionService predictionService,
            ILogger<CustomVisionPredictionController> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _predictionService = predictionService;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        [Route("uploadvideo")]
        [DisableRequestSizeLimit]
        public async Task<string> UploadVideoForPrediction([FromForm] IFormFile file)
        {
            var frontEndRenderPath = string.Empty;
            if (file?.Length > 0)
            {
                string saveImagesTo = $"{_webHostEnvironment.ContentRootPath}\\ClientApp\\build\\videos"; //cloud
                // string saveImagesTo = $"{_webHostEnvironment.ContentRootPath}\\ClientApp\\public\\videos"; //local

                var fileName = $"{Guid.NewGuid()}-{file.FileName}";
                var filePath = Path.Combine(saveImagesTo, fileName);
                frontEndRenderPath = ReplaceBasePath(filePath);

                await using var stream = System.IO.File.Create(filePath);
                await file.CopyToAsync(stream);
            }

            return frontEndRenderPath;
        }

        private string ReplaceBasePath(string fullFilePath)
        {
            var basePath = $"{_webHostEnvironment.ContentRootPath}\\ClientApp\\build"; //cloud
            // var basePath = $"{_webHostEnvironment.ContentRootPath}\\ClientApp"; //local
            var path = fullFilePath.Replace(basePath, string.Empty);

            return path;
        }
    }
}
