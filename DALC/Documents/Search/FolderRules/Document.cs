using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.FolderRules
{
    [Option("FolderRules.Document", typeof (Document))]
    public class Document : ValueOption
    {
        public Document(XmlElement el)
            : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            htmlItemPrefix = Resources.GetString("htmlItemPrefix");
            textItemPrefix = "'";
            textItemPostfix = "'";
            errorText = Resources.GetString("GetSQL");
        }
    }
}
