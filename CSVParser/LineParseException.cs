using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvParser
{
    /// <summary>
    /// Exception containing the column at which the conversion exception ocurred.
    /// </summary>
    public class LineParseException:Exception 
    {
        private int _column;
        
        public int Column
        {
            get
            { return _column; }
        }
        public LineParseException(string message, int column)
            : base(message)
        {
            _column = column;
        }

    }
}
