using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hammock.WinRT.Compat
{
	public static class DateTimeExtensions
	{

		public static string ToShortTimeString(this DateTime value)
		{
			return value.ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortDatePattern + " " + System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortTimePattern);
		}
	}
}
