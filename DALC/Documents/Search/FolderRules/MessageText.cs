using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.FolderRules
{
    [Option("FolderRules.MessageText", typeof (MessageText))]
    public class MessageText : TextOption
    {
        public MessageText(XmlElement el)
            : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPostfix = "";
            htmlPrefix = Resources.GetString("htmlPrefix");

            textItemPrefix = "'";
            textItemPostfix = "'";
        }

        public override string GetSQL(bool throwOnError)
        {
            if (throwOnError && (Value.Length == 0))
                throw new Exception(Resources.GetString("GetSQL"));
            return null;
        }

        public override string GetText()
        {
            return string.Empty;
        }

        public override string GetShortText()
        {
            return string.Empty;
        }
    }
}
