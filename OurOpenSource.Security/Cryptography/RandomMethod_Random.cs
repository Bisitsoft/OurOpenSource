using System;
using System.Collections.Generic;
using System.Text;

namespace OurOpenSource.Security.Cryptography
{
    /// <summary>
    /// 利用`System.Random`生成随机数的方法。
    /// Method of using `System.Random` generate random number.
    /// </summary>
    public class RandomMethod_Random : IRandomMethod
    {
        /// <summary>
        /// 随机数。
        /// Random.
        /// </summary>
        private Random random;

        /// <summary>
        /// 种子字节数。
        /// Seed bytes.
        /// </summary>
        public int SeedNeedLength { get { return sizeof(int); } }
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
            SetSeed(BitConverter.ToInt32(seed));
        }
        /// <summary>
        /// 设置种子。
        /// Set seed
        /// </summary>
        /// <param name="seed">
        /// 随机数种子。
        /// seed of random.
        /// </param>
        public void SetSeed(int seed)
        {
            random = new Random(seed);
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
        /// 一个随机的整形(0~Int32.MaxValue)。
        /// One random `Int32` number(0~Int32.MaxValue).
        /// </returns>
        public int GetInt(int minValue, int maxValue)
        {
			if (maxValue == int.MaxValue)
			{
                if(random.Next(0, 46341) == 0 && random.Next(0, 46341) == 0) // floor(2^(15.5))+1=46341
				{
                    return int.MaxValue;
				}
				else
				{
                    return random.Next(minValue, int.MaxValue);
				}
			}
            return random.Next(minValue, maxValue + 1);
        }

        /// <summary>
        /// 构造默认RandomMethod_Random。
        /// Construct default RandomMethod_Random.
        /// </summary>
        public RandomMethod_Random()
        {
            this.random = new Random();
        }
        /// <summary>
        /// 用随机数种子构造RandomMethod_Random。
        /// Construct RandomMethod_Random with seed.
        /// </summary>
        /// <param name="seed"></param>
        public RandomMethod_Random(int seed)
        {
            this.random = new Random(seed);
        }
    }
}
