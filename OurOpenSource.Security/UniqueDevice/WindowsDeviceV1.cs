using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

using Newtonsoft.Json;

using OurOpenSource.Utility;

namespace OurOpenSource.Security.UniqueDevice
{
    //参考文献：https://blog.csdn.net/xyxdu/article/details/88196240
    /// <summary>
    /// 通用Windows设备唯一标识，版本：1。
    /// General Widnows unique device identifier, version: 1.
    /// </summary>
    public class WindowsDeviceV1 : IUniqueDevice
    {
        #region ===IUniqueDevice===
        /// <summary>
        /// 获取设备（唯一）信息类型名称。
        /// Get unique device informationtype.
        /// </summary>
        public string InfoType { get { return "WinMachineV1"; } }

        /// <summary>
        /// 主机名称的键名。
        /// Key of host name.
        /// </summary>
        public static readonly string KeyName_HostName = "HostName";
        /// <summary>
        /// SMBIOS UUID的键名。
        /// Key of SMBIOS UUID.
        /// </summary>
        public static readonly string KeyName_SMBIOSUUID = "SMBIOSUUID";
        /// <summary>
        /// Machine GUID的键名。
        /// Key of Machine GUID.
        /// </summary>
        public static readonly string KeyName_MachineGuid = "MachineGuid";
        /// <summary>
        /// MAC地址哈希值的键名。
        /// Key of MAC address hash code.
        /// </summary>
        public static readonly string KeyName_MACAddressesHashCode = "MACAddressesHashCode";
        /// <summary>
        /// Product ID的键名。
        /// Key of host Product ID.
        /// </summary>
        public static readonly string KeyName_ProductId = "ProductId";
        /// <summary>
        /// CPU Processor ID哈希值的键名。
        /// Key of CPU Processor ID hash code.
        /// </summary>
        public static readonly string KeyName_CPUProcessorIDsHashCode = "CPUProcessorIDsHashCode";
        /// <summary>
        /// IP地址的键名。
        /// Key of IP address.
        /// </summary>
        public static readonly string DefaultKey_IPAddress = "IPAddress";

        /// <summary>
        /// 设备唯一标识信息。
        /// Unique machine identifier infomation.
        /// </summary>
        private Dictionary<string, string> infos;
        /// <summary>
        /// 设备唯一标识信息。
        /// Unique machine identifier infomation.
        /// </summary>
        public Dictionary<string, string> Infos { get { return infos; } }

        /// <summary>
        /// 将本IUniqueDevice转化为字节流。
        /// Convert this instance to bytes stream.
        /// </summary>
        /// <returns>
        /// 转化后的字节流。
        /// Converted bytes stream.
        /// </returns>
        public byte[] ToByteArray()
        {
            string json = JsonConvert.SerializeObject(winMachineInfo, jsonSerializerSettings);
            return Encoding.UTF8.GetBytes(json);
        }

        /// <summary>
        /// 将字节流转换为为`WindowsDeviceV1`。
        /// Convert bytes stream to type `WindowsDeviceV1`.
        /// </summary>
        /// <typeparam name="T">
        /// 仅能为`WindowsDeviceV1`。
        /// It only can be `WindowsDeviceV1`。
        /// </typeparam>
        /// <param name="bytes">
        /// 由`WindowsDeviceV1`转化来的字节流。
        /// A bytes stream convert form an `WindowsDeviceV1`.
        /// </param>
        /// <returns>
        /// 被还原的`WindowsDeviceV1`。
        /// Converted `WindowsDeviceV1`.
        /// </returns>
        public T ToUniqueDevice<T>(byte[] bytes) where T : IUniqueDevice
        {
            if (this.GetType() == typeof(T))
            {
                return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
            }
            else
            {
                throw new InvalidOperationException("The type of the target is not WinMachineV1(:IUniqueDevice).");
            }
        }
        #endregion

        /// <summary>
        /// JSON序列化设置。
        /// JSON Serializer Settings.
        /// </summary>
        private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.None,
            //DefaultValueHandling=DefaultValueHandling.Include,
            //NullValueHandling=NullValueHandling.Include,
        };

        /// <summary>
        /// 用于存储通用Windows设备唯一标识信息的容器。
        /// Container of stroing general Widnows unique device identifier information.
        /// </summary>
        //[StructLayout(LayoutKind.Explicit)]
        //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 2)]
        public struct WinMachineInfo
        {
            /// <summary>
            /// 主机名。
            /// Host name.
            /// </summary>
            [JsonProperty("HostName")]
            public string hostName;

