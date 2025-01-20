using System;
namespace GstMagazin.Models
{
	public class TimeZoneHelper
	{
        private static readonly TimeZoneInfo MoroccoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Morocco Standard Time");

        public static DateTime GetCurrentMoroccoTime()
        {
            DateTime utcNow = DateTime.UtcNow;
            return TimeZoneInfo.ConvertTimeFromUtc(utcNow, MoroccoTimeZone);
        }
    }
}

