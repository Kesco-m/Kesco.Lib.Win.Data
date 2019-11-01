using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.FolderRules
{
    [Option("FolderRules.DocumentType", typeof (DocumentType))]
    public class DocumentType : ValueOption
    {
        public DocumentType(XmlElement el)
            : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            htmlItemPrefix = Resources.GetString("htmlItemPrefix");

            textItemPrefix = "'";
            textItemPostfix = "'";

            errorText = Resources.GetString("errorText");
        }
    }
}
