using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Kesco.Lib.Win.Data.Temp.Objects;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    /// <summary>
    /// Компонент доступа к правилам на штамп (факсимиле).
    /// </summary>
    public class StampChecksDALC : DALC
    {
        public StampChecksDALC(string connectionString)
            : base(connectionString)
        {
            tableName = "ПраваНаУстановкуШтампов";
            idField = "КодШтампа";
        }

        #region Get Data

        public List<StampRule> GetStampRules(int stampId, string lang)
        {
            return GetRecords<StampRule>(
                "SELECT П.КодПраваНаУстановкуШтампов, П.КодСотрудника, " + (lang == "ru" ? "С.ФИО" : "С.IOF ФИО") +
                ", ISNULL(П.КодТипаДокумента, -1) AS КодТипаДокумента, " +
                (lang == "ru" ? "Д.ТипДокумента" : "Д.TypeDoc ТипДокумента") +
                ", ISNULL(П.КодЛица, -1) AS КодЛица, Кличка " +
                "FROM " + tableName + " П " +
                "JOIN Инвентаризация..Сотрудники С ON С.КодСотрудника=П.КодСотрудника " +
                "LEFT JOIN ТипыДокументов Д ON П.КодТипаДокумента = Д.КодТипаДокумента " +
                "LEFT JOIN Справочники..vwЛица Л ON П.КодЛица = Л.КодЛица " +
                "WHERE П." + idField + "=@StampId", delegate(SqlCommand cmd)
                                                        {
                                                            AddParam(cmd, "@StampId", SqlDbType.Int, stampId);
                                                        }, delegate(IDataRecord dr)
                                                               {
                                                                   return new StampRule
                                                                              {
                                                                                  RuleId = (int)dr["КодПраваНаУстановкуШтампов"],
                                                                                  UserId = (int)dr["КодСотрудника"],
                                                                                  UserName = dr["ФИО"].ToString(),
                                                                                  DocTypeID = (int)dr["КодТипаДокумента"],
                                                                                  DocTypeName = dr["ТипДокумента"].ToString(),
                                                                                  OrganizationID = (int)dr["КодЛица"],
                                                                                  OrganizationName = dr["Кличка"].ToString()
                                                                              };
                                                               });
        }

        #endregion

        #region Set Data

        public bool SetStampRules(int[] delIDs, int stampId, StampRule rule)
        {
            bool ret = false;

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                conn.Open();
                cmd.Transaction = conn.BeginTransaction();
                try
                {
                    if (delIDs != null && delIDs.Length > 0)
                    {
                        cmd.CommandText = "DELETE ПраваНаУстановкуШтампов WHERE КодПраваНаУстановкуШтампов IN (" +
                                          string.Join(", ", delIDs.Select(i => i.ToString()).ToArray()) + ")";
                        cmd.ExecuteNonQuery();
                    }

                    if (stampId > 0 && rule != null)
                    {
                        cmd.CommandText = "INSERT ПраваНаУстановкуШтампов(" + idField +
                                          ", КодСотрудника, КодТипаДокумента, КодЛица) VALUES(@Id, @UserId, @DocType, @PersId)";

                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = stampId;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = rule.UserId;
                        SqlParameter paramDocType = cmd.Parameters.Add("@DocType", SqlDbType.Int);
                        SqlParameter paramPersId = cmd.Parameters.Add("@PersId", SqlDbType.Int);

                        if (rule.DocTypeID > 0)
                            paramDocType.Value = rule.DocTypeID;
                        else
                            paramDocType.Value = DBNull.Value;
                        if (rule.OrganizationID > 0)
                            paramPersId.Value = rule.OrganizationID;
                        else
                            paramPersId.Value = DBNull.Value;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.Transaction.Commit();
                    ret = true;
                }
                catch (SqlException sqlEx)
                {
                    if (cmd.Transaction != null)
                        cmd.Transaction.Rollback();
                    ProcessSqlEx(sqlEx, cmd, "StampChecksDALC", false);
                }
                catch (InvalidOperationException ioEx)
                {
                    if (cmd.Transaction != null)
                        cmd.Transaction.Rollback();
                    ErrorMessage(false, ioEx, null, "StampChecksDALC", false);
                }
                catch (Exception ex)
                {
                    if (cmd.Transaction != null)
                        cmd.Transaction.Rollback();
                    ErrorMessage(false, ex, null, "StampChecksDALC");
                }
                finally
                {
                    cmd.Connection.Close();
                }
            }
            return ret;
        }

        public bool UpdStampRules(int[] delIDs, StampRule rule)
        {
            bool ret = false;

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                conn.Open();
                cmd.Transaction = conn.BeginTransaction();
                try
                {
                    if (delIDs != null && delIDs.Length > 0)
                    {
                        cmd.CommandText = "DELETE ПраваНаУстановкуШтампов WHERE КодПраваНаУстановкуШтампов IN (" +
                                          string.Join(", ", delIDs.Select(i => i.ToString()).ToArray()) + ")";
                        cmd.ExecuteNonQuery();
                    }

                    if (rule != null)
                    {
                        cmd.CommandText =
                            "UPDATE ПраваНаУстановкуШтампов SET КодСотрудника = @UserId, КодТипаДокумента = @DocType, КодЛица = @PersId WHERE КодПраваНаУстановкуШтампов = @RuleId";

                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = rule.UserId;
                        SqlParameter paramDocType = cmd.Parameters.Add("@DocType", SqlDbType.Int);
                        SqlParameter paramPersId = cmd.Parameters.Add("@PersId", SqlDbType.Int);
                        cmd.Parameters.Add("@RuleId", SqlDbType.Int).Value = rule.RuleId;

                        if (rule.DocTypeID > 0)
                            paramDocType.Value = rule.DocTypeID;
                        else
                            paramDocType.Value = DBNull.Value;
                        if (rule.OrganizationID > 0)
                            paramPersId.Value = rule.OrganizationID;
                        else
                            paramPersId.Value = DBNull.Value;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.Transaction.Commit();
                    ret = true;
                }
                catch (SqlException sqlEx)
                {
                    if (cmd.Transaction != null)
                        cmd.Transaction.Rollback();
                    ProcessSqlEx(sqlEx, cmd, "StampChecksDALC", false);
                }
                catch (InvalidOperationException ioEx)
                {
                    if (cmd.Transaction != null)
                        cmd.Transaction.Rollback();
                    ErrorMessage(false, ioEx, null, "StampChecksDALC", false);
                }
                catch (Exception ex)
                {
                    if (cmd.Transaction != null)
                        cmd.Transaction.Rollback();
                    ErrorMessage(false, ex, null, "StampChecksDALC");
                }
                finally
                {
                    cmd.Connection.Close();
                }
            }
            return ret;
        }

        #endregion
    }
}
