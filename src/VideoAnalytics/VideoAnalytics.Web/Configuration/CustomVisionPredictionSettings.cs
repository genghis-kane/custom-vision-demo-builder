using VideoAnalytics.Web.Configuration.Interfaces;

namespace VideoAnalytics.Web.Configuration
{
    public class CustomVisionPredictionSettings : ICustomVisionServiceSettings
    {
        public string AccountRegion { get; set; }
        public string AccountName { get; set; }
        public string AccountEndpoint => $"https://{AccountName}.cognitiveservices.azure.com/";
        public string AccountKey { get; set; }
    }
}