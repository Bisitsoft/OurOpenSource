using System;
using System.Collections.Generic;
using System.Text;

using OurOpenSource.Security.Cryptography;

namespace OurOpenSource.Security
{
    /// <summary>
    /// 秘密数据。
    /// Secret data.
    /// </summary>
    /// <remarks>
    /// 只具有保密性，不能保护数据完整性。
    /// It only has confidentiality and cannot protect data integrity.
    /// </remarks>
    public class SecretData
    {
        private byte[] data;
        /// <summary>
        /// 加密的数据。
        /// Encrypted data.
        /// </summary>
        /// <remarks>
        /// 结构：     |   随机数据长度   |  真实数据长度  |[      乱序的随机数据位置       ]|[   真实数据与随机数据   ]|
        /// Structure: |Random data length|Real data length|[Disorderly random data position]|[Real data &amp; random data ]|
        /// </remarks>
        public byte[] EncryptedData { get { return data; } }
        /// <summary>
        /// 解密数据。
        /// Decrypt data.
        /// </summary>
        /// <param name="password">
        /// 密码。
        /// Password.
        /// </param>
        /// <returns>
        /// 解密后的数据。
        /// Decrypted data.
        /// </returns>
        public byte[] GetData(byte[] password)
        {
            byte[] data = new Rijndael().Decrypt(this.data, password);
            int i, n;
            long dIndex, rIndex, dCount;
            byte[] bytes;

            n = BitConverter.ToInt32(data, 0);
            dIndex = sizeof(int);

            bytes = new byte[sizeof(long)];
            Array.Copy(data, dIndex, bytes, 0, bytes.Length);
            byte[] r = new byte[BitConverter.ToInt64(bytes)];
            dIndex += bytes.Length;

            List<long> insertPositions = new List<long>(n);
            bytes = new byte[sizeof(long)];
            for (i = 0; i < n; i++)
            {
                Array.Copy(data, dIndex, bytes, 0, sizeof(long));
                insertPositions.Add(BitConverter.ToInt64(bytes, 0));
                dIndex += sizeof(long);
            }
            insertPositions.Sort();// 默认是升序，即由小到大。 The default is ascending, that is, from small to large.

            rIndex = 0;
            for (i = 0; i < n; i++)
            {
                dCount = insertPositions[i] - rIndex - i;
                Array.Copy(data, dIndex, r, rIndex, dCount);
                rIndex += dCount;
                dIndex += dCount + 1;
            }
            Array.Copy(data, dIndex, r, rIndex, r.LongLength - rIndex);
            return r;
        }

