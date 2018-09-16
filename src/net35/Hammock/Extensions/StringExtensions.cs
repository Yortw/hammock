using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
#if SILVERLIGHT && !WindowsPhone
using System.Windows.Browser;
#endif

#if WindowsPhone
using System.Web;
#endif

#if !SILVERLIGHT && !MonoTouch && !NETCF && !WINRT
using System.Web;
#endif

namespace Hammock.Extensions
{
	internal static class StringExtensions
	{
		private static readonly Dictionary<char, char> UnreservedChars = (from c in Authentication.OAuth.OAuthTools.Unreserved.ToCharArray() select c).ToDictionary((c) => c);

		public static bool IsNullOrBlank(this string value)
		{
			return String.IsNullOrEmpty(value) ||
						 (!String.IsNullOrEmpty(value) && value.Trim() == String.Empty);
		}

		public static bool EqualsIgnoreCase(this string left, string right)
		{
#if !WINRT
			return String.Compare(left, right, StringComparison.InvariantCultureIgnoreCase) == 0;
#else
					return String.Compare(left, right, StringComparison.OrdinalIgnoreCase) == 0;
#endif
		}

		public static bool EqualsAny(this string input, params string[] args)
		{
			return args.Aggregate(false, (current, arg) => current | input.Equals(arg));
		}

		public static string FormatWith(this string format, params object[] args)
		{
			return String.Format(format, args);
		}

		public static string FormatWithInvariantCulture(this string format, params object[] args)
		{
			return String.Format(CultureInfo.InvariantCulture, format, args);
		}

		public static string Then(this string input, string value)
		{
			return String.Concat(input, value);
		}

		public static string UrlEncode(this string value)
		{
			// [DC] This is more correct than HttpUtility; it escapes spaces as %20, not +
			return Uri.EscapeDataString(value);
		}

		public static string UrlDecode(this string value)
		{
			return Uri.UnescapeDataString(value);
		}

		public static Uri AsUri(this string value)
		{
			return new Uri(value);
		}

		public static string ToBase64String(this byte[] input)
		{
			return Convert.ToBase64String(input);
		}

		public static byte[] GetBytes(this string input)
		{
			return Encoding.UTF8.GetBytes(input);
		}

		public static string PercentEncode(this string s) => new string(PercentEncode(s.ToCharArray()).ToArray());

		private static IEnumerable<char> PercentEncode(char[] chars)
		{
			if (chars == null)
			{
				yield break;
			}

			var i = 0;
			while (i < chars.Length)
			{
				var thisChar = chars[i];
				if (UnreservedChars.ContainsKey(thisChar) || thisChar == '%')
				{
					i += 1;
					yield return thisChar;
				}
				else
				{
					var nChars = GetNumberOfCharsForCharacter(chars, i);
					var bytes = Encoding.UTF8.GetBytes(chars, i, nChars);
					foreach (var b in bytes)
					{
						// Support proper encoding of special characters (\n\r\t\b)
						var resultString = (b > 7 && b < 11) || b == 13
								? $"%0{b:X}"
								: $"%{b:X}";
						foreach (var resultChar in resultString.ToCharArray())
						{
							yield return resultChar;
						}
					}
					i += nChars;
				}
			}
		}

		public static int GetNumberOfCharsForCharacter(char[] str, int index)
		{
			var firstChar = str[index];
#if WINRT
			var isSurrogate = CharUnicodeInfo.GetUnicodeCategory(firstChar) == UnicodeCategory.Surrogate;
#else
			var isSurrogate = char.GetUnicodeCategory(firstChar) == UnicodeCategory.Surrogate;
#endif
			if (!isSurrogate)
			{
				return 1;
			}
			if (index + 1 >= str.Length)
			{
				throw new ArgumentException($"Character at position {index} is a Surrogate but is the last character in the string.");
			}
#if WINRT
			if (CharUnicodeInfo.GetUnicodeCategory(str[index + 1]) != UnicodeCategory.Surrogate)
#else
			if (char.GetUnicodeCategory(str[index + 1]) != UnicodeCategory.Surrogate)
#endif
			{
				throw new ArgumentException($"Character at position {index} is a Surrogate but the next character is not a Surrogate.");
			}
			return 2;
		}

		public static IDictionary<string, string> ParseQueryString(this string query)
		{
			// [DC]: This method does not URL decode, and cannot handle decoded input
			if (query.StartsWith("?")) query = query.Substring(1);

			if (query.Equals(string.Empty))
			{
				return new Dictionary<string, string>();
			}

			var parts = query.Split(new[] { '&' });

			return parts.Select(
					part => part.Split(new[] { '=' })).ToDictionary(
							pair => pair[0], pair => pair[1]
					);
		}

		public static bool TryParse(this string value, out double number)
		{
			try
			{
				number = double.Parse(value);
				return true;
			}
			catch
			{
				number = 0;
				return false;
			}
		}

		private const RegexOptions Options =
#if !SILVERLIGHT && !MonoTouch && !WINRT
						RegexOptions.Compiled | RegexOptions.IgnoreCase;
#else
            RegexOptions.IgnoreCase;
#endif
	}
}