            /// <summary>
            /// SMBIOS UUID。
            /// SMBIOS UUID.
            /// </summary>
            [JsonProperty("SMBIOSUUID")]
            public Guid smBIOSUUID;

            /// <summary>
            /// Windows的MachineGUID。
            /// Windows' Machine GUID.
            /// </summary>
            [JsonProperty("MachineGUID")]
            public Guid machineGUID;

            /// <summary>
            /// 把所有CPU ProcessorID按升序排序后的字符串连后的Hash值。
            /// The hash value after concatenating the strings sorted by all CPU Processor IDs in ascending order.
            /// </summary>
            /// /// <remarks>
            /// 同一批次的CPU的CPU Processor ID可能一样。
            /// The CPU Processor ID of the same batch of CPUs maybe is same.
            /// </remarks>
            [JsonProperty("CPUProcessorIDsHashCode")]
            public byte[] cpuProcessorIDsHashCode;

            ///// <summary>
            ///// 把所有硬盘序列号按升序排序后的字符串连后的Hash值。
            ///// The hash value after concatenating the strings sorted by all disk driver serial numbers in ascending order.
            ///// </summary>
            ///// <remarks>
            ///// 非重要值。
            ///// Unimportant value.
            ///// </remarks>
            //public byte[] diskDriveSerialNumbersHashCode;

            /// <summary>
            /// Windows的Product ID。
            /// Windows' Product ID.
            /// </summary>
            /// <remarks>
            /// 非重要值。
            /// Unimportant value.
            /// </remarks>
            [JsonProperty("ProductId")]
            public string productID;

            //public string macAddress;
            //public byte[] macAddress;//6 bytes

            /// <summary>
            /// 把所有网卡MAC地址按升序排序后的字符串连后的Hash值。
            /// The hash value after concatenating the strings sorted by all MAC addresses in ascending order.
            /// </summary>
            ///// <remarks>
            ///// 非重要值。
            ///// Unimportant value.
            ///// </remarks>
            [JsonProperty("MACAddressesHashCode")]
            public byte[] macAddressesHashCode;

            /// <summary>
            /// IP地址。
            /// IP address.
            /// </summary>
            /// <remarks>
            /// 仅由服务器获取并记录。
            /// Get and record by server.
            /// </remarks>
            [JsonProperty("")]
            public IPAddress ipAddress;
        }
        /// <summary>
        /// 通用Windows设备唯一标识信息。
        /// General Widnows unique device identifier information.
        /// </summary>
        private WinMachineInfo winMachineInfo;
        /// <summary>
        /// 通用Windows设备唯一标识信息。
        /// General Widnows unique device identifier information.
        /// </summary>
#pragma warning disable IDE1006 // 命名样式
        public WinMachineInfo _WinMachineInfo
#pragma warning restore IDE1006 // 命名样式
        {
            get { return winMachineInfo; }
        }

        /// <summary>
        /// 获取通用Windows设备唯一标识信息。
        /// Get general Widnows unique device identifier information.
        /// </summary>
        /// <returns>
        /// 通用Windows设备唯一标识信息。
        /// General Widnows unique device identifier information.
        /// </returns>
        public static WindowsDeviceV1 Create()
        {
            WindowsDeviceV1 winMachine = new WindowsDeviceV1(
                GetHostName(),
                new Guid(GetSMBIOSUUID()),
                new Guid(GetMachineGUID()),
                GetStringsHashCode(GetMacAddresses()),
                GetProductID(),
                GetStringsHashCode(GetCPUProcessorIDs())
                );
            return winMachine;
        }

        /// <summary>
        /// 获取本机的主机名。
        /// Get localhost.
        /// </summary>
        /// <returns>
        /// 本机的主机名。
        /// localhost.
        /// </returns>
        public static string GetHostName()
        {
            RegistryKey target = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters");
            return (string)target.GetValue("HostName");
        }

