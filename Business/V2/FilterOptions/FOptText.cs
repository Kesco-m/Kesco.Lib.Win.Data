using System.Text.RegularExpressions;

namespace Kesco.Lib.Win.Data.Business.V2.FilterOptions
{
	public class FOptText : FOpt
	{
		public static Regex wordPattern = new Regex("((?<=\")[^\"]+(?=\"))|[0-9À-ßA-Z_]+", RegexOptions.IgnoreCase);
		protected MatchCollection words;
		string text = "";
		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		protected bool rl;

		public static string SqlEscape(string s)
		{
			var chr9 = new string((char)9, 1);
			var chr10 = new string((char)10, 1);
			var chr13 = new string((char)13, 1);
			var chr160 = new string((char)160, 1);

			return s.Replace(chr9, " ").
				Replace(chr10, " ").
				Replace(chr13, " ").
				Replace(chr10, " ").
				Replace(chr160, " ").
				Replace("[", "[[]").
				Replace("_", "[_]").
				Replace("%", "[%]").
				Replace("'", "''");
		}

		public static string GetWords(string s, string wordPattern)
		{
			return GetWords(s, new Regex(wordPattern, RegexOptions.IgnoreCase));
		}

		public static string GetWords(string s, Regex wordPattern)
		{
			string k = "";
			MatchCollection m = wordPattern.Matches(s);
			for (int i = 0; i < m.Count; i++) k += (i > 0 ? " " : "") + m[i].Value;
			return k;
		}

		public string GetWords(string s)
		{
			return GetWords(s, wordPattern);
		}

		public FOptText(Dso ds, string id)
			: base(ds, id)
		{
		}
	}
}