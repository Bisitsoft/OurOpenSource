using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OurOpenSource.Security.Cryptography
{
    /// <summary>
    /// Rijndael（AES）算法。
    /// Rijndael(AES)Cryptography.
    /// </summary>
    public class Rijndael : ICryptography
    {
        //参考原文：https://www.cnblogs.com/liqipeng/archive/2013/03/23/4576174.html {

        /// <summary>
        /// SHA256。
        /// SHA256.
        /// </summary>
        private static readonly SHA256 sha256 = SHA256.Create();
        /// <summary>
        /// 默认密钥向量。
        /// Default VI.
        /// </summary>
        private static readonly byte[] d_vi = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// 加密字符串。
        /// Encrypt string.
        /// </summary>
        /// <param name="plainText">
        /// 明文字符串。
        /// Plain text.
        /// </param>
        /// <param name="pswdBytes">
        /// 密钥。
        /// Password.
        /// </param>
        /// <returns>
        /// 加密后的密文。
        /// Encrypted cipher text.
        /// </returns>
        /// <remarks>
        /// 注意，实际使用的是`<paramref name="pswdBytes"/>`的SHA256哈希值。
        /// Caution, in fact, it use SHA256 hash code of `<paramref name="pswdBytes"/>` instead of `<paramref name="pswdBytes"/>`.
        /// 默认使用`<see cref="System.Text.Encoding.UTF8"/>`作为解码器。
        /// Use `<see cref="System.Text.Encoding.UTF8"/>` as default encoder.
        /// </remarks>
        public byte[] EncryptString(string plainText, byte[] pswdBytes)
        {
            byte[] inputByteArray = Encoding.UTF8.GetBytes(plainText);// 得到需要加密的字节数组 Get the byte array need to be encrypted.
            return Encrypt(inputByteArray, pswdBytes);
        }
        /// <summary>
        /// 加密数据。
        /// Encrypt data.
        /// </summary>
        /// <param name="plainData">
        /// 明文数据。
        /// Plain data.
        /// </param>
        /// <param name="pswdBytes">
        /// 密钥。
        /// Password.
        /// </param>
        /// <returns>
        /// 加密后的数据。
        /// Encrypted data.
        /// </returns>
        /// <remarks>
        /// 注意，实际使用的是`<paramref name="pswdBytes"/>`的SHA256哈希值。
        /// Caution, in fact, it use SHA256 hash code of `<paramref name="pswdBytes"/>` instead of `<paramref name="pswdBytes"/>`.
        /// </remarks>
        public byte[] Encrypt(byte[] plainData, byte[] pswdBytes)
        {
            // 分组加密算法。 Block encryption algorithm.
            SymmetricAlgorithm des = System.Security.Cryptography.Rijndael.Create();
            // 设置密钥及密钥向量。 Set key and key vector.
            byte[] _pswd = ProcessPassword(pswdBytes);
            des.KeySize = _pswd.Length * 8;
            des.Key = _pswd;
            des.IV = d_vi;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(plainData, 0, plainData.Length);
            cs.FlushFinalBlock();
            byte[] cipherBytes = ms.ToArray();// 得到加密后的字节数组。 Get the encrypted byte array.
            cs.Close();
            ms.Close();
            return cipherBytes;
        }

        /// <summary>
        /// 解密字符串。
        /// Decrypt string.
        /// </summary>
        /// <param name="cipherText">
        /// 密文。
        /// Cipher text.
        /// </param>
        /// <param name="pswdBytes">
        /// 密钥。
        /// Password.
        /// </param>
        /// <returns>
        /// 解密后的密文。
        /// Decrypted cipher text.
        /// </returns>
        /// <remarks>
        /// 注意，实际使用的是`pswdBytes`的SHA256哈希值。
        /// Caution, in fact, it use SHA256 hash code of `pswdBytes` instead of `pswdBytes`.
        /// 默认使用`System.Text.Encoding.UTF8`作为解码器。
        /// Use `System.Text.Encoding.UTF8` as default encoder.
        /// </remarks>
        public string DecryptString(byte[] cipherText, byte[] pswdBytes)
        {
            return Encoding.UTF8.GetString(Decrypt(cipherText,pswdBytes));
        }
        /// <summary>
        /// 解密数据。
        /// Decrypt data.
        /// </summary>
        /// <param name="cipherData">
        /// 密文数据。
        /// Cipher data.
        /// </param>
        /// <param name="pswdBytes">
        /// 密钥。
        /// Password.
        /// </param>
        /// <returns>
        /// 解密后的明文数据。
        /// Decrypted plain data.
        /// </returns>
        /// <remarks>
        /// 注意，实际使用的是`pswdBytes`的SHA256哈希值。
        /// Caution, in fact, it use SHA256 hash code of `pswdBytes` instead of `pswdBytes`.
        /// </remarks>
        public byte[] Decrypt(byte[] cipherData, byte[] pswdBytes)
        {
            SymmetricAlgorithm des = System.Security.Cryptography.Rijndael.Create();
            byte[] _pswd = ProcessPassword(pswdBytes);
            des.KeySize = _pswd.Length * 8;
            des.Key = _pswd;
            des.IV = d_vi;
            byte[] decryptBytes = new byte[cipherData.Length];
            MemoryStream ms = new MemoryStream(cipherData);
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read);
            cs.Read(decryptBytes, 0, decryptBytes.Length);
            cs.Close();
            ms.Close();
            return decryptBytes;
        }

        /// <summary>
        /// 获取密码的SHA256哈希值。
        /// Get SHA256 hash code of password.
        /// </summary>
        /// <param name="password">
        /// 密码。
        /// Password.
        /// </param>
        /// <returns>
        /// 密码的SHA256哈希值。
        /// SHA256 hash code of password.
        /// </returns>
        private byte[] ProcessPassword(byte[] password)
        {
            // 不管128、92，直接256。 However, we use 256.
            return sha256.ComputeHash(password);
        }
        //}
    }
}
