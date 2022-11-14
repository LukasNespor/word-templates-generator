using Api.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using System;

namespace Api
{
    public class GetUploadBlobToken
    {
        private readonly BlobService _blobService;
        private readonly TableService _tableService;

        public GetUploadBlobToken(BlobService blobService, TableService tableService)
        {
            _blobService = blobService;
            _tableService = tableService;
        }

        [FunctionName(nameof(GetUploadBlobToken))]
        public IActionResult Run([HttpTrigger("get", Route = "uploadtoken")] HttpRequest req)
        {
            var container = _blobService.GetContainer(Constants.TemplatesContainerName);
            var sas = GetContainerSasUri(container, BlobContainerSasPermissions.Create);
            return new OkObjectResult($"BlobEndpoint={_blobService._service.Uri};SharedAccessSignature={sas.Query}");
        }

        static Uri GetContainerSasUri(BlobContainerClient container, BlobContainerSasPermissions permissions)
        {
            var builder = CreateAdHocSasBuilder(container.Name, permissions);
            return container.GenerateSasUri(builder);
        }

        static BlobSasBuilder CreateAdHocSasBuilder(string containerName, BlobContainerSasPermissions permissions)
        {
            BlobSasBuilder builder = new()
            {
                // Set start time to five minutes before now to avoid clock skew.
                StartsOn = DateTime.UtcNow.AddMinutes(-5),
                ExpiresOn = DateTime.UtcNow.AddHours(1),
                BlobContainerName = containerName
            };
            builder.SetPermissions(permissions);
            return builder;
        }
    }
}