using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.NotSend
{
    [Option("Image.ДатаНеОтправлялось", typeof (ДатаНеОтправлялось))]
    public class ДатаНеОтправлялось : DateOption
    {
        protected ДатаНеОтправлялось(XmlElement el)
            : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            htmlPrefix = Resources.GetString("htmlPrefix");
            shortTextPostfix = htmlPostfix = "";
        }

        public override string GetSQL(bool throwOnError)
        {
            if (throwOnError && (Mode == Modes.None))
                throw new Exception(Resources.GetString("GetSQL"));
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
