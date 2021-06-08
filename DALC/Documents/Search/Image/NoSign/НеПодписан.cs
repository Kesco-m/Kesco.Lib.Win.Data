using System;
using System.Linq;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.NoSign
{
    [Option("Image.NoSing.����������", typeof (����������))]
	[SeparateOption("Image.NoSing.����������", typeof(�����������))]
	public class ���������� : EmployeeListOption
    {
        private string pattern;

        protected ����������(XmlElement el)
            : base(el)
        {
            NegativeValueOption = new[] {"Image.Sing.��������", "Image.Sing.������������"};
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
    WHERE TI.������������=T0.������������ AND "+ (IsSeparate() ? "TI.����������������������� IS NOT NULL AND ":"")+"TI.����������<>101 AND (TI.��������������� = @VAL OR TI.������������� = @VAL))"
																)
                           : @"
    NOT EXISTS (SELECT *
    FROM ���������.dbo.����������������� TI WITH(NOLOCK)
    WHERE "+ (IsSeparate() ? "TI.����������������������� IS NOT NULL AND " : "")+"TI.����������<>101 AND TI.������������=T0.������������)";
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