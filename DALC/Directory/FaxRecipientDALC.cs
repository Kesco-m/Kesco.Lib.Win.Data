using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Directory
{
	public class FaxRecipientDALC : DALC
	{
        private const string recipientField = "Recipient";
        private const string personIDField = "�������";
        private const string personLinkIDField = "�����������";
        private const string contactField = "�������";
        private const string contactRLField = "�������RL";
        private const string contactTypeField = "�����������";
        private const string categoryField = "���������";
        private const string contactTypeCodeField = "���������������";

		public FaxRecipientDALC(string connectionString)
			: base(connectionString)
		{
			tableName = "�����������.dbo.vw������������";

			idField = "�����������";
			nameField = "������";
			descriptionField = "����������";
		}

		#region Accessors

		public string RecipientField
		{
			get { return recipientField; }
		}

		public string PersonIDField
		{
			get { return personIDField; }
		}

		public string PersonLinkIDField
		{
			get { return personLinkIDField; }
		}

		public string ContactField
		{
			get { return contactField; }
		}

		public string ContactRLField
		{
			get { return contactRLField; }
		}

		public string ContactTypeField
		{
			get { return contactTypeField; }
		}

        public string ContactTypeCodeField
		{
			get { return contactTypeCodeField; }
		}

		public string CategoryField
		{
			get { return categoryField; }
		}
		#endregion

		#region Get Data

        public DataTable GetPersonRecipients(int personID)
		{
            return GetDataTable("SELECT " +
				"K." + idField + ", " +
				"K." + contactField + ", " +
				"K." + contactRLField + ", " +
				"K." + personIDField + ", " +
				"K." + personLinkIDField + ", " +
				"K." + descriptionField + " " + descriptionField + ", " +
				"��." + contactTypeField + " " + contactTypeField + ", " +
				"��." + categoryField + " " + categoryField +
				" FROM " + tableName + " K INNER JOIN " +
				" �����������.dbo.������������� �� ON K.��������������� = ��.��������������� " +
				" LEFT OUTER JOIN �����������.dbo.vw�������� li ON K.����������� = li.����������� " +
				" WHERE " +
				"(K.������� = @ID) " +
				"AND  ��." + categoryField + " = 4 AND( GETUTCDATE() BETWEEN li.�� AND li.�� OR K.����������� IS NULL) "+//(��." + categoryField + " = 3 OR)
				" ORDER BY K." + personLinkIDField + ", K." + idField + " DESC",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@ID", SqlDbType.Int, personID);
				}
			);
		}

		public DataRow GetPersonContact(int contactID)
		{
			return GetFirstRow(
				"SELECT TOP 1 " +
				"K." + idField + " " + idField + ", " +
				"K." + contactField + " " + contactField + ", " +
				"K." + personIDField + " " + personIDField + ", " +
				"K." + personLinkIDField + " " + personLinkIDField + ", " +
				"K." + descriptionField + " " + descriptionField + ", " +
				"��." + contactTypeField + " " + contactTypeField + ", " +
                "��." + categoryField + " " + categoryField + ", " +
                "��." + contactTypeCodeField + " " + contactTypeCodeField +
				" FROM " + tableName + " K INNER JOIN " +
				" �����������.dbo.������������� �� ON K.��������������� = ��.��������������� " +
				" WHERE " + idField + "= @ID",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@ID", SqlDbType.Int, contactID);
				}
			);
		}

		#endregion
	}
}