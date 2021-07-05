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
    //本地要有cert，再从服务器验证cert，不对即停止
    public class SslServer : ITransporter
    {
        private TcpClient tcpClient;
        public TcpClient TcpClient { get { return tcpClient; } }
        private X509Certificate x509Certificate;
        public X509Certificate X509Certificate { get { return x509Certificate; } }
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

        public SslServer(TcpClient connection, bool clientCertificateRequired = true, X509Certificate certificate = null)
        {
            this.tcpClient = connection;
            //serverCertificate = X509Certificate.CreateFromCertFile(certificate);
            this.x509Certificate = certificate;
            if (this.clientCertificateRequired)
            {
                if (certificate == null)
                {
                    throw new ArgumentNullException("certificate");
                }
            }
            this.clientCertificateRequired = clientCertificateRequired;

            this.sslStream = new SslStream(connection.GetStream(), false);

            this.sslStream.AuthenticateAsServer(certificate, clientCertificateRequired, System.Security.Authentication.SslProtocols.Tls13, true);

            this.Timeout = 10_000;
        }
        ~SslServer()
        {
            if (tcpClient.Connected)
            {
                Stop();
            }
        }
    }
}
