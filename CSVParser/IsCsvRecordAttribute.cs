using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvParser
{
    /// <summary>
    /// Classes with this attribute will be accepted as csv types by the CsvParser
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class IsCsvRecordAttribute: System.Attribute 
    {
        public bool IsCsvRecord
        { get; set; }

        public IsCsvRecordAttribute()
        {
            IsCsvRecord = true;
        }

    }
}
