using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm.Sign
{
    [Option("EForm.Sing.ПодписанМной", typeof (ПодписанМной))]
    public class ПодписанМной : MyOption
    {
        protected ПодписанМной(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"EForm.NoSing.НеПодписанМной"};
            NegativeValueOption = new[] {"EForm.NoSing.НеПодписан"};

            shortTextPrefix = Resources.GetString("shortTextPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
                EXISTS (SELECT *
                FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
                WHERE TI.КодДокумента=T0.КодДокумента AND TI.КодИзображенияДокумента IS NULL AND 
                (TI.КодСотрудникаЗа = " +
                Value + " OR TI.КодСотрудника = " + Value + "))" + Environment.NewLine;
        }
    }
}