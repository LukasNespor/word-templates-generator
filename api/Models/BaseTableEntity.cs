using Azure;
using Azure.Data.Tables;
using System;

namespace Api.Models
{
    public class BaseTableEntity : ITableEntity
    {
        public BaseTableEntity()
        {

        }

        public BaseTableEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
