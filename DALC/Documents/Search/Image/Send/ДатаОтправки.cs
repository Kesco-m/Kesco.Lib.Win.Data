using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.Send
{
    [Option("Image.������������", typeof (������������), typeof(�����������),2)]
    public class ������������ : DateOption
    {
        protected ������������(XmlElement el) : base(el)
        {
			emptyValueText = Resources.GetString("emptyValueText");
            //Resources.GetString("htmlPrefix");
            htmlPrefix = shortTextPrefix =shortTextPostfix = htmlPostfix = "";
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
