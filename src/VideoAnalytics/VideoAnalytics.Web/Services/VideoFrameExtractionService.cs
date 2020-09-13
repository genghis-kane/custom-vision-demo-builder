using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VideoAnalytics.Web.Services.Interfaces;
using Xabe.FFmpeg;

namespace VideoAnalytics.Web.Services
{
    public class VideoFrameExtractionService : IVideoFrameExtractionService
    {
        public async Task SaveImageFrames(string videoFile, string saveImagesTo, int frameStepSeconds, int maxFrames)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(videoFile);
            var duration = (int)mediaInfo.VideoStreams.FirstOrDefault()?.Duration.TotalSeconds;

            for (int i = 0; i < maxFrames; i++)
            {
                string output = Path.Combine(saveImagesTo, Guid.NewGuid() + ".png");
                int fromSeconds = (i * frameStepSeconds);

                if (fromSeconds > duration) break;

                IConversion conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(videoFile, output, TimeSpan.FromSeconds(fromSeconds));
                IConversionResult result = await conversion.Start();
            }
        }
    }
}
