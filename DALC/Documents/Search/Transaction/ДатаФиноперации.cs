using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Transaction
{
    [Option("Transaction.ДатаФиноперации", typeof (ДатаФиноперации))]
    public class ДатаФиноперации : DateOption
    {
        protected ДатаФиноперации(XmlElement el) : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            htmlPrefix = Resources.GetString("htmlPrefix");
            shortTextPostfix = htmlPostfix = "";
        }

        public override string GetSQL(bool throwOnError)
        {
            string sql;
            try
            {
                sql =
                    @"
                    EXISTS (SELECT *
                    FROM dbo.vwТранзакции T1 WITH(NOLOCK)
                    WHERE T1.КодДокументаОснования = T0.КодДокумента AND " +
                    GetSQLCondition("T1.Дата") + ")" +
                    @" OR EXISTS (SELECT *
                    FROM dbo.vwТранзакции T1 WITH(NOLOCK)
                    WHERE T1.КодДокументаПодтверждения = T0.КодДокумента AND " +
                    GetSQLCondition("T1.Дата") + ")";

            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw ex;
                return null;
            }
            return sql;
        }
    }
}
