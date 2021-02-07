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

namespace OurOpenSource.Security.UniqueDevice
{
    //参考文献：https://blog.csdn.net/xyxdu/article/details/88196240
    public class WindowsDeviceV1 : IUniqueDevice
    {
        #region ===IUniqueDevice===
        public string InfoType { get { return "WinMachineV1"; } }

        public static readonly string DefaultKey_HostName = "HostName";
        public static readonly string DefaultKey_SMBIOSUUID = "SMBIOSUUID";
        public static readonly string DefaultKey_MachineGuid = "MachineGuid";
        public static readonly string DefaultKey_MACAddressesHashCode = "MACAddressesHashCode";
        public static readonly string DefaultKey_ProductId = "ProductId";
        public static readonly string DefaultKey_CPUProcessorIDsHashCode = "CPUProcessorIDsHashCode";
        public static readonly string DefaultKey_IPAddress = "IPAddress";
        private Dictionary<string, string> infos;
        public Dictionary<string, string> Infos { get { return infos; } }

        public byte[] ToByteArray()
        {
            string json = JsonConvert.SerializeObject(winMachineInfo, jsonSerializerSettings);
            return Encoding.UTF8.GetBytes(json);
        }

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

        JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.None,
            //DefaultValueHandling=DefaultValueHandling.Include,
            //NullValueHandling=NullValueHandling.Include,
        };

        //[StructLayout(LayoutKind.Explicit)]
        //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 2)]
        public struct WinMachineInfo
        {
            [JsonProperty("HostName")]
            public string hostName;

            [JsonProperty("SMBIOSUUID")]
            public Guid smBIOSUUID;

            /// <summary>
            /// Windows的MachineGUID。
            /// </summary>
            [JsonProperty("MachineGUID")]
            public Guid machineGUID;

            /// <summary>
            /// 所有CPU ProcessorID的升序的字符串连接后的Hash值
            /// </summary>
            [JsonProperty("CPUProcessorIDsHashCode")]
            public byte[] cpuProcessorIDsHashCode;

            ///// <summary>
            ///// 所有硬盘序列号的升序的字符串连接后的Hash值。
            ///// </summary>
            ///// <remarks>
            ///// 非重要值。
            ///// </remarks>
            //public byte[] diskDriveSerialNumbersHashCode;

            /// <remarks>
            /// 非重要值。
            /// </remarks>
            [JsonProperty("ProductId")]
            public string productID;

            //public string macAddress;
            //public byte[] macAddress;//6 bytes

            /// <summary>
            /// 所有网卡MAC地址的升序的字符串连接后的Hash值。
            /// </summary>
            ///// <remarks>
            ///// 非重要值。
            ///// </remarks>
            [JsonProperty("MACAddressesHashCode")]
            public byte[] macAddressesHashCode;

            /// <remarks>
            /// 仅由服务器获取并记录。
            /// </remarks>
            [JsonProperty("")]
            public IPAddress ipAddress;
        }
        private WinMachineInfo winMachineInfo;
        public WinMachineInfo _WinMachineInfo
        {
            get { return winMachineInfo; }
        }

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

        public static string GetHostName()
        {
            RegistryKey target = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters");
            return (string)target.GetValue("HostName");
        }

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

        public static string GetMachineGUID()
        {
            RegistryKey target = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography");
            return (string)target.GetValue("MachineGuid");
        }

        //http://www.360doc.com/content/13/1221/19/432969_339077802.shtml
        //https://www.cnblogs.com/dyfisgod/p/9199006.html
        //https://www.cnblogs.com/diulela/archive/2012/04/07/2436111.html
        /////<summary>
        ///// 通过WMI读取系统信息里的网卡MAC
        /////</summary>
        /////<returns></returns>
        /////<remarks>
        /////不可靠。
        /////</remarks>
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
        public static bool CheckMacAddress(string macAddress)
        {
            return Regex.Match(macAddress, "[a-zA-Z0-9]{2}:[a-zA-Z0-9]{2}:[a-zA-Z0-9]{2}:[a-zA-Z0-9]{2}:[a-zA-Z0-9]{2}:[a-zA-Z0-9]{2}").Success;
        }
        ///<summary>
        /// 通过WMI读取系统信息里的网卡MAC
        ///</summary>
        ///<returns></returns>
        ///<remarks>
        ///不可靠。
        ///</remarks>
        public static string[] GetMacAddresses()
        {
            List<string> macs = new List<string>();
            string mac = "";
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
        public static bool CheckMacAddressesHashCode(byte[] macAddressesHashCode)
        {
            return macAddressesHashCode.Length == 32;
        }

        public static string GetProductID()
        {
            RegistryKey target = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
            return (string)target.GetValue("ProductId");
        }
        public static bool CheckProductID(string productID)
        {
            return Regex.Match(productID, "[a-zA-Z0-9]{5}-[a-zA-Z0-9]{5}-[a-zA-Z0-9]{5}-[a-zA-Z0-9]{5}").Success;
        }

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
        private static bool CheckCpuProcessorIDsHashCode(byte[] cpuProcessorIDsHashCode)
        {
            return cpuProcessorIDsHashCode.Length == 32;
        }

        public static byte[] GetStringsHashCode(string[] strings)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string one in strings)
            {
                sb.Append(one);
            }

            return SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(sb.ToString()));
        }



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
                { "MACAddressesHashCode", BytesToString(winMachineInfo.macAddressesHashCode) },
                { "ProductId", winMachineInfo.productID },
                { "CPUProcessorIDsHashCode", winMachineInfo.cpuProcessorIDsHashCode == null ? null : BytesToString(winMachineInfo.cpuProcessorIDsHashCode) },
                { "IPAddress", winMachineInfo.ipAddress.ToString() }
            };
        }

        #warning 需要转移出类
        private static string BytesToString(byte[] bytes)
        {
            int i;
            StringBuilder r = new StringBuilder(bytes.Length * 2);

            for (i = 0; i < bytes.Length; i++)
            {
                r.Append(string.Format("{0:X2}", bytes[i]));
            }

            return r.ToString();
        }

        public WindowsDeviceV1(string hostName, Guid smBIOSUUID, Guid machineGUID, byte[] macAddressesHashCode, string productID, byte[] cpuProcessorIDsHashCode = null, IPAddress ipAddress = null)
        {
            InitialiseWinMachineV1(hostName, smBIOSUUID, machineGUID, macAddressesHashCode, productID, cpuProcessorIDsHashCode, ipAddress);
        }

        public WindowsDeviceV1(byte[] bytes)
        {
            WindowsDeviceV1 uniqueDevice = ToUniqueDevice<WindowsDeviceV1>(bytes);
        }
    }
}
