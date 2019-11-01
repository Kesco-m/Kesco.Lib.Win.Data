using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Buhgalteriya
{
    public class TransactionsDALC : DALC
    {
        internal string economicIDField = "КодХозОперации";
        internal string debField = "Дебет";
        internal string credField = "Кредит";
        internal string orderField = "Порядок";

        public TransactionsDALC(string connectionString) : base(connectionString)
        {
            tableName = "dbo.ХозОперации_Проводки";
            idField = "КодПроводки";
        }

        #region Accessors

        public string EconomicIDField
        {
            get { return economicIDField; }
        }

        public string DebField
        {
            get { return debField; }
        }


        public string CredField
        {
            get { return credField; }
        }

        public string OrderField
        {
            get { return orderField; }
        }

        #endregion

        #region Get Data

        public DataSet GetEconomicTransactions(int economicID)
        {
            using (
                var cmd =
                    new SqlDataAdapter(
                        "SELECT * " + " FROM " + tableName + " WHERE " + economicIDField + " = @EconomicID " +
                        " ORDER BY " + orderField, new SqlConnection(connectionString)))
            {
                AddParam(cmd.SelectCommand, "@EconomicID", SqlDbType.Int, economicID);

                return CMD_FillDS(cmd);
            }
        }

        #endregion

        #region Change Data

        public bool AddTransaction(int economicID, string debStr, string credStr, string descr)
        {
            using (
                var cmd =
                    new SqlCommand(
                        "INSERT INTO " + tableName + " (" + economicIDField + ", " + debField + ", " + credField + ", " +
                        orderField + ", " + descriptionField + ") " + "SELECT @EconomicID, @Deb, @Cred, ISNULL(MAX(" +
                        orderField + ") + 1, 1), @Descr " + "FROM " + tableName + " WHERE " + economicIDField +
                        " = @EconomicID", new SqlConnection(connectionString)))
            {
                AddParam(cmd, "@EconomicID", SqlDbType.Int, economicID);
                AddParam(cmd, "@Deb", SqlDbType.VarChar, debStr);
                AddParam(cmd, "@Cred", SqlDbType.VarChar, credStr);
                AddParam(cmd, "@Descr", SqlDbType.VarChar, descr);

                return CMD_Exec(cmd);
            }
        }

        public bool ChangeTransaction(int transID, string debStr, string credStr, string descr)
        {
            using (
                var cmd =
                    new SqlCommand(
                        "UPDATE " + tableName + " SET " + debField + " = @Deb, " + credField + " = @Cred, " +
                        descriptionField + " = @Descr " + "WHERE " + idField + " = @ID",
                        new SqlConnection(connectionString)))
            {
                AddParam(cmd, "@ID", SqlDbType.Int, transID);
                AddParam(cmd, "@Deb", SqlDbType.VarChar, debStr);
                AddParam(cmd, "@Cred", SqlDbType.VarChar, credStr);
                AddParam(cmd, "@Descr", SqlDbType.VarChar, descr);

                return CMD_Exec(cmd);
            }
        }

        public bool ChangeOrder(int upID, int downID)
        {
            using (
                var cmd =
                    new SqlCommand(
                        "UPDATE " + tableName + " SET " + orderField + " = (SELECT " + orderField + " FROM " + tableName +
                        " ХП " + "WHERE " + idField + " = CASE WHEN " + tableName + "." + idField +
                        " = @DownID THEN @UpID ELSE @DownID END) " + "WHERE " + idField + " IN ( @DownID, @UpID)",
                        new SqlConnection(connectionString)))
            {
                AddParam(cmd, "@UpID", SqlDbType.Int, upID);
                AddParam(cmd, "@DownID", SqlDbType.Int, downID);

                return CMD_Exec(cmd);
            }
        }

        #endregion
    }
}
