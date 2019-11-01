using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Transaction
{
    /// <summary>
    /// Summary description for ФиноперацияПодтверждение.
    /// </summary>

    [Option("Transaction.Подтверждение", typeof (ФиноперацияПодтверждение))]
    public class ФиноперацияПодтверждение : Option
    {
        protected ФиноперацияПодтверждение(XmlElement el) : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
                EXISTS (SELECT *
                FROM dbo.vwТранзакции T1 WITH(NOLOCK)
                WHERE (T1.КодДокументаПодтверждения = T0.КодДокумента))
                ";
        }
    }
}
