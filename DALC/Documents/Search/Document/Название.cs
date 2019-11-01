using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
    /// <summary>
    /// Summary description for Название.
    /// </summary>
    [Option("Название", typeof (Название))]
    public class Название : TextOption
    {
        protected Название(XmlElement el)
            : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            emptyValueText = Resources.GetString("emptyValueText");

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPostfix = "";

            textItemPrefix = "'";
            textItemPostfix = "'";
        }

        public override string GetSQL(bool throwOnError)
        {
            if (Value.Length == 0)
                return " T0.НазваниеДокументаRL <> '' ";
            return GetSQLCondition("T0.НазваниеДокументаRL", true) +
                   " OR (T0.КодТипаДокумента IN (SELECT КодТипаДокумента FROM ТипыДокументов TD (NOLOCK) WHERE " +
                   GetSQLCondition("TD.ТипДокумента") + " OR " + GetSQLCondition("TD.TypeDoc") +
                   ") AND T0.НазваниеДокументаRL = '')";
        }

        protected string GetSQLCondition(string field, bool rl)
        {
            string val = Options.PrepareTextParameter(rl ? Replacer.ReplaceRusLat(Value) : Value).Trim();
			switch (Mode)
            {
                case Modes.Contains:
                    return field + " LIKE '%" + Regex.Replace(val, "[ ]{1,}", "%") + "%'";
                default:
                    return "' '+" + field + " LIKE '" + Regex.Replace(" " + val, "[ ]{1,}", "% ") + "%'";
            }
        }
    }
}