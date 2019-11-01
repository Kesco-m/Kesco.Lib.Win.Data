using System;
using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
    [Option("������������", typeof (������������))]
    public class ������������ : ListOption
    {
        protected ������������(XmlElement el)
            : base(el)
        {
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
            if (!Regex.IsMatch(Value, "^[1-9][0-9]{0,9}(,[1-9][0-9]{0,9})*$"))
            {
                if (throwOnError)
                    throw new Exception("���������� ���������� ����� �� ���� ���������, �.�. ��� ��������� �� ������. ");
                return null;
            }
            return GetSQLCondition("T0.������������ = @VAL");
        }
    }
}
