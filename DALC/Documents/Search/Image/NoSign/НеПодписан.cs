using System;
using System.Linq;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.NoSign
{
    [Option("Image.NoSing.НеПодписан", typeof (НеПодписан))]
	[SeparateOption("Image.NoSing.НеПодписан", typeof(Изображение))]
	public class НеПодписан : EmployeeListOption
    {
        private string pattern;

        protected НеПодписан(XmlElement el)
            : base(el)
        {
            NegativeValueOption = new[] {"Image.Sing.Подписан", "Image.Sing.ПодписанМной"};
            emptyValueText = Resources.GetString("emptyValueText");

            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPrefix2 = Resources.GetString("htmlPrefix2");
            htmlPostfix = "";

            textItemPrefix = "[";
            textItemPostfix = "]";
        }

        protected override string GetSQLCondition(string pattern)
        {
            string[] values = GetValues(false);
            return values.Aggregate("", (current, t) => current + ((current.Length > 0 ? (Mode == Modes.AND ? " OR " : " AND ") : "") + "(" + pattern.Replace("@VAL", GetSQLParameter(t)) + ")"));
        }

        public override string GetSQL(bool throwOnError)
        {
            try
            {
                string[] vals = GetValues(throwOnError);

                return vals.Length != 0
                           ? GetSQLCondition(
							   @"
    NOT EXISTS (SELECT *
    FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
    WHERE TI.КодДокумента=T0.КодДокумента AND "+ (IsSeparate() ? "TI.КодИзображенияДокумента IS NOT NULL AND ":"")+"TI.ТипПодписи<>101 AND (TI.КодСотрудникаЗа = @VAL OR TI.КодСотрудника = @VAL))"
																)
                           : @"
    NOT EXISTS (SELECT *
    FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
    WHERE "+ (IsSeparate() ? "TI.КодИзображенияДокумента IS NOT NULL AND " : "")+"TI.ТипПодписи<>101 AND TI.КодДокумента=T0.КодДокумента)";
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