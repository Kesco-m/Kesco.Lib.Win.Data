using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm
{
    [Option("ѕечатна€‘орма", typeof (ѕечатна€‘орма))]
    public class ѕечатна€‘орма : ListOption
    {
        protected ѕечатна€‘орма(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"»зображениеќтсутствует", "Ёл‘ормаќтсутствует"};
            NegativeValueOption = new[] {"ќтсутствуетѕечатна€‘орма"};
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
                @"	EXISTS (SELECT *
                FROM ƒокументы.dbo.vw»зображени€ƒокументов TI WITH(NOLOCK)
                WHERE TI. одƒокумента=T0. одƒокумента AND  одѕечатной‘ормы ";

            if (Value.Length > 0)
                if (Mode == Modes.OR)
                    sql += "IN (SELECT  одѕечатной‘ормы FROM vwѕечатные‘ормы WITH(NOLOCK) WHERE " +
                           GetSQLCondition("RTRIM(ѕечатна€‘орма) = '@VAL'") + "))";
                else
                    sql =
                        GetSQLCondition(sql +
                                        "IN (SELECT  одѕечатной‘ормы FROM vwѕечатные‘ормы WITH(NOLOCK) WHERE RTRIM(ѕечатна€‘орма) = '@VAL'))");
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
