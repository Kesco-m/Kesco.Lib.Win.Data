using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Message.Incoming
{
    /// <summary>
    /// Поиск по сообщениям, отправленным поченеными.
    /// </summary>
    [Option("Message.Incoming.Employee", typeof (Incoming_Employee))]
    public class Incoming_Employee : MyOption
    {
        protected Incoming_Employee(XmlElement el) : base(el)
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
