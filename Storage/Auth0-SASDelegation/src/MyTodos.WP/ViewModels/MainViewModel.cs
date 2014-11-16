using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;

using Auth0.SDK;
using MyTodos.WP.Infrastructure;
using MyTodos.WP.Model;
using MyTodos.WP.Services;

namespace MyTodos.WP.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly MyTodosService _service;

        #region Properties

        private TableInfo _tableInfo;

        public TableInfo TableInfo
        {
            get { return _tableInfo; }
            set
            {
                _tableInfo = value;
                OnPropertyChanged();
            }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private bool _isComplete;

        public bool IsComplete
        {
            get { return _isComplete; }
            set
            {
                _isComplete = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<TodoEntity> _todos;

        public ObservableCollection<TodoEntity> Todos
        {
            get { return _todos; }
            set
            {
                _todos = value;
                OnPropertyChanged();
                OnPropertyChanged("HasTodos");
            }
        }

        public bool HasTodos
        {
            get { return Todos != null && Todos.Any(); }
        }

        public ICommand SignInCommand
        {
            get;
            set;
        }

        public ICommand NewTodoCommand
        {
            get;
            set;
        }

        #endregion

        public MainViewModel(MyTodosService service)
        {
            _service = service;

            // Initialize VM properties.
            Title = "My new todo";
            SignInCommand = new RelayCommand(async obj =>
            {
                await TryAuthenticate();
            });
            NewTodoCommand = new RelayCommand(async obj =>
            {
                await PersistNewTodo();
            });
        }

        public async Task TryAuthenticate()
        {
            try
            {
                var auth0 = new Auth0Client(
                    "youraccount.auth0.com",
                    "clientid");
                var user = await auth0.LoginAsync();
                var profile = user.Profile.ToString();
                
                // Show a small welcome message. Assumes we authenticated with Google and we have the user's given name.
                var givenName = user.Profile["given_name"];
                Dialog.Show("Login Success", String.Format("Welcome {0}", givenName));
                
                // Initialize the service.
                var tableInfo = TableInfoParser.FromJObject(user.Profile);
                _service.Init(tableInfo.AccountName, tableInfo.TableName, tableInfo.SharedAccessSignature, tableInfo.PartitionKey);

                // Load my todos.
                await LoadTasks();
            }
            catch (Exception ex)
            {
                Dialog.Show("Error", ex.Message);
            }
        }

        private async Task LoadTasks()
        {
            var myTodos = await _service.GetAll();
            Todos = new ObservableCollection<TodoEntity>(myTodos);
        }

        private async Task PersistNewTodo()
        {
            try
            {
                await _service.CreateNew(Title, IsComplete);
                await LoadTasks();
            }
            catch (Exception ex)
            {
                Dialog.Show("Error", ex.Message);
            }
        }
    }
}
