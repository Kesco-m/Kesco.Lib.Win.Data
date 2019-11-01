using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.FolderRules
{
    /// <summary>
    /// Summary description for SignedBy.
    /// </summary>
    [Option("FolderRules.SignedBy", typeof (SignedBy))]
    public class SignedBy : EmployeeOption
    {
        public SignedBy(XmlElement el) : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            htmlItemPrefix = Resources.GetString("htmlItemPrefix");
            textItemPrefix = "\"";
            textItemPostfix = "\"";

            errorText = Resources.GetString("errorText");
        }

    }
}
