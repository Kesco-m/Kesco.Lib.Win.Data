using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.NoSign
{
	[Option("Image.NoSing.НеАннулирован", typeof(НеАннулирован))]
	public class НеАннулирован : Option
	{

		protected НеАннулирован(XmlElement el) : base(el)
		{
			NegativeOption = new[ ] { "Image.Sing.Аннулировано" };
			shortTextPrefix = Resources.GetString("shortTextPrefix");
		}

		public override string GetSQL(bool throwOnError)
		{
			return @"NOT EXISTS (SELECT *
                FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
                WHERE TI.КодДокумента=T0.КодДокумента
                AND TI.ТипПодписи=2)
                ";
		}
	}
}