using System.Xml;
using Kesco.Lib.Win.Data.Business;
using Kesco.Lib.Win.Data.Business.Documents;
using Kesco.Lib.Win.Data.DALC.Documents.Search.FolderRules.Incoming;
using Kesco.Lib.Win.Data.DALC.Documents.Search.FolderRules.Outgoing;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.FolderRules
{
    public enum FolderRuleMode
    {
        Undefined = 0,
        MessageSent = 1,
        MessageReceived = 2
    }

    public class FolderRuleOptions
    {
        private string name;
        private FolderRuleMode mode;
        private Folder folder;

        public static int GetOptions(FolderRuleMode mode, ref OptionAttribute[] metas)
        {
            int i = 0;
            switch (mode)
            {
                case FolderRuleMode.MessageSent:
                    metas[i] = Option.GetMeta(typeof (MessageIn));
                    i++;
                    metas[i] = Option.GetMeta(typeof (MessageText));
                    i++;
                    metas[i] = Option.GetMeta(typeof (SignedBy));
                    i++;
                    metas[i] = Option.GetMeta(typeof (DocumentType));
                    i++;
                    metas[i] = Option.GetMeta(typeof (Person));
                    i++;
                    metas[i] = Option.GetMeta(typeof (Document));
                    i++;
                    break;
                case FolderRuleMode.MessageReceived:
                    metas[i] = Option.GetMeta(typeof (MessageFrom));
                    i++;
                    metas[i] = Option.GetMeta(typeof (MessageText));
                    i++;
                    metas[i] = Option.GetMeta(typeof (SignedBy));
                    i++;
                    metas[i] = Option.GetMeta(typeof (DocumentType));
                    i++;
                    metas[i] = Option.GetMeta(typeof (Person));
                    i++;
                    metas[i] = Option.GetMeta(typeof (Document));
                    i++;
                    break;
            }
            return i;
        }

        #region ACCESSORS

        public event ChangedDelegate ModeChanged;
        public event ChangedDelegate OptionEnabledDisabled;

        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    value = "";
                name = value.Trim();
            }
        }

        public FolderRuleMode Mode
        {
            get { return mode; }
            set
            {
                if (mode == value)
                    return;
                mode = value;
                OnModeChanged();
            }
        }

        public bool IsEmpty
        {
            get { return true; }
        }

        /// <summary>
        /// папка документов, к которой применяется данное правило
        /// </summary>
        public Folder @Folder
        {
            get { return folder; }
            set
            {
                if (folder == value) return;
                folder = value;
            }
        }

        #endregion

        public void OnModeChanged()
        {
            switch (mode)
            {
                case FolderRuleMode.MessageSent:
                    break;
                case FolderRuleMode.MessageReceived:

                    break;
            }
            if (ModeChanged != null)
                ModeChanged();
        }

        public static void FlatternDocument(XmlDocument doc)
        {
            if (doc.DocumentElement == null) return;
            XmlElement elIncoming = doc.CreateElement("Option");
            elIncoming.SetAttribute("name", "Incoming");
            XmlElement elOutgoing = doc.CreateElement("Option");
            elOutgoing.SetAttribute("name", "Outgoing");

            XmlNodeList list;
            list = doc.SelectNodes("Options/Option[@name='Incoming']");
            foreach (XmlElement elOption in list) doc.DocumentElement.RemoveChild(elOption);
            list = doc.SelectNodes("Options/Option[@name='Outgoing']");
            foreach (XmlElement elOption in list) doc.DocumentElement.RemoveChild(elOption);

            list = doc.SelectNodes("Options/Option");
            foreach (XmlElement elOption in list)
            {
                switch (elOption.GetAttribute("name"))
                {
                    case "Incoming.MessageFrom":
                        elIncoming.SetAttribute("enabled", "true");
                        break;

                    case "Outgoing.MessageIn":
                        elOutgoing.SetAttribute("enabled", "true");
                        break;
                }
            }

            if (elOutgoing.Attributes.Count > 1)
                doc.DocumentElement.AppendChild(elOutgoing);
            if (elIncoming.Attributes.Count > 1)
                doc.DocumentElement.AppendChild(elIncoming);
        }

        private static XmlElement GetElOption(XmlDocument xml, string name)
        {
            XmlElement elOption;
            elOption = (XmlElement) xml.SelectSingleNode("Options/Option[@name=\"" + name + "\"]");
            if (elOption == null)
            {
                elOption = xml.CreateElement("Option");
                elOption.SetAttribute("name", name);
                xml.DocumentElement.AppendChild(elOption);
            }

            return elOption;
        }
    }
}
