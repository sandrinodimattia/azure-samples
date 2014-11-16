using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace MyTodos.WP.Model
{
    public class TodoEntity : TableEntity
    {
        public string Title
        {
            get;
            set;
        }

        public DateTime CreatedOn
        {
            get;
            set;
        }

        public bool IsComplete
        {
            get;
            set;
        }
    }
}
