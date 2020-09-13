using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICustomVisionAuthoringService _authoringService;
        private readonly IVideoFrameExtractionService _videoFrameExtractionService;

        public CustomVisionAuthoringController(
            IWebHostEnvironment webHostEnvironment,
            ICustomVisionAuthoringService authoringService, 
            IVideoFrameExtractionService videoFrameExtractionService)
        {
            _webHostEnvironment = webHostEnvironment;
            _authoringService = authoringService;
            _videoFrameExtractionService = videoFrameExtractionService;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var response = await _authoringService.GetOrCreateProject();

            // Ideally this would move to blob storage, but I'm not sure the ffmpeg library will handle it
            string videoFile = $"{_webHostEnvironment.ContentRootPath}\\ClientApp\\videos\\training-video.mp4";
            string saveImagesTo = $"{_webHostEnvironment.ContentRootPath}\\ClientApp\\frames";
            int frameStep = 15;
            int maxFrames = 50;

            await _videoFrameExtractionService.SaveImageFrames(videoFile, saveImagesTo, frameStep, maxFrames);

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
