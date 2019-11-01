using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Kesco.Lib.Win.Data.Documents;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	/// <summary>
	/// DALC для доступа к vwФаксы.
	/// </summary>
	public class FaxDALC : DALC
	{
		protected string faxFolderIDField;

		protected const string directionField = "Direction";
		protected const string statusField = "Status";

		protected string docImageIDField;
		protected string documentIDField;
		protected const string dateField = "Дата";
		protected const string senderField = "Отправитель";
		protected const string senderAddressField = "АдресОтправителя";
		protected const string recipField = "Получатель";
		protected const string fileNameField = "FileName";

		protected const string folderFaxIDField = "КодПапкиФаксов";
		protected const string transferEndDateField = "ВремяКонцаПередачи";
		protected const string modemIDField = "ModemID";
		protected const string speedField = "Baudrate";
		protected const string durationField = "Duration";
		protected const string csidField = "CSID";

		protected const string pageRecvCountField = "ItemsRecv";

		protected const string readField = "Прочитан";
		protected const string spamField = "Спам";

		protected const string pageSentCountField = "ItemsSent";
		protected const string recvAddressField = "АдресПолучателя";

		protected const string savedField = "Сохранен";

		protected const string organizationIDField = "КодЛица";

		protected const string spSendFax = "master.dbo.sp_SendFax";
		protected const string folderFaxDB = "vwПапкиФаксов";

		protected const string faxDocImageTable = "Факсы_ИзображенияДокументов";

		protected const string logTableName = "logEmail";

		private DocImageDALC docImageData;
		private FaxFolderDALC faxFolderData;

		protected string multiSelectString;
		protected string caseAddString;
		protected string multiWhereString;
		protected string multiOrderByString;

		public FaxDALC(string connectionString) : base(connectionString)
		{
			tableName = "vwФаксы";
			idField = "КодФакса";
			nameField = null;

			var docData = new DocumentDALC(null);
			documentIDField = docData.IDField;

			docImageData = new DocImageDALC(null);
			docImageIDField = docImageData.IDField;

			faxFolderData = new FaxFolderDALC(null);
			faxFolderIDField = faxFolderData.IDField;

			multiSelectString =
				"DATEADD(hour, @Diff, " + transferEndDateField + ") AS " + dateField + ", " +
				folderFaxIDField + ", " +
				directionField + ", " +
				descriptionField + ", " +
				senderField + ", " +
				senderAddressField + ", " +
				csidField + ", " +
				recipField + ", " +
				recvAddressField + ", " +
				tableName + "." + idField + ", " +
				fileNameField + ", " +
				statusField + ", " +
				readField + ", " +
				spamField + ", ";
			caseAddString =
				"CASE WHEN EXISTS (SELECT * FROM " + faxDocImageTable + " (nolock) " +
				" WHERE " + faxDocImageTable + "." + idField + " = " + tableName + "." + idField +
				") THEN CONVERT(bit, 1) ELSE CONVERT(bit, 0) END " + savedField;

			multiOrderByString =
				" ORDER BY " + dateField + " DESC";
		}

		#region Accessors

		public string DocImageIDField
		{
			get { return docImageIDField; }
		}

		public string DateField
		{
			get { return dateField; }
		}

		public string SenderField
		{
			get { return senderField; }
		}

		public string SenderAddressField
		{
			get { return senderAddressField; }
		}

		public string RecipField
		{
			get { return recipField; }
		}

		public string FileNameField
		{
			get { return fileNameField; }
		}

		public string TransferEndDateField
		{
			get { return transferEndDateField; }
		}

		public string ModemIDField
		{
			get { return modemIDField; }
		}

		public string SpeedField
		{
			get { return speedField; }
		}

		public string DurationField
		{
			get { return durationField; }
		}

		public string StatusField
		{
			get { return statusField; }
		}

		public string CSIDField
		{
			get { return csidField; }
		}

		public string DirectionField
		{
			get { return directionField; }
		}

		public string SavedField
		{
			get { return savedField; }
		}

		public string PageRecvCountField
		{
			get { return pageRecvCountField; }
		}

		public string ReadField
		{
			get { return readField; }
		}

		public string SpamField
		{
			get { return spamField; }
		}

		public string PageSentCountField
		{
			get { return pageSentCountField; }
		}

		public string RecvAddressField
		{
			get { return recvAddressField; }
		}

		public string FolderFaxIDField
		{
			get { return folderFaxIDField; }
		}

		#endregion

		#region Get Data

		public DataTable GetFolderFaxes(int faxFolderID, bool unsaved)
		{
			string selectString =
				"SELECT " + multiSelectString + ((unsaved) ? " CONVERT(bit, 0) " + savedField : caseAddString) +
				" FROM " + tableName +
				" (nolock) WHERE " + faxFolderIDField + " = @FaxFolderID";

			if(unsaved)
				selectString +=
					" AND NOT EXISTS" +
					" (" +
					"SELECT * FROM " + faxDocImageTable + " (nolock) " +
					" WHERE " + faxDocImageTable + "." + idField + " = " + tableName + "." + idField +
					")";

			selectString += multiOrderByString;

			return GetDataTable(selectString,
								delegate(SqlCommand cmd)
								{
									AddParam(cmd, "@Diff", SqlDbType.Int, GetTimeDiff().Hours);
									AddParam(cmd, "@FaxFolderID", SqlDbType.Int, faxFolderID);
								});
		}

		public DataRow GetSenderPersonInfo(string senderAdres)
		{
			if(!String.IsNullOrEmpty(senderAdres) && senderAdres.Trim().Replace("+", "").Length >= 7)
			{
				const string selectSenderString = "SELECT TOP 1 L.КодЛица AS КодЛицаОтправителя, L.Кличка AS КличкаОтправителя FROM Справочники.dbo.vwКонтакты K (nolock) " +
												  "INNER JOIN Справочники.dbo.vwЛица L (nolock) ON K.КодЛица = L.КодЛица " +
												  "INNER JOIN  Справочники.dbo.ТипыКонтактов T ON K.КодТипаКонтакта = T.КодТипаКонтакта " +
												  "WHERE T.Категория = 3 AND K.КонтактRL LIKE '%' + REVERSE(SUBSTRING(REVERSE(RTRIM(@FromAdres)),1,7))";

				return GetFirstRow(selectSenderString,
								   delegate(SqlCommand cmd)
								   {
									   AddParam(cmd, "@FromAdres", SqlDbType.NVarChar, senderAdres);
								   });
			}

			return null;
		}

		public DataRow GetRecipPersonInfo(int folderFaxesIDField)
		{
			const string selectRecipString = "SELECT F.КодЛица AS КодЛицаПолучателя, P.Кличка AS КличкаПолучателя FROM vwПапкиФаксов F (nolock) " +
											 "INNER JOIN Справочники.dbo.vwЛица P (nolock) ON  F.КодЛица = P.КодЛица " +
											 "WHERE F.КодПапкиФаксов = @ID";

			return GetFirstRow(selectRecipString,
							   delegate(SqlCommand cmd)
							   {
								   AddParam(cmd, "@ID", SqlDbType.Int, folderFaxesIDField);
							   });
		}

		public bool FaxHasDocImage(int faxID)
		{
			return GetIntField(
				"SELECT TOP 1 " + docImageIDField +
				" FROM " + faxDocImageTable +
				" WITH (NOLOCK) WHERE " + idField + " = @ID",
				docImageIDField,
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@ID", SqlDbType.Int, faxID);
				}) > 0;
		}

		public List<int> GetFaxDBDocs(int faxID)
		{
			return GetRecords<int>(
				"SELECT " + documentIDField + " FROM " + docImageData.TableName + " (nolock) " +
				" INNER JOIN " + faxDocImageTable + " (nolock) " +
				" ON " + docImageData.TableName + "." + docImageIDField + " = " +
				faxDocImageTable + "." + docImageIDField +
				" WHERE " + idField + " = @FaxID",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@FaxID", SqlDbType.Int, faxID);
				});
		}

		public int GetFaxIDFromFileName(string fileName)
		{
			return GetIntField("SELECT " + idField +
							   " FROM " + tableName + " (nolock) " +
							   " WHERE " + fileNameField + " = @FileName", idField,
							   delegate(SqlCommand cmd)
							   {
								   AddParam(cmd, "@FileName", SqlDbType.VarChar, fileName);
							   });
		}

		public bool GetStatus(int faxFolderID, out int unread, out int overall, bool unsaved)
		{
			overall = 0;
			unread = 0;

			string query =
				"SELECT " + spamField + "," + readField + " FROM " + tableName +
				" (nolock) WHERE " + faxFolderIDField + " = @FaxFolderID AND " + spamField + " = 0";

			if(unsaved)
				query +=
					" AND NOT EXISTS" +
					" (" +
					"SELECT * FROM " + faxDocImageTable + " (nolock) " +
					" WHERE " + faxDocImageTable + "." + idField + " = " + tableName + "." + idField +
					")";

			//query += " OPTION ( MAXDOP 1)";
			using(var cmd = new SqlCommand(query, new SqlConnection(connectionString)))
			{
				AddParam(cmd, "@FaxFolderID", SqlDbType.Int, faxFolderID);
				cmd.CommandTimeout = cmdTimeout;

				try
				{
					cmd.Connection.Open();
					using(SqlDataReader dr = cmd.ExecuteReader())
					{
						while(dr.Read())
						{
							overall++;
							if(!dr.GetBoolean(1))
								unread++;
						}
						dr.Close();
						return true;
					}
				}
				catch(SqlException sqlEx)
				{
					ProcessSqlEx(sqlEx, cmd);
				}
				catch(Exception ex)
				{
					ErrorMessage(ex, null, "GetRecords");
				}
				finally
				{
					cmd.Connection.Close();
				}
			}

			return false;
		}

		#endregion

		#region Change Data

		public bool SendFax(int imageServer, string recipient, string fax, string subject, string fileName, int docImageID)
		{
			return Exec(spSendFax,
						delegate(SqlCommand cmd)
						{
							cmd.CommandType = CommandType.StoredProcedure;

							AddParam(cmd, "@КодАрхиваИзображений", SqlDbType.Int, imageServer);
							AddParam(cmd, "@RecipientName", SqlDbType.NVarChar, recipient);
							AddParam(cmd, "@FAXnumber", SqlDbType.VarChar, fax);
							AddParam(cmd, "@Subject", SqlDbType.NVarChar, subject);
							AddParam(cmd, "@FileName", SqlDbType.VarChar, fileName);

							if(docImageID > 0)
								AddParam(cmd, "@Code", SqlDbType.Int, docImageID);

							AddParam(cmd, "@Status", SqlDbType.Int, 0);
							cmd.Parameters["@Status"].Direction = ParameterDirection.ReturnValue;
						},
						delegate(SqlCommand cmd)
						{
							return (int)cmd.Parameters["@Status"].Value == 0;
						});
		}


		public bool CheckFax(int faxID, int imageID)
		{
			return Exec("INSERT INTO " + faxDocImageTable +
						" (" +
						idField + ", " +
						docImageIDField +
						")" +
						" VALUES (@FaxID, @ImageID)",
						delegate(SqlCommand cmd)
						{
							AddParam(cmd, "@ImageID", SqlDbType.Int, imageID);
							AddParam(cmd, "@FaxID", SqlDbType.Int, faxID);
						});
		}

		public bool MarkSpam(ArrayList faxIDs, bool spam)
		{
			if(faxIDs.Count > 0)
			{
				string idString = string.Join(",", faxIDs.Cast<int>().Select(id => id.ToString()).ToArray());
				if(idString.Length > 0)
				{
					return Exec("UPDATE " + tableName +
								" SET " +
								spamField + " = @Spam" +
								" WHERE " + idField + " IN (" + idString + ")",
								delegate(SqlCommand cmd)
								{
									AddParam(cmd, "@Spam", SqlDbType.Bit, spam ? 1 : 0);
								});
				}
			}

			return false;
		}

		public bool MarkSpam(int faxID, bool spam)
		{
			return Exec("UPDATE " + tableName +
						" SET " +
						spamField + " = @Spam" +
						" WHERE " + idField + " = @ID",
						delegate(SqlCommand cmd)
						{
							AddParam(cmd, "@Spam", SqlDbType.Bit, spam ? 1 : 0);
							AddParam(cmd, "@ID", SqlDbType.Int, faxID);
						});
		}

		public bool MarkResend(int faxID)
		{
			return Exec("UPDATE " + tableName +
						" SET " +
						spamField + " = 1" +
				//statusField + " = -2 " +
						" WHERE " + idField + " = @ID AND " + statusField + " = -1",
						delegate(SqlCommand cmd)
						{
							AddParam(cmd, "@ID", SqlDbType.Int, faxID);
						});
		}

		#endregion
	}
}