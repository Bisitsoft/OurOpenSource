using System;
using System.Collections.Generic;
using System.Text;

namespace OurOpenSource.Utility
{
    /// <summary>
    /// 一些用于比较的方法。
    /// Some methods for compare.
    /// </summary>
    public static class ComparerMethods
    {
        /// <summary>
        /// 比较长度相同的字节数组。
        /// Compare same length bytes.
        /// </summary>
        /// <param name="x">
        /// 用于比较的字节数组。
        /// A byte array for compare.
        /// </param>
        /// <param name="y">
        /// 用于比较的字节数组。
        /// A byte array for compare.
        /// </param>
        /// <returns>
        /// 如果x与y相等则返回0；
        /// if `x` as same as `y`, it will return 0;
        /// 如果`x`大于`y`，则返回一个大于0的值；
        /// if `x` is greater than `y`, it will return a value be greater than 0;
        /// 如果`x`小于`y`，则返回一个小于0的值。
        /// if `x` is less than `y`, it will return a value be less than 0.
        /// </returns>
        /// <remarks>
        /// 如果你想要通过填充短一些的字节数组来进行比较，请搞清楚他们的字节序。
        /// If you want to fill the shorter array for compare, please make sure their endian.
        /// </remarks>
        public static int ComapreSameLengthByteArray(byte[] x, byte[] y)
        {
            int i, temp;
            if (x.Length != y.Length)
            {
                throw new ArgumentException("x.Length not equals y.Length .");
            }
            for (i = 0; i < x.Length; i++)
            {
                temp = (int)((short)(x[i]) - (short)(y[i]));
                if(temp != 0)
                {
                    return temp;
                }
            }
            return 0;
        }
    }
}
