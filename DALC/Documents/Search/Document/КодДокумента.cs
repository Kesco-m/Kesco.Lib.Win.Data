using System;
using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
    [Option("КодДокумента", typeof (КодДокумента))]
    public class КодДокумента : ListOption
    {
        protected КодДокумента(XmlElement el)
            : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPrefix2 = Resources.GetString("htmlPrefix2");
            htmlPostfix = "";

            textItemPrefix = "[";
            textItemPostfix = "]";
        }

        public override string GetSQL(bool throwOnError)
        {
            if (!Regex.IsMatch(Value, "^[1-9][0-9]{0,9}(,[1-9][0-9]{0,9})*$"))
            {
                if (throwOnError)
                    throw new Exception("Невозможно произвести поиск по коду документа, т.к. код документа не указан. ");
                return null;
            }
            return GetSQLCondition("T0.КодДокумента = @VAL");
        }
    }
}
