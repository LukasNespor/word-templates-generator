using Api.Models;
using Api.Services;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using OpenXmlHelpers.Word;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    public class GenerateDocument
    {
        private readonly BlobService _blobService;

        public GenerateDocument(BlobService blobService)
        {
            _blobService = blobService;
        }

        [FunctionName(nameof(GenerateDocument))]
        public async Task<IActionResult> Run([HttpTrigger("post", Route = "generate")] GenerateDocumentModel data, ILogger log)
        {
            try
            {
                var blob = _blobService.GetBlobClient(Constants.TemplatesContainerName, data.BlobName);
                var props = await blob.GetPropertiesAsync();

                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    await blob.DownloadToAsync(stream);

                    using WordprocessingDocument doc = WordprocessingDocument.Open(stream, true);
                    foreach (var field in data.Fields)
                    {
                        if (!string.IsNullOrEmpty(field.Value))
                        {
                            string name = field.Name;
                            if (name.Contains(" "))
                                name = "\"" + name + "\"";

                            doc.GetMergeFields(name).ReplaceWithText(field.Value);
                        }
                    }

                    var todayField = doc.GetMergeFields("dnes");
                    if (todayField.Any())
                        todayField.ReplaceWithText(DateTime.UtcNow.ToString("d. MMMM yyyy", new CultureInfo("cs")));

                    doc.MainDocumentPart.Document.Save();
                    doc.Close();

                    bytes = new byte[stream.Length];
                    bytes = stream.ToArray();
                }

                return new FileContentResult(bytes, props.Value.ContentType);
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                throw;
            }
        }
    }
}
