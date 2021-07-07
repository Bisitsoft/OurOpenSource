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
    public class Rijndael : ICryptography
    {
        //参考原文：https://www.cnblogs.com/liqipeng/archive/2013/03/23/4576174.html {

        /// <summary>
        /// SHA256。
        /// SHA256.
        /// </summary>
        private static SHA256 sha256 = SHA256.Create();
        /// <summary>
        /// 默认密钥向量。
        /// </summary>
        private static byte[] d_vi = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /// <summary>
        /// AES加密算法。
        /// </summary>
        /// <param name="plainText">明文字符串<。/param>
        /// <param name="pswdBytes">密钥。</param>
        /// <returns>加密后的密文字节数组。</returns>
        public byte[] EncryptString(string plainText, byte[] pswdBytes)
        {
            byte[] inputByteArray = Encoding.UTF8.GetBytes(plainText);//得到需要加密的字节数组
            return Encrypt(inputByteArray, pswdBytes);
        }
        /// <summary>
        /// AES加密算法。
        /// </summary>
        /// <param name="plainData">明文数据。</param>
        /// <param name="pswdBytes">密钥。</param>
        /// <returns>加密后的密文字节数组。</returns>
        public byte[] Encrypt(byte[] plainData, byte[] pswdBytes)
        {
            //分组加密算法
            SymmetricAlgorithm des = System.Security.Cryptography.Rijndael.Create();
            //设置密钥及密钥向量
            byte[] _pswd = ProcessPassword(pswdBytes);
            des.KeySize = _pswd.Length * 8;
            des.Key = _pswd;
            des.IV = d_vi;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(plainData, 0, plainData.Length);
            cs.FlushFinalBlock();
            byte[] cipherBytes = ms.ToArray();//得到加密后的字节数组
            cs.Close();
            ms.Close();
            return cipherBytes;
        }
        private byte[] ProcessPassword(byte[] password)
        {
            //不管128、92，直接256
            return sha256.ComputeHash(password);
        }

        /// <summary>
        /// AES解密。
        /// </summary>
        /// <param name="cipherText">密文字节数组。</param>
        /// <param name="pswdBytes">密钥。</param>
        /// <returns>解后的字符串。</returns>
        public string DecryptString(byte[] cipherText, byte[] pswdBytes)
        {
            return Encoding.UTF8.GetString(Decrypt(cipherText,pswdBytes));
        }
        /// <summary>
        /// AES解密。
        /// </summary>
        /// <param name="cipherData">密文字节数组。</param>
        /// <param name="pswdBytes">密钥。</param>
        /// <returns>解密后的明文数据。</returns>
        public byte[] Decrypt(byte[] cipherData, byte[] pswdBytes)
        {
            SymmetricAlgorithm des = System.Security.Cryptography.Rijndael.Create();
            des.KeySize = pswdBytes.Length * 8;
            des.Key = pswdBytes;
            des.IV = d_vi;
            byte[] decryptBytes = new byte[cipherData.Length];
            MemoryStream ms = new MemoryStream(cipherData);
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read);
            cs.Read(decryptBytes, 0, decryptBytes.Length);
            cs.Close();
            ms.Close();
            return decryptBytes;
        }
        //}
    }
}
