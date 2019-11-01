using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Transaction
{
    /// <summary>
    /// Summary description for ФиноперацииОтсутствуют.
    /// </summary>
    [Option("Transaction.Hasnot", typeof (ФиноперацииОтсутствуют))]
    public class ФиноперацииОтсутствуют : Option
    {
        protected ФиноперацииОтсутствуют(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"Transaction.Has"};

            shortTextPrefix = "";
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
                NOT EXISTS (SELECT *
                FROM dbo.vwТранзакции T1 WITH(NOLOCK)
                WHERE T1.КодДокументаОснования = T0.КодДокумента)
                AND NOT EXISTS (SELECT *
                FROM dbo.vwТранзакции T1 WITH(NOLOCK)
                WHERE T1.КодДокументаПодтверждения = T0.КодДокумента)
                ";
        }
    }
}