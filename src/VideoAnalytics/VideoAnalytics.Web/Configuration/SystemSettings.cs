using VideoAnalytics.Web.Configuration.Interfaces;

namespace VideoAnalytics.Web.Configuration
{
    public class SystemSettings : ISystemSettings
    {
        public string WorkingDirectory { get; set; }
    }
}