using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image
{
    [Option("Image.Изображение", typeof (Изображение))]
    public class Изображение : Option
    {
        protected Изображение(XmlElement el)  : base(el)
        {
            NegativeOption = new[] {"ИзображениеОтсутствует", "ОтсутствуетПечатнаяФорма"};
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPostfix = "";
        }


        public override string GetSQL(bool throwOnError)
        {
            return null;
        }
    }
}