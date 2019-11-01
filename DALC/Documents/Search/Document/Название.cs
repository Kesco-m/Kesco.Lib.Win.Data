using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
    /// <summary>
    /// Summary description for ��������.
    /// </summary>
    [Option("��������", typeof (��������))]
    public class �������� : TextOption
    {
        protected ��������(XmlElement el)
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
                return " T0.�����������������RL <> '' ";
            return GetSQLCondition("T0.�����������������RL", true) +
                   " OR (T0.���������������� IN (SELECT ���������������� FROM �������������� TD (NOLOCK) WHERE " +
                   GetSQLCondition("TD.������������") + " OR " + GetSQLCondition("TD.TypeDoc") +
                   ") AND T0.�����������������RL = '')";
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