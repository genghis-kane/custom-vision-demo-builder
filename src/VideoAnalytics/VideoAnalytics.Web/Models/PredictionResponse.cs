using System;
using System.Collections.Generic;

namespace VideoAnalytics.Web.Models
{
    public class PredictionResponse
    {
        public DateTime Timestamp { get; set; }
        public int Millisecond { get; set; }
        public IEnumerable<PredictionObjectResponse> PredictionObjects { get; set; }
    }
}