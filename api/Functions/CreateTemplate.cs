using Api.Models;
using Api.Services;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using OpenXmlHelpers.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Api
{
    public class CreateTemplate
    {
        private readonly BlobService _blobService;
        private readonly TableService _tableService;

        public CreateTemplate(BlobService blobService, TableService tableService)
        {
            _blobService = blobService;
            _tableService = tableService;
        }

        [FunctionName(nameof(CreateTemplate))]
        public async Task<IActionResult> Run([HttpTrigger("post", Route = "templates")] TemplateModel data, ILogger log)
        {
            try
            {
                var blob = _blobService.GetBlobClient(Constants.TemplatesContainerName, data.BlobName);

                var fields = new List<string>();
                using (var sourceStream = new MemoryStream())
                {
                    await blob.DownloadToAsync(sourceStream);

                    using WordprocessingDocument doc = WordprocessingDocument.Open(sourceStream, false);
                    foreach (var field in doc.GetMergeFields())
                    {
                        string fieldName = OpenXmlWordHelpers.GetFieldNameFromMergeField(field.InnerText).Trim('\"');
                        if (!fields.Contains(fieldName) &&
                            fieldName.Trim() != "PAGE" &&
                            fieldName.Trim() != @"PAGE   \* MERGEFORMAT" &&
                            fieldName.Trim() != "FORMCHECKBOX" &&
                            !fieldName.Equals("dnes", StringComparison.OrdinalIgnoreCase))
                        {
                            fields.Add(fieldName);
                        }
                    }
                }

                string fieldsString = string.Join(";", fields);
                string id = Guid.NewGuid().ToString("N");
                var template = new TemplateEntity(Constants.TemplatesPartitionKey, id)
                {
                    Name = data.Name,
                    BlobName = data.BlobName,
                    Group = data.Group,
                    Description = data.Description,
                    Fields = fieldsString
                };
                await _tableService.CreateRecordAsync(Constants.TemplatesTableName, template);

                log.LogInformation("Template processed");

                return new OkObjectResult(new TemplateModel()
                {
                    Id = id,
                    Name = data.Name,
                    BlobName = data.BlobName,
                    Group = data.Group,
                    Description = data.Description,
                    Fields = fields
                });
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                throw;
            }
        }
    }
}
