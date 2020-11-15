using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using VideoAnalytics.Web.Configuration.Interfaces;
using VideoAnalytics.Web.Models;
using VideoAnalytics.Web.Services.Interfaces;

namespace VideoAnalytics.Web.Services
{
    public class CustomVisionProjectService : ICustomVisionProjectService
    {
        private readonly ICustomVisionProjectSettings _projectSettings;
        private readonly CustomVisionTrainingClient _trainingApi;

        public CustomVisionProjectService(
            ICustomVisionProjectSettings projectSettings, 
            ICustomVisionServiceSettings serviceSettings)
        {
            _projectSettings = projectSettings;
            _trainingApi = new CustomVisionTrainingClient(new ApiKeyServiceClientCredentials(serviceSettings.AccountKey))
            {
                Endpoint = serviceSettings.AccountEndpoint
            };
        }

        public async Task<IEnumerable<string>> ListCustomVisionProjects()
        {
            IList<Project> projects = await _trainingApi.GetProjectsAsync();

            return projects.Select(project => project.Name).ToList();
        }

        public async Task<CustomVisionCreateProjectOperationResponse> GetOrCreateProjectByName(string projectName)
        {
            if (await ProjectExists(projectName))
            {
                return new CustomVisionCreateProjectOperationResponse { Success = true };
            }

            // Create project if it doesn't already exist
            var domains = await _trainingApi.GetDomainsAsync();
            var objectDetectionDomain = domains.FirstOrDefault(d => d.Type == _projectSettings.ProjectType);

            if (objectDetectionDomain != null)
            {
                var project = await _trainingApi.CreateProjectAsync(projectName, null, objectDetectionDomain.Id);
                return new CustomVisionCreateProjectOperationResponse { Success = true };
            }

            return new CustomVisionCreateProjectOperationResponse { Success = false };
        }

        public async Task<bool> ProjectExists(string projectName)
        {
            IList<Project> projects = await _trainingApi.GetProjectsAsync();
            return projects.Any(p => p.Name == projectName);
        }

        public async Task<Guid> GetProjectId(string projectName)
        {
            if (await ProjectExists(projectName))
            {
                IList<Project> projects = await _trainingApi.GetProjectsAsync();
                return projects.First(p => p.Name == projectName)?.Id ?? Guid.Empty;
            }
            
            return Guid.Empty;
        }

        public async Task<string> GetProjectCurrentPublishedModelName(string projectName)
        {
            if (await ProjectExists(projectName))
            {
                var projectId = await GetProjectId(projectName);
                var iterations = await _trainingApi.GetIterationsAsync(projectId);

                // Models don't seem to have an explicit published status, but PublishName is null on anything that isn't currently published.
                return iterations.OrderByDescending(i => i.TrainedAt).First(i => !string.IsNullOrEmpty(i.PublishName))?.PublishName ?? string.Empty;
            }

            return string.Empty;
        }
    }
}
