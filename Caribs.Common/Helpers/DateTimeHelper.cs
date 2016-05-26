using System;
using System.ComponentModel;
using System.Windows.Forms.VisualStyles;

namespace Caribs.Common.Helpers
{
    public enum TimeZone
    {
        [Description("FLE Standard Time")]
        Ukraine,
        [Description("Atlantic Standard Time")]
        Caribs
    }
    public static class DateTimeHelper
    {
        public static DateTime ToLocalizedDateTime(this DateTime dt, TimeZone timeZone)
        {
            var timeZoneName = timeZone.GetAttributeOfType<DescriptionAttribute>().Description;
            var info = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
            var localServerTime = DateTimeOffset.Now;
            return TimeZoneInfo.ConvertTime(localServerTime, info).DateTime;
        }
    }
}
