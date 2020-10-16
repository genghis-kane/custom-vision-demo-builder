using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using VideoAnalytics.Common.Services.Interfaces;

namespace VideoAnalytics.Common.Services
{
    /// <summary>
    /// Responsible for blob storage CRUD operations.
    /// </summary>
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobStorageService"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AzureBlobStorageService(string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        /// <summary>
        /// Create the specified container.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        public async Task CreateContainer(string containerName)
        {
            if (containerName == null) throw new ArgumentNullException(nameof(containerName));

            var container = _blobServiceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();
        }

        /// <summary>
        /// Delete the specified container and all of its contents.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        public async Task DeleteContainer(string containerName)
        {
            if (containerName == null) throw new ArgumentNullException(nameof(containerName));

            var container = _blobServiceClient.GetBlobContainerClient(containerName);
            await container.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Insert the file into the specified container.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fullFileName">The full path to the local file.</param>
        /// <param name="containerGroup">The optional folder within the container to save the file to.</param>
        public async Task UploadFileToContainer(string containerName, string fullFileName, string containerGroup = "")
        {
            if (containerName == null) throw new ArgumentNullException(nameof(containerName));
            if (fullFileName == null) throw new ArgumentNullException(nameof(fullFileName));

            var container = _blobServiceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();

            using (var uploadFileStream = File.OpenRead(fullFileName))
            {
                var fileName = Path.GetFileName(fullFileName);
                var blobFileName = !string.IsNullOrEmpty(containerGroup) ? $"{containerGroup}/{fileName}" : fileName;

                var blockBlob = container.GetBlobClient(blobFileName);

                var response = await blockBlob.UploadAsync(uploadFileStream);
            }
        }
    }
}