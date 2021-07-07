namespace OurOpenSource.Security.Cryptography
{
    /// <summary>
    /// 加密算法。
    /// Cryptography.
    /// </summary>
    public interface ICryptography
    {
        /// <summary>
        /// 加密。
        /// Do encrypt.
        /// </summary>
        /// <param name="data">
        /// 需要被加密的数据。
        /// The data need to encrypt.
        /// </param>
        /// <param name="password">
        /// 用于加密的密码。
        /// The password for encrypt.
        /// </param>
        /// <returns>
        /// 加密后的数据。
        /// Encrypted data.
        /// </returns>
        byte[] Encrypt(byte[] data, byte[] password);
        /// <summary>
        /// 解密。
        /// Do decrypt.
        /// </summary>
        /// <param name="encryptedData">
        /// 需要被解密的数据。
        /// The data need to decrypt.
        /// </param>
        /// <param name="password">
        /// 用于解密的密码
        /// The password for decrypt.
        /// </param>
        /// <returns>
        /// 解密后的数据。
        /// Decrypted data.
        /// </returns>
        byte[] Decrypt(byte[] encryptedData, byte[] password);
    }
}
