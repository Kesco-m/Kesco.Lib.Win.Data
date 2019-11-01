using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Kesco.Lib.Win.Data.DALC.Corporate;
using Kesco.Lib.Win.Data.DALC.Directory;
using Kesco.Lib.Win.Data.Documents;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    /// <summary>
    /// DAL-��������� ��� ������� � ������ ���������.dbo.vw���������
    /// </summary>
    public class DocumentDALC : DALC
    {
        #region Variables

        // ReSharper disable once InconsistentNaming
        private const string createDateField = "���������";

        // ReSharper disable once InconsistentNaming
        private const string creatorField = "��������";
        private const string dateField = "�������������";
        private const string protectedField = "�������";
        private const string numberField = "��������������";
        private string archiveIDField;
        private string docTypeIDField;
        private const string mainImageIDField = "��������������������������������";

        private string docTypeTable;
        private string docTypeField;
        private string docTypeEngField;
        private string docTypeFinField;

        private const string docSubscribersTable = "vw��������������������";

        private const string workDocView = "vw����������������";
        private const string unreadDocView = "vw��������������������";
        private const string workDocIDField = "�������������������";
        private const string workDocReadField = "��������";
        private string workFolderIDField;
        private string workEmpIDField;

        private const string spentField = "���������";

        private const string workDocMessageView = "vw�������������������������";

        private const string docPersonTable = "vw��������������";
        private string personIDField;
        private const string personPositionField = "���������";
        protected const string personIsValidField = "�������������������������";

        private string docDataTable;

        private const string docFoundTable = "vw������������������";
        private const string docFoundListTable = "vw������������������������";

        private const string spDocRights = "sp_���������������";
        private const string inWorkField = "�������";

        private const string spSimilarDocs = "sp_����������������";
        private const string rightsField = "�����";
        private const string haveDataField = "����������";

        private const string sp_DocFullPath = "sp_DocFullPath";

        private const string sp_ShareArchiveFolder = "sp_���������������������";

        private string archiveTable;

        private string empTable;
        private string empField;
        private const string empEngField = "Employee";
        private string empIDField;

        private string personTable;
        private string personViewName;
        private string projectField;

        /// <summary>
        /// ��� ������� ��������
        /// </summary>
        private string docSignatureTable;

        /// <summary>
        /// ���� ���� � ������� ��������
        /// </summary>
        private string docSignatureDataFeild;

        /// <summary>
        /// ������� ���������� � 1� ���������� ��� ���������
        /// </summary>
        private string buhDocumentTable;

        /// <summary>
        /// ������� ���������� � 1� ���������� ��� �����������
        /// </summary>
        private string buhDictTable;

        private const string spDocsList = "sp_DocsList";

        private const string sp_DeleteDoc = "master.dbo.sp_Delete_��������";

        private const string sp_DocViewStart = "sp_������_DocView";

        private string joinDocTypeTable;

        private string orderBy;
        private string orderByDate;

        private const string sp_MessageCount = "sp_������������������������������";
        private const string messagesCountField = "���������";

        private DocTypeDALC docTypeData;
        private DocDataDALC docDataData;
        private ArchiveDALC archiveData;
        private BuhParamDocDALC buhParamDate;
        private EmployeeDALC empData;
        private PersonDALC personData;
        private FolderDALC folderData;

        private const string messageField = "���������";

        #endregion

        #region Constructor

        public DocumentDALC(string connectionString)
            : base(connectionString)
        {
            tableName = "vw���������";

            idField = "������������";
            nameField = "�����������������";

            docTypeData = new DocTypeDALC(null);
            docDataData = new DocDataDALC(null);
            archiveData = new ArchiveDALC(null);
            empData = new EmployeeDALC(null);
            personData = new PersonDALC(null);
            folderData = new FolderDALC(null);
            buhParamDate = new BuhParamDocDALC(null);
            var signData = new DocSignatureDALC(null);

            docTypeTable = docTypeData.TableName;
            docTypeIDField = docTypeData.IDField;
            docTypeField = docTypeData.NameField;
            docTypeEngField = docTypeData.TypeDocField;
            docTypeFinField = docTypeData.FinansField;

            docDataTable = docDataData.TableName;

            archiveTable = archiveData.TableName;
            archiveIDField = archiveData.IDField;

            buhDocumentTable = buhParamDate.TableName;
            buhDictTable = buhParamDate.BuhParamContrTableName;

            workFolderIDField = folderData.IDField;

            empTable = empData.TableName;
            empField = empData.NameField;
            empIDField = empData.IDField;

            workEmpIDField = empIDField;

            personTable = personData.TableName;
            personViewName = personData.PersonViewName;

            projectField = personData.ProjectField;

            personIDField = personData.IDField;

            docSignatureTable = signData.TableName;
            docSignatureDataFeild = signData.DataField;
            joinDocTypeTable =
                " INNER JOIN " + docTypeTable + " (NOLOCK)" +
                " ON " + tableName + "." + docTypeIDField + " = " + docTypeTable + "." + docTypeIDField;

            orderBy = " ORDER BY " + tableName + "." + idField;
            orderByDate = " ORDER BY " + tableName + "." + dateField + " DESC";
        }

        #region win32

        private const int NO_ERROR = 0;
        private const int MIB_TCP_STATE_ESTAB = 5;

        public struct MIB_TCPTABLE
        {
            public int dwNumEntries;
            public MIB_TCPROW[] table;

        }

        public struct MIB_TCPROW
        {
            public string StrgState;
            public int iState;
            public IPEndPoint Local;
            public IPEndPoint Remote;
        }

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern int GetTcpTable(byte[] pTcpTable, out int pdwSize, bool bOrder);

        public static MIB_TCPTABLE GetTable(byte[] buffer)
        {
            var TcpConnetion = new MIB_TCPTABLE();

            int nOffset = 0;
            // number of entry in the
            TcpConnetion.dwNumEntries = Convert.ToInt32(buffer[nOffset]);
            nOffset += 4;
            TcpConnetion.table = new MIB_TCPROW[TcpConnetion.dwNumEntries];

            for (int i = 0; i < TcpConnetion.dwNumEntries; i++)
            {
                int st = Convert.ToInt32(buffer[nOffset]);
                TcpConnetion.table[i].StrgState = st.ToString();
                TcpConnetion.table[i].iState = st;
                nOffset += 4;
                string LocalAdrr = buffer[nOffset].ToString() + "." + buffer[nOffset + 1].ToString() + "." +
                                   buffer[nOffset + 2].ToString() + "." + buffer[nOffset + 3].ToString();
                nOffset += 4;
                int LocalPort = ((buffer[nOffset]) << 8) + ((buffer[nOffset + 1]));
                nOffset += 4;
                TcpConnetion.table[i].Local = new IPEndPoint(IPAddress.Parse(LocalAdrr), LocalPort);

                string RemoteAdrr = buffer[nOffset].ToString() + "." + buffer[nOffset + 1].ToString() + "." +
                                    buffer[nOffset + 2].ToString() + "." + buffer[nOffset + 3].ToString();
                nOffset += 4;
                int RemotePort;
                if (RemoteAdrr == "0.0.0.0")
                {
                    RemotePort = 0;
                }
                else
                {
                    RemotePort = ((buffer[nOffset]) << 8) + ((buffer[nOffset + 1]));
                }
                nOffset += 4;
                TcpConnetion.table[i].Remote = new IPEndPoint(IPAddress.Parse(RemoteAdrr), RemotePort);
            }
            return TcpConnetion;
        }

        #endregion

		public static bool TestConnection(string connectionString, out string ipAddress)
		{
			ipAddress = null;
			string server = "";

			using(var c = new SqlConnection(connectionString))
			{
				try
				{
					c.Open();
					if(c.State == ConnectionState.Open)
					{
						server = c.DataSource;
						NetworkInterface[ ] coll = NetworkInterface.GetAllNetworkInterfaces();
						bool check = false;
						for(int k = 0; k < coll.Length; k++)
						{
							var ni = coll[k];
							if(ni.OperationalStatus != OperationalStatus.Up)
								continue;
							if(ni.GetIPProperties().GatewayAddresses.Count > 0 &&
								ni.GetIPProperties().GatewayAddresses.Count(x => x.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) > 0 && 
								ni.GetIPProperties().UnicastAddresses.Count > 0 && 
								ni.GetIPProperties().UnicastAddresses.Count(x => x.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !IPAddress.IsLoopback(x.Address) && (x.IPv4Mask.Address & x.Address.Address) == (ni.GetIPProperties().GatewayAddresses.Where(z => z.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).First().Address.Address & x.IPv4Mask.Address)) > 1)
							{
								check = true;
								break;
							}

							if(ni.GetIPProperties().GatewayAddresses.Count > 0 &&
								ni.GetIPProperties().GatewayAddresses.Count(x => x.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) > 0 &&
								ni.GetIPProperties().UnicastAddresses.Count > 0 &&
								ni.GetIPProperties().UnicastAddresses.Count(x => x.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !IPAddress.IsLoopback(x.Address)
								&& (x.IPv4Mask.Address & x.Address.Address) == (ni.GetIPProperties().GatewayAddresses.Where(z => z.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).First().Address.Address & x.IPv4Mask.Address)) == 1)
								if(ipAddress == null)
									ipAddress = ni.GetIPProperties().UnicastAddresses.Where(x => x.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !IPAddress.IsLoopback(x.Address)
										&& (x.IPv4Mask.Address & x.Address.Address) == (ni.GetIPProperties().GatewayAddresses.Where(z => z.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).First().Address.Address & x.IPv4Mask.Address)).First().Address.ToString();
								else
								{
									check = true;
									break;
								}
						}
						if(!check) return true;

						byte[ ] buffer;
						int pdwSize;
						int res = GetTcpTable(null, out pdwSize, true);
						if(res != 0)
						{
							pdwSize = (int)(pdwSize * 1.3);
							buffer = new byte[pdwSize];
							res = GetTcpTable(buffer, out pdwSize, true);
							if(res == 0)
							{
								MIB_TCPTABLE tab = GetTable(buffer);
								IPHostEntry iph = Dns.GetHostEntry(server);
								for(int i = 0; i < tab.dwNumEntries; i++)
								{
									if(tab.table[i].iState == MIB_TCP_STATE_ESTAB &&
										tab.table[i].Remote.Port == 1433)
									{
										//IPHostEntry iph = Dns.GetHostEntry(tab.table[i].Remote.Address);
										//Match m = Regex.Match(iph.HostName, "^" + server + "(.[0-9a-z]|$)",
										//                      RegexOptions.IgnoreCase);
										//if (m.Success)
										//{
										//    ipAddress = tab.table[i].Local.Address.ToString();
										//    break;
										//}
										if(iph.AddressList.Contains(tab.table[i].Remote.Address))
										{
											ipAddress = tab.table[i].Local.Address.ToString();
											break;
										}
									}
								}
							}
							else if(res == 122)
								Env.WriteToLog("������ ������ �� ����������");
							else
								Env.WriteToLog("��� ��������� ip ������ ������� ��������� ������ " + res.ToString());
						}
						else
							Env.WriteToLog("�� ������� ������ ������");

						return true;
					}
				}
				catch(Exception ex)
				{
					Env.WriteToLog(ex);
				}
				finally
				{
					if(c.State != ConnectionState.Closed)
						c.Close();
				}
			}
			return false;
		}

        #endregion

        #region Accessors

        public string DateField
        {
            get { return dateField; }
        }

        /// <summary>
        /// ���������
        /// </summary>
        public string CreateDateField
        {
            get { return createDateField; }
        }

        /// <summary>
        /// ��������
        /// </summary>
        public string CreatorField
        {
            get { return creatorField; }
        }

        public string NumberField
        {
            get { return numberField; }
        }

        public string ProtectedField
        {
            get { return protectedField; }
        }

        public string SpDocRights
        {
            get { return spDocRights; }
        }

        public string WorkDocIDField
        {
            get { return workDocIDField; }
        }

        public string WorkDocReadField
        {
            get { return workDocReadField; }
        }

        public string SpentField
        {
            get { return spentField; }
        }

        public string WorkFolderIDField
        {
            get { return workFolderIDField; }
        }

        public string WorkEmpIDField
        {
            get { return workEmpIDField; }
        }

        public string PersonIDField
        {
            get { return personIDField; }
        }

        public string DocTypeIDField
        {
            get { return docTypeIDField; }
        }

        public string DocTypeField
        {
            get { return docTypeField; }
        }

        public string DocTypeEngField
        {
            get { return docTypeEngField; }
        }

        public string MainImageIDField
        {
            get { return mainImageIDField; }
        }

        public string EmpField
        {
            get { return empField; }
        }

        public string EmpEngField
        {
            get { return empEngField; }
        }

        public string EmpIDField
        {
            get { return empIDField; }
        }

        public string InWorkField
        {
            get { return inWorkField; }
        }

        public string RightsField
        {
            get { return rightsField; }
        }

        public string HaveDataField
        {
            get { return haveDataField; }
        }

        public string MessageField
        {
            get { return messageField; }
        }

        public string PersonPositionField
        {
            get { return personPositionField; }
        }

        public string PersonIsValidField
        {
            get { return personIsValidField; }
        }

        public string MessageCountField
        {
            get { return messagesCountField; }
        }

        #endregion

        #region Get Data

        public DataTable GetDocs(string path, out SqlCommand selectCommand)
        {
            return GetDataTable(spDocsList,
                                delegate(SqlCommand cmd)
                                    {
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        AddParam(cmd, "@Path", SqlDbType.VarChar, path);
                                    }, out selectCommand);
        }

		public DataTable GetDocs(string path, CancellationToken source)
		{
			return GetDataTable(spDocsList,
								delegate(SqlCommand cmd)
								{
									cmd.CommandType = CommandType.StoredProcedure;
									AddParam(cmd, "@Path", SqlDbType.VarChar, path);
								}, source);
		}

        public DataTable GetWorkFolderDocs(int folderID, int empID, string lang)
        {
            return GetDataTable(
                "SELECT " +
                idField + ", " +
                "DATEADD(hour, @Diff, " + messageField + ") AS " + messageField + ", " +
                docTypeIDField + ", " +
                (lang.StartsWith("ru") ? docTypeField : docTypeEngField) + " " + docTypeField + ", " +
                "CASE WHEN " + nameField + " <> '' THEN " + nameField + " ELSE " +
                (lang.StartsWith("ru") ? docTypeField : docTypeEngField) + " END " + nameField + ", " +
                dateField + ", " +
                numberField + ", " +
                descriptionField + ", " +
                mainImageIDField + ", " +
                workDocReadField + ", " +
                workDocIDField +
                " FROM " + workDocMessageView + " (NOLOCK) " +
                " WHERE " + workDocIDField + " IN(" +
                "SELECT " + workDocIDField +
                " FROM " + workDocView + " (NOLOCK)" +
                " WHERE " + "(" + empIDField + " = @EmpID)" +
                " AND " + "(" + workFolderIDField + ((folderID > 0) ? " = @FolderID" : " IS NULL") + "))" +
                " ORDER BY " + messageField + " DESC",
                delegate(SqlCommand cmd)
                    {
                        if (folderID > 0)
                            AddParam(cmd, "@FolderID", SqlDbType.Int, folderID);

                        AddParam(cmd, "@Diff", SqlDbType.Int, GetTimeDiff().Hours);
                        AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                    });
        }

        public DataTable GetWorkFolderDocs(string lang, int personID, int empID, int workFolderID, CancellationToken source)
        {
            return GetDataTable(
                "SELECT " +
                idField + ", " +
                empIDField + ", " +
                workFolderIDField + ", " +
                editedField + ", " +
                "DATEADD(hour, @Diff, " + messageField + ") AS " + messageField + ", " +
                docTypeIDField + ", " +
                (lang.StartsWith("ru") ? docTypeField : docTypeEngField) + " " + docTypeField + ", " +
                "CASE WHEN " + nameField + " <> '' THEN " + nameField + " ELSE " +
                (lang.StartsWith("ru") ? docTypeField : docTypeEngField) + " END " + nameField + ", " +
                dateField + ", " +
                numberField + ", " +
                descriptionField + ", " +
                mainImageIDField + ", " +
                workDocReadField + ", " +
                workDocIDField + ", " +
                " CASE WHEN EXISTS(SELECT * FROM " + buhDocumentTable + " (NOLOCK) WHERE " + idField + " = " +
                workDocMessageView + "." + idField + " AND ������� = 0" +
                ((personID > 0) ? " AND �������������� = @PersonID" : "") + ")" +
                " OR EXISTS(SELECT * FROM " + buhDictTable + " (NOLOCK) WHERE " + idField + " = " + workDocMessageView +
                "." + idField + " AND ������� = 0" + ((personID > 0) ? " AND �������������� = @PersonID" : "") +
                ") THEN '�' ELSE '' END" +
                "+CASE WHEN EXISTS(SELECT * FROM " + buhDocumentTable + " (NOLOCK) WHERE " + idField + " = " +
                workDocMessageView + "." + idField + " AND ������� = 1" +
                ((personID > 0) ? " AND �������������� = @PersonID" : "") + ")" +
                " OR EXISTS(SELECT * FROM " + buhDictTable + " (NOLOCK) WHERE " + idField + " = " + workDocMessageView +
                "." + idField + " AND ������� = 1 " + ((personID > 0) ? " AND �������������� = @PersonID" : "") +
                ") THEN '�' ELSE '' END " + spentField +
                " FROM " + workDocMessageView + " (NOLOCK) " +
                " WHERE " + empIDField + " = @EmpID AND " +
                workFolderIDField + (workFolderID > 0 ? " = @WorkFolderID" : " IS NULL") +
                " ORDER BY " + messageField + " DESC",
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@diff", SqlDbType.Int, GetTimeDiff().Hours);
                        if (personID > 0)
                            AddParam(cmd, "@PersonID", SqlDbType.Int, personID);
                        AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                        if (workFolderID > 0)
                            AddParam(cmd, "@WorkFolderID", SqlDbType.Int, workFolderID);
                    }, source);
        }

        public DataTable GetDoc(string lang, int docID, int folderID, int empID)
        {
            return GetDataTable(
                "SELECT " +
                idField + ", " +
                empIDField + ", " +
                workFolderIDField + ", " +
                editedField + ", " +
                "DATEADD(hour, @Diff, " + messageField + ") AS " + messageField + ", " +
                docTypeIDField + ", " +
                (lang.StartsWith("ru") ? docTypeField : docTypeEngField) + " " + docTypeField + ", " +
                "CASE WHEN " + nameField + " <> '' THEN " + nameField + " ELSE " +
                (lang.StartsWith("ru") ? docTypeField : docTypeEngField) + " END " + nameField + ", " +
                dateField + ", " +
                numberField + ", " +
                descriptionField + ", " +
                mainImageIDField + ", " +
                workDocReadField + ", " +
                workDocIDField + 
               
            " FROM " + workDocMessageView +
                " WHERE " + idField + " = @DocID AND " + workFolderIDField + (folderID > 0 ? " = @FolderID " : " IS NULL ") + " AND " + empIDField + " = @EmpID",
                delegate(SqlCommand cmd)
                {
                    AddParam(cmd, "@diff", SqlDbType.Int, GetTimeDiff().Hours);
                    AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                    if (folderID > 0)
                        AddParam(cmd, "@FolderID", SqlDbType.Int, folderID);
                    AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                });
        }

        public bool DocHasMessage(int docID)
        {
            return GetCount(sp_MessageCount,
                            delegate(SqlCommand cmd)
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;

                                    AddParam(cmd, "@������������", SqlDbType.Int, docID);
                                }) > 0;
        }

        public DataTable GetDocSubscribers(int docID)
        {
            return GetDataTable(
                "SELECT " +
                docSubscribersTable + "." + empIDField + ", " +
                empTable + "." + empData.FIOField +
                " FROM " + docSubscribersTable +
                " INNER JOIN " + empTable +
                " ON " + docSubscribersTable + "." + empIDField + " = " + empTable + "." + empIDField +
                " WHERE " + idField + " = @DocID AND  ��������� < 3 " +
                " ORDER BY " + empTable + "." + empData.FIOField,
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                    });
        }

        public DataTable GetDocSubscribers(string docIDs)
        {
            return GetDataTable("SELECT DISTINCT " +
                                docSubscribersTable + "." + empIDField + ", " +
                                empTable + "." + empData.FIOField +
                                " FROM " + docSubscribersTable +
                                " INNER JOIN " + empTable +
                                " ON " + docSubscribersTable + "." + empIDField + " = " + empTable + "." + empIDField +
                                " WHERE " + idField + " in (" + docIDs + ") AND  ��������� < 3 " +
                                " ORDER BY " + empTable + "." + empData.FIOField,
                                null);
        }

        /// <summary>
        /// �������� �������� ���������
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public DataRow GetDocProperties(int docId, string lang)
        {
            return GetFirstRow(
                "SELECT " +
                tableName + "." + idField + ", " +
                tableName + "." + docTypeIDField + ", " +
                tableName + "." + nameField + ", " +
                "ISNULL(" + docTypeTable + "." + (lang.Equals("ru") ? docTypeField : docTypeEngField) + ", " + tableName +
                "." + docTypeIDField + ") AS " + docTypeField + ", " +
                docTypeTable + "." + docTypeData.NameExistField + ", " +
                numberField + ", " +
                dateField + ", " +
                creatorField + ", " +
                createDateField + ", " +
                descriptionField + ", " +
                protectedField + ", " +
				tableName + "." + mainImageIDField + ", " +
                tableName + "." + editorField + ", " +
                tableName + "." + editedField +
                " FROM " + tableName +
                joinDocTypeTable +
                " WHERE " + idField + " = @DocID",
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@DocID", SqlDbType.Int, docId);
                    });
        }

        public DataTable GetDocPersons(int docID, DateTime docDate)
        {
            return GetDataTable(
                " DECLARE @Tbl TABLE (" + personIDField + " int, " + personPositionField + " tinyint, " +
                personIsValidField + " int)  \n" +
                " INSERT @Tbl SELECT DISTINCT " + docPersonTable + "." + personIDField + ", min(" + docPersonTable + "." +
                personPositionField + "), CASE WHEN @������������� IS NULL THEN 1 ELSE 0 END \n" +
                " FROM " + docPersonTable + " WHERE " + docPersonTable + "." + idField + " = @DocID  \n" +
                " GROUP BY " + docPersonTable + "." + personIDField + " \n" +

                " IF @������������� IS NOT NULL BEGIN  \n" +
                " UPDATE @Tbl SET " + personIsValidField + " = 1 FROM @Tbl Y  \n" +
                " WHERE EXISTS (SELECT * FROM " + personData.OrgCardTable + " X WHERE X." + personIDField + " = Y." +
                personIDField + " AND X." + personData.FromFieldName + " <= @������������� AND @������������� < X." +
                personData.ToFieldName + ") \n" +

                " UPDATE @Tbl SET " + personIsValidField + " =  1 FROM @Tbl Y \n" +
                " WHERE EXISTS (SELECT * FROM " + personData.PersCardTable + " X WHERE X." + personIDField + " = Y." +
                personIDField + " AND X." + personData.FromFieldName + " <= @������������� AND @������������� < X." +
                personData.ToFieldName + ") \n" +
                " END \n" +
                " SELECT Y." + personIDField + ", X." + personData.NameField + ", Y." + personPositionField + ", Y." +
                personIsValidField + " \n" +
                " FROM @Tbl Y INNER JOIN " + personTable + " X ON Y." + personIDField + " = X." + personIDField + " \n" +
                " ORDER BY Y." + personPositionField + ", X." + personData.NameField,
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                        AddParam(cmd, "@�������������", SqlDbType.DateTime, docDate);
                    });
        }

        public bool CheckIsPersonValid(int personID, DateTime docDate)
        {
            object obj = GetField(
                " DECLARE @Tbl TABLE(" + personIDField + " int) " +
                " IF (EXISTS(SELECT * FROM " + personData.OrgCardTable + " X WITH (nolock) WHERE X." + personIDField +
                " = @��� AND X." + personData.FromFieldName + " <= @���� AND @���� < X." + personData.ToFieldName +
                ") \n" +
                " OR EXISTS(SELECT * FROM " + personData.PersCardTable + " X WITH (nolock) WHERE X." + personIDField +
                " = @��� AND X." + personData.FromFieldName + " <= @���� AND @���� < X." + personData.ToFieldName +
                "))\n" +
                "   INSERT INTO @Tbl VALUES (@���) " +

                " SELECT COUNT(*) FROM @Tbl", "COUNT",
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@���", SqlDbType.Int, personID);
                        AddParam(cmd, "@����", SqlDbType.DateTime, docDate);
                    });

            if (obj is bool)
                return (bool) obj;
            else if (obj is byte)
                return (byte) obj > 0;
            else if (obj is int)
                return (int) obj > 0;

            return false;
        }

        public DataTable GetDocPersons(int docID)
        {
            return GetDocPersons(docID, false);
        }

		/// <summary>
		/// ��������� ����� ��� ��������� � ���� ������
		/// </summary>
		/// <param name="docID"></param>
		/// <returns></returns>
        public string GetDocPersonsIDs(int docID)
        {
            if (docID < 1)
                return "";

            string query = "SELECT " + docPersonTable + "." + personIDField + 
                           " FROM " + docPersonTable + " WHERE " + docPersonTable + "." + idField + " = @docID";
            return ReadString(query,
                              delegate(SqlCommand cmd)
                                  {
                                      AddParam(cmd, "@docID", SqlDbType.Int, docID);
                                  });
        }

		public string GetDocsPersonsID(string ids)
		{
			if(string.IsNullOrEmpty(ids))
				return "";

			string query = "SELECT DISTINCT " + docPersonTable + "." + personIDField +
						   " FROM " + docPersonTable + " WHERE " + docPersonTable + "." + idField + " in (" + ids + ")";
			return ReadString(query,null);
		}

		public int CheckFaxSenderRule(string ids)
		{
			if(string.IsNullOrEmpty(ids))
				return -1;

			string query = "SELECT TOP 1 ������������ " +
			"FROM [���������].[dbo].[vw���������] vw� WHERE ������������ IN (" + ids + ") " +
			"AND (�������������������������������� IS NULL " +
			" OR (NOT EXISTS(SELECT * FROM ��������������.dbo.fn_�����������() WHERE ������� IN (21,22) AND ������� = 0) " +
			"AND NOT EXISTS( SELECT * FROM vw�������������� WHERE ������������ = vw�.������������ AND ������� IN (SELECT ������� FROM ��������������.dbo.fn_�����������() WHERE ������� IN (21,22)))) "+
			"OR EXISTS(SELECT * FROM ����������������� WHERE ������������ = vw�.������������ AND ����������������������� = �������������������������������� AND ���������� = 101))";
			return GetRecord<int>(query, null, delegate(IDataRecord dr)
                            {
								if(dr!= null && !dr.IsDBNull(0))
									return dr.GetInt32(0);
								return 0;
                            });
		}

        /// <summary>
        /// ���������� ������� � �����-�������� ��������� � �������� ������� � ����������������� �����-������ �� ���� ���������
        /// </summary>
        /// <param name="docID">��� ���������</param>
        /// <param name="notOur"></param>
        public DataTable GetDocPersons(int docID, bool notOur)
        {
            return GetDataTable(
                " DECLARE @Tbl TABLE (" + personIDField + " int, " + personPositionField + " tinyint, " +
                personIsValidField + " int)  \n" +
                " DECLARE @������������� datetime  \n" +
                " SET @������������� = (SELECT vw���������.������������� FROM vw��������� WHERE vw���������." + idField +
                " = @DocID)  \n" +
                " INSERT @Tbl SELECT DISTINCT " + docPersonTable + "." + personIDField + ", min(vw��������������." +
                personPositionField + "), CASE WHEN @������������� IS NULL THEN 1 ELSE 0 END \n" +
                " FROM " + docPersonTable + " WHERE " + docPersonTable + "." + idField + " = @DocID  \n" +
                " GROUP BY " + docPersonTable + "." + personIDField + " \n" +

                " IF @������������� IS NOT NULL BEGIN  \n" +
                " UPDATE @Tbl SET " + personIsValidField + " = 1 FROM @Tbl Y  \n" +
                " WHERE EXISTS (SELECT * FROM " + personData.OrgCardTable + " X WHERE X." + personIDField + " = Y." +
                personIDField + " AND X." + personData.FromFieldName + " <= @������������� AND @������������� < X." +
                personData.ToFieldName + ") \n" +

                " UPDATE @Tbl SET " + personIsValidField + " =  1 FROM @Tbl Y \n" +
                " WHERE EXISTS (SELECT * FROM " + personData.PersCardTable + " X WHERE X." + personIDField + " = Y." +
                personIDField + " AND X." + personData.FromFieldName + " <= @������������� AND @������������� < X." +
                personData.ToFieldName + ") \n" +
                " END \n" +
                " SELECT Y." + personIDField + ", X." + personData.NameField + ", Y." + personPositionField + ", Y." +
                personIsValidField + " \n" +
                " FROM @Tbl Y INNER JOIN " + personTable + " X ON Y." + personIDField + " = X." + personIDField +
                (notOur ? (" AND X." + projectField + " IS NULL ") : "") + " \n" +
                " ORDER BY Y." + personPositionField + ", X." + personData.NameField,
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                    });
        }

        /// <summary>
        /// �������� id � ������������ ��� �����-������� ���������
        /// </summary>
        /// <param name="docID">��� ���������</param>
        /// <param name="notOur"></param>
        public DataTable GetDocPersonsLite(int docID, bool notOur)
        {
            return GetDataTable(
                " SELECT " + personTable + "." + personIDField + ", " + personTable + "." + personData.NameField + " \n" +
                " FROM " + docPersonTable + " (nolock) INNER JOIN " + personTable + " (nolock) ON " + personTable + "." +
                personIDField + " = " + docPersonTable + "." + personIDField +
                " AND " + docPersonTable + "." + idField + " = @DocID " +
                (notOur ? (" AND " + personTable + "." + projectField + " IS NULL ") : "") + " \n" +
                " ORDER BY " + personData.NameField,
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                    });
        }

        public object GetCurator(int docID)
        {
            return GetRecord<object>(
                @"SELECT �������������1 �������������
					FROM dbo.�������������� Child
					INNER JOIN dbo.�������������� Parent ON Child.L >= Parent.L AND Child.R <= Parent.R
					INNER JOIN dbo.vw������������������������ ON Child.���������������� = dbo.vw������������������������.����������������
					INNER JOIN dbo.vw��������������� ON dbo.vw������������������������._������������ = dbo.vw���������������.���������������������
					INNER JOIN dbo.vw��������� ON dbo.vw���������������.����������������������� = dbo.vw���������.������������
					WHERE Parent.���������������� = 2039 AND dbo.vw���������������.����������������������� = @DocID
					 AND  dbo.vw���������.���������������� NOT IN (SELECT     Child.����������������
                            FROM dbo.�������������� Parent
							INNER JOIN dbo.�������������� Child ON Parent.L <= Child.L AND Parent.R >= Child.R
                            WHERE Parent.���������������� = 2039)
UNION 
				SELECT �������������1 �������������
				FROM dbo.vw������������������������
				WHERE _������������ = @DocID AND ���������������� IN
                    (SELECT Child.���������������� FROM dbo.�������������� Parent 
					INNER JOIN dbo.�������������� Child ON Parent.L <= Child.L AND Parent.R >= Child.R
                    WHERE Parent.���������������� = 2039)",
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                    }, null);
        }

        public string GetFoundDocsIDQuery(int employeeID)
        {
            return "SELECT " + idField + " FROM " + docFoundListTable + " WHERE " + empIDField + " = " +
                   employeeID.ToString();
        }

        public string GetFoundDocsIDQuery(string sql, bool query)
        {
            if (!query)
                return "SELECT " + idField + " FROM " + "(" + sql + ") D";
            else
            {
                string retValue = sql.Substring(0, Data.DALC.Documents.Search.Options.SelectFrom.Length).Replace("*", "T0." + idField);
                retValue += sql.Substring(Data.DALC.Documents.Search.Options.SelectFrom.Length);
                return retValue;
            }
        }

		/// <summary>
		/// ��������� �������� ���������� �� �������
		/// </summary>
		/// <param name="sql">������ ��� ������ ����������</param>
		/// <param name="lang">���� ������</param>
		/// <param name="empID">��� ����������</param>
		/// <param name="personID">��� ���� ������</param>
		/// <returns>������� � ��������� ����������� �� �������</returns>
		public DataTable GetQueryDocs(string sql, string lang, int empID, int personID)
        {
            const string t0 = "T0";

            string selectString =
                t0 + "." + idField + ", " +
                docTypeTable + "." + docTypeIDField + ", " +
                docTypeTable + "." + (lang.StartsWith("ru") ? docTypeField : docTypeEngField) + " " + docTypeField +
                ", " +
                "CASE WHEN " + t0 + "." + nameField + " <> '' THEN " + t0 + "." + nameField + " ELSE " + docTypeTable +
                "." + (lang.StartsWith("ru") ? docTypeField : docTypeEngField) + " END " + nameField + ", " +
                dateField + ", " +
                numberField + ", " +
                mainImageIDField + ", " +
                "REPLACE(" + descriptionField + ", CHAR(13) + CHAR(10), CHAR(160) + ' ') " + descriptionField + ", " +
                
                "(CASE WHEN EXISTS(SELECT * FROM " + unreadDocView + " (NOLOCK) WHERE "
                + unreadDocView + "." + idField + " = " + t0 + "." + idField + " AND " + unreadDocView + "." + empIDField +
                " = @EmpID) THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END) AS " + workDocReadField + ", " +

                " CASE WHEN EXISTS(SELECT * FROM " + buhDocumentTable + " (NOLOCK) WHERE " + idField + " = " + t0 + "." +
                idField + " AND ������� = 0" + ((personID > 0) ? " AND �������������� = @PersonID" : "") + ")" +
                " OR EXISTS(SELECT * FROM " + buhDictTable + " (NOLOCK) WHERE " + idField + " = " + t0 + "." + idField +
                " AND ������� = 0" + ((personID > 0) ? " AND �������������� = @PersonID" : "") +
                ") THEN '�' ELSE '' END" +
                "+CASE WHEN EXISTS(SELECT * FROM " + buhDocumentTable + " (NOLOCK) WHERE " + idField + " = " + t0 + "." +
                idField + " AND ������� = 1" + ((personID > 0) ? " AND �������������� = @PersonID" : "") + ")" +
                " OR EXISTS(SELECT * FROM " + buhDictTable + " (NOLOCK) WHERE " + idField + " = " + t0 + "." + idField +
                " AND ������� = 1 " + ((personID > 0) ? " AND �������������� = @PersonID" : "") +
                ") THEN '�' ELSE '' END " + spentField + " ";

            const string orderString = " ORDER BY " + dateField + " DESC";
            string joinStr = joinDocTypeTable.Replace(tableName, t0);
            selectString = Data.DALC.Documents.Search.Options.SelectFrom.Replace("*", selectString) + joinStr;
            sql = sql.Substring(Data.DALC.Documents.Search.Options.SelectFrom.Length);
            sql = selectString + sql;
            sql += orderString;

            return GetDataTable(sql,
                                delegate(SqlCommand cmd)
                                    {
                                        AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                                        if (personID > 0)
                                            AddParam(cmd, "@PersonID", SqlDbType.Int, personID);
                                    });
        }

		/// <summary>
		/// ��������� �������� ����������
		/// </summary>
		/// <param name="lang">���� ������</param>
		/// <param name="employeeID">��� ����������</param>
		/// <param name="personID">��� ���� ������</param>
		/// <param name="cts">����� ������</param>
        public DataTable GetFoundDocs(string lang, int employeeID, int personID)
        {
			return GetFoundDocs(lang, employeeID, personID, CancellationToken.None);
        }

		/// <summary>
		/// ��������� �������� ����������
		/// </summary>
		/// <param name="lang">���� ������</param>
		/// <param name="employeeID">��� ����������</param>
		/// <param name="personID">��� ���� ������</param>
		/// <param name="cts">����� ������</param>
		/// <returns>������� � ��������� �����������</returns>
		public DataTable GetFoundDocs(string lang, int employeeID, int personID, CancellationToken cts)
		{
			return GetDataTable(
				"IF OBJECT_ID('tempdb..#T') IS NOT NULL DROP TABLE #T" + Environment.NewLine +
				"CREATE TABLE #T(" + empIDField + " int, " + idField + " int, " + docTypeIDField + " int, " + docTypeField + " nvarchar(100), " +
				 nameField + " nvarchar(100), " + dateField + " datetime, " + numberField + " nvarchar(50), " +
				mainImageIDField + " int, " + descriptionField + " nvarchar(500), " + workDocReadField + " bit, " + spentField + " varchar(2))" + Environment.NewLine +
			"INSERT	#T" + Environment.NewLine +
			"SELECT " + empIDField + ", " +
				idField + ", " +
				docTypeIDField + ", " +
				(lang.StartsWith("ru") ? docTypeField : docTypeEngField) + " " + docTypeField + ", " +
				"CASE WHEN " + nameField + " <> '' THEN " + nameField + " ELSE " +
				(lang.StartsWith("ru") ? docTypeField : docTypeEngField) + " END " + nameField + ", " +
				dateField + ", " +
				numberField + ", " +
				mainImageIDField + ", " +
				descriptionField + ", 1 " + workDocReadField + ", '' " + spentField + Environment.NewLine +
				" FROM " + docFoundListTable + " (NOLOCK)" + Environment.NewLine +
								((employeeID > 0) ? (" WHERE " + docFoundListTable + "." + empIDField + " = @EmpID") : "") + Environment.NewLine +
				"UPDATE	T " + Environment.NewLine +
				"SET " + workDocReadField + " = 0 " + Environment.NewLine +
				" FROM	#T T " + Environment.NewLine +
				"WHERE	EXISTS (SELECT * FROM " + unreadDocView + " X (nolock) " +
									"WHERE X." + idField + " = T." + idField + " AND  X." + empIDField + " = @EmpID)" + Environment.NewLine +

				"UPDATE	T " + Environment.NewLine +
				"SET	" + spentField + " = '�'" + Environment.NewLine +
				"FROM	#T T" + Environment.NewLine +
				"WHERE	EXISTS (SELECT * FROM " + buhDocumentTable + " X (NOLOCK) WHERE X." + idField + " = T." + idField + " AND ������� = 0" + ((personID > 0) ? " AND �������������� = @PersonID" : "") + ")" +
					"OR EXISTS (SELECT * FROM " + buhDictTable + " X (NOLOCK) WHERE X." + idField + " = T." + idField + " AND ������� = 0" + ((personID > 0) ? " AND �������������� = @PersonID" : "") + ")" + Environment.NewLine +
				"UPDATE	T " + Environment.NewLine +
				"SET	" + spentField + " = " + spentField + " + '�' " + Environment.NewLine +
				"FROM	#T T" + Environment.NewLine +
				"WHERE	EXISTS (SELECT * FROM " + buhDocumentTable + " X (NOLOCK) WHERE X." + idField + " = T." + idField + " AND ������� = 1" + ((personID > 0) ? " AND �������������� = @PersonID" : "") + ")" +
					"OR EXISTS (SELECT * FROM " + buhDictTable + " X (NOLOCK) WHERE X." + idField + " = T." + idField + " AND ������� = 1" + ((personID > 0) ? " AND �������������� = @PersonID" : "") + ")" + Environment.NewLine +
				"SELECT * FROM #T ORDER BY " + dateField + " DESC" + Environment.NewLine +
				"DROP TABLE #T",
				delegate(SqlCommand cmd)
				{
					if(employeeID > 0)
						AddParam(cmd, "@EmpID", SqlDbType.Int, employeeID);
					if(personID > 0)
						AddParam(cmd, "@PersonID", SqlDbType.Int, personID);
				}, cts);
		}


		public DataTable GetQueryDocs(string sql, string lang, int empID, int personID, out SqlCommand selectCommand)
		{
			const string t0 = "T0";

			string selectString =
				t0 + "." + idField + ", " +
				docTypeTable + "." + docTypeIDField + ", " +
				docTypeTable + "." + (lang.StartsWith("ru") ? docTypeField : docTypeEngField) + " " + docTypeField +
				", " +
				"CASE WHEN " + t0 + "." + nameField + " <> '' THEN " + t0 + "." + nameField + " ELSE " + docTypeTable +
				"." + (lang.StartsWith("ru") ? docTypeField : docTypeEngField) + " END " + nameField + ", " +
				dateField + ", " +
				numberField + ", " +
				mainImageIDField + ", " +
				"REPLACE(" + descriptionField + ", CHAR(13) + CHAR(10), CHAR(160) + ' ') " + descriptionField + ", " +

				"(CASE WHEN EXISTS(SELECT * FROM " + unreadDocView + " (NOLOCK) WHERE "
				+ unreadDocView + "." + idField + " = " + t0 + "." + idField + " AND " + unreadDocView + "." + empIDField +
				" = @EmpID) THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END) AS " + workDocReadField + ", " +

				" CASE WHEN EXISTS(SELECT * FROM " + buhDocumentTable + " (NOLOCK) WHERE " + idField + " = " + t0 + "." +
				idField + " AND ������� = 0" + ((personID > 0) ? " AND �������������� = @PersonID" : "") + ")" +
				" OR EXISTS(SELECT * FROM " + buhDictTable + " (NOLOCK) WHERE " + idField + " = " + t0 + "." + idField +
				" AND ������� = 0" + ((personID > 0) ? " AND �������������� = @PersonID" : "") +
				") THEN '�' ELSE '' END" +
				"+CASE WHEN EXISTS(SELECT * FROM " + buhDocumentTable + " (NOLOCK) WHERE " + idField + " = " + t0 + "." +
				idField + " AND ������� = 1" + ((personID > 0) ? " AND �������������� = @PersonID" : "") + ")" +
				" OR EXISTS(SELECT * FROM " + buhDictTable + " (NOLOCK) WHERE " + idField + " = " + t0 + "." + idField +
				" AND ������� = 1 " + ((personID > 0) ? " AND �������������� = @PersonID" : "") +
				") THEN '�' ELSE '' END " + spentField + " ";

			const string orderString = " ORDER BY " + dateField + " DESC";
			string joinStr = joinDocTypeTable.Replace(tableName, t0);
			selectString = Data.DALC.Documents.Search.Options.SelectFrom.Replace("*", selectString) + joinStr;
			sql = sql.Substring(Data.DALC.Documents.Search.Options.SelectFrom.Length);
			sql = selectString + sql;
			sql += orderString;

			return GetDataTable(sql,
								delegate(SqlCommand cmd)
								{
									AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
									if(personID > 0)
										AddParam(cmd, "@PersonID", SqlDbType.Int, personID);
								}, out selectCommand);
		}

		public DataTable GetQueryDocs(string sql, string lang, int empID, int personID, CancellationToken source)
		{
			const string t0 = "T0";

			string selectString =
				t0 + "." + idField + ", " +
				docTypeTable + "." + docTypeIDField + ", " +
				docTypeTable + "." + (lang.StartsWith("ru") ? docTypeField : docTypeEngField) + " " + docTypeField +
				", " +
				"CASE WHEN " + t0 + "." + nameField + " <> '' THEN " + t0 + "." + nameField + " ELSE " + docTypeTable +
				"." + (lang.StartsWith("ru") ? docTypeField : docTypeEngField) + " END " + nameField + ", " +
				dateField + ", " +
				numberField + ", " +
				mainImageIDField + ", " +
				"REPLACE(" + descriptionField + ", CHAR(13) + CHAR(10), CHAR(160) + ' ') " + descriptionField + ", " +

				"(CASE WHEN EXISTS(SELECT * FROM " + unreadDocView + " (NOLOCK) WHERE "
				+ unreadDocView + "." + idField + " = " + t0 + "." + idField + " AND " + unreadDocView + "." + empIDField +
				" = @EmpID) THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END) AS " + workDocReadField + ", " +

				" CASE WHEN EXISTS(SELECT * FROM " + buhDocumentTable + " (NOLOCK) WHERE " + idField + " = " + t0 + "." +
				idField + " AND ������� = 0" + ((personID > 0) ? " AND �������������� = @PersonID" : "") + ")" +
				" OR EXISTS(SELECT * FROM " + buhDictTable + " (NOLOCK) WHERE " + idField + " = " + t0 + "." + idField +
				" AND ������� = 0" + ((personID > 0) ? " AND �������������� = @PersonID" : "") +
				") THEN '�' ELSE '' END" +
				"+CASE WHEN EXISTS(SELECT * FROM " + buhDocumentTable + " (NOLOCK) WHERE " + idField + " = " + t0 + "." +
				idField + " AND ������� = 1" + ((personID > 0) ? " AND �������������� = @PersonID" : "") + ")" +
				" OR EXISTS(SELECT * FROM " + buhDictTable + " (NOLOCK) WHERE " + idField + " = " + t0 + "." + idField +
				" AND ������� = 1 " + ((personID > 0) ? " AND �������������� = @PersonID" : "") +
				") THEN '�' ELSE '' END " + spentField + " ";

			const string orderString = " ORDER BY " + dateField + " DESC";
			string joinStr = joinDocTypeTable.Replace(tableName, t0);
			selectString = Data.DALC.Documents.Search.Options.SelectFrom.Replace("*", selectString) + joinStr;
			sql = sql.Substring(Data.DALC.Documents.Search.Options.SelectFrom.Length);
			sql = selectString + sql;
			sql += orderString;

			return GetDataTable(sql,
								delegate(SqlCommand cmd)
								{
									AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
									if(personID > 0)
										AddParam(cmd, "@PersonID", SqlDbType.Int, personID);
								}, source);
		}

        public DataTable GetDocsByIDQuery(string idQuery, string lang)
        {
            return GetDataTable(@"DECLARE @T TABLE (������������ int)
				INSERT @T " + idQuery +
                                "\nSELECT DISTINCT " +
                                tableName + "." + idField + ", " +
                                tableName + "." + docTypeIDField + ", " +
                                (lang.StartsWith("ru") ? docTypeField : docTypeEngField) + " " + docTypeField + ", " +
                                "CASE WHEN " + nameField + " <> '' THEN " + nameField + " ELSE " +
                                (lang.StartsWith("ru") ? docTypeField : docTypeEngField) + " END " + nameField + ", " +
                                dateField + ", " +
                                numberField + ", " +
                                mainImageIDField + ", " +
                                "REPLACE(" + descriptionField + ", CHAR(13) + CHAR(10), CHAR(160) + ' ') " +
                                descriptionField +
                                "\nFROM " + tableName +
                                joinDocTypeTable +
                                " WHERE " +
                                tableName + "." + idField + " IN (SELECT ������������ FROM @T)" +
                                orderByDate, null);
        }

        public int GetDocCount(string sql)
        {
            return GetCount("SELECT COUNT(" + idField + ") AS " + countField + " FROM " + sql,
                            null);
        }

        public int FoundDocsCount()
        {
            return GetDocCount(docFoundTable);
        }

        /// <summary>
        /// ��������� ���� ��������� � ������ ������
        /// </summary>
        /// <param name="docID">��� ��������</param>
        /// <param name="path">��������� ����</param>
        /// <returns>������ �������� �� ���� � ������ �� ����������� ����</returns>
        public string GetDocPath(int docID, string path)
        {
            return Exec(
                sp_DocFullPath,
                delegate(SqlCommand cmd)
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddParam(cmd, "@Path", SqlDbType.VarChar, path);
                        AddParam(cmd, "@Kod", SqlDbType.Int, docID);
                        AddParam(cmd, "@FullPath", SqlDbType.VarChar, DBNull.Value);
                        AddParam(cmd, "@ValidKod", SqlDbType.TinyInt, DBNull.Value);
                        cmd.Parameters["@FullPath"].Direction = ParameterDirection.InputOutput;
                        cmd.Parameters["@FullPath"].Size = 2000;
                        cmd.Parameters["@ValidKod"].Direction = ParameterDirection.InputOutput;
                    },
                delegate(SqlCommand cmd)
                    {
                        return cmd.Parameters["@FullPath"].Value.ToString();
                    }, null);
        }

        public string GetFullPath(string path)
        {
            return GetDocPath(0, path);
        }

        public DataTable GetSimilarDocs(int docTypeID, string number, DateTime date, string personIDs, int docID, int searchType)
        {
            return GetDataTable(
                spSimilarDocs,
                delegate(SqlCommand cmd)
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddParam(cmd, "@����������������", SqlDbType.Int, docTypeID);
                        AddParam(cmd, "@��������������", SqlDbType.NVarChar, number);
                        AddParam(cmd, "@�������", SqlDbType.NVarChar, personIDs);
                        if (searchType >= 0)
                            AddParam(cmd, "@����������������", SqlDbType.TinyInt, (byte) searchType);

                        if (date > DateTime.MinValue)
                            AddParam(cmd, "@�������������", SqlDbType.DateTime, date.Date);
                        else
                            AddParam(cmd, "@�������������", SqlDbType.DateTime, DBNull.Value);

                        AddParam(cmd, "@������������", SqlDbType.Int, docID);
                    });
        }

        /// <summary>
        /// ������� ��������� ���� �� ���������
        /// </summary>
        /// <param name="docID">��� ���������</param>
        /// <returns>������� ����</returns>
        public DataTable GetDocRights(int docID)
        {
            return GetDataTable(
                spDocRights,
                delegate(SqlCommand cmd)
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddParam(cmd, "@������������", SqlDbType.Int, docID);
                    });
        }

        public bool IsDocAvailable(int docID)
        {
            return FieldExists(docID);
        }

        /// <summary>
        /// �������� �� ������� ����������� ������� ���������� � ���������
        /// </summary>
        /// <param name="docID">��� ���������</param>
        public int CanCreateTransaction(int docID, CancellationToken ct)
        {
            object obj = GetField("SELECT " + docTypeFinField +
                                  " FROM " + tableName + " (NOLOCK) " +
                                  joinDocTypeTable +
                                  " WHERE " + tableName + "." + idField + " = @DocID",
                                  docTypeFinField,
                                  delegate(SqlCommand cmd)
                                      {
                                          AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                                      }, ct);
            if (obj is int)
                return (int) obj;
            if (obj is byte)
                return (byte) obj;
            return 0;
        }

        public bool CheckDocAndCurrentPerson(int docID, int personID)
        {
            return GetIntField("SELECT TOP 1 " + idField +
                               " FROM " + docPersonTable +
                               " WITH (NOLOCK) WHERE " + idField + " = @DocID AND " +
                               personIDField + " = @PersonID", idField,
                               delegate(SqlCommand cmd)
                                   {
                                       AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                                       AddParam(cmd, "@PersonID", SqlDbType.Int, personID);
                                   }) > 0;
        }

        public int GetCurrentPersonFromDoc(int docID)
        {
            return GetIntField("SELECT TOP 1 " + docPersonTable + "." + personIDField +
                               " FROM " + docPersonTable + " (nolock) INNER JOIN " + personViewName + " (nolock)" +
                               " ON " + docPersonTable + "." + personIDField + " = " + personViewName + "." +
                               personIDField +
                               " WHERE " + docPersonTable + "." + idField + " = @DocID",
                               personIDField, delegate(SqlCommand cmd)
                                                  {
                                                      AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                                                  });
        }

        public string GetTitliesQuery()
        {
            return "SELECT TOP 100 " + nameField +
                   " FROM " + tableName + " (nolock) WHERE " + docTypeIDField + " = @TypeID AND " + nameField + " <> '' AND " +
                   editorField + " = @EmpID GROUP BY " + nameField + " ORDER BY MAX(" + editedField + ") DESC ";
        }

        public DataTable GetLinkDocs(string ids)
        {
            return GetDataTable("SELECT ������������, �����������������, " +
                                (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("ru")
                                     ? docTypeField
                                     : docTypeEngField) + " ������������, \n" +
                                "��������������, �������������, ��������, CASE WHEN EXISTS (SELECT * FROM vw��������������� \n" +
                                "WHERE ��������������������� = dbo.vw���������.������������ OR ����������������������� = vw���������.������������) THEN 1 ELSE 0 END ����� \n" +
                                "FROM vw��������� (NOLOCK) INNER JOIN �������������� (NOLOCK) ON vw���������.���������������� = ��������������.���������������� WHERE ������������ IN (" +
                                ids + ")",
                                null);
        }

        #endregion

        #region Change Data

        /// <summary>
        /// ���������� ������� ���������
        /// </summary>
        /// <param name="docID">��� ���������</param>
        /// <param name="typeID">��� ���� ���������</param>
        /// <param name="name">�������� ���������</param>
        /// <param name="number">����� ���������</param>
        /// <param name="date">���� ���������</param>
        /// <param name="protectedDoc">������� �� ������� ����</param>
        /// <param name="descr">�������� ���������</param>
        /// <returns>���������� ��� ���������</returns>
        public int SetDocProperties(int docID, int typeID, string name, string number, DateTime date, bool protectedDoc,
                                    string descr)
        {
            bool dateIn = (date != DateTime.MinValue);
            var sb = new StringBuilder();
            if (docID > 0)
            {

                sb.Append("UPDATE ");
                sb.Append(tableName);
                sb.Append(" SET ");
                sb.Append(docTypeIDField);
                sb.Append(" = @TypeID, ");

                sb.Append(nameField);
                sb.Append(" = @Name, ");

                // number
                sb.Append(numberField);
                sb.Append(" = @Number, ");

                // date
                sb.Append(dateField);
                sb.Append(dateIn ? " = @Date, " : " = NULL, ");

                // protected
                sb.Append(protectedField);
                sb.Append(" = @Protected, ");
                // description
                sb.Append(descriptionField);
                sb.Append(" = @Descr");
                sb.Append(" WHERE ");
                sb.Append(idField);
                sb.Append(" = @DocID");
            }
            else
            {
                sb.Append("INSERT INTO ");
                sb.Append(tableName);
                sb.Append(" (");
                sb.Append(docTypeIDField);

                sb.Append(", ");
                sb.Append(nameField);

                // number
                sb.Append(", ");
                sb.Append(numberField);

                // date
                if (dateIn)
                {
                    sb.Append(", ");
                    sb.Append(dateField);
                }

                // protected
                sb.Append(", ");
                sb.Append(protectedField);

                // description
                sb.Append(", ");
                sb.Append(descriptionField);

                sb.Append(") VALUES (@TypeID, @Name, @Number");

                // date
                if (dateIn)
                    sb.Append(", @Date");

                sb.Append(", @Protected, @Descr); SELECT SCOPE_IDENTITY() AS ");
                sb.Append(identityField);
            }

            return ExecID(sb.ToString(), delegate(SqlCommand cmd)
                                             {
                                                 AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                                                 AddParam(cmd, "@TypeID", SqlDbType.Int, typeID);
                                                 AddParam(cmd, "@Name", SqlDbType.NVarChar, name);
                                                 AddParam(cmd, "@Number", SqlDbType.NVarChar, number);

                                                 if (dateIn)
                                                     AddParam(cmd, "@Date", SqlDbType.DateTime, date);

                                                 AddParam(cmd, "@Protected", SqlDbType.TinyInt,
                                                          (protectedDoc ? (byte) 1 : (byte) 0));
                                                 AddParam(cmd, "@Descr", SqlDbType.NVarChar, descr);
                                             }, docID);


        }

        public bool SetDocPersons(int docID, int[] personIDs)
        {
            string personsStr = "";
            for (int j = 0; j < personIDs.Length; j++)
            {
                if (personsStr.Length > 0) personsStr += ", ";
                personsStr += personIDs[j].ToString();
            }
            string queryString =
                "DELETE FROM " + docPersonTable + " WHERE " + idField + " = @DocID AND " + personPositionField + " = 0";
            if (personsStr.Length > 0)
                queryString += " AND " + personIDField + " NOT IN (" + personsStr + ")" + Environment.NewLine +
                               "AND NOT EXISTS ( SELECT * FROM " + docSignatureTable + " WHERE " + docSignatureTable +
                               "." + idField + " = " + docPersonTable + "." + idField + " AND " + docSignatureTable +
                               "." + docSignatureDataFeild + " = " + docPersonTable + "." + editedField + ")";
            queryString += Environment.NewLine + Environment.NewLine;

            for (int i = 0; i < personIDs.Length; i++)
                queryString +=
                    "IF NOT EXISTS( SELECT * FROM " + docPersonTable + " (NOLOCK) WHERE " + idField + " = @DocID AND " +
                    personIDField + " = " + personIDs[i] + ")" + Environment.NewLine +
                    "INSERT INTO " + docPersonTable + " (" + idField + ", " + personIDField + ")" +
                    " VALUES (@DocID, " + personIDs[i] + ")" + Environment.NewLine;

            return Exec(queryString,
                        delegate(SqlCommand cmd)
                            {
                                AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                            });
        }

        public bool SearchDocs(string sql, bool inFound, bool addTo, int employeeID, int maxSearchResults)
        {
            string queryString = "";
            if (!inFound && !addTo) // �����
                queryString =
                    "DELETE FROM " + docFoundTable + " WHERE " + empIDField + " = @EmpID " +
                    Environment.NewLine + Environment.NewLine +
                    "INSERT INTO " + docFoundTable + " (" + idField + ") " + sql;
            else if (inFound)
                queryString =
                    "DELETE FROM " + docFoundTable +
                    " WHERE " + idField + " NOT IN (" + sql + ") AND " + empIDField + " = @EmpID";
            else if (addTo)
                queryString =
                    "INSERT INTO " + docFoundTable + " (" + idField + ")" +
                    " " + sql + " AND " + idField + " NOT IN (SELECT " + idField + " FROM " + docFoundTable + ")";

                return Exec(
    queryString,
    delegate(SqlCommand cmd)
    {
        AddParam(cmd, "@EmpID", SqlDbType.Int, employeeID);
    });
        }

        public bool DeleteFromFound(int docID, int employeeID)
        {
            return Exec(
                "DELETE FROM " + docFoundTable + " WHERE " + idField + " = @DocID AND " + empIDField + " = @EmpID ",
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                        AddParam(cmd, "@EmpID", SqlDbType.Int, employeeID);
                    });
        }

        public bool SetMainImage(int docID, int imageID)
        {
            using (
                var cmd =
                    new SqlCommand(
                        "UPDATE " + tableName + " SET " + mainImageIDField + " = @ImageID" + " WHERE " + idField +
                        " = @DocID", new SqlConnection(connectionString)))
            {
                AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                AddParam(cmd, "@ImageID", SqlDbType.Int, imageID);

                return CMD_Exec(cmd);
            }
        }

        public bool ShareArchiveFolder(string path, int empID)
        {
            using (var cmd = new SqlCommand(sp_ShareArchiveFolder, new SqlConnection(connectionString)))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                AddParam(cmd, "@Path", SqlDbType.VarChar, path);
                AddParam(cmd, "@�������������", SqlDbType.Int, empID);

                return CMD_Exec(cmd);
            }
        }

        public bool DeleteDoc(int mainDocID, int secDocID, bool delete)
        {
            try
            {
#if AdvancedLogging
                Log.Logger.EnterMethod(this, "DeleteDoc(int mainDocID, int secDocID, bool delete) secDocID= " + secDocID + " ;mainDocID= " + mainDocID);
#endif

                using (var cmd = new SqlCommand(sp_DeleteDoc, new SqlConnection(connectionString)))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    AddParam(cmd, "@����������������������", SqlDbType.Int, secDocID);
                    AddParam(cmd, "@�����������������������", SqlDbType.Int, mainDocID);
                    AddParam(cmd, "@���������", SqlDbType.Bit, (delete ? 1 : 0));

                    return CMD_Exec(cmd, false, 0, null);
                }
            }
            finally
            {
#if AdvancedLogging
                Log.Logger.LeaveMethod(this, "DeleteDoc(int mainDocID, int secDocID, bool delete) secDocID= " + secDocID + " ;mainDocID= " + mainDocID);
#endif
            }
        }

        #endregion

		public Dictionary<int, int> GetMainImages(string ids)
		{
			return GetRecords<KeyValuePair<int, int>>("SELECT dbo.vw���������.������������, dbo.vw���������.�������������������������������� ��������������, dbo.�����������������.���������� " +
				" FROM dbo.vw��������� LEFT OUTER JOIN dbo.����������������� ON dbo.vw���������.�������������������������������� = dbo.�����������������.����������������������� AND " +
				" dbo.vw���������.������������ = dbo.�����������������.������������������� AND dbo.�����������������.���������� = 101" +
				" WHERE dbo.vw���������.������������ IN (" + ids + ")", null, delegate(IDataRecord dr)
				{
					if(!dr.IsDBNull(1) && dr.IsDBNull(2))
						return new KeyValuePair<int, int>(dr.GetInt32(0), dr.GetInt32(1));
					else
						if(dr.IsDBNull(1))
							return new KeyValuePair<int, int>(dr.GetInt32(0), 0);
						else
							if(101.Equals(dr.IsDBNull(2)))
								return new KeyValuePair<int, int>(dr.GetInt32(0), -1);
					return null;
				}).ToDictionary(x => x.Key, x => x.Value);
		}
	}
}