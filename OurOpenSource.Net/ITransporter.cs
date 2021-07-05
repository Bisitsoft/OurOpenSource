using System;
using System.Collections.Generic;
using System.Text;

namespace OurOpenSource.Net
{
    public abstract class ITransporter
    {
        /// <summary>
        /// 发送字符串。
        /// </summary>
        /// <param name="str">字符串。</param>
        /// <param name="encoding">编码器。</param>
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
        /// </summary>
        /// <param name="data">数据。</param>
        public abstract void Send(byte[] data);

        /// <summary>
        /// 接收字符串。
        /// </summary>
        /// <param name="maxSize">接收最大字节数。</param>
        /// <param name="encoding">编码器。</param>
        /// <returns>接收到的字符串。</returns>
        /// <remarks>
        /// maxSize是最大字节数，不是最大字符数。
        /// </remarks>
        public virtual string ReceiveAsString(int maxSize = -1, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetString(Receive(maxSize));
        }
        /// <summary>
        /// 接收数据包。
        /// </summary>
        /// <param name="maxSize">接收的最大字节数。设置为-1代表无限制。</param>
        /// <returns>接收到的数据包。空包则为null。</returns>
        public abstract byte[] Receive(int maxSize = -1);
    }
}
