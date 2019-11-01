using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Message.Incoming
{
    [Option("Message.Incoming.Unread", typeof (Incoming_Unread))]
    public class Incoming_Unread : Option
    {
        protected Incoming_Unread(XmlElement el) : base(el)
        {
			NegativeOption = new[] { "Message.Incoming.Read" };
        }

        public override string GetSQL(bool throwOnError)
        {
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
