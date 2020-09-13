using Autofac;
using VideoAnalytics.Web.Configuration;
using VideoAnalytics.Web.Services;
using VideoAnalytics.Web.Services.Interfaces;

namespace VideoAnalytics.Web.Modules
{
    public class CustomVisionDependencyRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(ctx =>
            {
                var projectSettings = new CustomVisionProjectSettings
                {
                    ProjectName = "",
                    ProjectType = ""
                };

                var serviceSettings = new CustomVisionAuthoringSettings
                {
                    AccountRegion = "",
                    AccountName = "",
                    AccountKey = ""
                };

                return new CustomVisionAuthoringService(projectSettings, serviceSettings);

            }).As<ICustomVisionAuthoringService>();

            builder.Register(ctx =>
            {
                var serviceSettings = new CustomVisionPredictionSettings
                {
                    AccountRegion = "",
                    AccountName = "",
                    AccountKey = ""
                };

                return new CustomVisionPredictionService(serviceSettings);

            }).As<ICustomVisionAuthoringService>();
        }
    }
}
