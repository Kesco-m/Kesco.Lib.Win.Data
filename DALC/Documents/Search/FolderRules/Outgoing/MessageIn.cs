using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.FolderRules.Outgoing
{
    [Option("FolderRules.Outgoing.MessageIn", typeof (MessageIn))]
    public class MessageIn : EmployeeOption
    {
        public MessageIn(XmlElement el) : base(el)
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
