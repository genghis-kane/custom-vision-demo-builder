using VideoAnalytics.Web.Configuration.Interfaces;
using VideoAnalytics.Web.Services.Interfaces;

namespace VideoAnalytics.Web.Services
{
    public class CustomVisionPredictionService : ICustomVisionPredictionService
    {
        private readonly ICustomVisionServiceSettings _settings;

        public CustomVisionPredictionService(ICustomVisionServiceSettings settings)
        {
            _settings = settings;
        }
    }
}
