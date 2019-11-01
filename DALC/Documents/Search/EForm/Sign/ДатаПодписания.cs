using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm.Sign
{
    [Option("EForm.Sing.ДатаПодписания", typeof (ДатаПодписания))]
    public class ДатаПодписания : DateOption
    {
        protected ДатаПодписания(XmlElement el) : base(el)
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
EXISTS (SELECT     *
FROM	ПодписиДокументов TI WITH(NOLOCK)
WHERE TI.КодДокумента=T0.КодДокумента AND TI.КодИзображенияДокумента IS NULL AND " +
                    GetSQLCondition("TI.Дата") + ")";

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
