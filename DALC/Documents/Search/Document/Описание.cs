using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
    /// <summary>
    /// Summary description for Описание.
    /// </summary>
    [Option("Описание", typeof (Описание))]
    public class Описание : TextListOption
    {
        protected Описание(XmlElement el)
            : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPrefix2 = Resources.GetString("htmlPrefix2");

            textItemPrefix = "'";
            textItemPostfix = "'";
        }

        public override string GetSQL(bool throwOnError)
        {
            if (Value.Length == 0)
            {
                if (throwOnError)
                    throw new Exception(Resources.GetString("GetSQL"));
                return null;
            }
            return GetSQLCondition("T0.Описание LIKE '%@VAL%'");
        }
    }
}