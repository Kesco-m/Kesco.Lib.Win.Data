using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;
using Kesco.Lib.Win.Data.Business.Corporate;
using Kesco.Lib.Win.Data.Business.Documents.FolderRuleOptions;
using Kesco.Lib.Win.Data.Options;
using Kesco.Lib.Win.Data.Types;

namespace Kesco.Lib.Win.Data.Business.Documents
{
    public enum FolderRuleMode
    {
        Undefined = 0,
        MessageSent = 1,
        MessageReceived = 2
    }

    public class FolderRule : BaseEE
    {
        private string name;

        private FolderRuleMode mode;
        private Folder folder;

        private MessageFrom optMessageFrom;
        private MessageTo optMessageTo;
        private MessageText optMessageText;
        private SignedBy optSignedBy;
        private FolderRuleOptions.DocumentType optDocumentType;
        private Person optPerson;
        private FolderRuleOptions.Document optDocument;
        private NoPerson optNoPerson;
        private NoDocument optNoDocument;
        private NotMessageText optNoMessageText;
        private NotSignetBy optNotSignedBy;


        private Option[] allOptions;

        public const int maxGroupCount = 10;
        public const int maxOptionsInGroupCount = 20;

        public int GetFolderRuleOptions(Option[] options)
        {
            int i = 0;
            switch (mode)
            {
                case FolderRuleMode.MessageSent:
                    options[i] = optMessageTo;
                    i++;
                    options[i] = optMessageText;
                    i++;
                    options[i] = optSignedBy;
                    i++;
                    options[i] = optDocumentType;
                    i++;
                    options[i] = optPerson;
                    i++;
                    options[i] = optDocument;
                    i++;
                    options[i] = optNotSignedBy;
                    i++;
                    options[i] = optNoPerson;
                    i++;
                    options[i] = optNoDocument;
                    i++;
                    break;
                case FolderRuleMode.MessageReceived:
                    options[i] = optMessageFrom;
                    i++;
                    options[i] = optMessageText;
                    i++;
                    options[i] = optSignedBy;
                    i++;
                    options[i] = optDocumentType;
                    i++;
                    options[i] = optPerson;
                    i++;
                    options[i] = optDocument;
                    i++;
                    options[i] = optNotSignedBy;
                    i++;
                    options[i] = optNoPerson;
                    i++;
                    options[i] = optNoDocument;
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
            get
            {
                LoadIfDelayed();
                return name;
            }
            set { name = value == null ? string.Empty : value.Trim(); }
        }

        public FolderRuleMode Mode
        {
            get
            {
                LoadIfDelayed();
                return mode;
            }
            set
            {
                if (mode == value) return;
                mode = value;
                OnModeChanged();
            }
        }

        public bool IsEmpty
        {
            get { return allOptions.All(t => !t.Enabled || !t.Validate(false)); }
        }

        /// <summary>
        /// папка документов, к которой применяется данное правило
        /// </summary>
        public Folder @Folder
        {
            get
            {
                LoadIfDelayed();
                return folder;
            }
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
                    optMessageFrom.Enabled = false;
                    break;
                case FolderRuleMode.MessageReceived:
                    optMessageTo.Enabled = false;
                    break;
                default:
                    optMessageFrom.Enabled = false;
                    optMessageTo.Enabled = false;
                    break;
            }
            if (ModeChanged != null) ModeChanged();
        }

        public Option GetOption(string name)
        {
            return allOptions.FirstOrDefault(t => t.Name.Equals(name));
        }

        public string GetHtml()
        {
            string s = allOptions.Where(t => t.Enabled).Aggregate("",
                                                                  (current, t) =>
                                                                  current +
                                                                  ((current.Length == 0 ? "" : StringResources.and + " ") +
                                                                   t.GetHtml() + "<br>"));
            if (s.Length > 0)
                s = StringResources.FR1 + " \"" + (Folder != null ? Folder.Name : "unknown") + "\" " +
                    StringResources.at +
                    ((mode == FolderRuleMode.MessageSent) ? StringResources.send : StringResources.recieve) + " " +
                    StringResources.FR2 + ": <br>" + s;
            return s;
        }

        public string GetShortText()
        {
            return allOptions.Where(t => t.Enabled && t.Validate(false)).Aggregate("",
                                                                                   (current, t) =>
                                                                                   current +
                                                                                   ((current.Length == 0 ? "" : " ") +
                                                                                    t.GetShortText()));
        }

        public FolderRule(int id)
            : base(id)
        {

        }

