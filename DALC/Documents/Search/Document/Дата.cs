using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
    [Option("Дата", typeof (Дата))]
    public class Дата : DateOption
    {
        protected Дата(XmlElement el) : base(el)
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
            sql += GetSQLCondition("T0.ДатаДокумента");
            sql += "\n";

            return sql;
        }
    }
}