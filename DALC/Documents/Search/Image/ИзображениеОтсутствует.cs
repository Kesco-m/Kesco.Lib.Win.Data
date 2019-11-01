using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image
{
    [Option("ИзображениеОтсутствует", typeof (ИзображениеОтсутствует))]
    public class ИзображениеОтсутствует : Option
    {
        protected ИзображениеОтсутствует(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"Image.Изображение", "ПечатнаяФорма"};
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPostfix = "";
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
                NOT EXISTS (SELECT *
                FROM Документы.dbo.vwИзображенияДокументов TI WITH(NOLOCK)
                WHERE TI.КодДокумента=T0.КодДокумента)
                ";
        }
    }
}