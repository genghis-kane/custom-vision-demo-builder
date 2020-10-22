using System.Collections.Generic;

namespace VideoAnalytics.Web.Models
{
    public class VideoPredictionResponse
    {
        public string VideoFilePath { get; set; }
        public IEnumerable<PredictionResponse> Predictions { get; set; }
    }
}