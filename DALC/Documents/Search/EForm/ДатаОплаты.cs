using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm
{
    [Option("EForm.ДатаОплаты", typeof (ДатаОплаты))]
    public class ДатаОплаты : DateOption
    {
        protected ДатаОплаты(XmlElement el)
            : base(el)
        {
            shortTextPrefix = htmlPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = htmlPostfix = "";
        }

        public override string GetSQL(bool throwOnError)
        {
            string sql;
            try
            {
                sql =
                    @"
EXISTS (SELECT * FROM vwДокументыДокументыДанные TI WITH(NOLOCK) WHERE TI.КодТипаДокумента IN (SELECT КодТипаДокумента FROM dbo.ПоляДокументов  WITH(NOLOCK)
WHERE ПолеДокумента = 'Дата оплаты' AND КолонкаТаблицы = 'Дата2') AND TI.КодДокумента=T0.КодДокумента AND " +
                    GetSQLCondition("TI.Дата2") + ")";

            }
            catch (Exception ex)
            {
                if (throwOnError) throw ex;
                return null;
            }
            return sql;
        }
    }
}
