using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    public class GetLists
    {
        private readonly BlobService _blobService;
        private readonly TableService _tableService;

        public GetLists(BlobService blobService, TableService tableService)
        {
            _blobService = blobService;
            _tableService = tableService;
        }

        [FunctionName(nameof(GetLists))]
        public async Task<IActionResult> Run([HttpTrigger("get", Route = "lists")] HttpRequest req)
        {
            var items = await _tableService.GetRecordsAsync<ListEntity>(Constants.ListsTableName);
            return new OkObjectResult(items.Select(x => new
            {
                Id = x.RowKey,
                Uid = Guid.NewGuid().ToString(),
                Type = x.PartitionKey,
                x.Value
            }));
        }
    }
}
