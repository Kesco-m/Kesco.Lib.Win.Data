using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image
{
    /// <summary>
    /// Summary description for “ип‘иноперации.
    /// </summary>
    [Option("Image.“ип‘айла»зображени€", typeof (“ип‘айла»зображени€))]
    public class “ип‘айла»зображени€ : ValueOption
    {

        protected “ип‘айла»зображени€(XmlElement el)
            : base(el)
        {
            emptyValueText = Resources.GetString("emptyValueText");

            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPostfix = "";

            textItemPrefix = "[";
            textItemPostfix = "]";
        }

        public override string GetItemText(string key)
        {
            string str = emptyValueText;
            if (!Regex.IsMatch(key, "^\\d{0,9}$"))
                return emptyValueText;
            int val = int.Parse(key);
            switch (val)
            {
                case 2:
                    str = "PDF";
                    break;
                case 1:
                    str = "TIFF";
                    break;
            }

            return str;
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