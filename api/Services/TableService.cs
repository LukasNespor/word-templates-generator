using Azure;
using Azure.Data.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Services
{
    public class TableService
    {
        readonly TableServiceClient _service;

        public TableService(string connectionString)
        {
            _service = new TableServiceClient(connectionString);
        }

        public async Task EnsureTableAsync(string tableName)
        {
            var client = _service.GetTableClient(tableName);
            await client.CreateIfNotExistsAsync();
        }

        public async Task<IEnumerable<T>> GetRecordsAsync<T>(string tableName) where T : class, ITableEntity, new()
        {
            var items = new List<T>();
            var table = _service.GetTableClient(tableName);

            var query = table.QueryAsync<T>().AsPages();
            await foreach (Page<T> page in query)
            {
                if (page.Values.Count > 0)
                    items.AddRange(page.Values);
            }

            return items;
        }

        public async Task CreateRecordAsync<T>(string tableName, T data) where T : ITableEntity, new()
        {
            var table = _service.GetTableClient(tableName);
            await table.AddEntityAsync(data);
        }

        public async Task<T> GetRecordAsync<T>(string tableName, string partitionKey, string rowKey) where T : class, ITableEntity, new()
        {
            var table = _service.GetTableClient(tableName);
            return await table.GetEntityAsync<T>(partitionKey, rowKey);
        }

        public async Task DeleteRecordAsync(string tableName, ITableEntity record)
        {
            var table = _service.GetTableClient(tableName);
            await table.DeleteEntityAsync(record.PartitionKey, record.RowKey);
        }
    }
}
