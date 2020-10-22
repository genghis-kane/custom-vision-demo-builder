using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using VideoAnalytics.Web.Configuration.Interfaces;
using VideoAnalytics.Web.Services.Interfaces;

namespace VideoAnalytics.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomVisionAuthoringController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICustomVisionProjectService _projectService;
        private readonly ICustomVisionAuthoringService _authoringService;
        private readonly IVideoFrameExtractionService _videoFrameExtractionService;
        private readonly ISystemSettings _systemSettings;

        public CustomVisionAuthoringController(
            IWebHostEnvironment webHostEnvironment,
            ICustomVisionProjectService projectService, 
            ICustomVisionAuthoringService authoringService, 
            IVideoFrameExtractionService videoFrameExtractionService,
            ISystemSettings systemSettings)
        {
            _webHostEnvironment = webHostEnvironment;
            _projectService = projectService;
            _authoringService = authoringService;
            _videoFrameExtractionService = videoFrameExtractionService;
            _systemSettings = systemSettings;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            // var img1 = "frames/0d6efcbb-7e17-4e3f-ab47-5cd461964df9.png";
            // var img2 = "frames/174a6bed-929f-4959-84bd-76d9d28626ab.png";
            // var img3 = "frames/31374798-fc75-431a-ac64-2a216479394c.png";
            //
            // return new List<string> { img1, img2, img3 };

            
            var response = await _projectService.GetOrCreateProject();
            
            // Ideally this would move to blob storage, but I'm not sure the ffmpeg library will handle it
            string videoFile = $"{_webHostEnvironment.ContentRootPath}\\{_systemSettings.WorkingDirectory}\\videos\\training\\training-video.mp4";
            string saveImagesTo = $"{_webHostEnvironment.ContentRootPath}\\{_systemSettings.WorkingDirectory}\\frames\\training";
            int frameStep = 15000;
            int maxFrames = 150000;
            
            var result = await _videoFrameExtractionService.SaveImageFrames(videoFile, saveImagesTo, frameStep, maxFrames);
            
            var paths = new List<string>();
            foreach (var videoFrame in result.VideoFrames)
            {
                var basePath = $"{_webHostEnvironment.ContentRootPath}\\{_systemSettings.WorkingDirectory}";
                var path = videoFrame.FilePath.Replace(basePath, string.Empty);
                paths.Add(path);
            }
            
            return paths.ToArray();
        }

        [HttpPost]
        [Route("uploadvideoframes")]
        public async Task UploadVideoFrames([FromBody] IList<string> videoFrames)
        {
            //TODO
            var paths = new List<string>();
            foreach (var imageFilePath in videoFrames)
            {
                var basePath = $"{_webHostEnvironment.ContentRootPath}\\{_systemSettings.WorkingDirectory}";
                var path = $"{basePath}{imageFilePath}";
                paths.Add(path);
            }

            await _authoringService.PublishImages(paths);
        }
    }
}
