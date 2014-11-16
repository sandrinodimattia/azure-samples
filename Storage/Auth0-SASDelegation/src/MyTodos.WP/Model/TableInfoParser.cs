using Newtonsoft.Json.Linq;

namespace MyTodos.WP.Model
{
    public static class TableInfoParser
    {
        public static TableInfo FromJObject(JObject obj)
        {
            return new TableInfo
            {
                AccountName = obj["storageAccountName"].Value<string>(),
                PartitionKey = obj["storagePartitionKey"].Value<string>(),
                SharedAccessSignature = obj["storageSas"].Value<string>(),
                TableName= obj["storageTableName"].Value<string>()
            };
        }
    }
}