using System;
using System.Collections.Generic;
using System.Text;

using OurOpenSource.Security.Cryptography;

namespace OurOpenSource.Security
{
    /// <summary>
    /// 秘密字符串。
    /// Secret string.
    /// </summary>
    /// <remarks>
    /// 只具有保密性，不能保护数据完整性。
    /// It only has confidentiality and cannot protect data integrity.
    /// </remarks>
    public class SecretString : SecretData
    {
        /// <summary>
        /// 解密字符串。
        /// Decrypt string.
        /// </summary>
        /// <param name="password">
        /// 密码。
        /// Password.
        /// </param>
        /// <returns>
        /// 解密后的字符串。
        /// Decrypted string.
        /// </returns>
        public new string GetData(byte[] password)
        {
            return System.Text.Encoding.UTF8.GetString(base.GetData(password));
        }
        /// <summary>
        /// 解密并用指定解码器获取字符串。
        /// Decrypt and encoding string with designative encoder.
        /// </summary>
        /// <param name="password">
        /// 密码。
        /// Password.
        /// </param>
        /// <param name="encoding">
        /// 指定的解码器
        /// Designative encoder.
        /// </param>
        /// <returns>
        /// 解密后的字符串。
        /// Decrypted string.
        /// </returns>
        public string GetData(byte[] password, Encoding encoding)
        {
            return encoding.GetString(base.GetData(password));
        }

        /// <summary>
        /// 初始化秘密字符串。
        /// Initialize secret string.
        /// </summary>
        /// <param name="plainText">
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
        public SecretString(string plainText, IRandomMethod randomMethod, byte[] password, int maxRandomDataSize = 8) : base()
        {
            byte[] plainData = System.Text.Encoding.UTF8.GetBytes(plainText);
            base.SetData(plainData, randomMethod, password, maxRandomDataSize);
        }
        /// <summary>
        /// 用指定的解码器初始化秘密字符串。
        /// Initialize secret string with designative encoder.
        /// </summary>
        /// <param name="plainText">
        /// 需要加密的明文字符串。
        /// The plain text need to encrypt.
        /// </param>
        /// <param name="encoding">
        /// 字符串的解码器。
        /// Encoder of string.
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
        public SecretString(string plainText, Encoding encoding, IRandomMethod randomMethod, byte[] password, int maxRandomDataSize = 8) : base()
        {
            byte[] plainData = encoding.GetBytes(plainText);
            base.SetData(plainData, randomMethod, password, maxRandomDataSize);
        }
    }
}
