using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Transaction
{
    /// <summary>
    /// Summary description for ФиноперацияОснование.
    /// </summary>

    [Option("Transaction.Основание", typeof (ФиноперацияОснование))]
    public class ФиноперацияОснование : Option
    {
        protected ФиноперацияОснование(XmlElement el) : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");

        }

        public override string GetSQL(bool throwOnError)
        {
            return @"
                    EXISTS (SELECT *
                    FROM dbo.vwТранзакции T1 WITH(NOLOCK)
                    WHERE (T1.КодДокументаОснования = T0.КодДокумента))
                    ";
        }
    }
}
