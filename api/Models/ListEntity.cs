namespace Api.Models
{
    public class ListEntity : BaseTableEntity
    {
        public ListEntity()
        {
        }

        public ListEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {

        }

        public string Value { get; set; }
    }
}
