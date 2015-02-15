using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Agile.Framework;

namespace Agile.CaptchaDictionaryGenerator
{

	class Program
	{
		private static readonly Regex wordPattern = new Regex(@"^[a-z]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static void Main(string[] args)
		{
			string source = @"~/basic-words.txt"; //日常使用单词
			//string source = @"~/common-words.txt";//常见单词
			//string source = @"~/full-words.txt";//所有单词

			string output = @"~/dictionary.txt";
			var sourcePath = WebHelper.MapPath(source);
			var minLength = 5;
			var maxLength = 7;
			if (string.IsNullOrWhiteSpace(source))
			{
				Error("source is null");
				return;
			}
			if (string.IsNullOrWhiteSpace(output))
			{
				Error("output is null");
				return;
			}
			if (!System.IO.File.Exists(sourcePath))
			{
				Error("source file not exists.{0}", sourcePath);
				return;
			}
			using (var sourceFs = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
			using (var sourceSr = new StreamReader(sourceFs))
			using (var outFs = new FileStream(WebHelper.MapPath(output), FileMode.Create, FileAccess.Write))
			using (var outSw = new StreamWriter(outFs))
			{
				var line = sourceSr.ReadLine();
				while (line != null)
				{
					var word = line.ToLower().Trim();
					if (word.Length >= minLength && word.Length <= maxLength && wordPattern.IsMatch(word))
					{
						outSw.WriteLine(word.PadRight(maxLength));
					}
					line = sourceSr.ReadLine();
				}
			}
			Error("COMPLETE...");
			Console.ReadKey();
		}

		static void Error(string format, params object[] args)
		{
			var old = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine(format, args);
			Console.ForegroundColor = old;
		}
	}
}
