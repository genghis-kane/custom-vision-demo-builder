using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoAnalytics.Web.Models;

namespace VideoAnalytics.Web.Services.Interfaces
{
    public interface ICustomVisionProjectService
    {
        Task<IEnumerable<string>> ListCustomVisionProjects();
        Task<CustomVisionCreateProjectOperationResponse> GetOrCreateProjectByName(string projectName);
        Task<bool> ProjectExists(string projectName);
        Task<Guid> GetProjectId(string projectName);
        Task<string> GetProjectCurrentPublishedModelName(string projectName);
    }
}