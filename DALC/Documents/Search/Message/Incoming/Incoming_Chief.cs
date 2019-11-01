using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Message.Incoming
{
    /// <summary>
    /// Поиск по отправленным руководителями сообщениям.
    /// </summary>
    [Option("Message.Incoming.Chief", typeof (Incoming_Chief))]
    public class Incoming_Chief : MyOption
    {
        protected Incoming_Chief(XmlElement el) : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";
            htmlPostfix = "";
            htmlPrefix = Resources.GetString("htmlPrefix");

            textItemPrefix = "[";
            textItemPostfix = "]";
        }

        public override bool OpenWindow()
        {
            return false;
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
