using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using System.ComponentModel;
using CSVParser;


namespace CsvParser
{
    /// <summary>
    /// Class to parse csv files
    /// The structure of the csv file must be passed as type T
    /// </summary>
    /// <typeparam name="T">Type which corresponds to the structure of csv file</typeparam>
    public class CsvParser<T> where T : new()
    {
        public delegate void ValidationErrorOccuredHandler(int line, int col, string message);

        /// <summary>
        /// Event raised when a validation error occurs
        /// </summary>
        public event ValidationErrorOccuredHandler ValidationErrorOccurred;

        /// <summary>
        /// Total line count of input file
        /// </summary>
        public int TotalLineCount { get; set; }

        /// <summary>
        /// Total count of imported lines
        /// </summary>
        public int ImportedLineCount { get; set; }


        public CsvParser()
        {
            if (typeof(T).GetCustomAttributes(typeof(IsCsvRecordAttribute), true) == null)
                throw new Exception(string.Format("The type {0} lacks the attribute IsCsvRecordAttribute", typeof(T).Name));
        }

        /// <summary>
        /// Converts the text file to a list of type T
        /// </summary>
        /// <param name="fullFileName">Full filename of the csv file</param>
        /// <param name="separator">The separator of the csv file</param>
        /// <param name="hasHeader">if true, skips the first line during import</param>
        /// <returns></returns>
        public List<T> ReadFromFile(string fullFileName, string separator, bool hasHeader)
        {
            List<T> RecordList = new List<T>();
            int currentLineCount = 0;
            //regex to identify csv fields 
            Regex regex = new Regex("(\"(?:[^\"]+|\"\")*\"|[^" + separator + "]*)($|" + separator + ")");

            using (TextReader textReader = File.OpenText(fullFileName))
            {
                //skip first line if hasHeader
                if (hasHeader && textReader.Peek() != -1)
                {
                    textReader.ReadLine();
                }

                while (textReader.Peek() != -1)
                {
                    MatchCollection currentLine = regex.Matches(textReader.ReadLine());

                    if (typeof(T).GetProperties().Count() != currentLine.Count - 1)
                    {
                        throw new Exception("Column count mismatch in csv file.");
                    }
                    List<string> columns = new List<string>();
                    for (int i = 0; i < currentLine.Count - 1; i++)
                    {
                        string currentValue = currentLine[i].Value;
                        currentValue = currentValue.Substring(currentValue.Length - 1) == separator ?
                            currentValue.Substring(0, currentValue.Length - 1) : currentValue;

                        columns.Add(currentValue);
                    }
                    try
                    {
                        RecordList.Add(ParseLine(columns));
                        ImportedLineCount += 1;
                    }
                    catch (LineParseException ex)
                    {
                        ValidationErrorOccurred(currentLineCount + 1, ex.Column, ex.Message);
                    }
                    finally
                    {
                        currentLineCount += 1;
                    }
                }
            }
            TotalLineCount = currentLineCount;
            return RecordList;
        }

        /// <summary>
        /// TODO: This is a copy paste version of ReadFromFile
        /// using some extension methods for stream.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="separator"></param>
        /// <param name="hasHeader"></param>
        /// <returns></returns>
        public List<T> ReadFromStream(Stream content, string separator, bool hasHeader)
        {
            var recordList = new List<T>();
            var currentLineCount = 0;
            //regex to identify csv fields 
            var regex = new Regex("(\"(?:[^\"]+|\"\")*\"|[^" + separator + "]*)($|" + separator + ")");

            //skip first line if hasHeader
            if (hasHeader && content.Peek() != -1)
            {
                content.ReadLine();
            }

            while (content.Peek() != -1)
            {
                var currentLine = regex.Matches(content.ReadLine());

                if (typeof(T).GetProperties().Count() != currentLine.Count - 1)
                {
                    throw new Exception("Column count mismatch in csv file.");
                }
                var columns = new List<string>();
                for (var i = 0; i < currentLine.Count - 1; i++)
                {
                    var currentValue = currentLine[i].Value;
                    currentValue = currentValue.Substring(currentValue.Length - 1) == separator ?
                        currentValue.Substring(0, currentValue.Length - 1) : currentValue;

                    columns.Add(currentValue);
                }
                try
                {
                    recordList.Add(ParseLine(columns));
                    ImportedLineCount += 1;
                }
                catch (LineParseException ex)
                {
                    ValidationErrorOccurred(currentLineCount + 1, ex.Column, ex.Message);
                }
                finally
                {
                    currentLineCount += 1;
                }
            }

            TotalLineCount = currentLineCount;
            return recordList;

        }

        /// <summary>
        /// Parses a list of strings and assignes the same to the properties of the type T.
        /// </summary>
        /// <param name="currentLine"></param>
        /// <returns></returns>
        private T ParseLine(List<string> currentLine)
        {
            T csvInstance = new T();
            for (int i = 0; i < typeof(T).GetProperties().Count(); i++)
            {
                PropertyInfo currentProperty = typeof(T).GetProperties().ElementAt(i);
                Type currentPropertyType = currentProperty.PropertyType;
                //check if field is mandatory
                if (currentProperty.GetCustomAttributes(typeof(MandatoryFieldAttribute), true).Count() != 0)
                {
                    if (string.IsNullOrWhiteSpace(currentLine.ElementAt(i)))
                    {
                        throw new LineParseException(string.Format("The field {0} is mandatory.", currentProperty.Name), i + 1);
                    }
                }

                //check if field has max length attribute
                if (currentProperty.GetCustomAttributes(typeof(MaxLengthAttribute), true).Count() != 0)
                {
                    int maxLength = ((MaxLengthAttribute)currentProperty.GetCustomAttributes(typeof(MaxLengthAttribute), true).ElementAt(0)).MaxLength;
                    if (currentLine.ElementAt(i).Length > maxLength)
                        throw new LineParseException(string.Format("The max length of the field {0} is {1}.", currentProperty.Name, maxLength), i + 1);
                }

                //try to set the value
                try
                {
                    if (!string.IsNullOrWhiteSpace(currentLine.ElementAt(i)))
                    {
                        currentProperty.SetValue(csvInstance,
                            TypeDescriptor.GetConverter(currentPropertyType).ConvertFromString(currentLine.ElementAt(i)),
                            null);
                    }
                    else
                    {
                        currentProperty.SetValue(csvInstance, null, null);
                    }
                }
                //catches exceptions defined in the csv class
                catch (TargetInvocationException ex)
                {
                    throw new LineParseException(ex.InnerException.Message, i + 1);
                }
                catch (Exception ex)
                {
                    throw new LineParseException(ex.Message, i + 1);
                }
            }
            return csvInstance;
        }
    }
}
