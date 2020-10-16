using System.Threading.Tasks;

namespace VideoAnalytics.Common.Services.Interfaces
{
    /// <summary>
    /// Responsible for blob storage CRUD operations.
    /// </summary>
    public interface IAzureBlobStorageService
    {
        /// <summary>
        /// Create the specified container.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        Task CreateContainer(string containerName);

        /// <summary>
        /// Delete the specified container and all of its contents.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        Task DeleteContainer(string containerName);

        /// <summary>
        /// Insert the file into the specified container.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fullFileName">The full path to the local file.</param>
        /// <param name="containerGroup">The optional folder within the container to save the file to.</param>
        Task UploadFileToContainer(string containerName, string fullFileName, string containerGroup = "");
    }
}