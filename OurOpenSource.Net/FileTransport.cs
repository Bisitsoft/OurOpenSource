using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OurOpenSource.Net
{
    /// <summary>
    /// 一个简易的传输文件类。
    /// A easy class for sending files.
    /// </summary>
    public class FileTransport
    {
        /// <summary>
        /// 该类使用的传输器。
        /// The using transporter of this class.
        /// </summary>
        private ITransporter transporter = null;
        /// <summary>
        /// 该类使用的传输器。
        /// The using transporter of this class.
        /// </summary>
        public ITransporter Transporter { get { return transporter; } }

        /// <summary>
        /// 发送文件。
        /// Send file.
        /// </summary>
        /// <param name="path">
        /// 源文件路径。
        /// The path of source file.
        /// </param>
        public void Send(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            byte[] buffer = new byte[4096];
            int r;

            fileStream.Seek(0, SeekOrigin.End);
            transporter.Send(BitConverter.GetBytes(fileStream.Position));
            fileStream.Seek(0, SeekOrigin.Begin);

            while (true)
            {
                r = fileStream.Read(buffer, 0, buffer.Length);
                if (r == buffer.Length)
                {
                    transporter.Send(buffer);
                }
                else if(r != 0)
                {
                    transporter.Send(buffer.Take(r).ToArray());
                }
                else
                {
                    break;
                }
            }

            fileStream.Close();
        }

        /// <summary>
        /// 接受文件并保存到本地。
        /// Receive file and save to location.
        /// </summary>
        /// <param name="path">
        /// 保存路径。
        /// The path for saving received file.
        /// </param>
        public void Receive(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.WriteThrough);
            byte[] buffer;
            long length, i;

            length = BitConverter.ToInt64(transporter.Receive(BitConverter.GetBytes((long)0).Length), 0);

            for (i = 0; i < length;)
            {
                buffer = transporter.Receive(4096);
                fileStream.WriteAsync(buffer, 0, buffer.Length);
                i += buffer.Length;
            }

            fileStream.Close();
        }
        /// <summary>
        /// 接受文件。
        /// Receive file.
        /// </summary>
        /// <returns>
        /// 接收的文件数据。
        /// Received file data.
        /// </returns>
        /// <remarks>
        /// 小心文件过大。
        /// Caution about large file.
        /// </remarks>
        public byte[] Receive()
        {
            byte[] buffer, r;
            long length, i;

            length = BitConverter.ToInt64(transporter.Receive(BitConverter.GetBytes((long)0).Length), 0);
            r = new byte[length];

            for (i = 0; i < length;)
            {
                buffer = transporter.Receive(4096);
                Array.Copy(buffer, 0, r, i, buffer.Length);
                i += buffer.Length;
            }

            return r;
        }

        /// <summary>
        /// 用一个传输器构造。
        /// Use a tansporter to construct.
        /// </summary>
        /// <param name="transporter">
        /// 用来传输文件的传输器。
        /// A transporter for transporting files.
        /// </param>
        /// <remarks>
        /// 传输器可以选择`SocketConnection`。
        /// You could choose `SocketConnection` for transporter.
        /// </remarks>
        public FileTransport(ITransporter transporter)
        {
            this.transporter = transporter;
        }
    }
}
