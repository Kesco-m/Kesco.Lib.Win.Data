using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.New_Search.Base;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm
{
	[NumericType("СуммаДокументаType", typeof(decimal))]
    [Option("СуммаДокумента", typeof (СуммаДокумента))]
    public class СуммаДокумента : MinMaxOption
    {
        protected СуммаДокумента(XmlElement el)
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
                    EXISTS (SELECT *
                    FROM Документы.dbo.vwДокументыДанные TI WITH(NOLOCK)
                    WHERE TI.КодДокумента=T0.КодДокумента AND " +
                    GetSQLCondition("TI.Money1") + ")";
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
