namespace OurOpenSource.Security.Cryptography
{
    public interface ICryptography
    {
        byte[] Encrypt(byte[] data);
        byte[] Decrypt(byte[] encryptedData);
    }
}