        /// <summary>
        /// 获取本机的SMBIOS UUID。
        /// Get localhost SMBIOS UUID.
        /// </summary>
        /// <returns>
        /// 本机的SMBIOS UUID。
        /// localhost SMBIOS UUID.
        /// </returns>
        public static string GetSMBIOSUUID()
        {
            string getInfo;
            //1 2  3 4  5 6  7 8  9 10 1112 1314 1516
            //CBC5 D14C-29E6-11B2-A85C-84C4 C714 784B
            using (Process process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = System.IO.Path.Combine(Environment.SystemDirectory, "cmd.exe");
                process.StartInfo.Arguments = "/U /E:OFF /V:OFF /C wmic csproduct get UUID";
                process.Start();
                //process.StandardInput.WriteLine("wmic csproduct get UUID");
                //process.StandardInput.WriteLine("exit");
                process.WaitForExit();
                process.StandardOutput.ReadLine();
                process.StandardOutput.ReadLine();
                getInfo = process.StandardOutput.ReadLine();//.Substring(0, 36).Remove(23, 1).Remove(18, 1).Remove(13, 1).Remove(8, 1);
            }
            return getInfo;
        }

        /// <summary>
        /// 获取本机的Machine GUID。
        /// Get localhost Machine GUID.
        /// </summary>
        /// <returns>
        /// 本机的Machine GUID。
        /// localhost Machine GUID.
        /// </returns>
        public static string GetMachineGUID()
        {
            RegistryKey target = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography");
            return (string)target.GetValue("MachineGuid");
        }

        //http://www.360doc.com/content/13/1221/19/432969_339077802.shtml
        //https://www.cnblogs.com/dyfisgod/p/9199006.html
        //https://www.cnblogs.com/diulela/archive/2012/04/07/2436111.html
        ///// <summary>
        /////  通过WMI读取系统信息里的网卡MAC。
        /////  Read the network card MAC in the system information through WMI.
        ///// </summary>
        ///// <returns>
        /////  MAC地址。
        /////  MAC Addresses.
        ///// </returns>
        ///// <remarks>
        /////  不可靠。
        /////  Unreliable.
        ///// </remarks>
        //public static string GetMacAddress()
        //{
        //    IPAddress localIp = null;
        //    IPAddress[] ipArray;
        //    ipArray = Dns.GetHostAddresses(Dns.GetHostName());
        //    localIp = ipArray.First(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        //    string resMac = "";
        //    ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        //    ManagementObjectCollection moc2 = mc.GetInstances();
        //    foreach (ManagementObject mo in moc2)
        //    {
        //        if ((bool)mo["IPEnabled"] == true && mo["IPAddress"] != null)
        //        {
        //            if (((string[])mo["IPAddress"])[0] == localIp.ToString())
        //            {
        //                resMac = mo["MacAddress"].ToString();
        //
        //                mo.Dispose();
        //
        //                return resMac;
        //            }
        //        }
        //    }
        //    return null;
        //}
        /// <summary>
        ///  检查MAC字符串格式是否正确。
        ///  Check is MAC address string format correct.
        /// </summary>
        /// <returns>
        ///  MAC字符串格式是否正确。
        ///  Is MAC address string format correct.
        /// </returns>
        public static bool CheckMacAddress(string macAddress)
        {
            return Regex.Match(macAddress, "[a-zA-Z0-9]{2}:[a-zA-Z0-9]{2}:[a-zA-Z0-9]{2}:[a-zA-Z0-9]{2}:[a-zA-Z0-9]{2}:[a-zA-Z0-9]{2}").Success;
        }
        /// <summary>
        ///  通过WMI读取系统信息里的网卡MAC。
        ///  Read the network card MAC in the system information through WMI.
        /// </summary>
        /// <returns>
        ///  MAC地址。
        ///  MAC Addresses.
        /// </returns>
        /// <remarks>
        ///  不可靠。
        ///  Unreliable.
        /// </remarks>
        public static string[] GetMacAddresses()
        {
            List<string> macs = new List<string>();
            string mac;//string mac = "";
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"])
                {
                    mac = mo["MacAddress"].ToString();
                    macs.Add(mac);
                }
            }

            moc.Dispose();
            mc.Dispose();

