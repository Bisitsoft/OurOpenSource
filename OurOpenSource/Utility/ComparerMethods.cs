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
        /// 如果`<paramref name="x"/>`与`<paramref name="y"/>相等则返回0；
        /// if `<paramref name="x"/>` as same as `<paramref name="y"/>`, it will return `0`;
        /// 如果`<paramref name="x"/>`大于`<paramref name="y"/>`，则返回一个大于`0`的值；
        /// if `<paramref name="x"/>` is greater than `<paramref name="y"/>`, it will return a value be greater than `0`;
        /// 如果`<paramref name="x"/>`小于`<paramref name="y"/>`，则返回一个小于`0`的值。
        /// if `<paramref name="x"/>` is less than `<paramref name="y"/>`, it will return a value be less than `0`.
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
