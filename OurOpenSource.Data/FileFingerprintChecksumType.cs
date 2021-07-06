using System;
using System.Collections.Generic;
using System.Text;

namespace OurOpenSource.Data
{
    /// <summary>
    /// 文件指纹校验值类型。
    /// File fingerprint checksum types.
    /// </summary>
    public enum FileFingerprintChecksumType : ushort
    {
        /// <summary>
        /// 无。
        /// None.
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// 包含MD5校验值。
        /// Including MD5 checksum.
        /// </summary>
        MD5 = 0x0001,
        /// <summary>
        /// 包含SHA1校验值。
        /// Including SHA1 checksum.
        /// </summary>
        SHA1 = 0x0010,
        /// <summary>
        /// 包含SHA256校验值。
        /// Including SHA256 checksum.
        /// </summary>
        SHA256 = 0x0100,

        /// <summary>
        /// 包含所以类型的校验值。
        /// Including all types of checksum.
        /// </summary>
        All = MD5 | SHA1 | SHA256
    }
}
