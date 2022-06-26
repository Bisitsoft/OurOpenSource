using System;
using System.Collections.Generic;
using System.Text;

namespace OurOpenSource.Security.Cryptography
{
    /// <summary>
    /// 利用`<see cref="System.Guid"/>`生成随机数的方法。
    /// Method of using `<see cref="System.Guid"/>` generate random number.
    /// </summary>
    public class RandomMethod_Guid : IRandomMethod
    {
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
        /// <remarks>
        /// 该行为会重置内部的随机数生成器。
        /// This action will reset inner random number generator.
        /// </remarks>
        public void SetSeed(byte[] seed)
        {
            throw new InvalidOperationException("Needn't seed.");
        }
        /// <summary>
        /// 获取一个随机的`<see cref="Int32"/>`。
        /// Get one random `<see cref="Int32"/>` number.
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
        /// 一个随机的`<see cref="Int32"/>`(`<see cref="Int32.MinValue"/>`~`<see cref="Int32.MaxValue"/>`)。
        /// One random `<see cref="Int32"/>` number(`<see cref="Int32.MinValue"/>`~`<see cref="Int32.MaxValue"/>`).
        /// </returns>
        public int GetInt(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentException("minValue > maxValue");
            }

            long r = Guid.NewGuid().GetHashCode();
            return (int)((r - int.MinValue) % ((long)maxValue - (long)minValue + 1L) + minValue);
        }

        /// <summary>
        /// 构造`<see cref="RandomMethod_Guid"/>`。
        /// Construct `<see cref="RandomMethod_Guid"/>`.
        /// </summary>
        public RandomMethod_Guid()
        {
            ;
        }
    }
}
