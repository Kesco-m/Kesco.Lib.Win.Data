using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Kesco.Lib.Win.Data.DALC.Corporate;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    /// <summary>
    /// DAL-��������� ��� ������� � ������ ���������.dbo.vw����������������.
    /// </summary>
    public class WorkDocDALC : DALC
    {
        private readonly string docIDField;
        private readonly string empIDField;
        private readonly string folderIDField;
        private const string readTimeField = "��������������";
        private const string readField = "��������";

        private readonly DocumentDALC docData;
        private readonly FolderDALC folderData;
        private readonly EmployeeDALC employeeData;

        public WorkDocDALC(string connectionString)
            : base(connectionString)
        {
            tableName = "���������.dbo.vw����������������";

            idField = "�������������������";
            nameField = null;

            docData = new DocumentDALC(null);
            docIDField = docData.IDField;

            employeeData = new EmployeeDALC(null);
            empIDField = employeeData.IDField;

            folderData = new FolderDALC(null);
            folderIDField = folderData.IDField;
        }

        #region Accessors

        public string ReadField
        {
            get { return readField; }
        }

        public string DocIDField
        {
            get { return docIDField; }
        }

        public string EmpIDField
        {
            get { return empIDField; }
        }

        public string ReadTimeField
        {
            get { return readTimeField; }
        }

        #endregion

        #region Get Data

        /// <summary>
        /// ������ � ��������� ��������� �����������
        /// </summary>
        /// <param name="docID">��� ���������</param>
        /// <param name="empID">��� ����������</param>
        /// <returns></returns>
        public bool IsRead(int docID, int empID)
        {
            return GetIntField("SELECT MIN(CONVERT(int, " + readField + ")) AS " + readField +
                               " FROM " + tableName +
                               " WHERE " + docIDField + " = @DocID AND " + empIDField + " = @EmpID",
                               readField, delegate(SqlCommand cmd)
                                              {
                                                  AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                                                  AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                                              }) > 0;
        }

        public bool IsInWork(int docID, int empID)
        {
            return FieldExists(
                " WHERE " + docIDField + " = @DocID AND " + empIDField + " = @EmpID",
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                        AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                    });
        }

        public bool IsInWork(string docIDs, int empID)
        {
            return FieldExists(
                " WHERE " + docIDField + " in (" + docIDs + ") AND " + empIDField + " = @EmpID",
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                    });
        }

        public object DocPresentInWorkFolders(int docID, int empID)
        {
            return GetField(
                "SELECT " + folderIDField +
                " FROM " + tableName +
                " WHERE " + docIDField + " = @DocID AND " + empIDField + " = @EmpID",
                folderIDField,
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                        AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                    });
        }

        public bool DocPresentInFolder(int docID, int folderID, int empID)
        {
            return FieldExists(
                " WHERE " + docIDField + " = @DocID" +
                " AND " + empIDField + " = @EmpID" +
                " AND " + folderIDField,
                delegate(SqlCommand cmd)
                    {
                        if (folderID != 0)
                            cmd.CommandText += " = @FolderID";
                        else
                            cmd.CommandText += " IS NULL";

                        if (folderID != 0)
                            AddParam(cmd, "@FolderID", SqlDbType.Int, folderID);

                        AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                        AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                    });
        }


        public int DocPresentInFolder(string docIDs, int folderID, int empID)
        {
            return GetCount("SELECT COUNT(" + docIDField + ") as " + CountField +
                            " FROM " + tableName + " WITH (NOLOCK) " +
                            " WHERE " + docIDField + " in (" + docIDs + ") AND " + empIDField + " = @EmpID" +
                            " AND " + folderIDField + (folderID != 0 ? " = @FolderID" : " IS NULL"),
                            delegate(SqlCommand cmd)
                                {
                                    if (folderID != 0)
                                        AddParam(cmd, "@FolderID", SqlDbType.Int, folderID);

                                    AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                                });
        }

        public bool DocAvailableToEmployee(int docID, int empID)
        {
            return FieldExists("WHERE " + docIDField + " = @DocID" +
                               " AND " + empIDField + " = @EmpID" +
                               " AND " + folderIDField + " IS NULL",
                               delegate(SqlCommand cmd)
                                   {
                                       AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                                       AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                                   });
        }

        public DataSet GetDocWorkFolders(int docID)
        {
            return GetData(
                "SELECT " + folderIDField + ", " + empIDField +
                " FROM " + tableName +
                " WHERE " + docIDField + " = @DocID",
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                    });
        }

        public DataTable GetDocWorkFolders(int docID, int empID)
        {
            return GetDataTable(
                "SELECT " + folderIDField +
                " FROM " + tableName +
                " WHERE " + docIDField + " = @DocID AND " + empIDField + " = @EmpID",
                delegate(SqlCommand cmd)
                {
                    AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                    AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                });
        }

        #endregion

        #region MarkAsRead

        /// <summary>
        /// ���������� ���������� � ���������� ���� ��������� ��� ���������� ���������.
        /// ������������ ��� ��������� ���������� ����� ������ ����������.
        /// </summary>
        /// <param name="receiverID">��� ����������-����������</param>
        /// <param name="docID">��� ���������</param>
        /// <param name="unreadOnly">���� TRUE, �� �������� ������ ���� �� ����������� ���������</param>
        /// <param name="minMessID">������������ ���������� ��� ���������</param>
        /// <param name="maxMessID">������������ ���������� ��� ���������</param>
        public bool GetDocMessagesIDs(int receiverID, int docID, bool unreadOnly, out int minMessID, out int maxMessID)
        {
            minMessID = 0;
            maxMessID = 0;
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("sp_���������MinMax�����������������������", conn))
            {
                try
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = cmdTimeout;

                    AddParam(cmd, "@������������", SqlDbType.Int, docID);
                    AddParam(cmd, "@�����������������������", SqlDbType.Int, receiverID);
                    AddParam(cmd, "@�������������������", SqlDbType.Int, (unreadOnly ? 1 : 0));

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            minMessID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            maxMessID = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                        }
                        reader.Close();

                        return minMessID > 0 && maxMessID > 0;
                    }
                }
                catch (SqlException sqlEx)
                {
                    ProcessSqlEx(sqlEx, cmd);
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                }
                finally
                {
                    conn.Close();
                }
                return false;
            }
        }

        /// <summary>
        /// �������� ������� ��������� �� ��������� ��� ��������� ��� �� ���������.
        /// ����� �������� ��� ��������� ����� ���������� ���������� � ���������� ������, ������������.
        /// </summary>
        /// <param name="empID">��� ����������-����������</param>
        /// <param name="minMessID">���������� ��� ���������</param>
        /// <param name="maxMessID">���������� ��� ���������</param>
        /// <param name="isRead">�������� ��� ������� ��������� ������������ ��� �� ������������</param>
        /// <returns>���������� True, ���� �������� ����������� �������, � False � ��������� ������</returns>
        public bool MarkAsRead(int empID, int minMessID, int maxMessID, bool isRead)
        {
            if (minMessID == 0 && maxMessID == 0)
                return false;
            using (var cmd = new SqlCommand("sp_������������������", new SqlConnection(connectionString)))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                AddParam(cmd, "@�����������������������", SqlDbType.Int, empID);
                AddParam(cmd, "@����������������������", SqlDbType.Int, minMessID);
                AddParam(cmd, "@����������������������", SqlDbType.Int, maxMessID);
                AddParam(cmd, "@���������", SqlDbType.TinyInt, (isRead ? 1 : 0));

                return CMD_Exec(cmd);
            }
        }

        public bool MarkAsRead(int empID, int docID)
        {
            int minMessID;
            int maxMessID;

            if (GetDocMessagesIDs(empID, docID, true, out minMessID, out maxMessID))
                using (var cmd = new SqlCommand("sp_������������������", new SqlConnection(connectionString)))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    AddParam(cmd, "@�����������������������", SqlDbType.Int, empID);
                    AddParam(cmd, "@����������������������", SqlDbType.Int, minMessID);
                    AddParam(cmd, "@����������������������", SqlDbType.Int, maxMessID);
                    AddParam(cmd, "@���������", SqlDbType.TinyInt, 1);

                    return CMD_Exec(cmd);
                }

            return false;
        }

        #endregion

        #region AddToWork

        public int[] AddDocToWorkFolder(int[] docIDs, int folderID, int empID)
        {
            if (docIDs == null || docIDs.Length == 0 || empID < 1)
                return docIDs;
            var result = new int[docIDs.Length];
            using (var cmd = new SqlCommand(
                "IF NOT EXISTS (SELECT * FROM " + tableName + " (nolock) WHERE " + 
                docIDField + " = @������������ AND " + empIDField + " = @������������� AND " + folderIDField + (folderID > 0 ? " = @������������������" : " IS NULL") + ")" +
                
                " INSERT INTO " + tableName + " (" + empIDField + ", " + folderIDField + ", " + docIDField + ")" +
                " SELECT @�������������, " + (folderID > 0 ? "@������������������" : "NULL") + ", @������������"
                , new SqlConnection(connectionString)))
            {
                try
                {
                    cmd.CommandTimeout = cmdTimeout;
                    cmd.Connection.Open();

                    for (int i = 0; i < docIDs.Length; i++)
                    {
                        cmd.Parameters.Clear();
                        AddParam(cmd, "@�������������", SqlDbType.Int, empID);
                        AddParam(cmd, "@������������", SqlDbType.Int, docIDs[i]);
                        if (folderID > 0)
                            AddParam(cmd, "@������������������", SqlDbType.Int, folderID);

                        result[i] = cmd.ExecuteNonQuery();
                    }
                }
                catch (SqlException sqlEx)
                {
                    ProcessSqlEx(sqlEx, cmd);
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                }
                finally
                {
                    cmd.Connection.Close();
                }
                return result;
            }
        }

        public bool AddDocToEmployee(int docID, int empID)
        {
            return AddDocToWorkFolder(new[] {docID}, 0, empID)[0] != 1;
        }

        public bool AddDocToWorkFolder(int docID, int folderID, int empID)
        {
            return AddDocToWorkFolder(new[] {docID}, folderID, empID)[0] != 1;
        }

        public bool AddDocToWorkFolder(int docID, int[] folderIDs, int empID)
        {
            if (folderIDs == null || folderIDs.Length == 0 || empID < 1 || docID < 1)
                return false;

            return folderIDs.Aggregate(true, (current, t) => current && AddDocToWorkFolder(docID, t, empID));
        }

        #endregion

        #region RemoveFromWork

        public bool RemoveDocFromEmployee(int docID, int empID)
        {
            return RemoveDocFromEmployee(docID, empID, true);
        }

        public bool RemoveDocFromEmployee(int docID, int empID, bool complete)
        {
            return Exec("DELETE FROM " + tableName +
                        " WHERE " +
                        empIDField + " = @EmpID" +
                        " AND " +
                        docIDField + " = @DocID",
                        delegate(SqlCommand cmd)
                            {
                                if (!complete)
                                    cmd.CommandText += " AND " + folderIDField + " IS NULL";

                                AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                                AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                            });
        }

        public bool RemoveDocFromEmployee(string docIDs, int empID, bool complete)
        {
            return Exec("DELETE FROM " + tableName +
                        " WHERE " +
                        empIDField + " = @EmpID" +
                        " AND " +
                        docIDField + " in (" + docIDs + ")",
                        delegate(SqlCommand cmd)
                            {
                                if (!complete)
                                    cmd.CommandText += " AND " + folderIDField + " IS NULL";

                                AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                            });
        }

        public bool RemoveDocFromEmployee(ArrayList docIDs, int empID)
        {
            if (docIDs.Count > 0)
            {
                string idString = string.Join(",", docIDs.Cast<int>().Select(id => id.ToString()).ToArray());
                if (idString.Length > 0)
                {
                    return Exec("DELETE FROM " + tableName +
                                " WHERE " +
                                empIDField + " = @EmpID" +
                                " AND " +
                                docIDField + " IN (" + idString + ")" +
                                " AND " + folderIDField + " IS NULL",
                                delegate(SqlCommand cmd)
                                    {
                                        AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                                    });
                }
            }

            return false;
        }

        public bool RemoveDocFromWorkFolder(object[] args, bool complete)
        {
            if (args.Length == 3 && args[2] is int)
            {
                if (args[1] is int)
                {
                    if (args[0] is int)
                        return RemoveDocFromWorkFolder((int) args[0], (int) args[1], (int) args[2], complete);
                    if (args[0] is int[])
                    {
                        var docIDs = (int[]) args[0];
                        return RemoveDocFromWorkFolder(string.Join(",", docIDs.Select(id => id.ToString()).ToArray()), (int)args[1], (int)args[2], complete);
                    }
                }
                else
                {
                    if (args[1] is string)
                    {
                        var folderIDs = args[1] as string;
                        if (folderIDs.Length == 0) return false;
                        if (folderIDs.StartsWith("0,"))
                        {
                            folderIDs = folderIDs.Substring(2, folderIDs.Length - 2);
                            if (!RemoveDocFromEmployee((int) args[0], (int) args[2])) return false;
                        }
                        else if (folderIDs.EndsWith(",0"))
                        {
                            folderIDs = folderIDs.Substring(0, folderIDs.Length - 2);
                            if (!RemoveDocFromEmployee((int) args[0], (int) args[2])) return false;
                        }
                        else if (folderIDs.IndexOf(",0,") > 0)
                        {
                            folderIDs = folderIDs.Replace(",0,", ",");
                            if (!RemoveDocFromEmployee((int) args[0], (int) args[2])) return false;
                        }

                        if (args[0] is int)
                        {
                            if (folderIDs.Length == 0) return true;
                            return Exec("DELETE FROM " + tableName +
                                        " WHERE " + folderIDField + " IN (" + folderIDs + ")" +
                                        " AND " + docIDField + " = @DocID" +
                                        " AND " + empIDField + " = @EmpID",
                                        delegate(SqlCommand cmd)
                                            {
                                                AddParam(cmd, "@DocID", SqlDbType.Int, (int) args[0]);
                                                AddParam(cmd, "@EmpID", SqlDbType.Int, (int) args[2]);
                                            });
                        }
                        else if (args[0] is int[])
                        {
                            var docIDs = (int[]) args[0];

                            return Exec("DELETE FROM " + tableName +
                                        " WHERE " + folderIDField + " IN (" + folderIDs + ")" +
                                        " AND " + docIDField + " IN (" +
                                        string.Join(",", docIDs.Select(id => id.ToString()).ToArray()) + ")" +
                                        " AND " + empIDField + " = @EmpID",
                                        delegate(SqlCommand cmd)
                                            {
                                                AddParam(cmd, "@EmpID", SqlDbType.Int, (int) args[2]);
                                            });
                        }
                    }
                }
            }
            return false;
        }

        public bool RemoveDocsFromWorkFolder(int folderID)
        {
            if (folderID == 0)
                return false;
            
            return Exec("DELETE FROM " + tableName +
                        " WHERE " + folderIDField + " = @FolderID",
                        delegate(SqlCommand cmd)
                            {
                                AddParam(cmd, "@FolderID", SqlDbType.Int, folderID);
                            });
        }

        public bool RemoveDocFromWorkFolder(int docID, int folderID, int empID, bool complete)
        {
            if (folderID == 0)
                return RemoveDocFromEmployee(docID, empID, complete);
            
            return Exec("DELETE FROM " + tableName +
                        " WHERE " + folderIDField + " = @FolderID" +
                        " AND " + docIDField + " = @DocID" +
                        " AND " + empIDField + " = @EmpID",
                        delegate(SqlCommand cmd)
                            {
                                AddParam(cmd, "@FolderID", SqlDbType.Int, folderID);
                                AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                                AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                            });
        }

        public bool RemoveDocFromWorkFolder(string docIDs, int folderID, int empID, bool complete)
        {
            if (folderID == 0)
                return RemoveDocFromEmployee(docIDs, empID, complete);
            
            return Exec("DELETE FROM " + tableName +
                        " WHERE " + folderIDField + " = @FolderID" +
                        " AND " + docIDField + " in (" + docIDs + ") " +
                        " AND " + empIDField + " = @EmpID",
                        delegate(SqlCommand cmd)
                            {
                                AddParam(cmd, "@FolderID", SqlDbType.Int, folderID);
                                AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                            });
        }

        public bool RemoveDocFromWorkFolder(int[] docIDs, int folderID, int empID)
        {
            if (docIDs == null || docIDs.Length == 0 || empID < 1)
                return false;

            bool result = true;
            using (var cmd = new SqlCommand(
                "DELETE FROM " + tableName +
                " WHERE " + empIDField + " = @EmpID" +
                " AND " + folderIDField + (folderID > 0 ? " = @FolderID" : " IS NULL") +
                " AND " + docIDField + " = @DocID"
                ,
                new SqlConnection(connectionString)))
            {
                try
                {
                    cmd.CommandTimeout = cmdTimeout;
                    cmd.Connection.Open();

                    for (int i = 0; i < docIDs.Length; i++)
                    {
                        cmd.Parameters.Clear();
                        if (folderID > 0)
                            AddParam(cmd, "@FolderID", SqlDbType.Int, folderID);
                        AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                        AddParam(cmd, "@DocID", SqlDbType.Int, docIDs[i]);

                        result = result && cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (SqlException sqlEx)
                {
                    ProcessSqlEx(sqlEx, cmd);
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                }
                finally
                {
                    cmd.Connection.Close();
                }
            }
            return result;
        }

        public bool RemoveDocFromWork(int docID, int empID)
        {
            return Exec("DELETE FROM " + tableName +
                        " WHERE " +
                        empIDField + " = @EmpID" +
                        " AND " +
                        docIDField + " = @DocID",
                        delegate(SqlCommand cmd)
                            {
                                AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                                AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                            });
        }

        public bool RemoveDocFromWork(int[] docIDs, int empID)
        {
            if (docIDs == null || docIDs.Length == 0 || empID < 1)
                return false;
            bool result = true;
            using (var cmd = new SqlCommand(
                "DELETE FROM " + tableName +
                    " WHERE " +
                    empIDField + " = @EmpID" +
                    " AND " +
                    docIDField + " = @DocID"
                ,
                new SqlConnection(connectionString)))
            {
                try
                {
                    cmd.CommandTimeout = cmdTimeout;
                    cmd.Connection.Open();

                    for (int i = 0; i < docIDs.Length; i++)
                    {
                        cmd.Parameters.Clear();
                        AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                        AddParam(cmd, "@DocID", SqlDbType.Int, docIDs[i]);

                        result = result && cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (SqlException sqlEx)
                {
                    ProcessSqlEx(sqlEx, cmd);
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                }
                finally
                {
                    cmd.Connection.Close();
                }
                return result;
            }
        }

        #endregion
    }
}