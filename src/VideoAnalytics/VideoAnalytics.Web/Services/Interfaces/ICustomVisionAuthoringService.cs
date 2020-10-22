using System.Collections.Generic;
using System.Threading.Tasks;

namespace VideoAnalytics.Web.Services.Interfaces
{
    public interface ICustomVisionAuthoringService
    {
        Task PublishImages(IList<string> imageFilePaths);
    }
}