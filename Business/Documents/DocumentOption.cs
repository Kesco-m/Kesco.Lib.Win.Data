using System.Xml;
using Kesco.Lib.Win.Data.Options;
using Kesco.Lib.Win.Data.Types;

namespace Kesco.Lib.Win.Data.Business.Documents
{
    public abstract class DocumentOption : Option
    {
        private Document document;

        #region ACCESSORS

        public Document @Document
        {
            get { return document; }
            set
            {
                if (document == value) return;
                document = value;
            }
        }

        public string DocumentName
        {
            get
            {
                if (@Document == null) return "";
                if (@Document.IsUnavailable) return "#" + @Document.ID;
                return @Document.FullName;
            }
        }

        #endregion

        public DocumentOption(string name) : base(name)
        {
        }

        #region XML

        public override void SaveToXmlElement(XmlElement el)
        {
            base.SaveToXmlElement(el);
            if (@Document == null)
            {
                el.SetAttribute("DocumentID", "");
                el.SetAttribute("DocumentName", "");
            }
            else
            {
                el.SetAttribute("DocumentID", @Document.ID.ToString());
                el.SetAttribute("DocumentName", DocumentName);
            }
        }

        public override void LoadFromXmlElement(XmlElement el)
        {
            base.LoadFromXmlElement(el);
            int id = KInt.FromXmlString(el.GetAttribute("DocumentID"));
            document = id == int.MinValue ? null : new Document(id);
        }

        #endregion
    }
}