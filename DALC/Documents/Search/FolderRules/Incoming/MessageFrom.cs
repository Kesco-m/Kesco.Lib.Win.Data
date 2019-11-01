using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.FolderRules.Incoming
{
    [Option("FolderRules.Incoming.MessageFrom", typeof (MessageFrom))]
    public class MessageFrom : EmployeeOption
    {
        public MessageFrom(XmlElement el)
            : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            htmlPrefix = Resources.GetString("htmlPrefix");
            shortTextPostfix = "";
            htmlPostfix = "";
            textItemPrefix = "[";
            textItemPostfix = "]";
        }
    }
}
