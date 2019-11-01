using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm
{
    [Option("������������������������", typeof (������������������������))]
    public class ������������������������ : ListOption
    {
        protected ������������������������(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"Image.�����������"};
            NegativeValueOption = new[] {"�������������"};
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
                    FROM ���������.dbo.vw��������������������� TI WITH(NOLOCK)
                    WHERE TI.������������=T0.������������ AND ���������������� ";

            if (Value.Length > 0)
                if (Mode == Modes.AND)
                    sql += "IN (SELECT ���������������� FROM vw������������� WITH(NOLOCK) WHERE " +
                           GetSQLCondition("RTRIM(�������������) = '@VAL'") + "))";
                else
                    sql =
                        GetSQLCondition(sql +
                                        "IN (SELECT ���������������� FROM vw������������� WITH(NOLOCK) WHERE RTRIM(�������������) = '@VAL'))")
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
