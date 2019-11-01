using System;
using System.Text.RegularExpressions;

namespace Kesco.Lib.Win.Data.Business.V2.FilterOptions
{
	public struct FOptItem
	{
		public string value;
		public FOptItemFlags flags;
		internal string rValue;
		internal string txtValue;

		public bool IsNull
		{
			get{return (flags&FOptItemFlags.IsNull)==FOptItemFlags.IsNull;}
		}
		
		public bool GetValue(ref DateTime dt)
		{
		    Match m = Regex.Match(value, "^(\\d{4,4}|)(\\d{2,2}|)(\\d{2,2}|)(\\d{2,2}|)(\\d{2,2}|)(\\d{2,2}|)$");

		    if (!m.Success)
		        return false;

		    int y = m.Groups[1].Length > 0 ? int.Parse(m.Groups[1].Value) : 0;
		    int MM = m.Groups[2].Length > 0 ? int.Parse(m.Groups[2].Value) : 0;
		    int d = m.Groups[3].Length > 0 ? int.Parse(m.Groups[3].Value) : 0;
		    int hh = m.Groups[4].Length > 0 ? int.Parse(m.Groups[4].Value) : 0;
		    int mm = m.Groups[5].Length > 0 ? int.Parse(m.Groups[5].Value) : 0;
		    int ss = m.Groups[6].Length > 0 ? int.Parse(m.Groups[6].Value) : 0;
		    dt = new DateTime(y, MM, d, hh, mm, ss);

		    return true;
		}

	    public bool GetValue(ref decimal d)
		{
			return decimal.TryParse(value.Replace(".", ","), out d);
		}

		public bool GetValue(ref double d)
		{
			return double.TryParse(value.Replace(".",","), out d);
		}

		public static FOptItem Null = new FOptItem("",FOptItemFlags.IsNull);
		
		
		public FOptItem(int value):this(value,FOptItemFlags.Equals){}
		public FOptItem(int value, FOptItemFlags flags):this(value.ToString(),flags){}
	
        public FOptItem(decimal value):this(value,FOptItemFlags.Equals){}
		public FOptItem(decimal value, FOptItemFlags flags):this(value.ToString().Replace(",","."),flags){}
		
		public FOptItem(double value):this(value,FOptItemFlags.Equals){}
		public FOptItem(double value, FOptItemFlags flags):this(value.ToString().Replace(",","."),flags){}
			
		public FOptItem(DateTime value):this(value,FOptItemFlags.Equals){}
		public FOptItem(DateTime value, FOptItemFlags flags):this(value.ToString("yyyyMMddHHmmss"),flags){}
		
		public FOptItem(string value):this(value,FOptItemFlags.Equals){}
		public FOptItem(string value, FOptItemFlags flags)
		{
		    this.value = value;
		    this.flags = flags;
		    rValue = string.Empty;
		    txtValue = string.Empty;
		}
	}
}
