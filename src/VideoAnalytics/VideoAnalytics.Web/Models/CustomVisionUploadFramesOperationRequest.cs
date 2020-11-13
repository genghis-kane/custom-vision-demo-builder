using System.Collections.Generic;

namespace VideoAnalytics.Web.Models
{
    public class CustomVisionUploadFramesOperationRequest
    {
        public string ProjectName { get; set; }

        public IList<string> Frames { get; set; }
    }
}
