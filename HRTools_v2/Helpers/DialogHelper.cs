using Microsoft.Win32;

namespace HRTools_v2.Helpers
{
    public class DialogHelper
    {

        private string _defaultExt;
        private string _filter;
        private string _title;

        public DialogHelper(string defaultExt, string filter)
        {
            _defaultExt = defaultExt;
            _filter = filter;
        }

        public DialogHelper(string defaultExt, string filter, string title) : this(defaultExt, filter)
        {
            _title = title;
        }

        public string ShowOpenDialog()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = _defaultExt
            };

            dialog.Filter = _filter;

            dialog.ShowDialog();
            return dialog.FileName;
        }

        public string ShowSaveDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = _filter,
                Title = _title
            };

            dialog.ShowDialog();

            return dialog.FileName;
        }

    }
}
