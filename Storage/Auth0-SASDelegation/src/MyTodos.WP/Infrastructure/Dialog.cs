using Windows.UI.Popups;

namespace MyTodos.WP.Infrastructure
{
    public static class Dialog
    {
        public static void Show(string title, string content)
        {
            var dialog = new MessageDialog(content, title);
            dialog.ShowAsync();
        }
    }
}
