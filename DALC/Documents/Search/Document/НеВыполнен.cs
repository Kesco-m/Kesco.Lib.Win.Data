using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
	[Option("НеВыполнен", typeof(НеВыполнен))]
	public class НеВыполнен : Option
	{
		protected НеВыполнен(XmlElement el)
			: base(el)
		{
			NegativeOption = new[] { "Выполнен"};
			shortTextPrefix = Resources.GetString("shortTextPrefix");
		}

		public override string GetSQL(bool throwOnError)
		{
			return
				@"(EXISTS(SELECT * FROM Документы.dbo.vwДокументыДанные TD  WITH(NOLOCK) WHERE TD.КодДокумента=T0.КодДокумента)
				AND NOT EXISTS (
					SELECT * FROM Документы.dbo.ПодписиДокументов TP WITH(NOLOCK)
					WHERE TP.КодДокумента=T0.КодДокумента AND TP.ТипПодписи=1 AND TP.КодИзображенияДокумента IS NULL
				))
			OR (NOT EXISTS (SELECT * FROM Документы.dbo.vwДокументыДанные TD1  WITH(NOLOCK) WHERE TD1.КодДокумента=T0.КодДокумента)
				AND EXISTS (SELECT * FROM Документы.dbo.vwИзображенияДокументов TI  WITH(NOLOCK) WHERE TI.КодДокумента=T0.КодДокумента
					AND NOT EXISTS (
						SELECT * FROM Документы.dbo.ПодписиДокументов TP WITH(NOLOCK)
						WHERE  TP.ТипПодписи=2 AND TP.КодИзображенияДокумента = TI.КодИзображенияДокумента)))
";
		}
	}
}