using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search._1C
{
    /// <summary>
    /// Поиск по бухгалтерским документам.
    /// </summary>
    [Option("Бухгалтерский", typeof (Бухгалтерский))]
    public class Бухгалтерский : Option
    {
        protected Бухгалтерский(XmlElement el) : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPostfix = "";
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
	            EXISTS (SELECT *
	            FROM Документы.dbo.ТипыДокументов TI
	            WHERE TI.КодТипаДокумента=T0.КодТипаДокумента AND (Бухгалтерский = 1 OR БухгалтерскийСправочник > 0))
	            ";
        }
    }
}
