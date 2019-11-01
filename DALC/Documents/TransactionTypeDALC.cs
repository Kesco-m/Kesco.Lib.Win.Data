using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    /// <summary>
    /// Summary description for TransactionTypeDALC.
    /// </summary>
    public class TransactionTypeDALC : DALC
    {
        private const string nameENField = "�������������EN";

        public TransactionTypeDALC(string connectionString)
            : base(connectionString)
        {
            tableName = "dbo.��������������";
            idField = "�����������������";
            nameField = "�������������";
        }

        public DataTable GetData(string lang)
        {
            var cmd = new SqlDataAdapter(
                "SELECT " + idField + ", " + ((lang.StartsWith("ru")) ? nameField : nameENField) + " " + nameField +
                " FROM " + tableName,
                new SqlConnection(connectionString));
            return CMD_FillDT(cmd.SelectCommand);
        }
    }
}
