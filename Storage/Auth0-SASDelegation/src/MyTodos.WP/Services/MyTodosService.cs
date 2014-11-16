using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using MyTodos.WP.Model;

namespace MyTodos.WP.Services
{
    public class MyTodosService
    {
        private CloudTable _table;

        private bool _initComplete;

        private string _partitionKey;

        public void Init(string accountName, string tableName, string sharedAccessSignature, string partitionKey)
        {
            _partitionKey = partitionKey;
            _table = new CloudTableClient(new Uri(String.Format("http://{0}.table.core.windows.net", accountName)), new StorageCredentials(sharedAccessSignature))
                .GetTableReference(tableName);
            _initComplete = true;
        }

        private void RequireInitialized()
        {
            if (!_initComplete)
                throw new InvalidOperationException("Service not initialized. Login first.");
        }

        public async Task<IEnumerable<TodoEntity>> GetAll()
        {
            RequireInitialized();

            var partitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, _partitionKey);
            var query = new TableQuery<TodoEntity>().Where(partitionFilter);
            return await _table.ExecuteQuerySegmentedAsync(query, null).AsTask();
        }

        public async Task CreateNew(string title, bool isComplete)
        {
            RequireInitialized();

            await _table.ExecuteAsync(TableOperation.Insert(new TodoEntity
            {
                CreatedOn = DateTime.UtcNow,
                PartitionKey = _partitionKey,
                RowKey = (DateTime.MaxValue - DateTime.UtcNow).Ticks.ToString(),
                Title = title,
                IsComplete = isComplete
            }));
        }
    }
}
