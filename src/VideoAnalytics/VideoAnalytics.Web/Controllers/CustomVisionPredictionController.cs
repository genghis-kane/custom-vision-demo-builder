using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public async Task<VideoPredictionResponse> UploadVideoForPrediction([FromForm] IFormFile file, [FromForm] string projectName)
        {
            var frontEndRenderPath = string.Empty;
            var results = new List<PredictionResponse>();

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
                int frameStepMilliseconds = 300;
                int maxDurationMilliseconds = 10000;
                var extractedFrames = await _videoFrameExtractionService.SaveImageFrames(filePath, saveFramesTo, frameStepMilliseconds, maxDurationMilliseconds);

                results = (await _predictionService.GetPredictionsFromFrameList(extractedFrames.VideoFrames, projectName)).ToList();
            }

            return new VideoPredictionResponse
            {
                VideoFilePath = frontEndRenderPath,
                Predictions = results
            };
        }

        private string ReplaceBasePath(string fullFilePath)
        {
            var basePath = $"{_webHostEnvironment.ContentRootPath}\\{_systemSettings.WorkingDirectory}"; //cloud
            var path = fullFilePath.Replace(basePath, string.Empty);

            return path;
        }
    }
}
