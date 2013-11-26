using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSVParser
{
    public class StreamCsvParser<T> : CsvParserBase<T> where T: new()
    {
        private readonly Stream _source;

        public StreamCsvParser(Stream source, string separator, bool hasHeader) : base(separator, hasHeader)
        {
            _source = source;
        }

        public override bool IsAtEnd
        {
            get { return _source.Peek() == -1; }
        }

        public override string ReadLine()
        {
            return _source.ReadLine();
        }

        public override void DisposeSource()
        {
            _source.Dispose();
        }
    }
}
