using System;
using System.Collections.Generic;
using System.Text;

namespace OurOpenSource.Security.Cryptography
{
    public class RandomMethod_CryptGenRandom : IRandomMethod
    {
        //https://www.codenong.com/19201461/
        /*
关于c＃：随机数生成器安全性：BCryptGenRandom与RNGCryptoServiceProvider
 2019-11-21 
.netc#cryptographyrandomsecurity

Random number generator security: BCryptGenRandom vs RNGCryptoServiceProvider
对于急忙的人来说，这与NIST SP800-90A内置的有争议的Dual_EC_DRBG无关。

关于两个RNG：

基于Microsoft BCRYPT层的C API。 BCryptGenRandom紧跟NIST SP800-90A的CTR_DRBG(即使用批准的分组密码AES创建随机位)。但是还不清楚是否使用硬件随机源作为种子(或种子的一部分)...

Microsoft .NET RNGCryptoServiceProvider是基于C＃的。查看.NET源代码(或此处)，我看到它最终调用了C ++方法CapiNative.GenerateRandomBytes()。对于C＃=> C ++过渡，应该有一个P / Invoke存根，但我在框架源代码的任何地方都找不到它。所以我不知道它是如何实现的。

有人有关于这两个随机数生成器的其他信息吗？是否使用硬件随机种子(要么通过旧版英特尔中的二极管噪声，要么通过最新版英特尔中有争议的RDRAND)。

PS：不确定这应该在安全性，StackOverflow或密码术中...


The Microsoft .NET RNGCryptoServiceProvider is a C# based

不完全是，托管框架类只是Windows内置的Crypto api的精简包装。名称以ServiceProvider结尾的所有System.Security.Cryptography类都是本机API的包装。名称以Managed结尾的代码是用纯托管代码实现的。因此，XxxServiceProvider类使用FIPS验证的加密技术，而XxxManaged类则没有。

它并非完全是pinvoke，它使用通用机制在CLR代码中进行直接调用。抖动查阅带有C ++函数地址的表并直接编译CALL机器代码指令。此答案中描述了该机制。看一下实际代码是不可能的，它不包含在SSCLI20发行版中，并且已更改为使用.NET 4中的QCall机制。

因此，该断言是无法证明的，但很可能RNGCryptoServiceProvider和您传递给BCryptGenRandom()的算法提供程序都使用相同的随机数源。在Windows中，这是advapi.dll中未命名的导出函数，此答案提供了有关其用途的出色摘要。

如果这确实与您有关，并且您希望获得可靠的信息源，请不要从免费的Q + A网站上获取有关您的安全需求的建议。致电Microsoft支持。

RFC 4086中提到了Microsoft RNGCryptoServiceProvider：

7.1.3. Windows CryptGenRandom

Microsoft's recommendation to users of the widely deployed Windows
operating system is generally to use the CryptGenRandom pseudo-random
number generation call with the CryptAPI cryptographic service
provider. This takes a handle to a cryptographic service provider
library, a pointer to a buffer by which the caller can provide entropy
and into which the generated pseudo-randomness is returned, and an
indication of how many octets of randomness are desired.

The Windows CryptAPI cryptographic service provider stores a seed
state variable with every user. When CryptGenRandom is called, this is
combined with any randomness provided in the call and with various
system and user data such as the process ID, thread ID, system clock,
system time, system counter, memory status, free disk clusters, and
hashed user environment block. This data is all fed to SHA-1, and the
output is used to seed an RC4 key stream. That key stream is used to
produce the pseudo-random data requested and to update the user's seed
state variable.

Users of Windows".NET" will probably find it easier to use the
RNGCryptoServiceProvider.GetBytes method interface.
         */
    }
}
