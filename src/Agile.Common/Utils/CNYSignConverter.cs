using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agile.Common.Utils
{
	/// <summary>
	/// 将人民币小写金额转换为人民币大写金额
	/// <example>1688.99 ==> 壹仟陆佰捌拾捌元玖角玖分</example>
	/// 将数字首先左边补0，补位至4的整数倍，然后按照4个数字一组分组。每组分别是 千 百 十 个 四位
	/// 例如： 123456 => 0012 3456
	/// 最后按照人民币大写规则转化即可
	/// </summary>
	public class CNYSignConverter
	{
		public static readonly string Prefix = "人民币";
		/// <summary>  
		/// 人民币中文大写数值数组，其值为："零","壹","贰","叁",
		/// "肆","伍","陆","柒","捌","玖"
		/// </summary>
		public static readonly string[] UPPER_DIGIT = new string[] { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };

		public static readonly string UPPER_ZERO = "零";
		/// <summary>  
		/// 货币单位数组，其值为："元","角","分"。
		/// </summary>
		public static readonly string[] MONEY_UNIT = new string[] { "元", "角", "分" };

		public static readonly string[] DIGIT_UNIT = new string[] { "", "拾", "佰", "仟" };

		public static readonly string[] DIVID_UNIT = new string[] { "", "万", "亿", "万亿" };
		//最多有两位小数
		public string ConverTo(decimal source)
		{
			if (source < 0) return "";
			if (source > 10000000000000000) throw new ArgumentOutOfRangeException("source", "超出千万亿了！");
			StringBuilder builder = new StringBuilder(Prefix);
			var charArray = source.ToString("#0.00").ToCharArray();
			var indexOfPoint = Array.FindIndex(charArray, ch => ch == '.');
			var integerPart = charArray.Take(indexOfPoint).ToList();
			var decimalPart = charArray.Skip(indexOfPoint + 1).ToArray();
			string decimalStr = new string(decimalPart).TrimEnd('0');//清除小数末尾的0
			decimalPart = decimalStr.ToCharArray();
			int decimalPartLength = decimalPart.Length;
			int integerPartLength = integerPart.Count;
			//补齐
			int numberOfPart = (int)Math.Ceiling((double)integerPartLength / 4);
			int leftSize = numberOfPart * 4 - integerPartLength;
			for (var index = 0; index < leftSize; index++)
			{
				integerPart.Insert(0, '0');
			}


			bool lastDividAllZero = false;
			for (int outerIndex = 0; outerIndex < numberOfPart; outerIndex++)
			{
				var list = new List<string>();
				int lastDigit = integerPart[0] - '0';
				for (int innerIndex = 0 + outerIndex * 4; innerIndex < 4 + outerIndex * 4; innerIndex++)
				{
					var ch = integerPart[innerIndex];
					int digit = ch - '0';
					if (digit != 0 || lastDigit != 0)//不可以出现连续的零
					{
						list.Add(UPPER_DIGIT[digit]);
					}

					if (digit != 0)
					{
						//拼接 千以内的单位
						int digitUnitIndexToAppend = (4 + outerIndex * 4 - innerIndex - 1) % 4;
						list.Add(DIGIT_UNIT[digitUnitIndexToAppend]);
					}
					lastDigit = digit;
				}
				if (list.LastOrDefault() == UPPER_ZERO)
				{
					list.RemoveAt(list.Count - 1);
				}
				if (!list.Any())
				{
					//ignore
					lastDividAllZero = true;
					continue;
				}

				//如果当前区间千位是0那么要添加一个零。例如100001 => 壹拾万零壹元整
				if (outerIndex != 0 && integerPart[outerIndex * 4] == '0')
				{
					builder.Append(UPPER_ZERO);
				}
				//如果上一个区间都是0，且当前区间千位不是0，那么要添加一个0。例如100001000 =>
				else if (outerIndex != 0 && lastDividAllZero)
				{
					builder.Append(UPPER_ZERO);
				}
				builder.Append(string.Join("", list)).Append(DIVID_UNIT[numberOfPart - outerIndex - 1]);
				lastDividAllZero = false;
			}
			builder.Append(MONEY_UNIT[0]);
			if (decimalPartLength == 0)
			{
				builder.Append(UPPER_ZERO);
			}
			else
			{
				if (decimalPart[0] != '0')
				{
					builder.Append(UPPER_DIGIT[decimalPart[0] - '0']).Append(MONEY_UNIT[1]);
				}
				else
				{
					builder.Append(UPPER_ZERO);
				}
				if (decimalPartLength == 2)
				{
					builder.Append(UPPER_DIGIT[decimalPart[1] - '0']).Append(MONEY_UNIT[2]);
				}
			}
			return builder.ToString();
		}
	}
}
