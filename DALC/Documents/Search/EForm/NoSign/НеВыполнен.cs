using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm.NoSign
{
    [Option("EForm.NoSing.НеВыполнен", typeof (НеВыполнен))]
    public class НеВыполнен : Option
    {
        protected НеВыполнен(XmlElement el)
            : base(el)
        {
            NegativeValueOption = new[] {"EForm.Sing.Выполнен"};
            shortTextPrefix = Resources.GetString("shortTextPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
                NOT EXISTS (SELECT *
                FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
                WHERE TI.КодДокумента=T0.КодДокумента
                AND TI.ТипПодписи=1 AND TI.КодИзображенияДокумента IS NULL)
                ";
        }
    }
}