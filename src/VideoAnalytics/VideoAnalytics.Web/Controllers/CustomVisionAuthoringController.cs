using System.Collections.Generic;
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
        public async Task<IEnumerable<string>> Get()
        {
//            var img1 = "frames/0d6efcbb-7e17-4e3f-ab47-5cd461964df9.png";
//            var img2 = "frames/174a6bed-929f-4959-84bd-76d9d28626ab.png";
//            var img3 = "frames/31374798-fc75-431a-ac64-2a216479394c.png";
//
//            return new List<string> { img1, img2, img3 };


            var response = await _authoringService.GetOrCreateProject();

            // Ideally this would move to blob storage, but I'm not sure the ffmpeg library will handle it
            string videoFile = $"{_webHostEnvironment.ContentRootPath}\\ClientApp\\videos\\training-video.mp4";
            string saveImagesTo = $"{_webHostEnvironment.ContentRootPath}\\ClientApp\\build\\frames";
            int frameStep = 15;
            int maxFrames = 50;

            var result = await _videoFrameExtractionService.SaveImageFrames(videoFile, saveImagesTo, frameStep, maxFrames);

            var paths = new List<string>();
            foreach (var imageFilePath in result.ImageFilePaths)
            {
                var basePath = $"{_webHostEnvironment.ContentRootPath}\\ClientApp\\build";
                var path = imageFilePath.Replace(basePath, string.Empty);
                paths.Add(path);
            }

            return paths.ToArray();
        }
    }
}
