using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    /// <summary>
    /// Summary description for FaxInDALC.
    /// </summary>
    public class FaxInDALC : FaxDALC
    {
        public FaxInDALC(string connectionString) : base(connectionString)
        {
        }

        #region Get Data

		public DataRow GetFaxIn(int id)
		{
			return GetFirstRow(
					 "SELECT " +
					 tableName + "." + idField + ", " +
					 folderFaxIDField + ", " +
					 directionField + ", " +
					 faxDocImageTable + "." + docImageIDField + ", " +
					 transferEndDateField + " AS " + dateField + ", " +
					 csidField + ", " +
					 pageRecvCountField + ", " +
					 durationField + ", " +
					 speedField + ", " +
					 modemIDField + ", " +
					 descriptionField + ", " +
					 senderField + ", " +
					 senderAddressField + ", " +
					 recipField + ", " +
					 fileNameField + ", " +
					 readField + ", " +
					 editorField + ", " +
					 editedField +
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

		public int GetFaxID(int imageID)
		{
			return GetIntField("SELECT " + idField + " FROM " + faxDocImageTable +
					" WHERE " + docImageIDField + " = @ImageID",
					idField,
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@ImageID", SqlDbType.Int, imageID);
				});
		}

        #endregion
    }
}