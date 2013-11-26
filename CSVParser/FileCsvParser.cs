using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSVParser
{
    public class FileCsvParser<T> : CsvParserBase<T> where T : new()
    {
        private readonly TextReader _textReader;

        /// <summary>
        /// Returns true if source is at end
        /// </summary>
        public override bool IsAtEnd
        {
            get
            {
                return _textReader.Peek() == -1;
            }

        }

        /// <summary>
        /// The constructor takes the filename, separator and hasHeader as parameters.
        /// 
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <param name="separator"></param>
        /// <param name="hasHeader"></param>
        public FileCsvParser(string fullFileName, string separator, bool hasHeader) : base(separator, hasHeader)
        {
            _textReader = File.OpenText(fullFileName);
        }
        
        /// <summary>
        /// Implementation of the ReadLine helper method of the Read Template
        /// </summary>
        /// <returns></returns>
        public override string ReadLine()
        {
            return _textReader.ReadLine();
        }

        /// <summary>
        /// Called by the template method to dispose the source
        /// </summary>
        public override void DisposeSource()
        {
            _textReader.Dispose();

        }

    }
}

