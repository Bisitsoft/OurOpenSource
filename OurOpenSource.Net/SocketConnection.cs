using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OurOpenSource.Net
{
    /// <summary>
    /// 套接字连接。
    /// Socket connection.
    /// </summary>
    public class SocketConnection : ITransporter
    {
        //public readonly int SOMAXCONN = 128;

        /// <summary>
        /// 套接字。
        /// Socket.
        /// </summary>
#pragma warning disable IDE0044 // 添加只读修饰符
        private Socket socket;
#pragma warning restore IDE0044 // 添加只读修饰符
        /// <summary>
        /// 套接字。
        /// Socket.
        /// </summary>
        public Socket Socket { get { return socket; } }

        /// <summary>
        /// 如果所占用套接字处于`Connected`或`IsBound`状态并且该实例的属性`DisposeSocketInDeconstruction`为`true`则会在析构时释放所用的套接字。
        /// If it's using socket in `Connected` or `IsBound` status, and this instance's properity `DisposeSocketInDeconstruction` is `true`, it will dispose using socket in deconstruction.
        /// </summary>
        private bool DisposeSocketInDeconstruction { get; set; }

        /// <summary>
        /// 创造一个标准的TCP套接字。
        /// Creat a standrad TCP socket.
        /// </summary>
        /// <returns>
        /// 返回一个标准的TCP套接字。
        /// Return a standrad TCP socket.
        /// </returns>
        public static Socket CreateSocket()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            return socket;
        }
        /// <summary>
        /// 将该套接连作为服务器。
        /// Use this socket connection as a server.
        /// </summary>
        /// <param name="local">
        /// 本服务器的终结点。
        /// This server's end point.
        /// </param>
        /// <param name="backlog">
        /// 等待队列长度。设置为`0`代表无限。
        /// Wait list length. Set `0` for unlimit.
        /// </param>
        public void StartServer(IPEndPoint local, int backlog = 0)
        {
            socket.Bind(local);
            socket.Listen(backlog);
        }
        /// <summary>
        /// （如果这个套接字连接是作为一个服务器的话）同意一个连接请求。
        /// (If this socket connection as a server) Accept a connect request.
        /// </summary>
        /// <returns>
        /// 接受的连接。
        /// Accepted connection.
        /// </returns>
        public SocketConnection Accept()
        {
            return new SocketConnection(socket.Accept(), true);
        }
        /// <summary>
        /// 将该套接连作为客户端。
        /// Use this socket connection as a client.
        /// </summary>
        /// <param name="remote">
        /// 目标服务器终结点。
        /// Target server end point.
        /// </param>
        public void StartClient(IPEndPoint remote)
        {
            socket.Connect(remote);
        }
        /// <summary>
        /// 终止连接。
        /// Stop connection。
        /// 如果该实例的属性`DisposeSocketInDeconstruction`为`true`则会释放所用的套接字。
        /// If this instance's properity `DisposeSocketInDeconstruction` is `true`, it will dispose using socket.
        /// </summary>
        public void Stop()
        {
            //因为服务器socket没有连接所以调用以下两个函数会报错。
            //socket.Shutdown(SocketShutdown.Both);
            //socket.Disconnect();

            socket.Close();
            if (DisposeSocketInDeconstruction)
            {
                socket.Dispose();
            }
        }

        /// <summary>
        /// 发送数据。
        /// Send data.
        /// </summary>
        /// <param name="data">
        /// 需要发送的数据。
        /// The data need to send.
        /// </param>
        public override void Send(byte[] data)
        {
            socket.Send(BitConverter.GetBytes(data.Length));
            socket.Send(data);
        }

        /// <summary>
        /// 接收数据。
        /// Receive data.
        /// </summary>
        /// <param name="maxSize">
        /// 接收数据的最大大小。
        /// Max size of receive data.
        /// 你可以设置任意值，包括负数。但是-1代表无限大。
        /// You can set any value, including negative. But `-1` representing unlimit.
        /// </param>
        /// <returns>
        /// 接收到的数据。如果是空包，则返回`null`。
        /// Received data. If it's an empty package, it will return `null`.
        /// </returns>
        public override byte[] Receive(int maxSize = -1)
        {
            //if (maxSize == 0)
            //{
            //    throw new InvalidOperationException("Can't recive any data when the maxSize is 0.");
            //}

            //接收数据包大小值
            byte[] temp = BitConverter.GetBytes(0);
            socket.Receive(temp, 0, temp.Length, SocketFlags.None, out SocketError error);
            if (error != SocketError.Success)
            {
                throw new SocketException((int)error);
            }
            int length = BitConverter.ToInt32(temp, 0);
            if (maxSize != -1)
            {
                if (length < 0)
                {
                    throw new ArgumentException("The data length value in package header less than 0.");
                }
                else if (length > maxSize)
                {
                    throw new ArgumentException(String.Format("The data length greater than maxSize(={0}).", maxSize));
                }
            }
            if (length == 0)
            {
                //一个空包
                return null;
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

        /// <summary>
        /// 创造一个标准的TCP连接。
        /// Create a standrad TCP connection.
        /// </summary>
        public SocketConnection()
        {
            this.DisposeSocketInDeconstruction = true;
            this.socket = CreateSocket();
        }
        /// <summary>
        /// 通过已有套接字构造。
        /// Construct with existed socket.
        /// </summary>
        /// <param name="socket">
        /// 已有套接字。
        /// Existed socket.
        /// </param>
        /// <param name="disposeSocketInDeconstruction">
        /// <seealso cref="DisposeSocketInDeconstruction"/>
        /// </param>
        public SocketConnection(Socket socket, bool disposeSocketInDeconstruction = true)
        {
            this.DisposeSocketInDeconstruction = disposeSocketInDeconstruction;
            this.socket = socket;
        }
        /// <summary>
        /// 释放套接字连接。
        /// Dispose socket connection.
        /// </summary>
        ~SocketConnection()
        {
            if (DisposeSocketInDeconstruction)
            {
                if (socket.Connected || socket.IsBound)
                {
                    Stop();
                }
            }
        }
    }
}
