using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Api
{
    public class RemoveTemplate
    {
        private readonly BlobService _blobService;
        private readonly TableService _tableService;

        public RemoveTemplate(BlobService blobService, TableService tableService)
        {
            _blobService = blobService;
            _tableService = tableService;
        }

        [FunctionName(nameof(RemoveTemplate))]
        public async Task<IActionResult> Run([HttpTrigger("delete", Route = "templates/{id}")] HttpRequest req, string id, ILogger log)
        {
            try
            {
                var record = await _tableService.GetRecordAsync<TemplateEntity>(Constants.TemplatesTableName, Constants.TemplatesPartitionKey, id);
                await _tableService.DeleteRecordAsync(Constants.TemplatesTableName, record);
                var blob = _blobService.GetBlobClient(Constants.TemplatesContainerName, record.BlobName);
                await blob.DeleteIfExistsAsync();
                return new NoContentResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                throw;
            }
        }
    }
}
