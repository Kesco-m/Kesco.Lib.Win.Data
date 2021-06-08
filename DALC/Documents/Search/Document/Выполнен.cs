using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
    [Option("Выполнен", typeof (Выполнен))]
    public class Выполнен : EmployeeListOption
    {
        protected Выполнен(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"НеВыполнен"};
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
				return @"EXISTS (SELECT * FROM Документы.dbo.ПодписиДокументов TP WITH(NOLOCK)
		WHERE TP.КодДокумента=T0.КодДокумента AND TP.ТипПодписи=1 AND TP.КодИзображенияДокумента IS NULL)
	OR
		(NOT EXISTS (SELECT *  FROM Документы.dbo.vwДокументыДанные TD1  WITH(NOLOCK) WHERE TD1.КодДокумента=T0.КодДокумента)
			AND
		NOT EXISTS (SELECT * FROM Документы.dbo.vwИзображенияДокументов TI  WITH(NOLOCK) WHERE TI.КодДокумента=T0.КодДокумента
			AND  NOT EXISTS	(SELECT * FROM Документы.dbo.ПодписиДокументов TP WITH(NOLOCK)
				WHERE  TP.ТипПодписи=2 AND TP.КодИзображенияДокумента = TI.КодИзображенияДокумента)))
";
			else
				return GetSQLCondition(@"EXISTS (SELECT * FROM Документы.dbo.ПодписиДокументов TP WITH(NOLOCK)
		WHERE TP.КодДокумента=T0.КодДокумента AND TP.ТипПодписи=1 AND TP.КодИзображенияДокумента IS NULL AND TP.КодСотрудника = @VAL)
	OR
		(NOT EXISTS (SELECT *  FROM Документы.dbo.vwДокументыДанные TD1  WITH(NOLOCK) WHERE TD1.КодДокумента=T0.КодДокумента)
			AND
		NOT EXISTS (SELECT * FROM Документы.dbo.vwИзображенияДокументов TI  WITH(NOLOCK) WHERE TI.КодДокумента=T0.КодДокумента
			AND  NOT EXISTS	(SELECT * FROM Документы.dbo.ПодписиДокументов TP WITH(NOLOCK)
				WHERE  TP.ТипПодписи=2 AND TP.КодИзображенияДокумента = TI.КодИзображенияДокумента AND TP.КодСотрудника = @VAL)))
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