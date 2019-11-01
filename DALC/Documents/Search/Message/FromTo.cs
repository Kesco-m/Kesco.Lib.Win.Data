using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Message
{
    /// <summary>
    /// ����� �� �����������, ����������� � ���������� ����������.
    /// </summary>
    [Option("Message.FromTo", typeof (FromTo))]
    public class FromTo : EmployeeTwoValueListOption
    {
        protected FromTo(XmlElement el) : base(el)
        {
            emptyValueText = Resources.GetString("emptyValueText");
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";
            htmlPostfix = "";
            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPrefix2 = Resources.GetString("htmlPrefix2");
            htmlSplitter = Resources.GetString("htmlSplitter");
            htmlSplitter2 = Resources.GetString("htmlSplitter2");
            emptyValueText2 = Resources.GetString("emptyValueText2");
            shortTextSplitter = Resources.GetString("shortTextSplitter");

            textItemPrefix = "[";
            textItemPostfix = "]";
            Mode = Modes.OR;
        }

        public override string GetSQL(bool throwOnError)
        {
            try
            {
                string s = GetSQLCondition("TV.������������������������=@VAL");
                if (s.Length > 0) s = " AND (" + s + ")";
                string s2 = GetSQLConditionFor2("TV.�����������������������=@VAL");
                if (s2.Length > 0) s += " AND (" + s2 + ")";
                return @"
                        EXISTS (SELECT *
                        FROM vw��������� TV WITH(NOLOCK)
                        WHERE TV.������������=T0.������������" +
                                               s + @")
                        ";

            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw ex;
                return null;
            }
        }
    }
}
