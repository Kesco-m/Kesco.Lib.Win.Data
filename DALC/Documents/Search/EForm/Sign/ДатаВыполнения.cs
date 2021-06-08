using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm.Sign
{
    [Option("EForm.Sing.ДатаВыполнения", typeof (ДатаВыполнения))]
	[SeparateOption("EForm.Sing.ДатаВыполнения", typeof(ЭлФорма))]
	public class ДатаВыполнения : DateOption
    {
        protected ДатаВыполнения(XmlElement el) : base(el)
        {
            shortTextPrefix = htmlPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = htmlPostfix = "";
        }

        public override string GetSQL(bool throwOnError)
        {
            if (throwOnError && (Mode == Modes.None))
                throw new Exception(Resources.GetString("GetSQL"));
            return null;
        }
    }
}
