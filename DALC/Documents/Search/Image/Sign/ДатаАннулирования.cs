using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.Sign
{
    /// <summary>
    /// Summary description for ДатаАннулирования.
    /// </summary>
    [Option("Image.Sing.ДатаАннулирования", typeof (ДатаАннулирования))]
	[SeparateOption("Image.Sing.ДатаАннулирования", typeof(Изображение))]
	public class ДатаАннулирования : DateOption
    {
        protected ДатаАннулирования(XmlElement el)
            : base(el)
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
