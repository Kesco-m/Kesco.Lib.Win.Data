using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm.Sign
{
    [Option("EForm.Sing.Подписан", typeof (Подписан))]
    public class Подписан : EmployeeListOption
    {
        protected Подписан(XmlElement el)
            : base(el)
        {
            NegativeValueOption = new[] {"EForm.NoSing.НеПодписан", "EForm.NoSing.НеПодписанМной"};

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
            try
            {
                string[] vals = GetValues(throwOnError);
                return vals.Length != 0
                           ? GetSQLCondition(
                               @"
EXISTS (SELECT *
FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
WHERE TI.КодДокумента=T0.КодДокумента AND TI.КодИзображенияДокумента IS NULL AND 
(TI.КодСотрудникаЗа = @VAL OR TI.КодСотрудника = @VAL))"
                                 )
                           : @"
EXISTS (SELECT *
FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
WHERE TI.КодДокумента=T0.КодДокумента AND TI.КодИзображенияДокумента IS NULL)";
            }
            catch (Exception ex)
            {
                if (throwOnError) throw ex;
                return null;
            }
        }

        public override string GetText()
        {
            string[] vals = GetValues(false);
            return vals.Length == 0 ? htmlPrefix + emptyValueText + htmlPostfix : base.GetText();
        }

        public override string GetShortText()
        {
            string[] vals = GetValues(false);
            return vals.Length == 0 ? shortTextPrefix + emptyValueText + shortTextPostfix : base.GetText();
        }
    }
}