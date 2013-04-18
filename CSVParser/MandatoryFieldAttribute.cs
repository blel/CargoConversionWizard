using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvParser
{
    /// <summary>
    /// Attribute to indicate whether a property is mandatory
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MandatoryFieldAttribute: System.Attribute
    {
        public bool IsMandatoryField
        {
            get;
            set;
        }

        public MandatoryFieldAttribute()
        {
            IsMandatoryField = true;
        }
    }
}
