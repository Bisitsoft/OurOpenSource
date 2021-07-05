using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace OurOpenSource.Net
{
    public class SslStreamHelper
    {
        public static void SendMessage(SslStream sslStream, byte[] data)
        {
            sslStream.Write(BitConverter.GetBytes(data.Length));
            sslStream.Write(data);
        }

        public static byte[] ReadMessage(SslStream sslStream, int maxSize = -1)
        {
            if (maxSize == 0)
            {
                throw new InvalidOperationException("Can't recive any data when the maxSize is 0.");
            }

            //接收数据包大小值
            byte[] temp = BitConverter.GetBytes(0);
            sslStream.Read(temp, 0, temp.Length);
            int length = BitConverter.ToInt32(temp, 0);
            if (length < 0)
            {
                throw new ArgumentException("The data length less than 0.");
            }
            else if (length == 0)
            {
                //一个空包
                return null;
            }
            else if (length > maxSize)
            {
                throw new ArgumentException(String.Format("The data length greater than maxSize(={0}).", maxSize));
            }

            //接收数据包
            temp = new byte[length];
            sslStream.Read(temp, 0, temp.Length);

            return temp;
        }


        
        //docs文档中抠下来的

        public static void DisplaySecurityLevel(SslStream stream)
        {
            Console.WriteLine("Cipher: {0} strength {1}", stream.CipherAlgorithm, stream.CipherStrength);
            Console.WriteLine("Hash: {0} strength {1}", stream.HashAlgorithm, stream.HashStrength);
            Console.WriteLine("Key exchange: {0} strength {1}", stream.KeyExchangeAlgorithm, stream.KeyExchangeStrength);
            Console.WriteLine("Protocol: {0}", stream.SslProtocol);
        }
        public static void DisplaySecurityServices(SslStream stream)
        {
            Console.WriteLine("Is authenticated: {0} as server? {1}", stream.IsAuthenticated, stream.IsServer);
            Console.WriteLine("IsSigned: {0}", stream.IsSigned);
            Console.WriteLine("Is Encrypted: {0}", stream.IsEncrypted);
        }
        public static void DisplayStreamProperties(SslStream stream)
        {
            Console.WriteLine("Can read: {0}, write {1}", stream.CanRead, stream.CanWrite);
            Console.WriteLine("Can timeout: {0}", stream.CanTimeout);
        }
        public static void DisplayCertificateInformation(SslStream stream)
        {
            Console.WriteLine("Certificate revocation list checked: {0}", stream.CheckCertRevocationStatus);

            X509Certificate localCertificate = stream.LocalCertificate;
            if (stream.LocalCertificate != null)
            {
                Console.WriteLine("Local cert was issued to {0} and is valid from {1} until {2}.",
                    localCertificate.Subject,
                    localCertificate.GetEffectiveDateString(),
                    localCertificate.GetExpirationDateString());
            }
            else
            {
                Console.WriteLine("Local certificate is null.");
            }
            // Display the properties of the client's certificate.
            X509Certificate remoteCertificate = stream.RemoteCertificate;
            if (stream.RemoteCertificate != null)
            {
                Console.WriteLine("Remote cert was issued to {0} and is valid from {1} until {2}.",
                    remoteCertificate.Subject,
                    remoteCertificate.GetEffectiveDateString(),
                    remoteCertificate.GetExpirationDateString());
            }
            else
            {
                Console.WriteLine("Remote certificate is null.");
            }
        }
    }
}
