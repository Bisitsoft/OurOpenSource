using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OurOpenSource.Net
{
    public class Connection
    {
        //public readonly int SOMAXCONN = 128;

        private Socket socket = null;
        public Socket Socket { get { return socket; } }

        public static Socket CreateSocket()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            return socket;
        }
        public void StartServer(IPEndPoint local, int backlog = 0)
        {
            socket.Bind(local);
            socket.Listen(backlog);
        }
        public Connection Accept()
        {
            return new Connection(socket.Accept());
        }
        public void StartClient(IPEndPoint remote)
        {
            socket.Connect(remote);
        }
        public void Stop()
        {
            socket.Disconnect(false);
            socket.Dispose();
        }

        public void Send(string str)
        {
            Send(Encoding.UTF8.GetBytes(str));
        }
        public void Send(byte[] data)
        {
            socket.Send(BitConverter.GetBytes(data.Length));
            socket.Send(data);
        }
        public string ReceiveAsString(int maxSize = -1)
        {
            return Encoding.UTF8.GetString(Receive(maxSize));
        }
        public byte[] Receive(int maxSize = -1)
        {
            if (maxSize == 0)
            {
                throw new InvalidOperationException("Can't recive any data when the maxSize is 0.");
            }

            SocketError error;

            //接收数据包大小值
            byte[] temp = BitConverter.GetBytes(0);
            socket.Receive(temp, 0, temp.Length, SocketFlags.None, out error);
            if (error != SocketError.Success)
            {
                throw new SocketException((int)error);
            }
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
            else if (maxSize > 0 && length > maxSize)
            {
                throw new ArgumentException(String.Format("The data length greater than maxSize(={0}).", maxSize));
            }

            //接收数据包
            temp = new byte[length];
            socket.Receive(temp, 0, temp.Length, SocketFlags.None, out error);
            if (error != SocketError.Success)
            {
                throw new SocketException((int)error);
            }

            return temp;
        }

        public Connection()
        {
            this.socket = CreateSocket();
        }
        public Connection(Socket socket)
        {
            this.socket = socket;
        }
    }
}
