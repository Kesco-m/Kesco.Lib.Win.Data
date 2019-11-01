using System.Data;
using System.Text.RegularExpressions;

namespace Kesco.Lib.Win.Data.Types
{
    /// <summary>
    /// Summary description for String.
    /// </summary>
    public class KString : KType
    {
        private string s;

        public static string FromXmlString(string s)
        {
            return s;
        }

        public static string ToXmlString(string s)
        {
            return s;
        }

        public override string Xml
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// здесь опасные символы замен€ютс€ на esacpe последовательности используемые в SQL выражени€х
        /// (аналог функции »нвентаризаци€.dbo.fn_ReplaceKeySymbols)
        /// </summary>
        public string SqlEscaped
        {
            get
            {
                var chr9 = new string((char) 9, 1);
                var chr10 = new string((char) 10, 1);
                var chr13 = new string((char) 13, 1);
                var chr160 = new string((char) 160, 1);

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
        }

        public string SqlLikeWords
        {
            get { return "% " + Regex.Replace(SqlEscaped, "[ ]{1,}", "% ").Trim() + "%"; }
        }

        public override void Fill(DataRow row)
        {
            if (isUndefined = row.IsNull(SqlColumnName)) return;
            s = (string) row[SqlColumnName];
        }

        public KString(string s) : base("", "", SqlDbType.Int, "")
        {
            this.s = s;
        }

        public KString() : base("", "", SqlDbType.Int, "")
        {

        }
    }
}
