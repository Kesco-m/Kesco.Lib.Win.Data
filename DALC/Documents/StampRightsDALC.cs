using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Kesco.Lib.Win.Data.DALC.Corporate;
using Kesco.Lib.Win.Data.Temp.Objects;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    /// <summary>
    /// Компонент доступа к правам на штамп (факсимиле).
    /// </summary>
    public class StampRightsDALC : DALC
    {
        private string employeeIDField;
        private string giveRightsField;

        public StampRightsDALC(string connectionString)
            : base(connectionString)
        {
            tableName = "vwПраваШтампы";
            idField = "КодШтампа";
            giveRightsField = "ДаватьПраваЗамещающим";
            var data = new EmployeeDALC(null);
            employeeIDField = data.IDField;
        }

        #region Get Data

        public List<StampRight> GetStampRights(int stampId)
        {
            return
                GetRecords<StampRight>("SELECT П.КодСотрудника, С.ФИО, П.ДаватьПраваЗамещающим FROM vwПраваШтампы П " +
                                       "JOIN Инвентаризация..Сотрудники С ON С.КодСотрудника=П.КодСотрудника WHERE П." +
                                       idField + "=@StampId", delegate(SqlCommand cmd)
                                                                  {
                                                                      AddParam(cmd, "@StampId", SqlDbType.Int, stampId);
                                                                  }, delegate(IDataRecord dr)
                                                                         {
                                                                             return new StampRight
                                                                                        {
                                                                                            UserId = (int)dr["КодСотрудника"],
                                                                                            UserName = dr["ФИО"].ToString(),
                                                                                            EnableProxies =(bool)dr["ДаватьПраваЗамещающим"]
                                                                                        };
                                                                         });
        }

        #endregion

        #region Set Data

        public bool SetStampRights(int stampId, List<StampRight> rights)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("DELETE " + tableName + " WHERE " + idField + "=@Id", conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = stampId;
                conn.Open();
                using (cmd.Transaction = conn.BeginTransaction())
                {
                    try
                    {
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "INSERT " + tableName + " (" + idField + ", " + employeeIDField + ", " +
                                          giveRightsField + ") VALUES(@Id, @UserId, @EnableProxies)";
                        SqlParameter paramUserId = cmd.Parameters.Add("@UserId", SqlDbType.Int);
                        SqlParameter paramEnableProxies = cmd.Parameters.Add("@EnableProxies", SqlDbType.Bit);

                        foreach (StampRight right in rights)
                        {
                            paramUserId.Value = right.UserId;
                            paramEnableProxies.Value = right.EnableProxies ? 1 : 0;
                            cmd.ExecuteNonQuery();
                        }

                        cmd.Transaction.Commit();
                        return true;
                    }
                    catch (SqlException sqlEx)
                    {
                        if (cmd.Transaction != null)
                            cmd.Transaction.Rollback();
                        ProcessSqlEx(sqlEx, cmd, "StampRightsDALC", false);
                    }
                    catch (InvalidOperationException ioEx)
                    {
                        if (cmd.Transaction != null)
                            cmd.Transaction.Rollback();
                        ErrorMessage(false, ioEx, null, "StampRightsDALC", false);
                    }
                    catch (Exception ex)
                    {
                        if (cmd.Transaction != null)
                            cmd.Transaction.Rollback();
                        ErrorMessage(false, ex, null, "StampRightsDALC");
                    }
                    finally
                    {
                        cmd.Connection.Close();
                    }
                }
            }
            return false;
        }

        #endregion
    }
}
