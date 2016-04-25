using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Utils
{
    public class DateUtil
    {
        public static DateTime GetDayEnd(DateTime time)
        {
            return new DateTime(time.Year, time.Month, time.Day, 23, 59, 59);
        }

        public static DateTime GetDayStart(DateTime time)
        {
            return new DateTime(time.Year, time.Month, time.Day);
        }

        public static int DaysBetween(DateTime newer, DateTime older)
        {
            return (int)(
                new DateTime(newer.Year, newer.Month, newer.Day)
                - new DateTime(older.Year, older.Month, older.Day)
                ).TotalDays;
        }
    }
}
