using VideoAnalytics.Web.Configuration.Interfaces;

namespace VideoAnalytics.Web.Configuration
{
    public class CustomVisionProjectSettings : ICustomVisionProjectSettings
    {
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
    }
}
