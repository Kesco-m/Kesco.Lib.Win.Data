using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.Sign
{
    /// <summary>
    /// Summary description for ПордписанМной.
    /// </summary>
    /// 
    [Option("Image.Sing.ПодписанМной", typeof (ПодписанМной))]
    public class ПодписанМной : MyOption
    {
        protected ПодписанМной(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"Image.NoSing.НеПодписанМной"};
            NegativeValueOption = new[] {"Image.NoSing.НеПодписан"};
            shortTextPrefix = Resources.GetString("shortTextPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            return
				@"EXISTS (SELECT *
                    FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
                    WHERE TI.КодДокумента=T0.КодДокумента AND TI.ТипПодписи<>101 AND TI.КодИзображенияДокумента IS NOT NULL AND
                    (TI.КодСотрудникаЗа = " +
                Value + " OR TI.КодСотрудника = " + Value + "))" + Environment.NewLine;
        }
    }
}