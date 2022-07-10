using ExcelDataReader;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private string _inputPath = string.Empty;
        private string _outputPath = string.Empty;
        private readonly string[] _languages = {
        "ES",
        "CAT",
        "EUS",
        "GAL",
        "ENG",
        "DEU",
        "FRA",
        "ITA",
        "NL",
        "RUS"
        };
        private KeyViewModel _keyViewModel = new();
        private List<string> _keys = new();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _keyViewModel;
        }
        private void BtnGetInputFile(object sender, RoutedEventArgs e)
        {
            _inputPath = GetInputFilePath();
        }

        private void BtnGetOutputPath(object sender, RoutedEventArgs e)
        {
            _outputPath = GetOutputFilePath();
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
                ProcessXlsDataForEachLanguage(_inputPath, _outputPath);

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void ProcessXlsDataForEachLanguage(string inputFile, string outputFile)
        {
            IDictionary<string, string> errors = new Dictionary<string, string>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            foreach (var language in _languages)
            {
                try
                {
                    using var inFile = File.Open(inputFile, FileMode.Open, FileAccess.Read);
                    using var reader = ExcelReaderFactory.CreateReader(inFile,
                    new ExcelReaderConfiguration { FallbackEncoding = Encoding.GetEncoding(1252) });

                    var dataSet = reader.AsDataSet();

                    var dataTable = dataSet.Tables[0];

                    var languageValues = GetLanguageValues(language, dataTable).ToList();
                    if (languageValues.Any())
                        WriteJson(languageValues, outputFile, language);
                }
                catch (Exception e)
                {
                    errors.Add(language, e.Message);
                }
            }
            if (errors.Any())
                ShowErrors(errors);
        }

        private string GetInputFilePath()
        {
            OpenFileDialog openfile = new()
            {
                DefaultExt = ".xlsx",
                Filter = "(.xlsx)|*.xlsx",
            };

            openfile.ShowDialog();
            return txtInputFile.Text = openfile.FileName;
        }

        private static IEnumerable<string> GetLanguageValues(string language, DataTable table)
        {
            IEnumerable<string> valuesSelectedLanguage = new List<string>();

            foreach (DataRow dtRow in table.Rows)
            {
                foreach (DataColumn dc in table.Columns)
                {
                    var value = dtRow[dc].ToString();
                    if (!string.IsNullOrWhiteSpace(value) && value.ToUpperInvariant().Equals(language.ToUpperInvariant()))
                    {
                        var partNumber = table.Columns[dc.ColumnName];

                        if (partNumber == null)
                        {
                            throw new Exception("There is no content for this language");
                        }
                        valuesSelectedLanguage = from row in table.AsEnumerable()
                                                 where !string.IsNullOrWhiteSpace(row[partNumber].ToString())
                                                 && table.Rows.IndexOf(row) > table.Rows.IndexOf(dtRow)
                                                 select row[partNumber]?.ToString();
                        break;

                    }
                }
            }

            return valuesSelectedLanguage;
        }
        private void WriteJson(List<string> values, string outputFile, string language)
        {
            using var outFile = File.CreateText(outputFile + $@"{Path.DirectorySeparatorChar}{language}.json");
            using var writer = new JsonTextWriter(outFile);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();
            for (int i = 0; i < values.Count; i++)
            {
                writer.WritePropertyName(_keys.Count > i ? _keys[i] : "default");
                writer.WriteValue(values[i]);
            }
            writer.WriteEndObject();
        }

        private string GetOutputFilePath()
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();

            dialog.ShowDialog();
            return txtOutputPath.Text = dialog.SelectedPath;

        }

        private static void ShowErrors(IDictionary<string, string> errors)
        {
            StringBuilder errorStrBuilder = new();
            errorStrBuilder.AppendLine("Could not generate a Json file for the following languages:");
            errorStrBuilder.AppendLine();
            foreach (var error in errors)
            {
                errorStrBuilder.AppendLine($"{error.Key} {error.Value}");
            }
            MessageBox.Show(errorStrBuilder.ToString());
        }
    }
}
