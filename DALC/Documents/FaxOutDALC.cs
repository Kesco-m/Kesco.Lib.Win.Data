using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    /// <summary>
    /// Summary description for FaxOutDALC.
    /// </summary>
    public class FaxOutDALC : FaxDALC
    {
        public FaxOutDALC(string connectionString)
            : base(connectionString)
        {
        }

        #region Get Data

		public DataRow GetFaxOut(int id)
		{
			return GetFirstRow("SELECT " +
					faxDocImageTable + "." + docImageIDField + ", " +
					 transferEndDateField + " AS " + dateField + ", " +
					durationField + ", " +
					recvAddressField + ", " +
					pageSentCountField + ", " +
					speedField + ", " +
					csidField + ", " +
					modemIDField + ", " +
					senderField + ", " +
					recipField + ", " +
					descriptionField + ", " +
					fileNameField + ", " +
					statusField + ", " +
					editorField + ", " +
					editedField + ", " +
					senderField + ", " +
					senderAddressField + ", " +
					folderFaxIDField +
					" FROM " + tableName +
					" LEFT JOIN " + faxDocImageTable +
					" ON " + faxDocImageTable + "." + idField + " = " +
					tableName + "." + idField +
					" WHERE " + tableName + "." + idField + " = @ID",
					delegate(SqlCommand cmd)
					{
						AddParam(cmd, "@ID", SqlDbType.Int, id);
					});
		}

        #endregion
    }
}