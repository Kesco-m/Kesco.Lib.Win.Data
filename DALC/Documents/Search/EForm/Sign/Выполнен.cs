using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm.Sign
{
    [Option("EForm.Sing.Выполнен", typeof (Выполнен))]
    public class Выполнен : EmployeeListOption
    {
        protected Выполнен(XmlElement el)
            : base(el)
        {
            NegativeValueOption = new[] {"EForm.NoSing.НеВыполнен"};
            emptyValueText = Resources.GetString("emptyValueText");

            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPrefix2 = Resources.GetString("htmlPrefix2");
            htmlPostfix = "";

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
            string[] vals = GetValues(false);
            if (vals.Length == 0) return htmlPrefix + emptyValueText + htmlPostfix;
            return base.GetText();
        }

        public override string GetShortText()
        {
            string[] vals = GetValues(false);
            if (vals.Length == 0) return shortTextPrefix + emptyValueText + shortTextPostfix;
            return base.GetText();
        }
    }
}