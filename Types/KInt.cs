namespace Kesco.Lib.Win.Data.Types
{
    /// <summary>
    /// Summary description for KInt.
    /// </summary>
    public class KInt
    {
        public static int FromXmlString(string s)
        {
            return FromXmlString(s, int.MinValue);
        }

        public static int FromXmlString(string s, int defaultValue)
        {
            return !string.IsNullOrEmpty(s) ? int.Parse(s) : defaultValue;
        }

        public static string ToXmlString(int n)
        {
            return (n == int.MinValue) ? "" : n.ToString();
        }
    }
}
