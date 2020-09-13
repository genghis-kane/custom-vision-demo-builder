using System.Threading.Tasks;

namespace VideoAnalytics.Web.Services.Interfaces
{
    public interface IVideoFrameExtractionService
    {
        Task SaveImageFrames(string videoFile, string saveImagesTo, int frameStepSeconds, int maxFrames);
    }
}