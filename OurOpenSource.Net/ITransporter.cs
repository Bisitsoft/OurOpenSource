using System;
using System.Collections.Generic;
using System.Text;

namespace OurOpenSource.Net
{
    /// <summary>
    /// 传输器。
    /// Transporter
    /// </summary>
    public abstract class ITransporter
    {
        /// <summary>
        /// 发送字符串。
        /// Send string.
        /// </summary>
        /// <param name="str">
        /// 字符串。
        /// A string.
        /// </param>
        /// <param name="encoding">
        /// 字符串的编码器。
        /// Encoding of string.
        /// </param>
        public virtual void Send(string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            Send(encoding.GetBytes(str));
        }
        /// <summary>
        /// 发送数据包。
        /// Send data package.
        /// </summary>
        /// <param name="data">
        /// 数据。
        /// Data.
        /// </param>
        public abstract void Send(byte[] data);

        /// <summary>
        /// 接收字符串。
        /// Receive string.
        /// </summary>
        /// <param name="maxSize">
        /// 接收最大字节数。设置为`-1`代表无限制。
        /// Max size of receive package. Set `-1` for unlimited.
        /// </param>
        /// <param name="encoding">
        /// 编码器。
        /// Encoding.
        /// </param>
        /// <returns>
        /// 接收到的字符串。空包则为`""`。
        /// Received string. If received an empty package, it will return `""`.
        /// </returns>
        /// <remarks>
        /// `maxSize`是最大字节数，不是最大字符数。
        /// `maxSize` is max size of bytes but not characters.
        /// </remarks>
        public virtual string ReceiveAsString(int maxSize = -1, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] received = Receive(maxSize);
            if(received == null)
            {
                return String.Empty;
            }
            return encoding.GetString(received);
        }
        /// <summary>
        /// 接收数据包。
        /// Receive data package.
        /// </summary>
        /// <param name="maxSize">
        /// 接收最大字节数。设置为`-1`代表无限制。
        /// Max size of receive package. Set `-1` for unlimited.
        /// </param>
        /// <returns>
        /// 接收到的数据包。空包则为`<see langword="null"/>`。
        /// Received data package. If received an empty package, it will return `<see langword="null"/>`.
        /// </returns>
        public abstract byte[] Receive(int maxSize = -1);
    }
}
