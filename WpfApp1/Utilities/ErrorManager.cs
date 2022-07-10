using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace WpfApp1.Utilities
{
    public static class ErrorManager
    {
        public static void ShowReadXlsError(string errorMessage, string inputFile)
        {
            StringBuilder errorStrBuilder = new();
            errorStrBuilder.AppendLine($"Could not open {inputFile}");
            errorStrBuilder.AppendLine("Error:");
            errorStrBuilder.AppendLine(errorMessage);
            MessageBox.Show(errorStrBuilder.ToString(), "Copies Companion", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowWriteErrors(Dictionary<string, string> errors)
        {
            MessageBox.Show(CreateErrorMessages(errors), "Copies Companion", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private static string CreateErrorMessages(Dictionary<string, string> errors)
        {
            StringBuilder errorStrBuilder = new();
            errorStrBuilder.AppendLine("Could not generate a Json file for the following languages:");
            errorStrBuilder.AppendLine();
            foreach (var error in errors)
            {
                errorStrBuilder.AppendLine($"{error.Key} {error.Value}");
            }
            return errorStrBuilder.ToString();
        }
    }
}
