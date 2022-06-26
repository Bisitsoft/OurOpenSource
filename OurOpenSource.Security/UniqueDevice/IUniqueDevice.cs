using System.Collections.Generic;

namespace OurOpenSource.Security.UniqueDevice
{
    /// <summary>
    /// 设备唯一标识。
    /// Unique device identifier.
    /// </summary>
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
        /// 将本`<see cref="IUniqueDevice"/>`实例转化为字节流。
        /// Convert this `<see cref="IUniqueDevice"/>` instance to bytes stream.
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
        /// 目标`<see cref="IUniqueDevice"/>`类型。
        /// Target type `<see cref="IUniqueDevice"/>`。
        /// </typeparam>
        /// <param name="bytes">
        /// 由`<see cref="IUniqueDevice"/>`转化来的字节流。
        /// A bytes stream convert form an `<see cref="IUniqueDevice"/>`.
        /// </param>
        /// <returns>
        /// 被还原的`<see cref="IUniqueDevice"/>`。
        /// Converted `<see cref="IUniqueDevice"/>`.
        /// </returns>
        /// <remarks>
        /// `T`通常只能为该实例的真实类型。
        /// Usually, `T` can only be actually type of this instance.
        /// 例如，对于`<seealso cref="WindowsDeviceV1"/>`，`T`只能是`<seealso cref="WindowsDeviceV1"/>`，即`WindowsDeviceV1.ToUniqueDevice&lt;WindowsDeviceV1&gt;(aByteStreamConvertFormWindowsDeviceV1)`。
        /// For example, To `<seealso cref="WindowsDeviceV1"/>`, `T` can only be `<seealso cref="WindowsDeviceV1"/>`, that is `WindowsDeviceV1.ToUniqueDevice&lt;WindowsDeviceV1&gt;(aByteStreamConvertFormWindowsDeviceV1)`.
        /// </remarks>
        T ToUniqueDevice<T>(byte[] bytes) where T : IUniqueDevice;
    }
}
