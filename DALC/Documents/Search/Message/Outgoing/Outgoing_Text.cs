using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Message.Outgoing
{
    /// <summary>
    /// Outgoing_Text
    /// </summary>
    [Option("Message.Outgoing.Text", typeof (Outgoing_Text))]
    public class Outgoing_Text : TextOption
    {
        protected Outgoing_Text(XmlElement el) : base(el)
        {
            htmlPostfix = "";
            htmlPrefix = Resources.GetString("htmlPrefix");
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
