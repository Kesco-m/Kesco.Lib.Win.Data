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
	FROM vwСвязиДокументов TI WITH(NOLOCK) INNER JOIN
	vwДокументыДокументыДанные TL WITH(NOLOCK) ON TI.КодДокументаОснования = TL.КодДокумента
		WHERE TL.КодТипаДокумента IN (SELECT c.КодТипаДокумента FROM dbo.ТипыДокументов c INNER JOIN
			dbo.ТипыДокументов p ON c.L >= p.L AND c.R <= p.R AND p.КодТипаДокумента=2039) AND TI.КодДокументаВытекающего=T0.КодДокумента AND (" +
                ((Value.Length > 0) ? GetSQLCondition("TL.КодСотрудника1 = @VAL") : "TL.КодСотрудника1 IS NOT NULL") +
                "))";
        }

        public override bool OpenWindow()
        {
            return true;
        }
    }
}
