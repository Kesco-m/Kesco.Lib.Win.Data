using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search
{
    /// <summary>
    /// Summary description for UnsupportedOption.
    /// </summary>
    /// 
    [Option("UnsupportedOption", typeof (UnsupportedOption))]
    public class UnsupportedOption : Option
    {
        protected UnsupportedOption(XmlElement el)
            : base(el)
        {
        }

        public override string GetSQL(bool throwOnError)
        {
            return null;
        }
    }
}
