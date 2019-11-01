using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
    [Option("����", typeof (����))]
    public class ���� : DateOption
    {
        protected ����(XmlElement el) : base(el)
        {
			shortTextPrefix = htmlPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = htmlPostfix = "";
			emptyValueText =  Resources.GetString("emptyValueText");
        }

		public override bool HasEmpty()
		{
			return true;
		}

        public override string GetSQL(bool throwOnError)
        {
            string sql = "\n";
            sql += GetSQLCondition("T0.�������������");
            sql += "\n";

            return sql;
        }
    }
}