        protected override void Init()
        {
            base.Init();
            connectionString = Settings.DS_document;
            mode = FolderRuleMode.Undefined;

            optMessageFrom = new MessageFrom("MessageSender");
            optMessageTo = new MessageTo("MessageReceiver");
            optMessageText = new MessageText("MessageText");
            optSignedBy = new SignedBy("DocumentSigner");
            optDocumentType = new FolderRuleOptions.DocumentType("DocumentType");
            optPerson = new Person("LinkedPerson");
            optDocument = new FolderRuleOptions.Document("LinkedDocuments");
            optNoMessageText = new NotMessageText("NotMessageText");
            optNotSignedBy = new NotSignetBy("NoDocumentSigner");
            optNoPerson = new NoPerson("NoLinkedPerson");
            optNoDocument = new NoDocument("NoLinkedDocuments");

            allOptions = new Option[]
                             {
                                 optMessageFrom,
                                 optMessageTo,
                                 optMessageText,
                                 optSignedBy,
                                 optDocumentType,
                                 optPerson,
                                 optDocument,
                                 optNoDocument,
                                 optNoMessageText,
                                 optNoPerson,
                                 optNotSignedBy
                             };
        }

        #region XML

        public override void LoadFromXmlElement(XmlElement el)
        {
            foreach (Option t in allOptions)
            {
                XmlElement elChild = (XmlElement) el.SelectSingleNode("Option[@Name=\"" + t.Name + "\"]");
                if (elChild != null) t.LoadFromXmlElement(elChild);
            }
        }

        public override void SaveToXmlElement(XmlElement el)
        {
            base.SaveToXmlElement(el);

            foreach (Option t in allOptions.Where(t => el.OwnerDocument != null))
            {
                XmlElement elChild;
                el.AppendChild(elChild = el.OwnerDocument.CreateElement("Option"));
                t.SaveToXmlElement(elChild);
            }
        }

        #endregion

        #region DB

        protected override string FillFrom_Table
        {
            get { return "Документы.dbo.vwПравилаПапкиДокументов"; }
        }

        protected override string ID_Field
        {
            get { return "КодПравилаПапкиДокументов"; }
        }

        private const string nameField = "Описание";
        private const string folderIDField = "КодПапкиДокументов";
        private const string modeField = "Режим";

        private const string messageFromField = "КодСотрудникаОтправителя";
        private const string messageToField = "КодСотрудникаПолучателя";
        private const string messageTextField = "Сообщение";
        private const string signedByField = "КодСотрудникаПодписавшего";
        private const string documentTypeField = "КодТипаДокумента";
        private const string personField = "КодЛица";
        private const string documentField = "КодДокумента";
        private const string modeDocTypeField = "РежимПоискаТипов";
        private const string noDocumentField = "КодДокументаНеСвязаного";
        private const string noPersonField = "КодОтсутствующегоЛица";
        private const string notSignedByField = "КодСотрудникаНеПодписавшего";

        protected override void Fill(DataRow row)
        {
            base.Fill(row);
            name = row.IsNull(nameField) ? "" : (string) row[nameField];
            folder = row.IsNull(folderIDField) ? null : new Folder((int) row[folderIDField]);
            mode = row.IsNull(modeField) ? FolderRuleMode.Undefined : mode = (FolderRuleMode) (byte) row[modeField];

            optMessageFrom.Employee = row.IsNull(messageFromField) ? null : new Employee((int) row[messageFromField]);
            optMessageFrom.Enabled = (optMessageFrom.Employee != null);

            optMessageTo.Employee = row.IsNull(messageToField) ? null : new Employee((int) row[messageToField]);
            optMessageTo.Enabled = (optMessageTo.Employee != null);

            optMessageText.Text = row.IsNull(messageTextField) ? "" : (string) row[messageTextField];
            optMessageText.Enabled = !optMessageText.Text.Equals("");

            optSignedBy.Employee = row.IsNull(signedByField) ? null : new Employee((int) row[signedByField]);
            optSignedBy.Enabled = (optSignedBy.Employee != null);

            optDocumentType.Type = row.IsNull(documentTypeField) ? null : new DocumentType((int) row[documentTypeField]);
            optDocumentType.Filter = (row.IsNull(modeDocTypeField) ? (byte) 0 : (byte) row[modeDocTypeField]);
            optDocumentType.Enabled = (optDocumentType.Type != null);

            optPerson.Person = row.IsNull(personField) ? null : new Persons.Person((int) row[personField]);
            optPerson.Enabled = (optPerson.Person != null);

            optDocument.Document = row.IsNull(documentField) ? null : new Document((int) row[documentField]);
            optDocument.Enabled = (optDocument.Document != null);

            optNoDocument.Document = row.IsNull(noDocumentField) ? null : new Document((int) row[noDocumentField]);
            optNoDocument.Enabled = (optNoDocument.Document != null);

            optNoPerson.Person = row.IsNull(noPersonField) ? null : new Persons.Person((int) row[noPersonField]);
            optNoPerson.Enabled = (optNoPerson.Person != null);

            optNotSignedBy.Employee = row.IsNull(notSignedByField) ? null : new Employee((int) row[notSignedByField]);
            optNotSignedBy.Enabled = (optNotSignedBy.Employee != null);
        }

