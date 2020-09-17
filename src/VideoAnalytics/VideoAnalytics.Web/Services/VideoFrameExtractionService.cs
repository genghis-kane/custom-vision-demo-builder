using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VideoAnalytics.Web.Models;
using VideoAnalytics.Web.Services.Interfaces;
using Xabe.FFmpeg;

namespace VideoAnalytics.Web.Services
{
    public class VideoFrameExtractionService : IVideoFrameExtractionService
    {
        public async Task<VideoFrameExtractionResponse> SaveImageFrames(string videoFile, string saveImagesTo, int frameStepSeconds, int maxFrames)
        {
            var imageFilePaths = new List<string>();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(videoFile);
            var duration = (int)mediaInfo.VideoStreams.First().Duration.TotalSeconds; //let's just assume not null

            for (int i = 0; i < maxFrames; i++)
            {
                string output = Path.Combine(saveImagesTo, Guid.NewGuid() + ".png");
                int fromSeconds = (i * frameStepSeconds);

                if (fromSeconds > duration) break;

                IConversion conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(videoFile, output, TimeSpan.FromSeconds(fromSeconds));
                IConversionResult result = await conversion.Start();

                imageFilePaths.Add(output);
            }

            return new VideoFrameExtractionResponse { Success = true, ImageFilePaths = imageFilePaths };
        }
    }
}
