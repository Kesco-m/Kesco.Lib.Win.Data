using System;

namespace Kesco.Lib.Win.Data.Types
{
    /// <summary>
    /// Summary description for KDate.
    /// </summary>
    public class KDate
    {
        public static string ToXmlString(DateTime dt)
        {
            return dt.Equals(DateTime.MinValue) ? "" : dt.ToString("dd.MM.yyyy");
        }

        public static DateTime FromXmlString(string s)
        {
            return FromXmlString(s, DateTime.MinValue);
        }

        public static DateTime FromXmlString(string s, DateTime defaultValue)
        {
            return !string.IsNullOrEmpty(s) ? DateTime.Parse(s) : defaultValue;
        }
    }
}
