using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideoAnalytics.Web.Configuration.Interfaces;
using VideoAnalytics.Web.Services.Interfaces;

namespace VideoAnalytics.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomVisionPredictionController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICustomVisionPredictionService _predictionService;
        private readonly IVideoFrameExtractionService _videoFrameExtractionService;
        private readonly ISystemSettings _systemSettings;

        public CustomVisionPredictionController(
            IWebHostEnvironment webHostEnvironment,
            ICustomVisionPredictionService predictionService,
            IVideoFrameExtractionService videoFrameExtractionService,
            ISystemSettings systemSettings)
        {
            _webHostEnvironment = webHostEnvironment;
            _predictionService = predictionService;
            _videoFrameExtractionService = videoFrameExtractionService;
            _systemSettings = systemSettings;
        }

        [HttpPost]
        [Route("uploadvideo")]
        [DisableRequestSizeLimit]
        public async Task<string> UploadVideoForPrediction([FromForm] IFormFile file)
        {
            var frontEndRenderPath = string.Empty;
            if (file?.Length > 0)
            {
                // 1. Upload video to file system
                string saveVideoTo = $"{_webHostEnvironment.ContentRootPath}\\{_systemSettings.WorkingDirectory}\\videos\\prediction";

                var fileName = $"{Guid.NewGuid()}-{file.FileName}";
                var filePath = Path.Combine(saveVideoTo, fileName);
                frontEndRenderPath = ReplaceBasePath(filePath);

                await using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                // 2. Extract image frames to send for prediction
                string saveFramesTo = $"{_webHostEnvironment.ContentRootPath}\\{_systemSettings.WorkingDirectory}\\frames\\prediction";
                await _videoFrameExtractionService.SaveImageFramesMilliseconds(filePath, saveFramesTo, 300, 10000);
            }

            return frontEndRenderPath;
        }

        private string ReplaceBasePath(string fullFilePath)
        {
            var basePath = $"{_webHostEnvironment.ContentRootPath}\\{_systemSettings.WorkingDirectory}"; //cloud
            var path = fullFilePath.Replace(basePath, string.Empty);

            return path;
        }
    }
}
