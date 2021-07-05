using System;
using System.Collections.Generic;
using System.Text;

namespace OurOpenSource.Utility
{
    /// <summary>
    /// Some methods for comparing.
    /// </summary>
    public static class ComparerMethods
    {
        /// <summary>
        /// Compare same length bytes.
        /// </summary>
        /// <param name="x">A byte array for compare.</param>
        /// <param name="y">A byte array for compare.</param>
        /// <returns>
        /// if x as same as y, it will return 0;
        /// if x is greater than y, it will return a number be greater than 0;
        /// if x is less than y, it will return a number be less than 0.
        /// </returns>
        /// <remarks>
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