        protected override void SaveToSqlParameters(Base original, SqlParameterCollection parameters)
        {
        }

        public override void DB_Update(Base originalObject, SqlTransaction tran)
        {
            originalObject = new FolderRule(ID);
            base.DB_Update(originalObject, tran);
        }

        public struct SqlParameters
        {
            public string modeValue;
            public string folderValue;
            public string idValue;
            public string messageFromValue;
            public string messageToValue;
            public string messageTextValue;
            public string signedByValue;
            public string documentTypeValue;
            public string modeDocTypeValue;
            public string personValue;
            public string documentValue;
            public string noDocumentValue;
            public string noPersonValue;
            public string notSignedByValue;
            public string nameValue;

            public SqlParameters(FolderRule rule)
            {
                modeValue = "null";
                if (rule.Mode == FolderRuleMode.MessageSent) modeValue = "1";
                if (rule.Mode == FolderRuleMode.MessageReceived) modeValue = "2";

                folderValue = (rule.Folder == null ? "null" : rule.Folder.ID.ToString());
                idValue = rule.ID.ToString();

                messageFromValue = "null";
                if (rule.optMessageFrom.Enabled && rule.optMessageFrom.Validate(false))
                    messageFromValue = rule.optMessageFrom.Employee.ID.ToString();

                messageToValue = "null";
                if (rule.optMessageTo.Enabled && rule.optMessageTo.Validate(false))
                    messageToValue = rule.optMessageTo.Employee.ID.ToString();

                signedByValue = "null";
                if (rule.optSignedBy.Enabled && rule.optSignedBy.Validate(false))
                    signedByValue = rule.optSignedBy.Employee.ID.ToString();

                documentTypeValue = "null";
                modeDocTypeValue = "3";
                if (rule.optDocumentType.Enabled && rule.optDocumentType.Validate(false))
                {
                    documentTypeValue = rule.optDocumentType.Type.ID.ToString();
                    modeDocTypeValue = rule.optDocumentType.Filter.ToString();
                }

                personValue = "null";
                if (rule.optPerson.Enabled && rule.optPerson.Validate(false))
                    personValue = rule.optPerson.Person.ID.ToString();

                documentValue = "null";
                if (rule.optDocument.Enabled && rule.optDocument.Validate(false))
                    documentValue = rule.optDocument.Document.ID.ToString();

                messageTextValue = "null";
                if (rule.optMessageText.Enabled && rule.optMessageText.Validate(false))
                    messageTextValue = "'" + (new KString(rule.optMessageText.Text)).SqlEscaped + "'";

                noDocumentValue = "null";
                if (rule.optNoDocument.Enabled && rule.optNoDocument.Validate(false))
                    noDocumentValue = rule.optNoDocument.Document.ID.ToString();

                notSignedByValue = "null";
                if (rule.optNotSignedBy.Enabled && rule.optNotSignedBy.Validate(false))
                    notSignedByValue = rule.optNotSignedBy.Employee.ID.ToString();

                noPersonValue = "null";
                if (rule.optNoPerson.Enabled && rule.optNoPerson.Validate(false))
                    noPersonValue = rule.optNoPerson.Person.ID.ToString();

                nameValue = "'" + (new KString(rule.Name)).SqlEscaped + "'";
            }
        }

