using System.Collections.Generic;
using System.Threading.Tasks;
using VideoAnalytics.Web.Models;

namespace VideoAnalytics.Web.Services.Interfaces
{
    public interface ICustomVisionAuthoringService
    {
        Task<CustomVisionOperationResponse> GetOrCreateProject();

        Task PublishImages(IList<string> imageFilePaths);
    }
}