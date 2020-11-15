using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using VideoAnalytics.Web.Configuration;
using VideoAnalytics.Web.Services;

namespace VideoAnalytics.Web.Tests
{
    [TestFixture]
    public class CustomVisionProjectServiceTests
    {
        private CustomVisionProjectSettings _projectSettings;
        private CustomVisionProjectService _customVisionProjectService;

        [SetUp]
        public void Setup()
        {
            var config = ConfigurationHelper.InitConfig();
            _projectSettings = new CustomVisionProjectSettings
            {
                ProjectName = config.GetValue<string>("CustomVision:Project:Name"),
                ProjectType = config.GetValue<string>("CustomVision:Project:Type")
            };

            var authoringSettings = new CustomVisionAuthoringSettings
            {
                AccountRegion = config.GetValue<string>("CustomVision:AuthoringService:AccountRegion"),
                AccountName = config.GetValue<string>("CustomVision:AuthoringService:AccountName"),
                AccountKey = config.GetValue<string>("CustomVision:AuthoringService:AccountKey")
            };

            _customVisionProjectService = new CustomVisionProjectService(_projectSettings, authoringSettings);
        }

        [Test]
        public async Task CanListExistingProjects()
        {
            var projects = await _customVisionProjectService.ListCustomVisionProjects();
            Assert.IsNotEmpty(projects);
        }

        [Test]
        public async Task CanGetExpectedExistsStatusOfExistingProject()
        {
            var exists = await _customVisionProjectService.ProjectExists(_projectSettings.ProjectName);
            Assert.True(exists);
        }

        [Test]
        public async Task CanGetExpectedExistsStatusOfNonExistingProject()
        {
            var exists = await _customVisionProjectService.ProjectExists("RandomProjectName");
            Assert.False(exists);
        }

        [Test]
        public async Task CanGetKnownProjectId()
        {
            var projectId = await _customVisionProjectService.GetProjectId(_projectSettings.ProjectName);
            Assert.NotNull(projectId);
        }

        [Test]
        public async Task CanGetCurrentPublishedModelInformation()
        {
            var publishedModelName = await _customVisionProjectService.GetProjectCurrentPublishedModelName(_projectSettings.ProjectName);
            Assert.NotNull(publishedModelName);
        }
    }
}