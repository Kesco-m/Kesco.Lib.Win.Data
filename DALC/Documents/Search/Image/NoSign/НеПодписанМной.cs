using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.NoSign
{
    [Option("Image.NoSing.НеПодписанМной", typeof (НеПодписанМной))]
    public class НеПодписанМной : MyOption
    {
        protected НеПодписанМной(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"Image.Sing.ПодписанМной"};
            NegativeValueOption = new[] {"Image.Sing.Подписан"};
            shortTextPrefix = Resources.GetString("shortTextPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            return
				@"
                NOT EXISTS (SELECT *
                FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
                WHERE TI.КодДокумента=T0.КодДокумента AND TI.КодИзображенияДокумента IS NOT NULL AND TI.ТипПодписи<>101 AND
                (TI.КодСотрудникаЗа = " +
                Value + " OR TI.КодСотрудника = " + Value + "))" + Environment.NewLine;
        }
    }
}