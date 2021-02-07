namespace OurOpenSource.Security.Cryptography
{
    public interface ICryptData
    {
        public byte[] Data { get; }
        public byte[] EcryptedData { get; }
    }
}
