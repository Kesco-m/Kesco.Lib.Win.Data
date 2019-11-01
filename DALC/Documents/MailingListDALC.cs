using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Kesco.Lib.Win.Data.DALC.Corporate;
using Kesco.Lib.Win.Data.Temp.Objects;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    public class MailingListDALC : DALC
    {
        private string empIDField;
        private string empTable;

        private const string mailingListEmpView = "���������.dbo.vw������������������������";
        private const string mailingListEmpViewAll = "���������.dbo.vw���������������������������";

        private const string tableAllName = "���������.dbo.vw�����������������";

        public MailingListDALC(string connectionString) : base(connectionString)
        {
            tableName = "���������.dbo.vw��������������";

            idField = "�����������������";
            nameField = "��������������";

            var employeeData = new EmployeeDALC(null);
            empIDField = employeeData.IDField;
            empTable = employeeData.TableName;
        }

        #region Get Data

        public List<MailingListItem> GetMailingListsEx(int curEmpID)
        {
            return GetRecords<MailingListItem>
                (
                    "SELECT " +
                    idField + ", " +
                    nameField + ", " +
                    "�����, " +
                    "Edit " +
                    " FROM " + tableAllName +
                    " ORDER BY " + nameField, null,
                    delegate(IDataRecord dr)
                        {
                            var id = (int) dr[idField];
                            var name = (string) dr[nameField];
                            var author = (string) dr["�����"];
                            var edit = (int) dr["Edit"];

                            return new MailingListItem(id, name, this, author) {Editable = edit != 0};
                        }
                );
        }

        public Employee GetMailingListEmp(int id)
        {
            return GetRecord<Employee>(
                "SELECT " +
                empTable + ".* " +
                " FROM " + tableName + " INNER JOIN " + empTable + " ON " + tableName + "." + empIDField + " = " +
                empTable + "." + empIDField +
                " WHERE " + idField + " = @ID",
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@ID", SqlDbType.Int, id);
                    },
                delegate(IDataRecord dr)
                    {
                        return new Employee((int) dr[empIDField], (string) dr["���"], (string) dr["���������"],
                                            (string) dr["IOF"], (string) dr["Employee"],
                                            new EmployeeDALC(connectionString));
                    });
        }

        public List<Employee> GetMailingListEmps(int id)
        {
            return GetEmployees(mailingListEmpViewAll, id);
        }

        public List<Employee> GetMailingListSharedEmps(int id)
        {
            return GetEmployees("vw�������������������", id);
        }

        private List<Employee> GetEmployees(string tableName, int id)
        {
            return GetRecords<Employee>(
                "SELECT �.�������������, �.���������, �.���, �.Employee, �.IOF" +
                " FROM " + tableName + " t INNER JOIN " +
                empTable + " � ON t." + empIDField + " = �." + empIDField +
                " WHERE t.����������������� = @ID",
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@ID", SqlDbType.Int, id);
                    },
                delegate(IDataRecord dr)
                    {
                        var empID = (int) dr[empIDField];
                        var fullName = (string) dr["���������"];
                        var shortName = (string) dr["���"];
                        var fullNameEn = (string) dr["Employee"];
                        var shortNameEn = (string) dr["IOF"];

                        // ����� ����� - �� ����, ��� ����� �� �� ������ �����������
                        return new Employee(empID, shortName, fullName, shortNameEn, fullNameEn,
                                            new EmployeeDALC(connectionString));
                    });
        }

        #endregion

        #region Change Data

        public bool SaveMailingList(MailingListItem ml)
        {
            using (var cn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandTimeout = cmdTimeout;

                    cn.Open();
                    SqlTransaction trans = cn.BeginTransaction();
                    cmd.Transaction = trans;

                    try
                    {
                        // renaming
                        cmd.CommandText =
                            "UPDATE " + tableName +
                            " SET " + nameField + " = @Name" +
                            " WHERE " + idField + " = @ID";

                        AddParam(cmd, "@Name", SqlDbType.NVarChar, ml.Name);
                        AddParam(cmd, "@ID", SqlDbType.Int, ml.ID);

                        if (cmd.ExecuteNonQuery() == 0)
                            throw new Exception("������ �������������� ������ �������� (ID = " + ml.ID + ")");

                        if (OverwriteEmployees(cmd, ml.ID, ml.Employees) == 0)
                            throw new Exception("������ ���������� ����������� ������ �������� (ID = " + ml.ID + ")");

                        trans.Commit();

                        return true;
                    }
                    catch (SqlException sex)
                    {
                        trans.Rollback();
                        ProcessSqlEx(sex, cmd, "SaveMailingList", true);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        ErrorMessage(true, ex, null, "SaveMailingList");
                    }
                    finally
                    {
                        cn.Close();
                    }
                    return false;
                }
            }
        }

        public MailingListItem SaveMailingList(string name, List<Employee> emps)
        {
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand())
            {
                cn.Open();
                cmd.Connection = cn;
                cmd.CommandTimeout = cmdTimeout;

                using (SqlTransaction trans = cn.BeginTransaction())
                {
                    cmd.Transaction = trans;

                    try
                    {
                        // inserting
                        cmd.CommandText =
                            "INSERT " + tableName +
                            " (" + nameField + ") VALUES (@Name);" +
                            " SELECT SCOPE_IDENTITY() AS " + identityField;

                        AddParam(cmd, "@Name", SqlDbType.NVarChar, name);

                        int id = 0;
                        object obj = cmd.ExecuteScalar();
                        if (obj is decimal)
                            id = (int) (decimal) obj;

                        if (id == 0)
                            throw new Exception("������ �������� ������ ��������");

                        if (OverwriteEmployees(cmd, id, emps) == 0)
                            throw new Exception("������ ���������� ����������� ������ �������� (ID = " + id + ")");

                        trans.Commit();

                        return new MailingListItem(id, name, emps, this);
                    }
                    catch (SqlException sex)
                    {
                        trans.Rollback();
                        ProcessSqlEx(sex, cmd, "SaveMailingList", true);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        ErrorMessage(true, ex, null, "SaveMailingList");
                    }
                    finally
                    {
                        cn.Close();
                    }

                }
            }
            return null;
        }

        private int OverwriteEmployees(SqlCommand cmd, int id, List<Employee> emps)
        {
            cmd.CommandText = "DELETE " + mailingListEmpView + " WHERE " + idField + " = @ID";
            for (int i = 0; i < emps.Count; i++)
                cmd.CommandText +=
                    Environment.NewLine +
                    "INSERT " + mailingListEmpView +
                    " (" + idField + ", " + empIDField + ") VALUES (@ID, @EmpID" + i + ")";

            AddParam(cmd, "@ID", SqlDbType.Int, id);
            for (int i = 0; i < emps.Count; i++)
                AddParam(cmd, "@EmpID" + i, SqlDbType.Int, emps[i].ID);

            return cmd.ExecuteNonQuery();
        }

        public bool SaveMailingListSharing(MailingListItem ml)
        {
            using (var cn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandTimeout = cmdTimeout;

                    cn.Open();
                    SqlTransaction trans = cn.BeginTransaction();
                    cmd.Transaction = trans;

                    try
                    {
                        // delete old
                        cmd.CommandText = "DELETE FROM vw������������������� WHERE ����������������� = @ID";
                        AddParam(cmd, "@ID", SqlDbType.Int, ml.ID);

                        cmd.ExecuteNonQuery();

                        if (ml.SharedEmploees.Count != 0)
                        {

                            cmd.CommandText = string.Empty;

                            for (int idx = 0; idx < ml.SharedEmploees.Count; idx++)
                            {
                                Employee emp = ml.SharedEmploees[idx];
                                cmd.CommandText +=
                                    string.Format(
                                        "INSERT INTO vw������������������� (�����������������, �������������) VALUES(@ID, @Emp{0}){1}",
                                        idx, Environment.NewLine);
                                AddParam(cmd, string.Format("@Emp{0}", idx), SqlDbType.Int, emp.ID);
                            }
                            cmd.ExecuteNonQuery();
                        }

                        trans.Commit();

                        return true;
                    }
                    catch (SqlException sex)
                    {
                        trans.Rollback();
                        ProcessSqlEx(sex, cmd, "SaveMailingListSharing", true);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        ErrorMessage(true, ex, null, "SaveMailingListSharing");
                    }
                    finally
                    {
                        cn.Close();
                    }
                    return false;
                }
            }
        }

        #endregion

        public MailingListItem CreateMailingList(List<Employee> emps)
        {
            return new MailingListItem(0, null, emps, this);
        }
    }
}