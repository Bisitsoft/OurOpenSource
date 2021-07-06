using System;
using System.Collections.Generic;
using System.Text;

namespace OurOpenSource.Security.Cryptography
{
    /// <summary>
    /// 利用`System.Guid`生成随机数的方法。
    /// Method of using `System.Guid` generate random number.
    /// </summary>
    public class RandomMethod_Guid : IRandomMethod
    {
        /// <summary>
        /// 种子字节数。
        /// Seed bytes.
        /// </summary>
        public int SeedNeedLength { get { return 0; } }
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

            long r = Guid.NewGuid().GetHashCode();
            return (int)((r - int.MinValue) % (maxValue - minValue + 1) + minValue);
        }

        public RandomMethod_Guid()
        {
            ;
        }
    }
}
