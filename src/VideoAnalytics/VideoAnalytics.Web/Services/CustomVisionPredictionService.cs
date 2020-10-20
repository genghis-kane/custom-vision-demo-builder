using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using VideoAnalytics.Web.Configuration.Interfaces;
using VideoAnalytics.Web.Services.Interfaces;

namespace VideoAnalytics.Web.Services
{
    public class CustomVisionPredictionService : ICustomVisionPredictionService
    {
        private readonly CustomVisionPredictionClient _predictionApi;

        public CustomVisionPredictionService(ICustomVisionServiceSettings settings)
        {
            _predictionApi = new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(settings.AccountKey))
            {
                Endpoint = settings.AccountEndpoint
            };
        }
    }
}