        public void SaveRule()
        {
            string queryString = "";

            var p = new SqlParameters(this);

            if (ID != 0)
                queryString =
                    "UPDATE " + FillFrom_Table +
                    " SET " +
                    modeField + "=" + p.modeValue + ", " +
                    messageFromField + "=" + p.messageFromValue + ", " +
                    messageToField + "=" + p.messageToValue + ", " +
                    messageTextField + "=" + p.messageTextValue + ", " +
                    signedByField + "=" + p.signedByValue + ", " +
                    documentTypeField + "=" + p.documentTypeValue + ", " +
                    modeDocTypeField + "=" + p.modeDocTypeValue + ", " +
                    personField + "=" + p.personValue + ", " +
                    documentField + "=" + p.documentValue + ", " +
                    noDocumentField + "=" + p.noDocumentValue + ", " +
                    notSignedByField + "=" + p.notSignedByValue + ", " +
                    noPersonField + "=" + p.noPersonValue + ", " +
                    nameField + "=" + p.nameValue + " " +
                    " WHERE " + ID_Field + "=" + p.idValue;
            else
                queryString =
                    "INSERT INTO " + FillFrom_Table + " (" +
                    folderIDField + ", " +
                    modeField + ", " +
                    messageFromField + ", " +
                    messageToField + ", " +
                    messageTextField + ", " +
                    signedByField + ", " +
                    documentTypeField + ", " +
                    modeDocTypeField + ", " +
                    personField + ", " +
                    documentField + ", " +
                    noDocumentField + ", " +
                    noPersonField + ", " +
                    notSignedByField + ", " +
                    nameField +
                    ") VALUES  (" +
                    p.folderValue + ", " +
                    p.modeValue + ", " +
                    p.messageFromValue + ", " +
                    p.messageToValue + ", " +
                    p.messageTextValue + ", " +
                    p.signedByValue + ", " +
                    p.documentTypeValue + ", " +
                    p.modeDocTypeValue + ", " +
                    p.personValue + ", " +
                    p.documentValue + ", " +
                    p.noDocumentValue + ", " +
                    p.noPersonValue + ", " +
                    p.notSignedByValue + ", " +
                    p.nameValue + ")";

            using (var cm = new SqlCommand(queryString))
            using (cm.Connection = new SqlConnection(connectionString))
            {
                try
                {
                    cm.Connection.Open();
                    cm.ExecuteNonQuery();
                }
                catch (SqlException sex)
                {
                    Env.WriteSqlToLog(sex, cm);
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                }
                finally
                {
                    cm.Connection.Close();
                }
            }
        }

        public bool Exists()
        {
            var p = new SqlParameters(this);
            object obj = null;
            using (
                var cm =
                    new SqlCommand("SELECT TOP 1 " + folderIDField + " FROM " + FillFrom_Table + " WHERE " + //showMode
                                   folderIDField + (p.folderValue.Equals("null") ? " is " : "=") + p.folderValue +
                                   " AND " + modeField + (p.modeValue.Equals("null") ? " is " : "=") + p.modeValue +
                                   " AND " + messageFromField + (p.messageFromValue.Equals("null") ? " is " : "=") +
                                   p.messageFromValue + " AND " + messageToField +
                                   (p.messageToValue.Equals("null") ? " is " : "=") + p.messageToValue + " AND " +
                                   messageTextField + (p.messageTextValue.Equals("null") ? " is " : "=") +
                                   p.messageTextValue + " AND " + signedByField +
                                   (p.signedByValue.Equals("null") ? " is " : "=") + p.signedByValue + " AND " +
                                   documentTypeField + (p.documentTypeValue.Equals("null") ? " is " : "=") +
                                   p.documentTypeValue + " AND " + modeDocTypeField + "=" + p.modeDocTypeValue + " AND " +
                                   personField + (p.personValue.Equals("null") ? " is " : "=") + p.personValue + " AND " +
                                   documentField + (p.documentValue.Equals("null") ? " is " : "=") + p.documentValue +
                                   " AND " + noDocumentField + (p.noDocumentValue.Equals("null") ? " is " : "=") +
                                   p.noDocumentValue + " AND " + noPersonField +
                                   (p.noPersonValue.Equals("null") ? " is " : "=") + p.noPersonValue + " AND " +
                                   notSignedByField + (p.notSignedByValue.Equals("null") ? " is " : "=") +
                                   p.notSignedByValue))
            {
                cm.Connection = new SqlConnection(connectionString);
                try
                {
                    cm.Connection.Open();
                    obj = cm.ExecuteScalar();
                }
                catch (SqlException sex)
                {
                    Env.WriteSqlToLog(sex, cm);
                    throw sex;
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                    throw ex;
                }
                finally
                {
                    cm.Connection.Close();
                    cm.Dispose();
                }
            }
            return (obj != null);
        }

        public bool NameExists()
        {
            object obj = null;
            var p = new SqlParameters(this);

            using (var cm = new SqlCommand(
                "SELECT TOP 1 " + folderIDField + " FROM " + FillFrom_Table +
                " WHERE " +
                folderIDField + (p.folderValue.Equals("null") ? " is " : "=") + p.folderValue + " AND " +
                nameField + "=" + p.nameValue))
            using (cm.Connection = new SqlConnection(connectionString))
            {
                try
                {
                    cm.Connection.Open();
                    obj = cm.ExecuteScalar();
                }
                catch (SqlException sex)
                {
                    Env.WriteSqlToLog(sex, cm);
                    throw sex;
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                    throw ex;
                }
                finally
                {
                    cm.Connection.Close();
                }
            }
            return (obj != null);
        }

        #endregion
    }
}