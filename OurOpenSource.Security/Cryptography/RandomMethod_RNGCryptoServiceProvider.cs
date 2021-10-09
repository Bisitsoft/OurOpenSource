using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace OurOpenSource.Security.Cryptography
{
    /// <summary>
    /// 利用`System.Security.Cryptography.RNGCryptoServiceProvider`生成随机数的方法。
    /// Method of using `System.Security.Cryptography.RNGCryptoServiceProvider` generate random number.
    /// </summary>
    public class RandomMethod_RNGCryptoServiceProvider : IRandomMethod
    {
        /// <summary>
        /// RNG Crypto Service Provider。
        /// RNG Crypto Service Provider.
        /// </summary>
        private static readonly RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();

        /// <summary>
        /// 种子字节数。
        /// Seed bytes.
        /// </summary>
        public int SeedNeedLength { get { return 0; } }
        /// <summary>
        /// 设置种子。
        /// Set seed
        /// </summary>
        /// <param name="seed">
        /// 随机数种子。
        /// seed of random.
        /// </param>
        public void SetSeed(byte[] seed)
        {
            throw new InvalidOperationException("Needn't seed.");
        }
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
        /// 一个随机的整形(Int32.MinValue~Int32.MaxValue)。
        /// One random `Int32` number(Int32.MinValue~Int32.MaxValue).
        /// </returns>
        public int GetInt(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentException("minValue > maxValue");
            }

            byte[] intBytes = new byte[sizeof(int)];
            rngCryptoServiceProvider.GetBytes(intBytes, 0, intBytes.Length);
            long r = BitConverter.ToInt32(intBytes, 0);
            return (int)((r - int.MinValue) % ((long)maxValue - (long)minValue + 1L) + minValue);
        }

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
        public byte[] GetBytes(int n, byte minValue, byte maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentException("minValue > maxValue");
            }

            int i;
            byte[] r = new byte[n];
            rngCryptoServiceProvider.GetBytes(r, 0, r.Length);
            for (i = 0; i < n; i++)
            {
                r[i]= (byte)((((int)r[i]) - byte.MinValue) % (maxValue - minValue + 1) + minValue);
            }
            return r;
        }

        /// <summary>
        /// 构造RandomMethod_RNGCryptoServiceProvider。
        /// Construct RandomMethod_RNGCryptoServiceProvider.
        /// </summary>
        public RandomMethod_RNGCryptoServiceProvider()
        {
            ;
        }
    }
}
