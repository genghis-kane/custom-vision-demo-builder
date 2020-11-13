using Autofac;
using VideoAnalytics.Web.Configuration;
using VideoAnalytics.Web.Configuration.Interfaces;
using VideoAnalytics.Web.Services;
using VideoAnalytics.Web.Services.Interfaces;

namespace VideoAnalytics.Web.Modules
{
    public class CustomVisionDependencyRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var projectSettings = new CustomVisionProjectSettings
            {
                ProjectName = "",
                ProjectType = ""
            };

            var authoringSettings = new CustomVisionAuthoringSettings
            {
                AccountRegion = "",
                AccountName = "",
                AccountKey = ""
            };

            var predictionSettings = new CustomVisionAuthoringSettings
            {
                AccountRegion = "",
                AccountName = "",
                AccountKey = ""
            };

            var customVisionProjectService = new CustomVisionProjectService(projectSettings, authoringSettings);
            builder.Register(ctx => customVisionProjectService).As<ICustomVisionProjectService>();

            builder.Register(ctx => new CustomVisionAuthoringService(customVisionProjectService, authoringSettings)).As<ICustomVisionAuthoringService>();

            builder.Register(ctx => new CustomVisionPredictionService(customVisionProjectService, projectSettings, predictionSettings)).As<ICustomVisionPredictionService>();

            builder.RegisterType<VideoFrameExtractionService>().As<IVideoFrameExtractionService>();
        }
    }
}
