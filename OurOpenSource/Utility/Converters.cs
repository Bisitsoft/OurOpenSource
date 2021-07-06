using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OurOpenSource.Utility
{
    /// <summary>
    /// 一些转化器。
    /// Some Converters.
    /// </summary>
    public class Converters
    {
        /// <summary>
        /// 将字节数组转化为字符串。
        /// Convert bytes to string.
        /// </summary>
        /// <param name="bytes">
        /// 需要被转化的字节数组。
        /// A bytes array need to convert.
        /// </param>
        /// <returns>
        /// 转化后的字符串。
        /// Converted string.
        /// </returns>
        /// <remarks>
        /// 不带前缀`"0x"`以及`空格`。如存储`0xFF_AF`的字节数组会被转化为`"FFAF"`。
        /// Without `"0x"` as prefix, no `spaces` for separation. For example, a bytes array which stores `0xFF_AF` will be convert to `"FFAF"`.
        /// </remarks>
        public static string BytesToString(byte[] bytes)
        {
            int i;
            StringBuilder r = new StringBuilder(bytes.Length * 2);

            for (i = 0; i < bytes.Length; i++)
            {
                r.Append(string.Format("{0:X2}", bytes[i]));
            }

            return r.ToString();
        }

        /// <summary>
        /// 将字符串转化为字节数组。
        /// Convert string to bytes.
        /// </summary>
        /// <param name="hex">
        /// 由十六进制数字组成的字符串。
        /// A string of hexadecimal digits.
        /// </param>
        /// <returns>
        /// 转化后的字节数组。
        /// Converted bytes array.
        /// </returns>
        /// <remarks>
        /// `hex`长度不应超过int.`MaxValue`的值。
        /// The string `hex` length can't over `int.MaxValue`.
        /// 字符串最后为低位；最低位存在数组索引为`0`的位置。
        /// Low-order start from the back of the string hex. Low-order will be store at the bytes array where index is `0`.
        /// </remarks>
        public static byte[] HexStringToBytes(string hex)
        {
            byte[] r;

            int i, length;

            if (hex.Length % 2 == 0)
            {
                length = hex.Length / 2;
                r = new byte[length];

                for (i = length - 1; i >= 0; i--)
                {
                    r[length - (i + 1)] = (byte)(HexToByte(hex[i * 2 + 1]) | (HexToByte(hex[i * 2]) << 4));//low|high
                }
            }
            else
            {
                length = (hex.Length + 1) / 2;
                r = new byte[length];
                
                for (i = length - 1; i >= 1; i--)
                {
                    r[length - (i + 1)] = (byte)(HexToByte(hex[i * 2]) | (HexToByte(hex[i * 2 - 1]) << 4));//low|high
                }
                r[length - 1] = (byte)HexToByte(hex[0]);//low|high
            }

            return r;
        }

        /// <summary>
        /// 将字符转化为比特。
        /// Convert one character to one byte.
        /// </summary>
        /// <param name="hex">
        /// 一个表示十六进制的字符。
        /// A character representing a hexadecimal digit.
        /// </param>
        /// <returns>
        /// 转化后的值。
        /// Converted value.
        /// </returns>
        public static byte HexToByte(char hex)
        {
            unchecked
            {
                if ('0' <= hex && hex <= '9')
                {
                    return (byte)(hex - '0');
                }
                else if ('A' <= hex && hex <= 'F')
                {
                    return (byte)(hex - 55);//'A'-10=55
                }
                else if ('a' <= hex && hex <= 'f')
                {
                    return (byte)(hex - 87);//'a'-10=87
                }
                throw new FormatException("Not a hexadecimal digit.");
            }
        }

        /// <summary>
        /// 将JSON文本转化为字典。
        /// Convert JSON document to dictionay.
        /// </summary>
        /// <param name="json">
        /// JSON文本。
        /// Json document.
        /// </param>
        /// <returns>
        /// 转化后的字典。
        /// Converted dictionary.
        /// </returns>
        public static Dictionary<string,string> JsonTextToDictionary(string json)
        {
            //https://www.cnblogs.com/ccuc/p/6781593.html
            return json.Trim(new char[] { '{', '}' }).Split(',').ToDictionary<string, string, string>(s => s.Split(':')[0], s => s.Split(':')[1]);

            //Newtonsoft.Json方法
            //https://blog.csdn.net/u011127019/article/details/59111241
        }
    }
}
