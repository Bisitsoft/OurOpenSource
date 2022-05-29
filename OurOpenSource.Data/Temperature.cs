using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OurOpenSource.Data
{
	/// <summary>
	/// 温度计量单位。
	/// </summary>
	public enum TemperatureUnit
	{
		/// <summary>
		/// 开尔文。
		/// </summary>
		Kelvin = 1,

		/// <summary>
		/// 摄氏度。
		/// </summary>
		DegreesCelsius = 2,

		/// <summary>
		/// 华氏度。
		/// </summary>
		DegreesFahrenheit = 3
	}

	/// <summary>
	/// 温度。
	/// 用存储温度数值的类。
	/// </summary>
	public class Temperature : ICloneable
	{
		private static readonly bool checkTemperature = false;
		/// <summary>
		/// 是否在每次修改`Temperature.Value`的值时，进行检查，检查其是否大于等于`0K`。
		/// </summary>
		/// <remarks>
		/// 检查过程中，如果检测到该值小于`0K`并且大于等于`-double.Epsilon`时，其会被自动纠正到`0K`。
		/// </remarks>
		public static bool CheckTemperature { get { return checkTemperature; } }

		/// <summary>
		/// 以摄氏度为单位的绝对零度。
		/// </summary>
		public static readonly double ZeroKelvin = -273.15;
		/// <summary>
		/// 获取`0K`。
		/// </summary>
		/// <param name="unit">目标温度单位。</param>
		/// <returns>以`unit`为单位表示的`0K`。</returns>
		public static double GetZeroKelvin(TemperatureUnit unit) { return Convert(0, TemperatureUnit.Kelvin, unit); }
		/// <summary>
		/// 检查温度是否大于等于`0K`。
		/// </summary>
		/// <remarks>
		/// 该方法不受`CheckTemperature`开关的影响。
		/// </remarks>
		/// <param name="value">温度。</param>
		/// <param name="unit">温度的单位。</param>
		/// <param name="correctedValue">纠正后的值。检查过程中，如果检测到该值小于`0K`并且大于等于`-double.Epsilon`时，其会被自动纠正到`0K`。</param>
		/// <returns>温度是否大于等于`0K`。</returns>
		public static bool IsGreaterOrEqualZeroKelvin(double value, TemperatureUnit unit, out double correctedValue)
		{
			if (unit == TemperatureUnit.Kelvin)
			{
				if (value >= 0)
				{
					correctedValue = value;
					return true;
				}
				else
				{
					correctedValue = 0;
					return false;
				}
			}
			else
			{
				double k = ToKelvin(value, unit);
				if (k < 0)
				{
					correctedValue = Convert(0, TemperatureUnit.Kelvin, unit); //自动纠正
					return k >= -double.Epsilon;
				}
				correctedValue = value;
				return true;
			}
		}

		/// <summary>
		/// 克隆温度。
		/// </summary>
		/// <returns>克隆到的温度。</returns>
		public object Clone()
		{
			return new Temperature(value, unit);
		}

		private double value;
		/// <summary>
		/// 温度数值。
		/// </summary>
		public double Value
		{
			get { return value; }
			set
			{
				if (CheckTemperature)
				{
					if (!IsGreaterOrEqualZeroKelvin(value, this.unit, out double correctedValue))
					{
						throw new ArgumentOutOfRangeException($"It should be greater than or equal to {GetZeroKelvin(unit)}.", "value");
					}
					this.value = correctedValue;

					//if (unit == TemperatureUnit.Kelvin)
					//{
					//	if (value >= 0)
					//	{
					//		this.value = value;
					//	}
					//	throw new ArgumentOutOfRangeException("It should be greater than or equal to 0K.", "value");
					//}
					//else
					//{
					//	double k = ToKelvin(value, this.unit);
					//	if (k < 0 && k >= -double.Epsilon)
					//	{
					//		if (k >= -double.Epsilon)
					//		{
					//			this.value = Convert(0, TemperatureUnit.Kelvin, this.unit); //自动纠正
					//			return;
					//		}
					//		throw new ArgumentOutOfRangeException($"It should be greater than or equal to {GetZeroKelvin(unit).ToString()}.", "value");
					//	}
					//}
				}
				this.value = value;
			}
		}
		private TemperatureUnit unit;
		/// <summary>
		/// 温度单位。
		/// </summary>
		public TemperatureUnit Unit
		{
			get { return unit; }
			private set
			{
				if (!Enum.IsDefined(typeof(TemperatureUnit), value))
				{
					throw new ArgumentException("Unknown TemperatureUnit.", "unit");
				}
				this.unit = value;
			}
		}

		/// <summary>
		/// 转化温度。
		/// </summary>
		/// <param name="origin">原温度。</param>
		/// <param name="targetUnit">目标温度单位。</param>
		/// <returns>转换后的温度。</returns>
		public static Temperature Convert(Temperature origin, TemperatureUnit targetUnit)
		{
			// 不必要，大部分情况都是需要转换的。
			//if (value.unit == targetUnit)
			//{
			//	return new Temperature(value.value, value.unit);
			//}
			return new Temperature(Convert(origin.Value, origin.Unit, targetUnit), targetUnit);
		}
		/// <summary>
		/// 转化温度。
		/// </summary>
		/// <param name="origin">原温度数值。</param>
		/// <param name="originUnit">原温度单位。</param>
		/// <param name="targetUnit">目标温度单位。</param>
		/// <returns>转换后的温度。</returns>
		/// <exception cref="ArgumentException">当温度单位不存在时，会抛出此异常。</exception>
		public static double Convert(double origin, TemperatureUnit originUnit, TemperatureUnit targetUnit)
		{
			switch (targetUnit)
			{
				case TemperatureUnit.Kelvin:
					return ToKelvin(origin, originUnit);
				case TemperatureUnit.DegreesCelsius:
					return ToCelsius(origin, originUnit);
				case TemperatureUnit.DegreesFahrenheit:
					return ToFahrenheit(origin, originUnit);
				default:
					throw new ArgumentException("Unknown TemperatureUnit.", "targetUnit");
			}
		}
		public static double KelvinToCelsius(double origin)
		{
			return origin - ZeroKelvin;
		}
		public static double KelvinToFahrenheit(double origin)
		{
			return CelsiusToFahrenheit(KelvinToCelsius(origin));
		}
		public static double CelsiusToKelvin(double origin)
		{
			return origin + ZeroKelvin;
		}
		public static double CelsiusToFahrenheit(double origin)
		{
			return origin * 1.8 + 32;
		}
		public static double FahrenheitToCelsius(double origin)
		{
			return (origin - 32) / 1.8;
		}
		public static double FahrenheitToKelvin(double origin)
		{
			return CelsiusToKelvin(FahrenheitToCelsius(origin));
		}
		public static double ToKelvin(double originValue, TemperatureUnit originUnit)
		{
			switch (originUnit)
			{
				case TemperatureUnit.Kelvin:
					return originValue;
				case TemperatureUnit.DegreesCelsius:
					return CelsiusToKelvin(originValue);
				case TemperatureUnit.DegreesFahrenheit:
					return FahrenheitToKelvin(originValue);
				default:
					throw new ArgumentException("Unknown TemperatureUnit.", "originUnit");
			}
		}
		public static double ToCelsius(double originValue, TemperatureUnit originUnit)
		{
			switch (originUnit)
			{
				case TemperatureUnit.Kelvin:
					return KelvinToCelsius(originValue);
				case TemperatureUnit.DegreesCelsius:
					return originValue;
				case TemperatureUnit.DegreesFahrenheit:
					return FahrenheitToCelsius(originValue);
				default:
					throw new ArgumentException("Unknown TemperatureUnit.", "originUnit");
			}
		}
		public static double ToFahrenheit(double originValue, TemperatureUnit originUnit)
		{
			switch (originUnit)
			{
				case TemperatureUnit.Kelvin:
					return FahrenheitToKelvin(originValue);
				case TemperatureUnit.DegreesCelsius:
					return FahrenheitToCelsius(originValue);
				case TemperatureUnit.DegreesFahrenheit:
					return originValue;
				default:
					throw new ArgumentException("Unknown TemperatureUnit.", "originUnit");
			}
		}

		/// <summary>
		/// 降温度转化为字符串。
		/// </summary>
		/// <returns>字符串。</returns>
		/// <exception cref="ArgumentException">当温度单位不存在时，会抛出此异常。</exception>
		public override string ToString()
		{
			switch (this.unit)
			{
				case TemperatureUnit.Kelvin:
					return $"{this.value}K";
				case TemperatureUnit.DegreesCelsius:
					return $"{this.value}C";
				case TemperatureUnit.DegreesFahrenheit:
					return $"{this.value}F";
				default:
					throw new ArgumentException("Unknown error.", "unit");
			}
		}

		private static readonly Regex parse_regex = new Regex("^[\\s]*([\\S]*?)[\\s]*([KkCcFf])[\\s]*$");
		/// <summary>
		/// 将字符串转化为温度。
		/// </summary>
		/// <remarks>
		/// 字符串格式应当为整数、浮点数或科学记数法表示的数字（E的大小写不分）+单位K（开尔文）、C（摄氏度）或F（华氏度）（不分大小写），两部分之间或之外允许有空白符（由Unicode定义）如：
		/// `300K`、`4.5 C   `、` -1E-1 F`。
		/// </remarks>
		/// <param name="text">字符串。</param>
		/// <returns>温度。</returns>
		/// <exception cref="FormatException">当字符串格式不对时，会抛出此异常。</exception>
		/// <exception cref="ArgumentException">当温度单位不存在时，会抛出此异常。</exception>
		public static Temperature Parse(string text)
		{
			Match match = parse_regex.Match(text);
			if (match.Success)
			{
				double value;
				try
				{
					value = double.Parse(match.Groups[1].Value);
				}
				catch (Exception ex)
				{
					throw new FormatException(ex.Message, ex);
				}

				TemperatureUnit unit;
				switch (match.Groups[2].Value[0])
				{
					case 'K':
					case 'k':
						unit = TemperatureUnit.Kelvin;
						break;
					case 'C':
					case 'c':
						unit = TemperatureUnit.DegreesCelsius;
						break;
					case 'F':
					case 'f':
						unit = TemperatureUnit.DegreesFahrenheit;
						break;
					default:
						throw new ArgumentException("Unknown error.", "unitCharacter");
				}

				return new Temperature(value, unit);
			}
			else
			{
				throw new FormatException("Syntax error.");
			}
		}

		/// <summary>
		/// 初始化温度。
		/// </summary>
		/// <param name="value">温度值。</param>
		/// <param name="unit">温度单位。</param>
		/// <param name="ignoreCheckTemperatureOnce">是否在初始化时跳过绝对零度检查。</param>
		public Temperature(double value, TemperatureUnit unit, bool ignoreCheckTemperatureOnce = false)
		{
			this.Unit = unit; //应当被先设置，因为value依赖unit。
			if (ignoreCheckTemperatureOnce)
			{
				this.value = value;
			}
			else
			{
				this.Value = value;
			}
		}
	}
}
