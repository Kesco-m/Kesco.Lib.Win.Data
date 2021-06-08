using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm.NoSign
{
	[Option("EForm.NoSing.НеПодписанМной", typeof(НеПодписанМной))]
	[SeparateOption("EForm.NoSing.НеПодписанМной", typeof(ЭлФорма))]
	public class НеПодписанМной : MyOption
	{
		protected НеПодписанМной(XmlElement el) : base(el)
		{
			NegativeOption = new[] { "EForm.Sing.ПодписанМной" };
			NegativeValueOption = new[] { "EForm.Sing.Подписан", "EForm.Sing.Выполнен"};
			shortTextPrefix = Resources.GetString("shortTextPrefix");
		}

		public override string GetSQL(bool throwOnError)
		{
			return
				@"
                    NOT EXISTS (SELECT *
                    FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
                    WHERE TI.КодДокумента=T0.КодДокумента AND TI.КодИзображенияДокумента IS NULL AND 
                    (TI.КодСотрудникаЗа = " +
				Value + " OR TI.КодСотрудника = " + Value + "))" + Environment.NewLine;
		}
	}
}