using Ookii.Dialogs.Wpf;
using System.Windows.Forms;

namespace WpfApp1.Utilities
{
    public static class FileBrowserManager
    {
        public static string GetInputFilePath()
        {
            OpenFileDialog openfile = new()
            {
                DefaultExt = ".xlsx",
                Filter = "(.xlsx)|*.xlsx",
            };

            openfile.ShowDialog();
            return openfile.FileName;
        }

        public static string GetOutPutFilePath()
        {
            var dialog = new VistaFolderBrowserDialog();

            dialog.ShowDialog();
            return dialog.SelectedPath;
        }
    }
}
