using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using VideoAnalytics.Web.Configuration.Interfaces;
using VideoAnalytics.Web.Models;
using VideoAnalytics.Web.Services.Interfaces;

namespace VideoAnalytics.Web.Services
{
    public class CustomVisionPredictionService : ICustomVisionPredictionService
    {
        private readonly ICustomVisionProjectService _projectService;
        private readonly ICustomVisionProjectSettings _projectSettings;
        private readonly CustomVisionPredictionClient _predictionApi;

        public CustomVisionPredictionService(
            ICustomVisionProjectService projectService,
            ICustomVisionProjectSettings projectSettings, 
            ICustomVisionServiceSettings settings)
        {
            _projectService = projectService;
            _projectSettings = projectSettings;

            _predictionApi = new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(settings.AccountKey))
            {
                Endpoint = settings.AccountEndpoint
            };
        }

        public async Task<IEnumerable<PredictionResponse>> GetPredictionsFromFrameList(IEnumerable<VideoFrame> requests)
        {
            var predictionResponses = new List<PredictionResponse>();

            var projectId = await _projectService.GetProjectId(_projectSettings.ProjectName);
            foreach (var request in requests)
            {
                var predictionResponse = await GetFramePrediction(request, projectId);
                predictionResponses.Add(predictionResponse);
            }

            return predictionResponses;
        }

        private async Task<PredictionResponse> GetFramePrediction(VideoFrame request, Guid projectId)
        {
            var predictionResponse = new PredictionResponse { Timestamp = DateTime.Now, Millisecond = request.Millisecond };

            using (var stream = File.OpenRead(request.FilePath))
            {
                var response = await _predictionApi.DetectImageWithHttpMessagesAsync(
                    projectId,
                    "Iteration2",
                    stream
                );

                var objects = new List<PredictionObjectResponse>();
                foreach (var prediction in response.Body.Predictions)
                {
                    var predictionObjectResponse = new PredictionObjectResponse
                    {
                        Label = prediction.TagName,
                        Confidence = prediction.Probability,
                        BoundingBox = new BoundingBox
                        {
                            Height = prediction.BoundingBox.Height,
                            Width = prediction.BoundingBox.Width,
                            Top = prediction.BoundingBox.Top,
                            Left = prediction.BoundingBox.Left
                        }
                    };

                    if (predictionObjectResponse.Confidence > 0.7)
                    {
                        objects.Add(predictionObjectResponse);
                    }
                }

                predictionResponse.PredictionObjects = objects;
            }

            return predictionResponse;
        }
    }
}