            return macs.Count == 0 ? null : macs.ToArray();
        }
        /// <summary>
        ///  检查MAC地址的哈希值长度是否正确。
        ///  Check is MAC addresses hash code length correct.
        /// </summary>
        /// <returns>
        ///  MAC地址的哈希值长度是否正确。
        ///  Is MAC addresses hash code length correct.
        /// </returns>
        public static bool CheckMacAddressesHashCode(byte[] macAddressesHashCode)
        {
            return macAddressesHashCode.Length == 32;
        }

        /// <summary>
        /// 获取本机的Product ID。
        /// Get localhost Product ID.
        /// </summary>
        /// <returns>
        /// 本机的Product ID。
        /// localhost Product ID.
        /// </returns>
        public static string GetProductID()
        {
            RegistryKey target = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
            return (string)target.GetValue("ProductId");
        }
        /// <summary>
        ///  检查Product ID字符串格式是否正确。
        ///  Check is Product ID string format correct.
        /// </summary>
        /// <returns>
        ///  Product ID字符串格式是否正确。
        ///  Is Product ID string format correct.
        /// </returns>
        public static bool CheckProductID(string productID)
        {
            return Regex.Match(productID, "[a-zA-Z0-9]{5}-[a-zA-Z0-9]{5}-[a-zA-Z0-9]{5}-[a-zA-Z0-9]{5}").Success;
        }

        /// <summary>
        /// 获取本机的CPUProcessorID。
        /// Get localhost CPUProcessorIDs.
        /// </summary>
        /// <returns>
        /// 本机的CPUProcessorID。
        /// localhost CPUProcessorIDs.
        /// </returns>
        public static string[] GetCPUProcessorIDs()
        {
            string temp;
            List<string> getInfos = new List<string>();
            //1 2  3 4  5 6  7 8  9 10 1112 1314 1516
            //CBC5 D14C-29E6-11B2-A85C-84C4 C714 784B
            using (Process process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = System.IO.Path.Combine(Environment.SystemDirectory, "cmd.exe");
                process.StartInfo.Arguments = "/U /E:OFF /V:OFF /C wmic cpu get processorid";
                process.Start();
                //process.StandardInput.WriteLine("wmic csproduct get UUID");
                //process.StandardInput.WriteLine("exit");
                process.WaitForExit();
                process.StandardOutput.ReadLine();
                process.StandardOutput.ReadLine();

                while (!process.StandardOutput.EndOfStream)
                {
                    temp = process.StandardOutput.ReadLine();
                    if (temp != String.Empty)
                    {
                        getInfos.Add(temp);
                    }
                }
            }
            getInfos.Sort();
            return getInfos.ToArray();
        }
        /// <summary>
        ///  检查CPUProcessorID的哈希值长度是否正确。
        ///  Check is CPUProcessorIDs hash code length correct.
        /// </summary>
        /// <returns>
        ///  CPUProcessorID的哈希值长度是否正确。
        ///  Is CPUProcessorIDs hash code length correct.
        /// </returns>
        private static bool CheckCpuProcessorIDsHashCode(byte[] cpuProcessorIDsHashCode)
        {
            return cpuProcessorIDsHashCode.Length == 32;
        }

        /// <summary>
        /// 获取字符串数组的哈希值。
        /// Get string array's hash code.
        /// </summary>
        /// <param name="strings">
        /// 被计算哈希值的字符串数组。
        /// A string array need to calculate hash code.
        /// </param>
        /// <returns>
        /// 字符串数组的哈希值。
        /// String array's hash code.
        /// </returns>
        public static byte[] GetStringsHashCode(string[] strings)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string one in strings)
            {
                sb.Append(one);
            }

            return SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(sb.ToString()));
        }

        /// <summary>
        /// 初始化通用Windows设备唯一标识。
        /// Initialise General Widnows unique device identifier.
        /// </summary>
        /// <param name="hostName">
        /// 主机名。
        /// Host name.
        /// </param>
        /// <param name="smBIOSUUID">
        /// SMBIOS UUID。
        /// SMBIOS UUID.
        /// </param>
        /// <param name="machineGUID">
        /// Windows的MachineGUID。
        /// Windows' Machine GUID.
        /// </param>
        /// <param name="macAddressesHashCode">
        /// 把所有网卡MAC地址按升序排序后的字符串连后的Hash值。
        /// The hash value after concatenating the strings sorted by all MAC addresses in ascending order.
        /// </param>
        /// <param name="productID">
        /// Windows的Product ID。
        /// Windows' Product ID.
        /// </param>
        /// <param name="cpuProcessorIDsHashCode">
        /// 把所有CPU ProcessorID按升序排序后的字符串连后的Hash值。
        /// The hash value after concatenating the strings sorted by all CPU Processor IDs in ascending order.
        /// </param>
        /// <param name="ipAddress">
        /// IP地址。
        /// IP address.
        /// </param>
        private void InitialiseWinMachineV1(string hostName, Guid smBIOSUUID, Guid machineGUID, byte[] macAddressesHashCode, string productID, byte[] cpuProcessorIDsHashCode = null, IPAddress ipAddress = null)
        {
            if (!CheckMacAddressesHashCode(macAddressesHashCode))
            {
                throw new FormatException("The format of macAddress should be like \"hh:hh:Hh:hH:HH:HH\".");
            }
            if (!CheckProductID(productID))
            {
                throw new FormatException("The format of macAddress should be like \"abCdz-01231-CAZsa-1d3AS\".");
            }
            if (cpuProcessorIDsHashCode != null && !CheckCpuProcessorIDsHashCode(cpuProcessorIDsHashCode))
            {
                throw new FormatException("cpuProcessorIDsHashCode should be null or a SHA256 code.");
            }

            winMachineInfo = new WinMachineInfo()
            {
                hostName = hostName,
                smBIOSUUID = smBIOSUUID,
                machineGUID = machineGUID,
                macAddressesHashCode = macAddressesHashCode,
                productID = productID,
                cpuProcessorIDsHashCode = cpuProcessorIDsHashCode,
                ipAddress = ipAddress
            };

            infos = new Dictionary<string, string>()
            {
                { "HostName", winMachineInfo.hostName },
                { "SMBIOSUUID", winMachineInfo.smBIOSUUID.ToString() },
                { "MachineGuid", winMachineInfo.machineGUID.ToString() },
                { "MACAddressesHashCode", Converters.BytesToString(winMachineInfo.macAddressesHashCode) },
                { "ProductId", winMachineInfo.productID },
                { "CPUProcessorIDsHashCode", winMachineInfo.cpuProcessorIDsHashCode == null ? null : Converters.BytesToString(winMachineInfo.cpuProcessorIDsHashCode) },
                { "IPAddress", winMachineInfo.ipAddress.ToString() }
            };
        }

        /// <summary>
        /// 初始化通用Windows设备唯一标识。
        /// Initialise General Widnows unique device identifier.
        /// </summary>
        /// <param name="hostName">
        /// 主机名。
        /// Host name.
        /// </param>
        /// <param name="smBIOSUUID">
        /// SMBIOS UUID。
        /// SMBIOS UUID.
        /// </param>
        /// <param name="machineGUID">
        /// Windows的MachineGUID。
        /// Windows' Machine GUID.
        /// </param>
        /// <param name="macAddressesHashCode">
        /// 把所有网卡MAC地址按升序排序后的字符串连后的Hash值。
        /// The hash value after concatenating the strings sorted by all MAC addresses in ascending order.
        /// </param>
        /// <param name="productID">
        /// Windows的Product ID。
        /// Windows' Product ID.
        /// </param>
        /// <param name="cpuProcessorIDsHashCode">
        /// 把所有CPU ProcessorID按升序排序后的字符串连后的Hash值。
        /// The hash value after concatenating the strings sorted by all CPU Processor IDs in ascending order.
        /// </param>
        /// <param name="ipAddress">
        /// IP地址。
        /// IP address.
        /// </param>
        public WindowsDeviceV1(string hostName, Guid smBIOSUUID, Guid machineGUID, byte[] macAddressesHashCode, string productID, byte[] cpuProcessorIDsHashCode = null, IPAddress ipAddress = null)
        {
            InitialiseWinMachineV1(hostName, smBIOSUUID, machineGUID, macAddressesHashCode, productID, cpuProcessorIDsHashCode, ipAddress);
        }

        /// <summary>
        /// 初始化通用Windows设备唯一标识。
        /// Initialise General Widnows unique device identifier.
        /// </summary>
        /// <param name="bytes">
        /// 由`WindowsDeviceV1`转化来的字节流。
        /// A bytes stream convert form an `WindowsDeviceV1`.
        /// </param>
        public WindowsDeviceV1(byte[] bytes)
        {
            WindowsDeviceV1 uniqueDevice = ToUniqueDevice<WindowsDeviceV1>(bytes);
            InitialiseWinMachineV1(
                uniqueDevice._WinMachineInfo.hostName,
                uniqueDevice._WinMachineInfo.smBIOSUUID,
                uniqueDevice._WinMachineInfo.machineGUID,
                uniqueDevice._WinMachineInfo.macAddressesHashCode,
                uniqueDevice._WinMachineInfo.productID,
                uniqueDevice._WinMachineInfo.cpuProcessorIDsHashCode,
                uniqueDevice._WinMachineInfo.ipAddress
            );
        }
    }
}
