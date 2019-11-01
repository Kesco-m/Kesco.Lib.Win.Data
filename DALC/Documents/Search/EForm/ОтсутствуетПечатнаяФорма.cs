using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm
{
    [Option("ќтсутствуетѕечатна€‘орма", typeof (ќтсутствуетѕечатна€‘орма))]
    public class ќтсутствуетѕечатна€‘орма : ListOption
    {
        protected ќтсутствуетѕечатна€‘орма(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"Image.»зображение"};
            NegativeValueOption = new[] {"ѕечатна€‘орма"};
            mask = "";
            emptyValueText = Resources.GetString("emptyValueText");
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPrefix2 = Resources.GetString("htmlPrefix2");
            htmlPostfix = "";

            textItemPrefix = "[";
            textItemPostfix = "]";
        }

        public override string GetSQL(bool throwOnError)
        {
            string sql =
                @"	NOT EXISTS (SELECT *
                    FROM ƒокументы.dbo.vw»зображени€ƒокументов TI WITH(NOLOCK)
                    WHERE TI. одƒокумента=T0. одƒокумента AND  одѕечатной‘ормы ";

            if (Value.Length > 0)
                if (Mode == Modes.AND)
                    sql += "IN (SELECT  одѕечатной‘ормы FROM vwѕечатные‘ормы WITH(NOLOCK) WHERE " +
                           GetSQLCondition("RTRIM(ѕечатна€‘орма) = '@VAL'") + "))";
                else
                    sql =
                        GetSQLCondition(sql +
                                        "IN (SELECT  одѕечатной‘ормы FROM vwѕечатные‘ормы WITH(NOLOCK) WHERE RTRIM(ѕечатна€‘орма) = '@VAL'))")
                            .Replace("OR", "AND");
            else
                sql += "> 0)";

            return sql;
        }

        public override string GetItemText(string key)
        {
            return string.IsNullOrEmpty(key) ? null : key;
        }

        public override string[] GetValues(bool throwOnError)
        {
            return Value.Length == 0 ? new string[] {} : Value.Split(';');
        }
    }
}
