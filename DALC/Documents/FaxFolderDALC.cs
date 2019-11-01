using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    public class FaxFolderDALC : DALC
    {
        private enum Direction
        {
            In = 1,
            Out
        }

        private const string networkPathField = "����";
        protected const string directionField = "Direction";
        protected const string nameFieldEng = "��������������";
        protected const string personIDField = "�������";

        public FaxFolderDALC(string connectionString) : base(connectionString)
        {
            tableName = "���������.dbo.vw�����������";

            idField = "��������������";
            nameField = "�����������";
        }

        #region Accessors

        public string NetworkPathField
        {
            get { return networkPathField; }
        }

        #endregion

        #region Get Data

		private DataTable GetFaxFolders(Direction dir, string lang)
		{
			return GetDataTable("SELECT " + idField + ", " +
				((lang.StartsWith("ru")) ? nameField : nameFieldEng) + " " + nameField + ", " +
				networkPathField + " FROM " + tableName + " WHERE " + directionField + " = @Dir" +
				" ORDER BY " + nameField,
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@Dir", SqlDbType.Int, (int)dir);
				});
		}

        public int GetFaxFolderPersonID(int id)
        {
            return GetIntField("SELECT " + personIDField +
                               " FROM " + tableName +
                               " WHERE " + idField + " = @ID",
                               personIDField,
                               delegate(SqlCommand cmd)
                                   {
                                       AddParam(cmd, "@ID", SqlDbType.Int, id);
                                   });
        }

        public DataTable GetFaxInFolders(string lang)
        {
            return GetFaxFolders(Direction.In, lang);
        }

        public DataTable GetFaxOutFolders(string lang)
        {
            return GetFaxFolders(Direction.Out, lang);
        }

        #endregion
    }
}