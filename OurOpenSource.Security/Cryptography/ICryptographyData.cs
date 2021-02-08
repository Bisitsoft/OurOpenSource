namespace OurOpenSource.Security.Cryptography
{
    public interface ICryptographyData
    {
        public byte[] Data { get; }
        public byte[] EcryptedData { get; }
    }
}
