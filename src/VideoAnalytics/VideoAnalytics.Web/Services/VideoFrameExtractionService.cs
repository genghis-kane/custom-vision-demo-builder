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
        public async Task<VideoFrameExtractionResponse> SaveImageFrames(string videoFile, string saveImagesTo, int frameStepMilliseconds, int maxDurationMilliseconds)
        {
            var extractedVideoFrames = new List<VideoFrame>();

            var videoFileName = Path.GetFileNameWithoutExtension(videoFile);
            var mediaInfo = await FFmpeg.GetMediaInfo(videoFile);

            var videoDurationMilliseconds = (int)mediaInfo.VideoStreams.First().Duration.TotalMilliseconds; //let's just assume not null

            var duration = (videoDurationMilliseconds <= maxDurationMilliseconds)
                ? videoDurationMilliseconds
                : maxDurationMilliseconds;

            for (var i = 0; i < duration; i++)
            {
                var fromMilliseconds = (i * frameStepMilliseconds);
                var fileName = $"{videoFileName}-{fromMilliseconds}";
                var filePath = Path.Combine(saveImagesTo, fileName + ".png");

                if (fromMilliseconds > duration) break;
                if (File.Exists(filePath))
                {
                    extractedVideoFrames.Add(new VideoFrame { Millisecond = fromMilliseconds, FilePath = filePath });
                    continue;
                }

                IConversion conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(videoFile, filePath, TimeSpan.FromMilliseconds(fromMilliseconds));
                IConversionResult result = await conversion.Start();

                extractedVideoFrames.Add(new VideoFrame { Millisecond = fromMilliseconds, FilePath = filePath });
            }

            return new VideoFrameExtractionResponse { Success = true, VideoFrames = extractedVideoFrames };
        }
    }
}
