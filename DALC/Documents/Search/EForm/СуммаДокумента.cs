using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.New_Search.Base;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm
{
	[NumericType("��������������Type", typeof(decimal))]
    [Option("��������������", typeof (��������������))]
    public class �������������� : MinMaxOption
    {
        protected ��������������(XmlElement el)
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
                    FROM ���������.dbo.vw��������������� TI WITH(NOLOCK)
                    WHERE TI.������������=T0.������������ AND " +
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
