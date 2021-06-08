using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Directory
{
	public class FaxRecipientDALC : DALC
	{
        private const string recipientField = "Recipient";
        private const string personIDField = "КодЛица";
        private const string personLinkIDField = "КодСвязиЛиц";
        private const string contactField = "Контакт";
        private const string contactRLField = "КонтактRL";
        private const string contactTypeField = "ТипКонтакта";
        private const string categoryField = "Категория";
        private const string contactTypeCodeField = "КодТипаКонтакта";

		public FaxRecipientDALC(string connectionString)
			: base(connectionString)
		{
			tableName = "Справочники.dbo.vwКонтактыЛица";

			idField = "КодКонтакта";
			nameField = "Кличка";
			descriptionField = "Примечание";
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
				"ТК." + contactTypeField + " " + contactTypeField + ", " +
				"ТК." + categoryField + " " + categoryField +
				" FROM " + tableName + " K INNER JOIN " +
				" Справочники.dbo.ТипыКонтактов ТК ON K.КодТипаКонтакта = ТК.КодТипаКонтакта " +
				" LEFT OUTER JOIN Справочники.dbo.vwСвязиЛиц li ON K.КодСвязиЛиц = li.КодСвязиЛиц " +
				" WHERE " +
				"(K.КодЛица = @ID) " +
				"AND  ТК." + categoryField + " = 4 AND( GETUTCDATE() BETWEEN li.От AND li.До OR K.КодСвязиЛиц IS NULL) "+//(ТК." + categoryField + " = 3 OR)
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
				"ТК." + contactTypeField + " " + contactTypeField + ", " +
                "ТК." + categoryField + " " + categoryField + ", " +
                "ТК." + contactTypeCodeField + " " + contactTypeCodeField +
				" FROM " + tableName + " K INNER JOIN " +
				" Справочники.dbo.ТипыКонтактов ТК ON K.КодТипаКонтакта = ТК.КодТипаКонтакта " +
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