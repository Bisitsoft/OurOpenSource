using System.Collections.Generic;

namespace OurOpenSource.Security.UniqueDevice
{
    public interface IUniqueDevice
    {
        /// <summary>
        /// 获取设备（唯一）信息类型名称。
        /// Get unique device informationtype.
        /// </summary>
        string InfoType { get; }

        /// <summary>
        /// 获取所有信息。
        /// Get all information.
        /// </summary>
        Dictionary<string, string> Infos { get; }

        /// <summary>
        /// 将本IUniqueDevice转化为字节流。
        /// Convert this instance to bytes stream.
        /// </summary>
        /// <returns>
        /// 转化后的字节流。
        /// Converted bytes stream.
        /// </returns>
        byte[] ToByteArray();

        /// <summary>
        /// 将字节流转换为为`T`。
        /// Convert bytes stream to type `T`.
        /// </summary>
        /// <typeparam name="T">
        /// 目标`IUniqueDevice`类型。
        /// Target type `IUniqueDevice`。
        /// </typeparam>
        /// <param name="bytes">
        /// 由`IUniqueDevice`转化来的字节流。
        /// A bytes stream convert form an `IUniqueDevice`.
        /// </param>
        /// <returns>
        /// 被还原的`IUniqueDevice`。
        /// Converted `IUniqueDevice`.
        /// </returns>
        /// <remarks>
        /// `T`通常只能为该实例的真实类型。
        /// Usually, `T` can only be actually type of this instance.
        /// 如，对于`WindowsDeviceV1`，`T`只能是`WindowsDeviceV1`，即`WindowsDeviceV1.ToUniqueDevice<WindowsDeviceV1>(aByteStreamConvertFormWindowsDeviceV1)`。
        /// For example, To `WindowsDeviceV1`, `T` can only be `WindowsDeviceV1`, that is `WindowsDeviceV1.ToUniqueDevice<WindowsDeviceV1>(aByteStreamConvertFormWindowsDeviceV1)`.
        /// </remarks>
        T ToUniqueDevice<T>(byte[] bytes) where T : IUniqueDevice;
    }
}
