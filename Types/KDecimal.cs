using System.Globalization;

namespace Kesco.Lib.Win.Data.Types
{
    /// <summary>
    /// Summary description for KDecimal.
    /// </summary>
    public class KDecimal
    {

        public static decimal FromXmlString(string s)
        {
            return FromXmlString(s, decimal.MinValue);
        }

        public static decimal FromXmlString(string s, decimal defaultValue)
        {
            return !string.IsNullOrEmpty(s) ? decimal.Parse(s.Replace(".", ",")) : defaultValue;
        }

        public static string ToHtmlString(decimal d, int scale)
        {
            if (d == decimal.MinValue)
                return "";

            var nfi = (NumberFormatInfo) NumberFormatInfo.CurrentInfo.Clone();
            nfi.CurrencySymbol = "";
            nfi.CurrencyDecimalDigits = scale;
            return d.ToString("C", nfi);

        }
    }
}
