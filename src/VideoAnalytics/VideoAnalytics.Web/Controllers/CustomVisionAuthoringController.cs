using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideoAnalytics.Web.Configuration.Interfaces;
using VideoAnalytics.Web.Models;
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
        [Route("listcvprojects")]
        public async Task<IEnumerable<string>> ListCustomVisionProjects()
        {
            return await _projectService.ListCustomVisionProjects();
        }

        [HttpPost]
        [Route("createcvproject")]
        public async Task<CustomVisionOperationResponse> CreateCustomVisionProject([FromBody] CustomVisionOperationRequest request)
        {
            return await _projectService.GetOrCreateProjectByName(request.ProjectName);
        }

        [HttpPost]
        [Route("uploadvideo")]
        [DisableRequestSizeLimit]
        public async Task<IEnumerable<string>> UploadVideo([FromForm]IFormFile file, [FromForm] int frameStepMilliseconds, [FromForm] int maxDurationMilliseconds)
        {
            var img1 = "frames/0d6efcbb-7e17-4e3f-ab47-5cd461964df9.png";
            var img2 = "frames/174a6bed-929f-4959-84bd-76d9d28626ab.png";
            var img3 = "frames/31374798-fc75-431a-ac64-2a216479394c.png";
            
            return new List<string> { img1, img2, img3 };

            var paths = new List<string>();

            if (file?.Length > 0)
            {
                // TODO - probably want a separate area for project management
                // var response = await _projectService.GetOrCreateProject();

                // 1. Upload video to file system
                string saveVideoTo = $"{_webHostEnvironment.ContentRootPath}\\{_systemSettings.WorkingDirectory}\\videos\\training";

                var fileName = $"{Guid.NewGuid()}-{file.FileName}";
                var filePath = Path.Combine(saveVideoTo, fileName);

                await using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                string saveFramesTo = $"{_webHostEnvironment.ContentRootPath}\\{_systemSettings.WorkingDirectory}\\frames\\training";
                var extractedFrames = await _videoFrameExtractionService.SaveImageFrames(filePath, saveFramesTo, frameStepMilliseconds, maxDurationMilliseconds);

                foreach (var videoFrame in extractedFrames.VideoFrames)
                {
                    var basePath = $"{_webHostEnvironment.ContentRootPath}\\{_systemSettings.WorkingDirectory}";
                    var path = videoFrame.FilePath.Replace(basePath, string.Empty);
                    paths.Add(path);
                }
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
