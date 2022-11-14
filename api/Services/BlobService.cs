using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;

namespace Api.Services
{
    public class BlobService
    {
        internal readonly BlobServiceClient _service;

        public BlobService(string connectionString)
        {
            _service = new BlobServiceClient(connectionString);
        }

        public BlobContainerClient GetContainer(string containerName)
        {
            return _service.GetBlobContainerClient(containerName);
        }

        public async Task EnsureContainerAsync(string containerName)
        {
            var container = _service.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();
        }

        public async Task CreateBlobAsync(string containerName, string blobName, Stream content)
        {
            var container = _service.GetBlobContainerClient(containerName);
            var client = container.GetBlobClient(blobName);
            await client.UploadAsync(content, overwrite: true);
        }

        public BlobClient GetBlobClient(string containerName, string blobName)
        {
            var container = _service.GetBlobContainerClient(containerName);
            return container.GetBlobClient(blobName);
        }
    }
}
