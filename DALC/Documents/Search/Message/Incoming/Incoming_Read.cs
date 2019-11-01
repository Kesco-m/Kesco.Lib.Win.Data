using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Message.Incoming
{
    [Option("Message.Incoming.Read", typeof (Incoming_Read))]
    public class Incoming_Read : Option
    {
        protected Incoming_Read(XmlElement el) : base(el)
        {
			NegativeOption = new[] { "Message.Incoming.Unread" };
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
