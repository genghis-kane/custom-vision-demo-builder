using System.Collections.Generic;
using VideoAnalytics.Web.Models;

namespace VideoAnalytics.Web.Configuration.Interfaces
{
    public interface IDemoSettings
    {
        IEnumerable<DemoSetting> Settings { get; set; }
    }
}