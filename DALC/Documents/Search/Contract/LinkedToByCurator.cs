using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Contract
{
    [Option("EForm.LinkedToByCurator", typeof (LinkedToByCurator))]
    public class LinkedToByCurator : EmployeeListOption
    {

        protected LinkedToByCurator(XmlElement el)
            : base(el)
        {
            Mode = Modes.OR;
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
            return
                @"EXISTS (SELECT TI.*
	FROM vw��������������� TI WITH(NOLOCK) INNER JOIN
	vw������������������������ TL WITH(NOLOCK) ON TI.��������������������� = TL.������������
		WHERE TL.���������������� IN (SELECT c.���������������� FROM dbo.�������������� c INNER JOIN
			dbo.�������������� p ON c.L >= p.L AND c.R <= p.R AND p.����������������=2039) AND TI.�����������������������=T0.������������ AND (" +
                ((Value.Length > 0) ? GetSQLCondition("TL.�������������1 = @VAL") : "TL.�������������1 IS NOT NULL") +
                "))";
        }

        public override bool OpenWindow()
        {
            return true;
        }
    }
}
