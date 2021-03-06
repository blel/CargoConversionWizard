﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CsvParser;

namespace CSVParser
{
    public abstract class CsvParserBase<T> where T : new()
    {
        private readonly string _separator;

        private readonly bool _hasHeader;

        public delegate void ValidationErrorOccuredHandler(int line, int col, string message);

        /// <summary>
        /// Event raised when a validation error occurs
        /// </summary>
        public event ValidationErrorOccuredHandler ValidationErrorOccurred;

        /// <summary>
        /// Abstract property. Derived class should return true if the source is at the end.
        /// This property is used by the read template method.
        /// </summary>
        public abstract bool IsAtEnd { get; }

        /// <summary>
        /// Abstract method readLine. Should read a text line from the source.
        /// This method is called by the read template method.
        /// </summary>
        /// <returns></returns>
        public abstract string ReadLine();

        /// <summary>
        /// Abstract method DisposeSource. Implementation should dispose the source.
        /// Is called at the end of the read template method.
        /// </summary>
        public abstract void DisposeSource();

        /// <summary>
        /// Total line count of input file
        /// </summary>
        public int TotalLineCount { get; set; }

        /// <summary>
        /// Total count of imported lines
        /// </summary>
        public int ImportedLineCount { get; set; }

        /// <summary>
        /// The abstract constructor
        /// </summary>
        /// <param name="separator">Separator in the csv file</param>
        /// <param name="hasHeader">Indicates if the file has header (if yes first line is skipped.)</param>
        protected CsvParserBase(string separator, bool hasHeader)
        {
            if (typeof(T).GetCustomAttributes(typeof(IsCsvRecordAttribute), true) == null)
                throw new Exception(string.Format("The type {0} lacks the attribute IsCsvRecordAttribute", typeof(T).Name));

            _separator = separator;
            _hasHeader = hasHeader;
        }

        /// <summary>
        /// Template method. Reads the file and converts it to the list of type T
        /// Converts the text file to a list of type T
        /// </summary>
        /// <returns></returns>
        public List<T> Read()
        {
            var recordList = new List<T>();

            var currentLineCount = 0;
            //regex to identify csv fields 
            var regex = new Regex("(\"(?:[^\"]+|\"\")*\"|[^" + _separator + "]*)($|" + _separator + ")");


            //skip first line if hasHeader
            if (_hasHeader && !IsAtEnd)
            {
               
                ReadLine();
            }

            while (!IsAtEnd)
            {
                var currentLine = regex.Matches(ReadLine());

                if (typeof(T).GetProperties().Count() != currentLine.Count - 1)
                {
                    throw new Exception("Column count mismatch in csv file.");
                }
                var columns = new List<string>();
                for (var i = 0; i < currentLine.Count - 1; i++)
                {
                    var currentValue = currentLine[i].Value;
                    currentValue = currentValue.Substring(currentValue.Length - 1) == _separator ?
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
            var csvInstance = new T();
            for (var i = 0; i < typeof(T).GetProperties().Count(); i++)
            {
                var currentProperty = typeof(T).GetProperties().ElementAt(i);
                var currentPropertyType = currentProperty.PropertyType;
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
                    var maxLength = ((MaxLengthAttribute)currentProperty.GetCustomAttributes(typeof(MaxLengthAttribute), true).ElementAt(0)).MaxLength;
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
