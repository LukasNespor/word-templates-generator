using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    public class GetTemplates
    {
        private readonly TableService _tableService;

        public GetTemplates(TableService tableService)
        {
            _tableService = tableService;
        }


        [FunctionName(nameof(GetTemplates))]
        public async Task<IActionResult> Run([HttpTrigger("get", Route = "templates")] HttpRequest req, ILogger log)
        {
            try
            {
                var records = await _tableService.GetRecordsAsync<TemplateEntity>(Constants.TemplatesTableName);
                var data = records
                    .Select(x => new TemplateModel
                    {
                        Id = x.RowKey,
                        Name = x.Name,
                        Description = x.Description,
                        BlobName = x.BlobName,
                        Group = x.Group,
                        Fields = x.Fields.Split(';')
                    })
                    .OrderBy(x => x.Name);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                throw;
            }
        }
    }
}
