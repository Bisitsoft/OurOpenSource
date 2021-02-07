using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OurOpenSource.Net
{
    public class FileTransport
    {
        private Connection connection = null;
        public Connection Connection { get { return connection; } }

        public void Send(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            byte[] buffer = new byte[4096];
            int r;

            fileStream.Seek(0, SeekOrigin.End);
            connection.Send(BitConverter.GetBytes(fileStream.Position));
            fileStream.Seek(0, SeekOrigin.Begin);

            while (true)
            {
                r = fileStream.Read(buffer, 0, buffer.Length);
                if (r == buffer.Length)
                {
                    connection.Send(buffer);
                }
                else if(r != 0)
                {
                    connection.Send(buffer.Take(r).ToArray());
                }
                else
                {
                    break;
                }
            }

            fileStream.Close();
        }

        public void Receive(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.WriteThrough);
            byte[] buffer;
            long length, i;

            length = BitConverter.ToInt64(connection.Receive(BitConverter.GetBytes((long)0).Length), 0);

            for (i = 0; i < length;)
            {
                buffer = connection.Receive(4096);
                fileStream.WriteAsync(buffer, 0, buffer.Length);
                i += buffer.Length;
            }

            fileStream.Close();
        }
        /// <remarks>
        /// 小心文件过大。
        /// </remarks>
        public byte[] Receive()
        {
            byte[] buffer, r;
            long length, i;

            length = BitConverter.ToInt64(connection.Receive(BitConverter.GetBytes((long)0).Length), 0);
            r = new byte[length];

            for (i = 0; i < length;)
            {
                buffer = connection.Receive(4096);
                Array.Copy(buffer, 0, r, i, buffer.Length);
                i += buffer.Length;
            }

            return r;
        }

        public FileTransport(Connection connection)
        {
            this.connection = connection;
        }
    }
}
