using System.Collections.Generic;

namespace OurOpenSource.Security.UniqueDevice
{
    public interface IUniqueDevice
    {
        /// <summary>
        /// 获取设备（唯一）信息类型名称。
        /// </summary>
        string InfoType { get; }

        /// <summary>
        /// 获取所有信息。
        /// </summary>
        Dictionary<string, string> Infos { get; }

        /// <summary>
        /// 将本IUniqueDevice转化为字节流。
        /// </summary>
        /// <returns>转化后的字节流。</returns>
        byte[] ToByteArray();

        /// <summary>
        /// 解码字节流为T。
        /// </summary>
        /// <typeparam name="T">目标IUniqueDevice类型</typeparam>
        /// <param name="bytes">由IUniqueDevice转化来的字节流。</param>
        /// <returns>被还原的IUniqueDevice。</returns>
        T ToUniqueDevice<T>(byte[] bytes) where T : IUniqueDevice;
    }
}
