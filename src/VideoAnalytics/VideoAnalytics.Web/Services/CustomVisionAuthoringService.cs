using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
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

        public async Task PublishImages(IList<string> imageFilePaths)
        {
            var imageFileEntries = new List<ImageFileCreateEntry>();
            foreach (var imageFilePath in imageFilePaths)
            {
                var fileName = Path.GetFileNameWithoutExtension(imageFilePath);
                var entry = new ImageFileCreateEntry(fileName, await File.ReadAllBytesAsync(imageFilePath));
                imageFileEntries.Add(entry);
            }

            var projectId = await GetProjectId(_projectSettings.ProjectName);
            await _trainingApi.CreateImagesFromFilesAsync(projectId, new ImageFileCreateBatch(imageFileEntries));

            // TODO - validations
            //64 item limit on publish batches
            //6MB limit on file sizes
        }

        private async Task<bool> ProjectExists(string projectName)
        {
            IList<Project> projects = await _trainingApi.GetProjectsAsync();
            return projects.Any(p => p.Name == projectName);
        }

        private async Task<Guid> GetProjectId(string projectName)
        {
            if (await ProjectExists(projectName))
            {
                IList<Project> projects = await _trainingApi.GetProjectsAsync();
                return projects.First(p => p.Name == projectName).Id;
            }
            
            return Guid.Empty;
        }
    }
}
