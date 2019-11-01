using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Message.Outgoing
{
    [Option("Message.Outgoing.By", typeof (Outgoing_By))]
    public class Outgoing_By : EmployeeListOption
    {
        protected Outgoing_By(XmlElement el) : base(el)
        {
            emptyValueText = Resources.GetString("emptyValueText");

            htmlPostfix = "";
            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPrefix2 = Resources.GetString("htmlPrefix2");

            textItemPrefix = "[";
            textItemPostfix = "]";

            Mode = Modes.OR;
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
