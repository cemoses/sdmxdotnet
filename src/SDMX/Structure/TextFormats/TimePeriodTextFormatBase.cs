using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Common;

namespace SDMX
{
    public abstract class TimePeriodTextFormatBase : TextFormat
    {

        public virtual string Serialize(object obj, out string startTime)
        {
            if (obj is DateTime)
                obj = FromDateTime((DateTime)obj);
            if (obj is DateTime? && ((DateTime?)obj).HasValue)
                obj = FromDateTime(((DateTime?)obj).Value);

            startTime = null;
            return Converter.ToXml(obj);
        }

        public override bool IsValid(object obj)
        {
            return obj is DateTime || obj is DateTimeOffset;
        }

        public override Type GetValueType()
        {
            return typeof(DateTimeOffset);
        }

        DateTimeOffset FromDateTime(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime.Ticks, TimeSpan.FromTicks(0));
        }
    }
}