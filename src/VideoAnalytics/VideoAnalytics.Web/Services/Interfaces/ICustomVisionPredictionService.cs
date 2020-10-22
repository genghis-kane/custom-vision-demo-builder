using System.Collections.Generic;
using System.Threading.Tasks;
using VideoAnalytics.Web.Models;

namespace VideoAnalytics.Web.Services.Interfaces
{
    public interface ICustomVisionPredictionService
    {
        Task<IEnumerable<PredictionResponse>> GetPredictionsFromFrameList(IEnumerable<VideoFrame> requests);
    }
}