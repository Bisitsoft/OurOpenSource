namespace OurOpenSource.Security.Cryptography
{
    public interface ICrypt
    {
        byte[] Encrypt(byte[] data);
        byte[] Decrypt(byte[] encryptedData);
    }
}
