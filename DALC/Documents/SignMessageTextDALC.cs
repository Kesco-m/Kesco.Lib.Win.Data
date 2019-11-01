using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	/// <summary>
	/// DALC для доступа к сообщениям для отправки при подписи документов
	/// </summary>
	public class SignMessageTextDALC:DALC
	{
        private const string signTypeField = "ТипПодписи";
        private const string nameEnField = "ТекстСообщенияEn";
        private const string employeeIDField = "КодСотрудника";

		public SignMessageTextDALC(string connectionString)
			: base(connectionString)
		{
			tableName = "ТипыДокументовСообщенияПодписи";
			idField = "КодТипаДокумента";
			nameField = "ТекстСообщения";
		}

		#region Accessors

		/// <summary>
		/// Название поля типа подписи
		/// </summary>
		public string SignTypeField
		{
			get { return signTypeField; }
		}

		/// <summary>
		/// Сообщение после подписи на англиском
		/// </summary>
		public string NameEnField
		{
			get { return nameEnField; }
		}

		#endregion

		#region Get data

		/// <summary>
		/// Загрузка строки по выбранному типу документа, типу подписи и языку
		/// </summary>
		/// <param name="docTypeID">код типа документа</param>
		/// <param name="signTypeID">тип подписи</param>
		/// <param name="twoLettersLanguageString">язык</param>
		/// <returns>строка текста сообщения</returns>
		public string GetSignMessageText(int docTypeID, byte signTypeID, string twoLettersLanguageString)
		{
			object obj = GetField("SELECT " + ((twoLettersLanguageString == "ru")?nameField:nameEnField) + " FROM " + tableName + " WHERE " + idField + " = @DocTypeID AND " + signTypeField + " = @SignTypeID AND " + employeeIDField + " = 0", nameField, delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@DocTypeID", SqlDbType.Int, docTypeID);
					AddParam(cmd, "@SignTypeID", SqlDbType.TinyInt, signTypeID);
				});
			return obj as string;
		}

		/// <summary>
		/// Получение всех текстов сообщений после подписи по определенному типу документа 
		/// </summary>
		/// <param name="docTypeID">код типа документа</param>
		/// <returns>dataset со всеми сообщеньями</returns>
		public DataSet GetDocTypeTextMessages(int docTypeID)
		{
			return GetData("SELECT " + signTypeField + ", " + nameField + ", " + nameEnField + " FROM " + tableName + " WHERE " + idField + " = @DocTypeID AND " + employeeIDField + " = 0", delegate(SqlCommand cmd)
			{
				AddParam(cmd, "@DocTypeID", SqlDbType.Int, docTypeID);
			});
		}

		#endregion

		#region Change Data

		public bool AddOrUpdate(int docTypeID, byte signTextTypeID, string text, string textEn, bool new1)
		{
			return Exec("IF ISNULL((SELECT " + nameField + " FROM " + tableName + " WHERE " + idField + " = @DocTypeID AND " + signTypeField + " = @SignTextTypeID AND " + employeeIDField + " = 0),'') <> @Text OR ISNULL((SELECT " + nameEnField + " FROM " + tableName + " WHERE " + idField + " = @DocTypeID AND " + signTypeField + " = @SignTextTypeID),'') <> @TextEn\n" +
				"IF EXISTS (SELECT * FROM " + tableName + " WHERE " + idField + " = @DocTypeID AND " + signTypeField + " = @SignTextTypeID AND " + employeeIDField + " = 0)\n	UPDATE " + tableName + " SET " + nameField + " = @Text, " + nameEnField + " = @TextEn WHERE " + idField + " = @DocTypeID AND " + signTypeField + " = @SignTextTypeID AND " + employeeIDField + " = 0 \n" +
				"ELSE INSERT " + tableName + " VALUES (@DocTypeID, 0, @SignTextTypeID, @Text, @TextEn)", delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@DocTypeID", SqlDbType.Int, docTypeID);
					AddParam(cmd, "@SignTextTypeID", SqlDbType.TinyInt, signTextTypeID);
					AddParam(cmd, "@Text", SqlDbType.NVarChar, text);
					AddParam(cmd, "@TextEn", SqlDbType.VarChar, textEn);
				});
		}

		public bool Delete(int docTypeID, byte signTextTypeID, string twoLettersLanguageString)
		{
			return Exec("DELETE FROM " + tableName + " WHERE " + idField + " = @DocTypeID AND " + signTypeField + " = @SignTextTypeID", delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@DocTypeID", SqlDbType.Int, docTypeID);
					AddParam(cmd, "@SignTextTypeID", SqlDbType.TinyInt, signTextTypeID);
				});
		}
		#endregion
	}
}
