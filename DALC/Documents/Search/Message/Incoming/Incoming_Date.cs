using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Message.Incoming
{
    [Option("Message.Incoming.Date", typeof (Incoming_Date))]
    public class Incoming_Date : DateOption
    {
        protected Incoming_Date(XmlElement el) : base(el)
        {
            htmlPrefix = Resources.GetString("htmlPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            if (throwOnError && (Mode == Modes.None)) throw new Exception(Resources.GetString("GetSQL"));
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
