using System.Collections.Generic;
using VideoAnalytics.Web.Configuration.Interfaces;
using VideoAnalytics.Web.Models;

namespace VideoAnalytics.Web.Configuration
{
    public class DemoSettings : IDemoSettings
    {
       public IEnumerable<DemoSetting> Settings { get; set; }
    }
}
