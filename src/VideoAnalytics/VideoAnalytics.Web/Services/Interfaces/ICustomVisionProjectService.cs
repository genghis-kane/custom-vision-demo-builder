using System;
using System.Threading.Tasks;
using VideoAnalytics.Web.Models;

namespace VideoAnalytics.Web.Services.Interfaces
{
    public interface ICustomVisionProjectService
    {
        Task<CustomVisionOperationResponse> GetOrCreateProject();
        Task<bool> ProjectExists(string projectName);
        Task<Guid> GetProjectId(string projectName);
    }
}