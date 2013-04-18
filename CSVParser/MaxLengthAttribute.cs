using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvParser
{
    /// <summary>
    /// Attribute to indicate the maxlength of a csv field
    /// </summary>
    public class MaxLengthAttribute:System.Attribute 
    {
        private int _maxLength;

        public int MaxLength
        {
            get { return _maxLength; }
        }

        public MaxLengthAttribute(int maxLength)
        {
            _maxLength = maxLength;
        }
    }
}
