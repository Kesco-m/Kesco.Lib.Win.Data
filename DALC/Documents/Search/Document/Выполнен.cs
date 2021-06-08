using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
    [Option("��������", typeof (��������))]
    public class �������� : EmployeeListOption
    {
        protected ��������(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"����������"};
            emptyValueText = Resources.GetString("emptyValueText");

            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPrefix2 = Resources.GetString("htmlPrefix2");
            htmlPostfix = "";

            textItemPrefix = "[";
            textItemPostfix = "]";
        }

        public override bool OpenWindow()
        {
            return false;
        }

        public override string GetSQL(bool throwOnError)
        {
			string[] values = GetValues(throwOnError);
			if(values.Length == 0)
				return @"EXISTS (SELECT * FROM ���������.dbo.����������������� TP WITH(NOLOCK)
		WHERE TP.������������=T0.������������ AND TP.����������=1 AND TP.����������������������� IS NULL)
	OR
		(NOT EXISTS (SELECT *  FROM ���������.dbo.vw��������������� TD1  WITH(NOLOCK) WHERE TD1.������������=T0.������������)
			AND
		NOT EXISTS (SELECT * FROM ���������.dbo.vw��������������������� TI  WITH(NOLOCK) WHERE TI.������������=T0.������������
			AND  NOT EXISTS	(SELECT * FROM ���������.dbo.����������������� TP WITH(NOLOCK)
				WHERE  TP.����������=2 AND TP.����������������������� = TI.�����������������������)))
";
			else
				return GetSQLCondition(@"EXISTS (SELECT * FROM ���������.dbo.����������������� TP WITH(NOLOCK)
		WHERE TP.������������=T0.������������ AND TP.����������=1 AND TP.����������������������� IS NULL AND TP.������������� = @VAL)
	OR
		(NOT EXISTS (SELECT *  FROM ���������.dbo.vw��������������� TD1  WITH(NOLOCK) WHERE TD1.������������=T0.������������)
			AND
		NOT EXISTS (SELECT * FROM ���������.dbo.vw��������������������� TI  WITH(NOLOCK) WHERE TI.������������=T0.������������
			AND  NOT EXISTS	(SELECT * FROM ���������.dbo.����������������� TP WITH(NOLOCK)
				WHERE  TP.����������=2 AND TP.����������������������� = TI.����������������������� AND TP.������������� = @VAL)))
");

		}

        public override string GetText()
        {
            string[] vals = GetValues(false);
            if (vals.Length == 0) return htmlPrefix + emptyValueText + htmlPostfix;
            return base.GetText();
        }

        public override string GetShortText()
        {
            string[] vals = GetValues(false);
            if (vals.Length == 0) return shortTextPrefix + emptyValueText + shortTextPostfix;
            return base.GetText();
        }
    }
}