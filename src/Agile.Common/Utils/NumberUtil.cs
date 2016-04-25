using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Utils
{
    public static class NumberUtil
    {
        
        /// <summary>
        ///  测试结果 执行 10000000 次，
        /// origin  string操作            decimalTruncate
        /// 3.772   3524(Milliseconds)      3757
        /// 3.77    3331                    3758
        /// 3.776   3533                    3758
        /// 3.7    3192                    3503
        /// 10000   1708                    2656
        /// 10000.0001  4867                2941
        /// string 比较长时，性能会变低，频繁的string操作也会对GC造成影响
        /// </summary>
        public static decimal SetScale(this decimal origin, int precision)
        {
            decimal integralValue = Math.Truncate(origin);

            decimal fraction = origin - integralValue;

            decimal factor = (decimal)Math.Pow(10, precision);

            decimal truncatedFraction = Math.Truncate(fraction * factor) / factor;

            decimal result = integralValue + truncatedFraction;

            return result;
        }

        public static decimal SetScale(this decimal origin)
        {
            return SetScale(origin, 2);
        }

        public static decimal? SetScale(this decimal? origin)
        {
            if (origin == null) return null;
            return SetScale(origin.Value, 2);
        }
    }
}
