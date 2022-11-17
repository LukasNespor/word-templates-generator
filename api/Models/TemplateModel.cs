using Newtonsoft.Json;
using System.Collections.Generic;

namespace Api.Models
{
    public class TemplateModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("blobName")]
        public string BlobName { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("fields")]
        public IEnumerable<string> Fields { get; set; }
    }
}
