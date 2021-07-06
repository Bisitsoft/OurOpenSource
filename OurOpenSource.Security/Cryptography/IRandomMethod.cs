using System;
using System.Collections.Generic;
using System.Text;

namespace OurOpenSource.Security.Cryptography
{
    /// <summary>
    /// 生成随机数的方法。
    /// Method of generating random number.
    /// </summary>
    public interface IRandomMethod
    {
        /// <summary>
        /// 种子字节数。
        /// Seed bytes.
        /// 如果不需要则设置为`0`。
        /// `0` means unnecessary.
        /// </summary>
        int SeedNeedLength { get; }
        /// <summary>
        /// 设置种子。
        /// Set seed
        /// </summary>
        /// <param name="seed">
        /// 随机数种子。
        /// seed of random.
        /// </param>
        void SetSeed(byte[] seed);
        /// <summary>
        /// 获取一个随机的整形。
        /// Get one random `Int32` number.
        /// </summary>
        /// <param name="minValue">
        /// 最小值。
        /// Min value.
        /// </param>
        /// <param name="maxValue">
        /// 最大值。
        /// Max value.
        /// </param>
        /// <returns>
        /// 一个随机的整形。
        /// One random `Int32` number.
        /// </returns>
        int GetInt(int minValue, int maxValue);
        /// <summary>
        /// 获取一个随机的字节数组。
        /// Get one random bytes array.
        /// </summary>
        /// <param name="n">
        /// 字节数。
        /// bytes number.
        /// </param>
        /// <param name="minValue">
        /// 最小值。
        /// Min value.
        /// </param>
        /// <param name="maxValue">
        /// 最大值。
        /// Max value.
        /// </param>
        /// <returns>
        /// 一个随机的字节数组。
        /// one random bytes array.
        /// </returns>
        virtual byte[] GetBytes(int n, byte minValue, byte maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentException("minValue > maxValue");
            }

            int i;
            byte[] r = new byte[n];
            for (i = 0; i < n; i++)
            {
                //unchecked //GetInt不一定正确。
                //{
                r[i] = (byte)GetInt(minValue, maxValue);
                //}
            }
            return r;
        }
    }
}
