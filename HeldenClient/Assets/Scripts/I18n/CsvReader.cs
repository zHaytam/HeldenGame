using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.I18n
{
    public static class CsvReader
    {

        #region Fields

        private const char Newline = '\n';
        private const char Comma = ',';
        private const char DoubleQuotes = '\"';

        #endregion

        #region Public Methods

        public static List<T> ParseToList<T>(string text, int numCols, bool ignoreHeader, Func<string[], T> converter)
        {
            var objects = new List<T>();

            foreach (var row in Parse(text, numCols, ignoreHeader))
            {
                objects.Add(converter(row));
            }

            return objects;
        }

        public static Dictionary<TKey, TValue> ParseToDictionary<TKey, TValue>(string text, int numCols, bool ignoreHeader,
            Func<string[], TKey> keySelector, Func<string[], TValue> valueSelector)
        {
            var dictionary = new Dictionary<TKey, TValue>();

            foreach (var row in Parse(text, numCols, ignoreHeader))
            {
                dictionary.Add(keySelector(row), valueSelector(row));
            }

            return dictionary;
        }

        #endregion

        #region Private Methods

        public static IEnumerable<string[]> Parse(string text, int numCols, bool ignoreHeader)
        {
            int lineNumber = 0;
            for (int i = 0; i < text.Length; i++)
            {
                (var row, int cols, bool doubleQuotesLeftOpen) = ParseRow(text, ref i, numCols);
                lineNumber++;

                if (doubleQuotesLeftOpen)
                    throw new Exception($"Missing closing double quotes in line {lineNumber}.");

                if (cols < numCols)
                    throw new Exception($"Line {lineNumber} doesn't contain {numCols} columns.");

                if (cols > numCols)
                    throw new Exception($"Line {lineNumber} contains more than {numCols} columns.");

                if (ignoreHeader && lineNumber == 1)
                    continue;

                yield return row;
            }
        }

        private static (string[] row, int cols, bool doubleQuotesLeftOpen) ParseRow(string text, ref int i, int numCols)
        {
            string[] row = new string[numCols];
            var sb = new StringBuilder();
            int currentCol = 0;
            bool insideDoubleQuotes = false;

            for (; i < text.Length; i++)
            {
                char c = text[i];

                // Double quotes?
                if (c == DoubleQuotes)
                {
                    // Should be escaped?
                    if (i + 1 < text.Length && text[i + 1] == DoubleQuotes)
                    {
                        sb.Append(DoubleQuotes);
                        i += 1;
                        continue;
                    }

                    insideDoubleQuotes = !insideDoubleQuotes;
                }
                // Append char if possible
                else if (CharShouldBeAppended(c, insideDoubleQuotes))
                {
                    sb.Append(c);
                }

                if (IsEndOfColumn(text, i, insideDoubleQuotes))
                {
                    row[currentCol] = sb.Length > 0 ? sb.ToString() : null;
                    currentCol++;
                    sb.Clear();
                }

                if (IsEndOfLine(text, i, insideDoubleQuotes))
                {
                    // Empty last column?
                    if (c == Comma)
                    {
                        if (currentCol < numCols)
                            row[currentCol] = null;

                        currentCol++;
                    }

                    break;
                }
            }

            return (row, currentCol, insideDoubleQuotes);
        }

        private static bool CharShouldBeAppended(char c, bool insideDoubleQuotes)
        {
            return (c != Comma || c == Comma && insideDoubleQuotes) &&
                   (c != Newline || c == Newline && insideDoubleQuotes);
        }

        private static bool IsEndOfLine(string text, int i, bool insideDoubleQuotes)
        {
            // It's the end of line when there is a Newline next or we're at the end of the text
            return !insideDoubleQuotes && (i == text.Length - 1 || text[i + 1] == Newline);
        }

        private static bool IsEndOfColumn(string text, int i, bool insideDoubleQuotes)
        {
            // It's the end of column when there is a comma or it's the end of line
            return (text[i] == Comma && !insideDoubleQuotes) || IsEndOfLine(text, i, insideDoubleQuotes);
        }

        #endregion

    }
}
