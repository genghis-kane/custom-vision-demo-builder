using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using VideoAnalytics.Web.Configuration;
using VideoAnalytics.Web.Configuration.Interfaces;
using VideoAnalytics.Web.Models;
using VideoAnalytics.Web.Services.Interfaces;

namespace VideoAnalytics.Web.Services
{
    public class CustomVisionAuthoringService : ICustomVisionAuthoringService
    {
        private readonly ICustomVisionProjectSettings _projectSettings;
        private readonly CustomVisionTrainingClient _trainingApi;

        public CustomVisionAuthoringService(
            ICustomVisionProjectSettings projectSettings, 
            ICustomVisionServiceSettings serviceSettings)
        {
            _projectSettings = projectSettings;
            _trainingApi = new CustomVisionTrainingClient(new ApiKeyServiceClientCredentials(serviceSettings.AccountKey))
            {
                Endpoint = serviceSettings.AccountEndpoint
            };
        }

        public async Task<CustomVisionOperationResponse> GetOrCreateProject()
        {
            if (await ProjectExists(_projectSettings.ProjectName))
            {
                return new CustomVisionOperationResponse { Success = true };
            }

            // Create project if it doesn't already exist
            var domains = await _trainingApi.GetDomainsAsync();
            var objectDetectionDomain = domains.FirstOrDefault(d => d.Type == _projectSettings.ProjectType);

            if (objectDetectionDomain != null)
            {
                var project = await _trainingApi.CreateProjectAsync(_projectSettings.ProjectName, null, objectDetectionDomain.Id);
                return new CustomVisionOperationResponse { Success = true };
            }

            return new CustomVisionOperationResponse { Success = false };
        }

        private async Task<bool> ProjectExists(string projectName)
        {
            IList<Project> projects = await _trainingApi.GetProjectsAsync();
            return projects.Any(p => p.Name == projectName);
        }
    }
}