        /// <summary>
        /// 初始化秘密数据。
        /// Initialize secret string.
        /// </summary>
        /// <param name="plainData">
        /// 需要加密的明文字符串。
        /// The plain text need to encrypt.
        /// </param>
        /// <param name="randomMethod">
        /// 生成随机数的函数。
        /// Method of generate random number.
        /// </param>
        /// <param name="password">
        /// 密码。
        /// Password.
        /// </param>
        /// <param name="maxRandomDataSize">
        /// 用于加密的随机数据大小。单位是一个随机数据块，不是字节。
        /// Size of random data of encryption. The unit of size is one random data block, but not byte.
        /// </param>
        public SecretData(byte[] plainData, IRandomMethod randomMethod, byte[] password, int maxRandomDataSize = 8)
        {
            SetData(plainData, randomMethod, password, maxRandomDataSize);
        }
        /// <summary>
        /// 仅为了开发方便而设置的构造函数。
        /// Consrtuct method setted for easyily developing.
        /// 请调用`<see cref="SetData(byte[], IRandomMethod, byte[], int)"/>`方法来写入加密数据。
        /// Please call `<see cref="SetData(byte[], IRandomMethod, byte[], int)"/>` to write secret data.
        /// </summary>
        protected SecretData() {; }
        //maxRandomDataSize一个为2个int大小
        /// <summary>
        /// 设置数据。
        /// Set data.
        /// </summary>
        /// <param name="plainData">
        /// 需要加密的明文字符串。
        /// The plain text need to encrypt.
        /// </param>
        /// <param name="randomMethod">
        /// 生成随机数的函数。
        /// Method of generate random number.
        /// </param>
        /// <param name="password">
        /// 密码。
        /// Password.
        /// </param>
        /// <param name="maxRandomDataSize">
        /// 用于加密的随机数据大小。单位是一个随机数据块，不是字节。
        /// Size of random data of encryption. The unit of size is one random data block, but not byte.
        /// </param>
        protected void SetData(byte[] plainData, IRandomMethod randomMethod, byte[] password, int maxRandomDataSize = 8)
        {
            // 检查参数。 Check arguments.
            if (maxRandomDataSize < 1)
            {
                throw new ArgumentException("maxRandomDataSize should be greater than 0.", "maxRandomDataSize");
            }

            int i, n;
            // 生成随机数据长度。 Generate random data length.
            if (maxRandomDataSize > plainData.LongLength + 1)// 此处设计的必要性暂且不讨论。 The necessity of design here will not be discussed for the time being.
            {
                n = randomMethod.GetInt(1, (int)(plainData.LongLength + 1));// 加1是为了避免产生0的空数据。 Add 1 to avoid generating null data of 0.
            }
            else
            {
                n = randomMethod.GetInt(1, maxRandomDataSize);
            }
            long insertPosition, dIndex, plainDataIndex, pdCount; // plainDataIndex: 下一个读取位置 Next plain data index.
            byte[] data = new byte[sizeof(int) + sizeof(long) + (sizeof(long) + sizeof(byte)) * n + plainData.LongLength];

            // 写入随机数据长度。 Write random data length.
            byte[] bytes = BitConverter.GetBytes(n);
            Array.ConstrainedCopy(bytes, 0, data, 0, bytes.Length);
            dIndex = bytes.Length;

            // 写入真实数据长度。 Write real data length.
            bytes = BitConverter.GetBytes(plainData.LongLength);
            Array.Copy(bytes, 0, data, dIndex, bytes.Length);
            dIndex += bytes.Length;

            List<long> insertPositions = new List<long>(n);
            // 生成乱序的随机数据位置。 Generate Disorderly random data position
            for (i = 0; i < n; i++)
            {
                do
                {
                    insertPosition = GetRandomLong(randomMethod, 0, plainData.LongLength + i);
                } while (insertPositions.Contains(insertPosition));
                insertPositions.Add(insertPosition);
            }
            // 写入乱序的随机数据位置。 Write Disorderly random data position
            List<long> insertPositions_Copy = new List<long>(insertPositions.ToArray());
            for (i = 0; i < n; i++)
            {
                bytes = BitConverter.GetBytes(insertPositions_Copy[i]);
                Array.Copy(bytes, 0, data, dIndex, bytes.Length);
                dIndex += bytes.Length;
            }
            insertPositions_Copy = null; // 标记为可回收。 Marked GC-able.
            insertPositions.Sort();// 默认是升序，即由小到大。 The default is ascending, that is, from small to large.

            // 写入真实数据与随机数据。 Write real data & random data.
            plainDataIndex = 0;
            for (i = 0; i < n; i++)
            {
                pdCount = insertPositions[i] - plainDataIndex - i;
                Array.Copy(plainData, plainDataIndex, data, dIndex, pdCount);
                plainDataIndex += pdCount;
                dIndex += pdCount;
                data[dIndex] = (byte)randomMethod.GetInt(byte.MinValue, byte.MaxValue);
                dIndex++;
            }
            Array.Copy(plainData, plainDataIndex, data, dIndex, plainData.LongLength - plainDataIndex);

            this.data = new Rijndael().Encrypt(data, password);
        }
        /// <summary>
        /// 获取一个随机的`<see langword="long"/>`数值。
        /// Get a random `<see langword="long"/>` value.
        /// </summary>
        /// <param name="randomMethod">
        /// 生成随机数的函数。
        /// Method of generate random number.
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
        /// 一个随机的`<see langword="long"/>`数值。
        /// A random `<see langword="long"/>` value.
        /// </returns>
        /// <remarks>
        /// 不检查<paramref name="minValue"/>是否小于<paramref name="maxValue"/>。
        /// Won't check whether is <paramref name="minValue"/> less than <paramref name="maxValue"/>.
        /// </remarks>
        protected static long GetRandomLong(IRandomMethod randomMethod, long minValue, long maxValue)
        {
            byte[] longBytes = randomMethod.GetBytes(sizeof(long), byte.MinValue, byte.MaxValue);
            decimal r = BitConverter.ToInt64(longBytes);
            return (byte)((r - long.MinValue) % (maxValue - minValue + 1) + minValue);
        }
    }
}
