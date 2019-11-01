namespace Kesco.Lib.Win.Data.Types
{
	/// <summary>
	/// Summary description for KBoolean.
	/// </summary>
	public class KBoolean
	{
		public static bool FromXmlString(string atr)
		{
			return FromXmlString(atr,false);
		}

		public static bool FromXmlString(string atr, bool defaultValue)
		{
		    if (string.IsNullOrEmpty(atr))
		        return defaultValue;

		    switch (atr.ToLower())
		    {
		        case "true":
		            return true;
		        case "false":
		            return false;
            }

            return defaultValue;
		}

	    public static string ToXmlString(bool b)
		{
			return b.ToString().ToLower();
		}
	}
}
