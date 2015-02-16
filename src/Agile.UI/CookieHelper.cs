using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Agile.UI
{
	public class CookieHelper
	{
		public static void SetCookie(HttpCookie cookie)
		{
			HttpContext.Current.Response.Cookies.Set(cookie);
		}

		public static void SetCookie(string key, string value)
		{
			if (string.IsNullOrWhiteSpace(key)) return;
			key = HttpContext.Current.Server.UrlEncode(key);
			value = HttpContext.Current.Server.UrlEncode(value);

			HttpCookie cookie = new HttpCookie(key, value);
			SetCookie(cookie);
		}
		public static void SetCookie(string key, string value, DateTime expire)
		{
			if (string.IsNullOrWhiteSpace(key)) return;
			key = HttpContext.Current.Server.UrlEncode(key);
			value = HttpContext.Current.Server.UrlEncode(value);

			HttpCookie cookie = new HttpCookie(key, value) {Expires = expire};
			SetCookie(cookie);
		}

		public static void SetCookie(string key, string value, TimeSpan expireAfter)
		{
			SetCookie(key, value, DateTime.Now + expireAfter);
		}

		public static void SetCookie(string key, string value, int minutes)
		{
			SetCookie(key, value, DateTime.Now.AddMinutes(minutes));
		}

		public static void RemoveCookie(string key)
		{
			SetCookie(key, "", DateTime.Now.AddYears(-10));
		}

		public static HttpCookie GetCookie(string key)
		{
			key = HttpContext.Current.Server.UrlEncode(key);
			return HttpContext.Current.Request.Cookies.Get(key);
		}

		public static string GetCookieValue(string key)
		{
			var cookie = GetCookie(key);
			return cookie != null ? HttpContext.Current.Server.UrlDecode(cookie.Value) : "";
		}

		#region encrypt cookie

		#endregion
	}
}
