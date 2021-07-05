using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;

//参考：https://docs.microsoft.com/zh-cn/dotnet/api/system.net.security.sslstream?view=netcore-3.1

namespace OurOpenSource.Net
{
    public class SslClient : ITransporter
    {
        private TcpClient tcpClient;
        public TcpClient TcpClient { get { return tcpClient; } }
        private X509Certificate serverX509Certificate;
        public X509Certificate ServerX509Certificate { get { return serverX509Certificate; } }
        private X509Certificate clientX509Certificate;
        public X509Certificate ClientX509Certificate { get { return clientX509Certificate; } }
        private bool clientCertificateRequired;
        public bool ClientCertificateRequired { get { return clientCertificateRequired; } }

        private SslStream sslStream;
        public SslStream SslStream { get { return sslStream; } }
        public int Timeout
        {
            set
            {
                sslStream.ReadTimeout = sslStream.WriteTimeout = value;
            }
        }

        public void Stop()
        {
            //sslStream.Close();
            sslStream.Dispose();

            //tcpClient.Close();
            tcpClient.Dispose();
        }

        /// <summary>
        /// 发送数据包。
        /// </summary>
        /// <param name="data">数据。</param>
        public override void Send(byte[] data)
        {
            SslStreamHelper.SendMessage(SslStream, data);
        }

        /// <summary>
        /// 接收数据包。
        /// </summary>
        /// <param name="maxSize">接收的最大字节数。设置为-1代表无限制。</param>
        /// <returns>接收到的数据包。空包则为null。</returns>
        public override byte[] Receive(int maxSize = -1)
        {
            return SslStreamHelper.ReadMessage(sslStream, maxSize);
        }

#warning HERE
        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

#warning HERE
        public SslClient(TcpClient connection, X509Certificate serverCertificate, bool clientCertificateRequired = true, X509Certificate clientCertificate = null)
        {
            this.tcpClient = connection;
            if (serverCertificate == null)
            {
                throw new ArgumentNullException("serverCertificate");
            }
            this.serverX509Certificate = serverCertificate;
            this.clientCertificateRequired = clientCertificateRequired;
            if (this.clientCertificateRequired)
            {
                if (clientCertificate == null)
                {
                    throw new ArgumentNullException("clientCertificate");
                }
            }
            this.clientX509Certificate = clientCertificate;

            this.sslStream = new SslStream(connection.GetStream(), false, (RemoteCertificateValidationCallback)ValidateServerCertificate, null);

            if (this.clientCertificateRequired)
            {
                X509CertificateCollection x509Certificates = null;
                if (this.clientX509Certificate != null)
                {
                    x509Certificates = new X509CertificateCollection();
                    x509Certificates.Add(this.clientX509Certificate);
                }
                this.sslStream.AuthenticateAsClient(serverCertificate.GetName(), x509Certificates, System.Security.Authentication.SslProtocols.Tls13, true);
            }

            this.Timeout = 10_000;
        }
        ~SslClient()
        {
            if (tcpClient.Connected)
            {
                Stop();
            }
        }
    }
}
