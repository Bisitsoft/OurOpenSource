namespace OurOpenSource.Security.Cryptography
{
    public interface ICryptography
    {
        /// <summary>
        /// Do encrypt.
        /// </summary>
        /// <param name="data">The data need to encrypt.</param>
        /// <param name="password">The password for encrypt.</param>
        /// <returns>Encrypted data.</returns>
        byte[] Encrypt(byte[] data, byte[] password);
        /// <summary>
        /// Do decrypt.
        /// </summary>
        /// <param name="encryptedData">The data need to decrypt.</param>
        /// <param name="password">The password for decrypt.</param>
        /// <returns>Decrypted data.</returns>
        byte[] Decrypt(byte[] encryptedData, byte[] password);
    }
}
