using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using WpfApp1.Models;
using WpfApp1.Resources;
using WpfApp1.Utilities;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DEFAULT_LANGUAGES = "DefaultLanguages";
        private string _inputPath = string.Empty;
        private string _outputPath = string.Empty;
        private string[] _languages;
        private KeyViewModel _keyViewModel = new();
        private List<string> _keys = new();
        public MainWindow()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            DataContext = _keyViewModel;
            _languages = LanguageKeys.ResourceManager.GetString(DEFAULT_LANGUAGES).Split(',');
            InitializeComponent();
            
        }
        private void BtnGetInputFile(object sender, RoutedEventArgs e)
        {
            _inputPath = txtInputFile.Text = FileBrowserManager.GetInputFilePath();
        }

        private void BtnGetOutputPath(object sender, RoutedEventArgs e)
        {
            _outputPath = txtOutputPath.Text = FileBrowserManager.GetOutPutFilePath();
        }

        private void BtnAddKey(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtKeys.Text))
            {
                var trimedStr = txtKeys.Text.Trim();
                _keyViewModel.Keys.Add(new KeyModel { Name = trimedStr });
                _keys.Add(trimedStr);
                txtKeys.Text = string.Empty;
            }

        }

        private void BtnProcessXls(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_inputPath) && !string.IsNullOrWhiteSpace(_outputPath))
            {
                var dataTable = ReadXlsAsDataTable();
                if (dataTable != null)
                {
                    WriteJsonForEachLanguage(dataTable);
                }
                else
                {
                    ErrorManager.ShowReadXlsError("DataTable was empty", _inputPath);
                }
            }
        }

        private DataTable? ReadXlsAsDataTable()
        {
            try
            {
                return DataProcessor.ReadXlsAsDataTable(_inputPath);
            }
            catch (System.Exception e)
            {
                ErrorManager.ShowReadXlsError(e.Message, _inputPath);
            }

            return null;

        }

        private void WriteJsonForEachLanguage(DataTable dataTable)
        {
            var errors = new Dictionary<string, string>();
            foreach (var language in _languages)
            {
                try
                {
                    var languageValues = DataProcessor.GetLanguageValues(language, dataTable).ToList();
                    if (languageValues.Any())
                        DataProcessor.WriteJson(languageValues, _outputPath, language, _keys);
                }
                catch (System.Exception e)
                {
                    errors.Add(language, e.Message);
                }
            }
            if (errors.Any())
                ErrorManager.ShowWriteErrors(errors);
        }
    }
}
