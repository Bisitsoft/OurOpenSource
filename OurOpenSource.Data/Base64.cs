using System;
using System.Collections.Generic;
using System.Text;

namespace OurOpenSource.Data
{
	/// <summary>
	/// 一个Base64转化类。
	/// A Base64 conveter class.
	/// </summary>
	public class Base64
	{
		/// <summary>
		/// 将字节数据转化为Base64格式字符串。
		/// Convert bytes data to Base64 format string.
		/// </summary>
		/// <param name="value">
		/// 字节数据。
		/// Bytes data.
		/// </param>
		/// <returns>
		/// 转化后Base64格式字符串。
		/// Converted Base64 format string.
		/// </returns>
		public string ToBase64(byte[] value)
		{
			return Convert.ToBase64String(value);
		}

		/// <summary>
		/// 将Base64格式的字符串转化为字节数据。
		/// Convert Base64 format string to bytes data.
		/// </summary>
		/// <param name="value">
		/// Base64格式字符串。
		/// Base64 format string.
		/// </param>
		/// <returns>
		/// 字节数据。
		/// Bytes data.
		/// </returns>
		public byte[] ToBytes(string value)
		{
			return Convert.FromBase64String(value);
		}
	}
}
