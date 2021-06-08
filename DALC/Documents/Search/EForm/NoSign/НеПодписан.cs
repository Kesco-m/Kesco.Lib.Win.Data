using System;
using System.Linq;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm.NoSign
{
    [Option("EForm.NoSing.����������", typeof (����������))]
	[SeparateOption("EForm.NoSing.����������", typeof(�������))]
    public class ���������� : EmployeeListOption
    {
        protected ����������(XmlElement el)
            : base(el)
        {
            NegativeValueOption = new[] {"EForm.Sing.��������", "EForm.Sing.������������", "EForm.Sing.��������" };

            emptyValueText = Resources.GetString("emptyValueText");

            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPrefix2 = Resources.GetString("htmlPrefix2");
            htmlPostfix = "";

            textItemPrefix = "[";
            textItemPostfix = "]";
        }

        protected override string GetSQLCondition(string pattern)
        {
            string[] values = GetValues(false);
            return values.Aggregate("", (current, t) => current + ((current.Length > 0 ? (Mode == Modes.AND ? " OR " : " AND ") : "") + "(" + pattern.Replace("@VAL", GetSQLParameter(t)) + ")"));
        }

        public override string GetSQL(bool throwOnError)
        {
            try
            {
                string[] vals = GetValues(throwOnError);
                return vals.Length != 0
                           ? GetSQLCondition(
                               @"
NOT EXISTS (SELECT *
FROM ���������.dbo.����������������� TI WITH(NOLOCK)
WHERE TI.������������=T0.������������ AND TI.����������������������� IS NULL AND 
(TI.��������������� = @VAL OR TI.������������� = @VAL))"
                                 )
                           : @"
NOT EXISTS (SELECT *
FROM ���������.dbo.����������������� TI WITH(NOLOCK)
WHERE TI.������������=T0.������������)";
            }
            catch (Exception ex)
            {
                if (throwOnError) throw ex;
                return null;
            }
        }

        public override string GetText()
        {
            string[] vals = GetValues(false);
            return vals.Length == 0 ? htmlPrefix + emptyValueText + htmlPostfix : base.GetText();
        }

        public override string GetShortText()
        {
            string[] vals = GetValues(false);
            return vals.Length == 0 ? shortTextPrefix + emptyValueText + shortTextPostfix : base.GetText();
        }
    }
}