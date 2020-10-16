using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using VideoAnalytics.Common.Services.Interfaces;
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
        private readonly IAzureBlobStorageService _blobStorageService;

        public CustomVisionAuthoringController(
            IWebHostEnvironment webHostEnvironment,
            ICustomVisionAuthoringService authoringService, 
            IVideoFrameExtractionService videoFrameExtractionService,
            IAzureBlobStorageService blobStorageService)
        {
            _webHostEnvironment = webHostEnvironment;
            _authoringService = authoringService;
            _videoFrameExtractionService = videoFrameExtractionService;
            _blobStorageService = blobStorageService;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            // var img1 = "frames/0d6efcbb-7e17-4e3f-ab47-5cd461964df9.png";
            // var img2 = "frames/174a6bed-929f-4959-84bd-76d9d28626ab.png";
            // var img3 = "frames/31374798-fc75-431a-ac64-2a216479394c.png";
            //
            // return new List<string> { img1, img2, img3 };

            
            var response = await _authoringService.GetOrCreateProject();
            
            // Ideally this would move to blob storage, but I'm not sure the ffmpeg library will handle it
            string videoFileName = "training-video.mp4";
            string videoFile = $"{_webHostEnvironment.ContentRootPath}\\ClientApp\\videos\\{videoFileName}";
            string saveImagesTo = $"{_webHostEnvironment.ContentRootPath}\\ClientApp\\build\\frames";
            int frameStep = 15;
            int maxFrames = 50;
            
            var result = await _videoFrameExtractionService.SaveImageFrames(videoFile, saveImagesTo, frameStep, maxFrames);
            
            // Once frames are extracted - move them to blob storage
            foreach (var imageFilePath in result.ImageFilePaths)
            {
                await _blobStorageService.UploadFileToContainer("trainingvideo", imageFilePath);
            }

            var paths = new List<string>();
            foreach (var imageFilePath in result.ImageFilePaths)
            {
                var basePath = $"{_webHostEnvironment.ContentRootPath}\\ClientApp\\build";
                var path = imageFilePath.Replace(basePath, string.Empty);
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
                var basePath = $"{_webHostEnvironment.ContentRootPath}\\ClientApp\\build\\"; //this will be different between local and published, what a pain
                var path = $"{basePath}{imageFilePath}";
                paths.Add(path);
            }

            await _authoringService.PublishImages(paths);
        }
    }
}
