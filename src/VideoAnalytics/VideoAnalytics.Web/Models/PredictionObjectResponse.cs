namespace VideoAnalytics.Web.Models
{
    public class PredictionObjectResponse
    {
        public string Label { get; set; }
        public double Confidence { get; set; }
        public BoundingBox BoundingBox { get; set; }
    }
}