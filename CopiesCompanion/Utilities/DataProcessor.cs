using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CopiesCompanion.Utilities
{
    public static class DataProcessor
    {
        public static DataTable ReadXlsAsDataTable(string inputFile, int sheetNum = 0)
        {
            using var inFile = File.Open(inputFile, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(inFile,
            new ExcelReaderConfiguration { FallbackEncoding = Encoding.GetEncoding(1252) });

            var dataSet = reader.AsDataSet();

            return dataSet.Tables[sheetNum];
        }

        public static void WriteJson(List<string> values, string outputFile, string language, List<string> keys)
        {
            using var outFile = File.CreateText(outputFile + $@"{Path.DirectorySeparatorChar}{language}.json");
            using var writer = new JsonTextWriter(outFile);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();
            for (int i = 0; i < values.Count; i++)
            {
                writer.WritePropertyName(keys.Count > i && !string.IsNullOrWhiteSpace(keys[i]) ? keys[i] : "default");
                writer.WriteValue(values[i]);
            }
            writer.WriteEndObject();
        }
        public static IEnumerable<string> GetLanguageValues(string language, DataTable table)
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
                                                 select row[partNumber].ToString();
                        break;

                    }
                }
            }
            return valuesSelectedLanguage;
        }
    }
}
