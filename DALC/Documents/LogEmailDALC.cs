using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    public class LogEmailDALC : DALC
    {
        private const string recieverField = "Получатель";
        private const string docImageIDField = "КодИзображенияДокумента";
        private const string senderField = "КодСотрудника";
        private const string sendTimeField = "ВремяОтправки";

        public LogEmailDALC(string connectionString) : base(connectionString)
        {
            tableName = "Документы.dbo.logEmail";

            idField = "КодEmail";
            nameField = "Email";
        }

        #region Accessors

        public string RecieverField
        {
            get { return recieverField; }
        }

        public string DocImageIDField
        {
            get { return docImageIDField; }
        }

        public string SendTimeField
        {
            get { return sendTimeField; }
        }

        public string SenderField
        {
            get { return senderField; }
        }

        #endregion

        #region Change Data

        public bool LogEmail(int docImageID, string reciever, string email)
        {
            if (docImageID <= 0)
                return false;

            using (
                var cmd =
                    new SqlCommand(
                        "INSERT INTO " + tableName + " (" + docImageIDField + ", " + recieverField + ", " + nameField +
                        ")" + " VALUES" + " (@ImageID, @Reciever, @Email)", new SqlConnection(connectionString)))
            {
                AddParam(cmd, "@ImageID", SqlDbType.Int, docImageID);
                AddParam(cmd, "@Reciever", SqlDbType.NVarChar, reciever);
                AddParam(cmd, "@Email", SqlDbType.VarChar, email);

                return CMD_Exec(cmd);
            }
        }

        public bool LogEmail(int[] docImageIDs, string reciever, string email)
        {
            var query = new StringBuilder();
            for (int i = 0; i < docImageIDs.Length; i++)
            {
                if (docImageIDs[i] > 0)
                    query.AppendLine("INSERT INTO " + tableName +
                                     " (" + docImageIDField + ", " +
                                     recieverField + ", " +
                                     nameField + ") VALUES (@ImageID" + i.ToString() + ", @Reciever, @Email) ");
            }
            if (query.Length == 0)
                return false;

            using (var cmd = new SqlCommand(query.ToString().TrimEnd(','), new SqlConnection(connectionString)))
            {
                for (int i = 0; i < docImageIDs.Length; i++)
                    AddParam(cmd, "@ImageID" + i.ToString(), SqlDbType.Int, docImageIDs[i]);

                AddParam(cmd, "@Reciever", SqlDbType.NVarChar, reciever);
                AddParam(cmd, "@Email", SqlDbType.VarChar, email);

                return CMD_Exec(cmd);
            }
        }

        public bool UpdateEmail(int basicDocImageID, int insertDocImageID)
        {
            using (
                var cmd =
                    new SqlCommand(
                        " INSERT INTO " + tableName + " (" + docImageIDField + ", " + recieverField + ", " + nameField +
                        ", " + senderField + ", " + sendTimeField + ")" + "  SELECT @ImageID, " + recieverField + ", " +
                        nameField + ", " + senderField + ", " + sendTimeField + " FROM " + tableName + " WHERE " +
                        docImageIDField + " = @BasicImageID", new SqlConnection(connectionString)))
            {
                AddParam(cmd, "@ImageID", SqlDbType.Int, insertDocImageID);
                AddParam(cmd, "@BasicImageID", SqlDbType.Int, basicDocImageID);
                return CMD_Exec(cmd);
            }
        }

        #endregion
    }
}