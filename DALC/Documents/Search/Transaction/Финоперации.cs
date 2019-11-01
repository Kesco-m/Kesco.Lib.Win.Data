using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Transaction
{
    /// <summary>
    /// Summary description for Transacton_Has.
    /// </summary>
    [Option("Transaction.Has", typeof (Финоперации))]
    public class Финоперации : Option
    {
        protected Финоперации(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"Transaction.Hasnot"};
            shortTextPrefix = Resources.GetString("shortTextPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
                EXISTS (SELECT *
                FROM dbo.vwТранзакции T1 WITH(NOLOCK)
                WHERE T1.КодДокументаОснования = T0.КодДокумента)
                OR EXISTS (SELECT *
                FROM dbo.vwТранзакции T1 WITH(NOLOCK)
                WHERE T1.КодДокументаПодтверждения = T0.КодДокумента)
                ";
        }
    }
}