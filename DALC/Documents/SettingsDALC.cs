using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	public class UserSettings
	{
		public string GroupOrder		=	"LT";	// ������� ����������� ���������� � ������
		public bool NotifyMessage		=	true;	// ���������� �� ��������� � ����������� �����
		public int ReadTimeout			=	5000;	// �������, ����� �������� ���������
													// �� ���. ��������� ������������
		public bool SubLevelDocs;	// �������� ��������� � ���������� (�������)
		public int FilterArchiveDate;		// ������ ���� �������������
		public int FilterDocDate;		// ������ ���� ���������
		public bool DeleteConfirm		=	true;	// ���������� �� ��������� ��� �������� �� ������
													// �� ������� Del
		public bool GroupConfirm		=	true;	// ������������� ��������� �������� (�� ������������)
		public bool ShowNews;	// ���������� ���� ���������
		public bool FaxesInUnsavedOnly	=	true;	// ���������� ������ �� ����������� �������� �����
		public bool FaxesOutUnsavedOnly;	// ���������� ������ �� ����������� ������������ �����
		public bool GotoNext			=	true;	// ���������� �� ��������� �������� ��� �������� ���������
		public bool GotoDocument		=	true;	// ������ ��������� ��������� �� ��������
		public bool MessageOnEndSign	=	true;	// ���������� ��������� ��� ����������� �������
        public bool SortMailingListByAuthor = true; // ������ ������ �������� ���������� �������
		public int PersonID;		// ��� ���� ����� ����������
		public ArrayList LinkDocIDs;				// ��������� � ������
		public bool NeedSave;						// ���� ���������, ���������� ���������.
        public byte ReadMessageOnEndWork = 2;       // ����������� �������� ��� ���������� ������ (������/�������/���������)
		public int FolderUpdateTime = -2;           // ����� ���������� ����� ��������
		public string PersonPrinter = "";    // �������
		private readonly SettingsDALC settingsData;

		public UserSettings(SettingsDALC sData)
		{
			LinkDocIDs = new ArrayList();
			settingsData = sData;
		}

		public void Load()
		{
			DataRow dr = settingsData.GetSettings();
			Dr2Settings(dr);
		}

		public void Reload()
		{
			DataRow dr = settingsData.ReloadSettings();
			Dr2Settings(dr);
		}

        private void Dr2Settings(DataRow dr)
        {
            if (dr != null)
                try
                {
                    object obj;

                    // current person id
                    obj = dr[settingsData.PersonIDField];
                    if (obj is int)
                        PersonID = (int)dr[settingsData.PersonIDField];

                    // get docs in tree
                    obj = dr[settingsData.LinksDocIDsField];
                    if (obj is string)
                    {
                        string[] ids = Regex.Split(obj.ToString(), ",");
                        for (int i = 0; i < ids.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(ids[i]))
                                try
                                {
                                    int testID = int.Parse(ids[i]);
                                    if (!LinkDocIDs.Contains(testID))
                                        LinkDocIDs.Add(testID);
                                }
                                catch { }
                        }
                    }

                    // group order
                    obj = dr[settingsData.GroupOrderField];
                    if (obj is string)
                        GroupOrder = (string)obj;

                    // new messages notification
                    obj = dr[settingsData.NotifyMessageField];
                    if (obj is bool)
                        NotifyMessage = (bool)obj;

                    // read timeout
                    obj = dr[settingsData.ReadTimeoutField];
                    if (obj is int)
                        ReadTimeout = (int)obj;

                    // sub level docs
                    obj = dr[settingsData.SubLevelDocsField];
                    if (obj is bool)
                        SubLevelDocs = (bool)obj;

                    // filter archive date
                    obj = dr[settingsData.FilterArchiveDateField];
                    if (obj is short)
                        FilterArchiveDate = (short)obj;

                    // filter doc date
                    obj = dr[settingsData.FilterDocDateField];
                    if (obj is short)
                        FilterDocDate = (short)obj;

                    // show message on removing doc from work
                    obj = dr[settingsData.DeleteConfirmField];
                    if (obj is bool)
                        DeleteConfirm = (bool)obj;

                    // show news
                    obj = dr[settingsData.ShowNewsField];
                    if (obj is bool)
                        ShowNews = (bool)obj;

                    // load faxes in unsaved only
                    obj = dr[settingsData.FaxesInUnsavedOnlyField];
                    if (obj is bool)
                        FaxesInUnsavedOnly = (bool)obj;

                    // load faxes out unsaved only
                    obj = dr[settingsData.FaxesOutUnsavedOnlyField];
                    if (obj is bool)
                        FaxesOutUnsavedOnly = (bool)obj;

                    // goto next doc after send message
                    obj = dr[settingsData.GotoNextField];
                    if (obj is bool)
                        GotoNext = (bool)obj;

                    // find documents
                    obj = dr[settingsData.GotoDocumentField];
                    if (obj is bool)
                        GotoDocument = (bool)obj;

                    // send message after end sign
                    obj = dr[settingsData.MessageOnEndSignFeild];
                    if (obj is bool)
                        MessageOnEndSign = (bool)obj;

                    // sort mailing list by author
                    obj = dr[settingsData.SortMailingListByAuthorField];
                    if (obj is bool)
                        SortMailingListByAuthor = (bool)obj;

                    // read message after ending work with document
                    obj = dr[settingsData.ReadMessageOnEndWorkField];
                    if (obj is byte)
                        ReadMessageOnEndWork = (byte)obj;

					// read message after ending work with document
					if(dr.Table.Columns.Contains(SettingsDALC.FolderUpdateTimeField))
					{
						obj = dr[SettingsDALC.FolderUpdateTimeField];
						if(obj is byte)
							FolderUpdateTime = (byte)obj;
					}

					// set printer default
					obj = dr[settingsData.PersonPrinterField];
                    if (obj is string)
                        PersonPrinter = (string)obj;
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                }
        }

		public void Save()
		{
			NeedSave = !settingsData.SetSettings(this);
		}
	}

	public struct UserSaveSettings
	{
		public bool AddToWork;
		public bool OpenSaved;
		public bool SendMessage;
		public bool SendMessageNeeded;
	}

	/// <summary>
	/// Summary description for SettingsDALC.
	/// </summary>
	public class SettingsDALC : DALC
	{
	    private const string groupOrderField = "������������������";
        private const string notifyMessageField = "��������������������";
        private const string readTimeoutField = "���������������������";
        private const string subLevelDocsField = "�������������������";
        private const string filterArchiveDateField = "�����������������������";
        private const string filterDocDateField = "�������������������";
        private const string deleteConfirmField = "�������������";
        private const string groupConfirmField = "����������������������";
        private const string showNewsField = "�����������������";
        private const string faxesInUnsavedOnlyField = "�������������������������������";
        private const string faxesOutUnsavedOnlyField = "�����������������������������������";
        private const string gotoNextField = "��������������������������������������";
        private const string gotoDocumentField = "������������������������������������";

        private const string saveAddToWorkField = "�������������������������";
        private const string saveOpenSavedField = "���������������������������";
        private const string saveSendMessageField = "��������������������������";
        private const string messageOnEndSignFeild = "�������������������������";
        private const string linksDocIDsField = "�����������������������";
        private const string personIDField = "�������";
        private const string sortMailingListByAuthorField = "�������������������������������������";
        private const string readMessageOnEndWorkField = "���������������������������������������";
		public const string FolderUpdateTimeField = "����������������������������";
	    private const string personPrinterField = "�������";

        private const string sp_Settings = "sp_���������";

		public SettingsDALC(string connectionString) : base(connectionString)
		{
			tableName = "���������.dbo.vw���������";

			idField = "";
			nameField = "";
		}

		#region Accessors

		public string GroupOrderField
		{
			get { return groupOrderField; }
		}

		public string NotifyMessageField
		{
			get { return notifyMessageField; }
		}

		public string ReadTimeoutField
		{
			get { return readTimeoutField; }
		}

		public string SubLevelDocsField
		{
			get { return subLevelDocsField; }
		}

		public string FilterArchiveDateField
		{
			get { return filterArchiveDateField; }
		}

		public string FilterDocDateField
		{
			get { return filterDocDateField; }
		}

		public string DeleteConfirmField
		{
			get { return deleteConfirmField; }
		}

		public string GroupConfirmField
		{
			get { return groupConfirmField;}
		}

		public string ShowNewsField
		{
			get { return showNewsField;}
		}

		public string FaxesInUnsavedOnlyField
		{
			get { return faxesInUnsavedOnlyField; }
		}
		
		public string FaxesOutUnsavedOnlyField
		{
			get { return faxesOutUnsavedOnlyField; }
		}

		public string GotoNextField
		{
			get { return gotoNextField; }
		}

		public string GotoDocumentField
		{
			get { return gotoDocumentField; }
		}

		public string MessageOnEndSignFeild
		{
			get { return messageOnEndSignFeild;}
		}

		public string SaveAddToWorkField
		{
			get { return saveAddToWorkField; }
		}

		public string SaveOpenSavedField
		{
			get { return saveOpenSavedField; }
		}

		public string SaveSendMessageField
		{
			get { return saveSendMessageField; }
		}

		public string PersonIDField
		{
			get { return personIDField;}
		}

		public string LinksDocIDsField 
		{
			get { return linksDocIDsField;}
		}

        public string SortMailingListByAuthorField
        {
            get { return sortMailingListByAuthorField; }
        }

        public string ReadMessageOnEndWorkField
        {
            get { return readMessageOnEndWorkField; }
        }

        public string PersonPrinterField
	    {
            get { return personPrinterField; }
	    }

		#endregion

		#region Get Data

		public DataRow GetSettings()
		{

			return GetFirstRow(
				sp_Settings,
				delegate(SqlCommand cmd)
				{
					cmd.CommandType = CommandType.StoredProcedure;
				});
		}

		public DataRow ReloadSettings()
		{

			return GetFirstRow(
				"SELECT TOP 1 * FROM " + tableName,
				null);
		}

		#endregion

		#region Change Data

		public bool SetSettings(UserSettings us)
		{
			var cmd = new SqlCommand(
				"UPDATE " + tableName +
				" SET " +
					personIDField + ((us.PersonID>0)?" = @PersonID, ":" = NULL, ") +
					linksDocIDsField + " = @IDs, " +
					groupOrderField + " = @GroupOrder, " +
					notifyMessageField + " = @NotifyMessage, " +
					readTimeoutField + " = @ReadTimeout, " +
					subLevelDocsField + " = @SubLevelDocs, " +
					filterArchiveDateField + " = @FilterArchiveDate, " +
					filterDocDateField + " = @FilterDocDate, " + 
					deleteConfirmField + " = @DeleteConfirm, " + 
					groupConfirmField + " = @GroupConfirm, " +
					showNewsField + " = @ShowNews, " +
					faxesInUnsavedOnlyField + " = @FaxesInUnsavedOnly, " +
					faxesOutUnsavedOnlyField + " = @FaxesOutUnsavedOnly, " +
					gotoNextField + " = @GotoNext, " + 
					gotoDocumentField + " = @GotoDocument, " +
					messageOnEndSignFeild + " = @MessageOnEndSign, " +
                    sortMailingListByAuthorField + " = @SortMailingListByAuthor, " +
                    readMessageOnEndWorkField + " = @ReadMessageOnEndWork, " +
					((us.FolderUpdateTime > -1)?FolderUpdateTimeField + " = @FolderUpdateTime, ":"") +
					personPrinterField + " = @PersonPrinter",
				new SqlConnection(connectionString));

			if(us.PersonID>0)
				AddParam(cmd, "@PersonID", SqlDbType.Int, us.PersonID);
		    AddParam(cmd, "@IDs", SqlDbType.VarChar, string.Join(",", us.LinkDocIDs.Cast<int>().Select(id => id.ToString()).ToArray()));
			AddParam(cmd, "@GroupOrder", SqlDbType.VarChar, us.GroupOrder);
			AddParam(cmd, "@NotifyMessage", SqlDbType.Bit, (us.NotifyMessage ? 1 : 0));
			AddParam(cmd, "@ReadTimeout", SqlDbType.Int, us.ReadTimeout);
			AddParam(cmd, "@SubLevelDocs", SqlDbType.Bit, (us.SubLevelDocs ? 1 : 0));
			AddParam(cmd, "@FilterArchiveDate", SqlDbType.Int, us.FilterArchiveDate);
			AddParam(cmd, "@FilterDocDate", SqlDbType.Int, us.FilterDocDate);
			AddParam(cmd, "@DeleteConfirm", SqlDbType.Bit, (us.DeleteConfirm ? 1 : 0));
			AddParam(cmd, "@GroupConfirm", SqlDbType.Bit, (us.GroupConfirm ? 1 : 0));
			AddParam(cmd, "@ShowNews", SqlDbType.Bit, (us.ShowNews ? 1 : 0));
			AddParam(cmd, "@FaxesInUnsavedOnly", SqlDbType.Bit, (us.FaxesInUnsavedOnly ? 1 : 0));
			AddParam(cmd, "@FaxesOutUnsavedOnly", SqlDbType.Bit, (us.FaxesOutUnsavedOnly ? 1 : 0));
			AddParam(cmd, "@GotoNext", SqlDbType.Bit, (us.GotoNext ? 1 : 0));
			AddParam(cmd, "@GotoDocument", SqlDbType.Bit, (us.GotoDocument ? 1 : 0));
			AddParam(cmd, "@MessageOnEndSign", SqlDbType.Bit, (us.MessageOnEndSign ? 1 : 0));
            AddParam(cmd, "@SortMailingListByAuthor", SqlDbType.Bit, (us.SortMailingListByAuthor ? 1 : 0));
            AddParam(cmd, "@ReadMessageOnEndWork", SqlDbType.TinyInt, us.ReadMessageOnEndWork);
			if(us.FolderUpdateTime > -1)
				AddParam(cmd, "@FolderUpdateTime", SqlDbType.TinyInt, us.FolderUpdateTime);
		    AddParam(cmd, "@PersonPrinter", SqlDbType.VarChar, us.PersonPrinter);

			return CMD_Exec(cmd);
		}

		public bool SetSaveSettings(UserSaveSettings uss)
		{
			string query =
				"UPDATE " + tableName +
				" SET " +
					saveAddToWorkField + " = @SaveAddToWork, " +
					saveOpenSavedField + " = @SaveOpenSaved";
			if (uss.SendMessageNeeded)
				query += ", " + saveSendMessageField + " = @SaveSendMessage";

			var cmd = new SqlCommand(query, new SqlConnection(connectionString));

			AddParam(cmd, "@SaveAddToWork", SqlDbType.Bit, (uss.AddToWork ? 1 : 0));
			AddParam(cmd, "@SaveOpenSaved", SqlDbType.Bit, (uss.OpenSaved ? 1 : 0));
			
			if (uss.SendMessageNeeded)
				AddParam(cmd, "@SaveSendMessage", SqlDbType.Bit, (uss.SendMessage ? 1 : 0));

			return CMD_Exec(cmd);
		}

		#endregion
	}
}