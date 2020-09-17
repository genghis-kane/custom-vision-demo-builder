using System.Collections.Generic;

namespace VideoAnalytics.Web.Models
{
    public class VideoFrameExtractionResponse
    {
        public bool Success { get; set; }

        public IList<string> ImageFilePaths { get; set; }
    }
}
