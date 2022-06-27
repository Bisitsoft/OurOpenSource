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
		/// Kelvin.
		/// </summary>
		Kelvin = 1,

		/// <summary>
		/// 摄氏度。
		/// Degrees Celsius.
		/// </summary>
		DegreesCelsius = 2,

		/// <summary>
		/// 华氏度。
		/// Degrees Fahrenheit.
		/// </summary>
		DegreesFahrenheit = 3
	}

	/// <summary>
	/// 用于存储温度数值的类。
	/// Class used to store temperature.
	/// </summary>
	public class Temperature : ICloneable
	{
		private static readonly bool checkTemperature = false;
		/// <summary>
		/// 是否在每次修改`<see cref="Temperature.Value"/>`的值时，进行检查，检查其是否大于等于`0K`。
		/// Whether to check whether the value of `<see cref="Temperature.Value"/>` is greater than or equal to `0K` each time you modify the value.
		/// </summary>
		/// <remarks>
		/// 检查过程中，如果检测到该值小于`0K`并且大于等于`-<see cref="double.Epsilon"/>`时，其会被自动纠正到`0K`。
		/// During the inspection, if it is detected that the value is less than `0K` and greater than or equal to `-<see cref="double.Epsilon"/>`, it will be automatically corrected to `0K`.
		/// </remarks>
		public static bool CheckTemperature { get { return checkTemperature; } }

		/// <summary>
		/// 以摄氏度为单位的绝对零度。
		/// Absolute zero in degrees Celsius.
		/// </summary>
		public static readonly double ZeroKelvin = -273.15;
		/// <summary>
		/// 获取`0K`。
		/// Get `0K`.
		/// </summary>
		/// <param name="unit">
		/// 目标温度单位。
		/// Target temperature unit.
		/// </param>
		/// <returns>
		/// 以`<paramref name="unit"/>`为单位表示的`0K`。
		/// `0K` in `<paramref name="unit"/>`.
		/// </returns>
		public static double GetZeroKelvin(TemperatureUnit unit) { return Convert(0, TemperatureUnit.Kelvin, unit); }
		/// <summary>
		/// 检查温度是否大于等于`0K`。
		/// Check whether the temperature is greater than or equal to `0K`.
		/// </summary>
		/// <remarks>
		/// 该方法不受`<see cref="CheckTemperature"/>`开关的影响。
		/// This method is not affected by the `<see cref="CheckTemperature"/>` switch.
		/// </remarks>
		/// <param name="value">
		/// 温度。
		/// Temperature.
		/// </param>
		/// <param name="unit">
		/// 温度的单位。
		/// Temperatue's unit.
		/// </param>
		/// <param name="correctedValue">
		/// 纠正后的值。检查过程中，如果检测到该值小于`0K`并且大于等于`-<see cref="double.Epsilon"/>`时，其会被自动纠正到`0K`。
		/// Corrected value. During the inspection, if it is detected that the value is less than `0K` and greater than or equal to `-<see cref="double.Epsilon"/>`, it will be automatically corrected to `0K`.
		/// </param>
		/// <returns>
		/// 如果温度不小于`0K`，则返回`<see langword="true"/>`。
		/// If temperature is not less than `0K`, the method will return `<see langword="true"/>`.
		/// </returns>
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
					correctedValue = Convert(0, TemperatureUnit.Kelvin, unit); // 自动纠正。 Automatically correct.
					return k >= -double.Epsilon;
				}
				correctedValue = value;
				return true;
			}
		}

		/// <summary>
		/// 克隆`<see cref="Temperature"/>`。
		/// Clone `<see cref="Temperature"/>`.
		/// </summary>
		/// <returns>
		/// 克隆到的`<see cref="Temperature"/>`。
		/// Cloned `<see cref="Temperature"/>`.
		/// </returns>
		public object Clone()
		{
			return new Temperature(value, unit);
		}

		private double value;
		/// <summary>
		/// 温度数值。
		/// Temperature value.
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
					//			this.value = Convert(0, TemperatureUnit.Kelvin, this.unit); // 自动纠正。 Automatically correct.
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
		/// Temperature unit.
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
		/// Convert temperature.
		/// </summary>
		/// <param name="origin">
		/// 原温度。
		/// Original temperature.
		/// </param>
		/// <param name="targetUnit">
		/// 目标温度单位。
		/// Target temperature unit.
		/// </param>
		/// <returns>
		/// 转换后的温度。
		/// Converted temperature.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// 当温度单位不存在时，会抛出此异常。
		/// This exception is thrown when the temperature unit does not exist.
		/// </exception>
		public static Temperature Convert(Temperature origin, TemperatureUnit targetUnit)
		{
			// 不必要，大部分情况都是需要转换的。 Unnecessarily, most cases need to be converted.
			//if (value.unit == targetUnit)
			//{
			//	return new Temperature(value.value, value.unit);
			//}
			return new Temperature(Convert(origin.Value, origin.Unit, targetUnit), targetUnit);
		}
		/// <summary>
		/// 转化温度。
		/// Convert temperature.
		/// </summary>
		/// <param name="origin">
		/// 原温度数值。
		/// Original temperature value.
		/// </param>
		/// <param name="originUnit">
		/// 原温度单位。
		/// Original temperature unit.
		/// </param>
		/// <param name="targetUnit">
		/// 目标温度单位。
		/// Target temperature unit.
		/// </param>
		/// <returns>
		/// 转换后的温度。
		/// Converted temperature.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// 当温度单位不存在时，会抛出此异常。
		/// This exception is thrown when the temperature unit does not exist.
		/// </exception>
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
		/// <summary>
		/// 将开尔文转化为摄氏度。
		/// Convert Kelvin to Celsius.
		/// </summary>
		/// <param name="origin">
		/// 开尔文数值。
		/// Value in Kelvin.
		/// </param>
		/// <returns>
		/// 摄氏度数值。
		/// Value in Celsius.
		/// </returns>
		public static double KelvinToCelsius(double origin)
		{
			return origin - ZeroKelvin;
		}
		/// <summary>
		/// 将开尔文转化为华氏度。
		/// Convert Kelvin to Fahrenheit.
		/// </summary>
		/// <param name="origin">
		/// 开尔文数值。
		/// Value in Kelvin.
		/// </param>
		/// <returns>
		/// 华氏度数值。
		/// Value in Fahrenheit.
		/// </returns>
		public static double KelvinToFahrenheit(double origin)
		{
			return CelsiusToFahrenheit(KelvinToCelsius(origin));
		}
		/// <summary>
		/// 将摄氏度转化为开尔文。
		/// Convert Celsius to Kelvin.
		/// </summary>
		/// <param name="origin">
		/// 摄氏度数值。
		/// Value in Celsius.
		/// </param>
		/// <returns>
		/// 开尔文数值。
		/// Value in Kelvin.
		/// </returns>
		public static double CelsiusToKelvin(double origin)
		{
			return origin + ZeroKelvin;
		}
		/// <summary>
		/// 将摄氏度转化为华氏度。
		/// Convert Celsius to Fahrenheit.
		/// </summary>
		/// <param name="origin">
		/// 摄氏度数值。
		/// Value in Celsius.
		/// </param>
		/// <returns>
		/// 华氏度数值。
		/// Value in Fahrenheit.
		/// </returns>
		public static double CelsiusToFahrenheit(double origin)
		{
			return origin * 1.8 + 32;
		}
		/// <summary>
		/// 将华氏度转化为摄氏度。
		/// Convert Fahrenheit to Celsius.
		/// </summary>
		/// <param name="origin">
		/// 华氏度数值。
		/// Value in Fahrenheit.
		/// </param>
		/// <returns>
		/// 摄氏度数值。
		/// Value in Celsius.
		/// </returns>
		public static double FahrenheitToCelsius(double origin)
		{
			return (origin - 32) / 1.8;
		}
		/// <summary>
		/// 将华氏度转化为开尔文。
		/// Convert Fahrenheit to Kelvin.
		/// </summary>
		/// <param name="origin">
		/// 华氏度数值。
		/// Value in Fahrenheit.
		/// </param>
		/// <returns>
		/// 开尔文数值。
		/// Value in Kelvin.
		/// </returns>
		public static double FahrenheitToKelvin(double origin)
		{
			return CelsiusToKelvin(FahrenheitToCelsius(origin));
		}
		/// <summary>
		/// 将任意单位的温度转化为开尔文。
		/// Convert temperature in any unit to Kelvin.
		/// </summary>
		/// <param name="originValue">
		/// 原温度数值。
		/// Original temperature value.
		/// </param>
		/// <param name="originUnit">
		/// 原温度单位。
		/// Original temperature unit.
		/// </param>
		/// <returns>
		/// 转换后的温度。
		/// Converted temperature.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// 当温度单位不存在时，会抛出此异常。
		/// This exception is thrown when the temperature unit does not exist.
		/// </exception>
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
		/// <summary>
		/// 将任意单位的温度转化为摄氏度。
		/// Convert temperature in any unit to Celsius.
		/// </summary>
		/// <param name="originValue">
		/// 原温度数值。
		/// Original temperature value.
		/// </param>
		/// <param name="originUnit">
		/// 原温度单位。
		/// Original temperature unit.
		/// </param>
		/// <returns>
		/// 转换后的温度。
		/// Converted temperature.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// 当温度单位不存在时，会抛出此异常。
		/// This exception is thrown when the temperature unit does not exist.
		/// </exception>
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
		/// <summary>
		/// 将任意单位的温度转化为华氏度。
		/// Convert temperature in any unit to Fahrenheit.
		/// </summary>
		/// <param name="originValue">
		/// 原温度数值。
		/// Original temperature value.
		/// </param>
		/// <param name="originUnit">
		/// 原温度单位。
		/// Original temperature unit.
		/// </param>
		/// <returns>
		/// 转换后的温度。
		/// Converted temperature.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// 当温度单位不存在时，会抛出此异常。
		/// This exception is thrown when the temperature unit does not exist.
		/// </exception>
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
		/// 将温度转化为字符串。
		/// Convert temperature to string.
		/// </summary>
		/// <returns>
		/// 字符串。
		/// String.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// 当温度单位不存在时，会抛出此异常。
		/// This exception is thrown when the temperature unit does not exist.
		/// </exception>
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
		/// Convert string to temperature.
		/// </summary>
		/// <remarks>
		/// 字符串格式应当为整数、浮点数或科学记数法表示的数字（`E`的大小写不分）+单位`K`（开尔文）、`C`（摄氏度）或`F`（华氏度）（不分大小写），
		/// 两部分之间或之外允许有空白符（由Unicode定义）如：`300K`、`4.5 C   `、` -1E-1 F`。
		/// The format of string should be integer/float/number in scientific counting (ignore `E` case) + unit `K`(Kelvin)/`C`(Celsius)/`F`(Fahrenheit) (all of them are ignored case).
		/// White space (defined by Unicode) is allowed between or outside two parts. Such as `300K`, `4.5 C   `, ` -1E-1 F`.
		/// </remarks>
		/// <param name="text">
		/// 字符串。
		/// String.
		/// </param>
		/// <returns>
		/// 温度。
		/// Temperature.
		/// </returns>
		/// <exception cref="FormatException">
		/// 当字符串格式不对时，会抛出此异常。
		/// This exception is thrown when the string format is incorrect.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// 当温度单位不存在时，会抛出此异常。
		/// This exception is thrown when the temperature unit does not exist.
		/// </exception>
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
		/// 初始化`<see cref="Temperature"/>`。
		/// Initialize `<see cref="Temperature"/>`.
		/// </summary>
		/// <param name="value">
		/// 温度值。
		/// Temperature value.
		/// </param>
		/// <param name="unit">
		/// 温度单位。
		/// Temperature unit.
		/// </param>
		/// <param name="ignoreCheckTemperatureOnce">
		/// 是否在初始化时跳过绝对零度检查。
		/// Whether to skip absolute zero check during initialization.
		/// </param>
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
