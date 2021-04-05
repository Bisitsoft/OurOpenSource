using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OurOpenSource.Utility
{
    public class Converters
    {
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

        /// <remarks>
        /// hex是仅有16进制数字的字符串。
        /// 长度不应超过ine.MaxValue的值。
        /// 字符串最后为低位；最低位存在数组索引为0的位置。
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

        public static Dictionary<string,string> JsonTextToDictionary(string json)
        {
            //https://www.cnblogs.com/ccuc/p/6781593.html
            return json.Trim(new char[] { '{', '}' }).Split(',').ToDictionary<string, string, string>(s => s.Split(':')[0], s => s.Split(':')[1]);

            //Newtonsoft.Json方法
            //https://blog.csdn.net/u011127019/article/details/59111241
        }
    }
}
