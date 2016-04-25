using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Agile.Common.Exceptions
{
    public static class Require
    {
        public static void EatException(Action act)
        {
            try
            {
                act.Invoke();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        /// <summary>
        /// 指定值不为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">指定值</param>
        /// <param name="name">名称</param>
        public static void NotNullOrEmpty<T>(T? value, string name) where T : struct
        {
            if (value == null)
            {
                throw new BusinessException($"{name}不能为空");
            }
        }
        /// <summary>
        /// 指定值不为空
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void NotNullOrEmpty(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new BusinessException($"{name}不能为空");
            }
        }
        /// <summary>
        /// 指定数组不为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void NotNullOrEmpty<T>(T[] value, string name)
        {
            if (value == null || value.Length == 0)
            {
                throw new BusinessException($"{name}不能为空");
            }
        }
        /// <summary>
        /// 指定值不为null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void NotNull<T>(T value, string name) where T : class
        {
            if (value == null)
            {
                throw new BusinessException($"{name}不能为空");
            }
        }
        /// <summary>
        /// 指定decimal比0大
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void LargerThanZero(decimal value, string name)
        {
            if (!(value > 0))
            {
                throw new BusinessException($"{name}必须大于0");
            }
        }
        /// <summary>
        /// 指定int比0大
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void LargerThanZero(int value, string name)
        {
            if (!(value > 0))
            {
                throw new BusinessException($"{name}必须大于0");
            }
        }

        public static void RangeWithin(decimal value, decimal min, decimal max, string name)
        {
            if (!(value > min && value < max))
            {
                throw new BusinessException($"{name}必须在 {min}~{max}之间");
            }
        }

        public static void RangeWithin(int value, int min, int max, string name)
        {
            if (!(value > min && value < max))
            {
                throw new BusinessException($"{name}必须在 {min}~{max}之间");
            }
        }

        public static void LengthWithin<T>(IEnumerable<T> collection, int minSize, int maxSize, string name)
        {
            if (collection == null)
            {
                return;
            }
            var count = collection.Count();
            if (!(count > minSize && count < maxSize))
            {
                throw new BusinessException($"{name}的长度必须在{minSize}~{maxSize}之间");
            }
        }

        public static void LengthWithin<T>(IEnumerable<T> collection, int maxSize, string name)
        {
            if (collection == null)
            {
                return;
            }
            var count = collection.Count();
            if (count >= maxSize)
            {
                throw new BusinessException($"{name}的长度必须小于{maxSize}");
            }
        }
    }
}
