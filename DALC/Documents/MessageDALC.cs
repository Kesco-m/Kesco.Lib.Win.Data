using System;
using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	/// <summary>
	/// Summary description for MessageDALC.
	/// </summary>
	public class MessageDALC : DALC
    {
        private const string sentField = "Отправлено";
        private const string senderField = "Отправитель";
        private const string senderEngField = "Sender";
        private const string recipientsField = "Получатели";
        private const string recipientsEngField = "Recipients";
        private const string readField = "Прочитано";

        private const string spMessages = "sp_Сообщения";
        private const string spSendMessage = "sp_SendMessage";

        private const string directionField = "КодСотрудникаОтправителя";
        private const string employeesField = "Сотрудники";
        private const string dateMessageField = "Дата/Сообщение";
        private const string dateField = "Дата";
        private const string titleField = "Заголовок";

        private const string originalEmployeesField = "ОригиналСотрудники";
        private const string originalDateMessageField = "ОригиналДата/Сообщение";

        public MessageDALC(string connectionString)
            : base(connectionString)
        {
            idField = "КодСообщения";
            nameField = "Сообщение";
        }

        #region Accessors

        public string SentField
        {
            get { return sentField; }
        }

        public string SenderField
        {
            get { return senderField; }
        }

        public string SenderEngField
        {
            get { return senderEngField; }
        }

        public string RecipientsField
        {
            get { return recipientsField; }
        }

        public string RecipientsEngField
        {
            get { return recipientsEngField; }
        }

        public string ReadField
        {
            get { return readField; }
        }

        public string SpSendMessage
        {
            get { return spSendMessage; }
        }

        public string DirectionField
        {
            get { return directionField; }
        }

        public string EmployeesField
        {
            get { return employeesField; }
        }

        public string DateMessageField
        {
            get { return dateMessageField; }
        }

        public string TitleField
        {
            get { return titleField; }
        }

        public string OriginalEmployeesField
        {
            get { return originalEmployeesField; }
        }

        public string OriginalDateMessageField
        {
            get { return originalDateMessageField; }
        }

        public string DateField
        {
            get { return dateField; }
        }

        #endregion

        #region Get Data

        public DataTable GetDocMessages(int docID, out SqlCommand cmd)
        {
            cmd = new SqlCommand(
                spMessages,
                new SqlConnection(connectionString));
            cmd.CommandType = CommandType.StoredProcedure;

            AddParam(cmd, "@КодДокумента", SqlDbType.Int, docID);

            return CMD_FillDT(cmd);
        }

		#endregion

		#region Change Data

		public bool SendOneRecipientMessage(int docID, int empID, string message, int recipientID, bool personal)
		{
			int id = 0;
			string sql = @"DECLARE @MessID int
	                IF NOT EXISTS(SELECT * FROM vwДокументы (nolock) WHERE КодДокумента = @DocID)
                    BEGIN
						RAISERROR('Документ с кодом %d был не найден или не доступен',12,1, @DocID)
						RETURN 
					END
					INSERT Сообщения
						(Сообщение, КодДокумента, КодСотрудникаОтправителя, Отправитель, Sender, Персональное, Получатели, Recipients)
						SELECT @Message, @DocID, Sender.КодСотрудника, Sender.ФИО, Sender.IOF, @Person, Recipient.ФИО, Recipient.IOF
						FROM Инвентаризация.dbo.Сотрудники Recipient CROSS JOIN Инвентаризация.dbo.Сотрудники Sender
						WHERE Sender.КодСотрудника = @EmpID AND Recipient.КодСотрудника = @RecipientID
					SET @MessID = SCOPE_IDENTITY()
					IF @MessID IS NULL OR @MessID < 1
					BEGIN
						RAISERROR('Не удалось добавить сообщениe',16,1)
						RETURN 
					END
					INSERT dbo.ПолучателиСообщенийНаОтправку (КодСообщения, КодСотрудникаПолучателя) SELECT @MessID, КодСотрудника FROM Инвентаризация.dbo.Сотрудники 
						WHERE КодСотрудника = @RecipientID
					IF @@ROWCOUNT = 0 
					BEGIN
						RAISERROR('Не удалось добавить получателей сообщения',16,1)
						RETURN 
					END
					SELECT @MessID " +
			identityField;
			using(SqlConnection cn = new SqlConnection(connectionString))
			using(SqlCommand cmd = new SqlCommand(sql, cn))
			{ 
				AddParam(cmd, "@DocID", SqlDbType.Int, docID);
				AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
				AddParam(cmd, "@Message", SqlDbType.NVarChar, message);
				AddParam(cmd, "@Person", SqlDbType.TinyInt, (personal ? 1 : 0));
				AddParam(cmd, "@RecipientID", SqlDbType.VarChar, recipientID);
				cmd.CommandTimeout = cmdTimeout;
				bool? repeat = null;
				while(!repeat.HasValue || repeat.Value)
					try
					{
						cn.Open();
						using(SqlTransaction trn = cn.BeginTransaction(IsolationLevel.ReadCommitted))
						{
							cmd.Transaction = trn;
							object obj = cmd.ExecuteScalar();

							repeat = false;
							id = (obj is int) ? (int)obj : 0;
							if(id< 1)
								trn.Rollback();
							else
								trn.Commit();
						}
					}
					catch(SqlException sqlEx)
					{
						if((sqlEx.Number != 6005 && sqlEx.Number != 10054) || (repeat.HasValue && repeat.Value))
						{
							repeat = false;
							if((sqlEx.Class == 11 && sqlEx.Number == 0) || (sqlEx.Class == 16 && sqlEx.Number == 3980))
								return false;
							else
								ProcessSqlEx(sqlEx, cmd);
						}
						else
							repeat = true;
					}
					catch(Exception ex)
					{
						repeat = false;
						ErrorMessage(ex, null, "CMD_GetField");
					}
			}
			if(id > 0)
			{
				return ExecNoError("sp_ОтправкаСообщения",
								   delegate (SqlCommand cmd)
								   {
									   cmd.Parameters.AddWithValue("@КодСообщения", id);
									   cmd.CommandType = CommandType.StoredProcedure;
								   });
			}
			return false;
		}

		public bool SendMessage2(int docID, int empID, string message, string recipientIDs, string recipients, string recipientsEn, bool personal)
		{
			int id = 0;
			string sql = @"DECLARE @MessID int
	            IF NOT EXISTS(SELECT * FROM vwДокументы (nolock) WHERE КодДокумента = @DocID)
                BEGIN
		            RAISERROR('Документ с кодом %d был не найден или не доступен',12,1, @DocID)
		            RETURN 
	            END
	            INSERT Сообщения
                    (Сообщение, КодДокумента, КодСотрудникаОтправителя, Отправитель, Sender, Персональное, Получатели, Recipients)
		            SELECT @Message, @DocID, КодСотрудника, ФИО, IOF, @Person, '', ''
		            FROM Инвентаризация.dbo.Сотрудники WHERE КодСотрудника = @EmpID
	            SET @MessID = SCOPE_IDENTITY()
	            IF @MessID IS NULL OR @MessID < 1
	            BEGIN
		            RAISERROR('Не удалось добавить сообщениe',16,1)
		            RETURN 
	            END
	            INSERT dbo.ПолучателиСообщенийНаОтправку (КодСообщения, КодСотрудникаПолучателя) SELECT @MessID, КодСотрудника FROM Инвентаризация.dbo.Сотрудники 
		            WHERE КодСотрудника IN (" +
					recipientIDs +
					@")
	            IF @@ROWCOUNT = 0 
	            BEGIN
		            RAISERROR('Не удалось добавить получателей сообщения',16,1)
		            RETURN 
	            END
                SELECT @MessID " + identityField;
			using(SqlConnection cn = new SqlConnection(connectionString))
			using(SqlCommand cmd = new SqlCommand(sql, cn))
			{
				AddParam(cmd, "@DocID", SqlDbType.Int, docID);
				AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
				AddParam(cmd, "@Message", SqlDbType.NVarChar, message);
				AddParam(cmd, "@Person", SqlDbType.TinyInt, (personal ? 1 : 0));
				cmd.CommandTimeout = cmdTimeout;
				bool? repeat = null;
				while(!repeat.HasValue || repeat.Value)
					try
					{
						cn.Open();
						using(SqlTransaction trn = cn.BeginTransaction(IsolationLevel.ReadCommitted))
						{
							cmd.Transaction = trn;
							object obj = cmd.ExecuteScalar();

							repeat = false;
							id = (obj is int) ? (int)obj : 0;
							if(id< 1)
								trn.Rollback();
							else
								trn.Commit();
						}
					}
					catch(SqlException sqlEx)
					{
						if((sqlEx.Number != 6005 && sqlEx.Number != 10054) || (repeat.HasValue && repeat.Value))
						{
							repeat = false;
							if((sqlEx.Class == 11 && sqlEx.Number == 0) || (sqlEx.Class == 16 && sqlEx.Number == 3980))
								return false;
							else
								ProcessSqlEx(sqlEx, cmd);
						}
						else
							repeat = true;
					}
					catch(Exception ex)
					{
						repeat = false;
						ErrorMessage(ex, null, "CMD_GetField");
					}
			}
			if(id > 0)
			{
				return ExecNoError("sp_ОтправкаСообщения",
								   delegate(SqlCommand cmd)
								   {
									   cmd.Parameters.AddWithValue("@КодСообщения", id);
									   cmd.CommandType = CommandType.StoredProcedure;
								   });
			}
			return false;
		}

		public bool SendLotOfDocsMessage(string docIDs, int docsCount, int empID, string message, string recipientIDs,
										 string recipients, string recipientsEn, bool personal)
		{
			int id = GetIntField(
					@"DECLARE @MessID int
                DECLARE @Count int
                DECLARE @MessIDs (КодСообщения int)
                DECLARE @DocIDs (КодДокумента int)
                INSERT @DocIDs (КодДокумента int)
	            SELECT КодДокумента FROM vwДокументы (nolock) WHERE КодДокумента IN (" +
					docIDs +
					@")

	            IF (SELECT COUNT(*) FROM @DocIDs) < @DocsCount
                BEGIN
		            RAISERROR('Не все документы были найдены или доступны',16,1)
		            RETURN 
	            END
	            INSERT Сообщения
                    (Сообщение, КодДокумента, КодСотрудникаОтправителя, Отправитель, Sender, Персональное, Получатели, Recipients)
		            SELECT @Message, @DocID, КодСотрудника, ФИО, IOF, @Person, '', ''
		            FROM Инвентаризация.dbo.Сотрудники WHERE КодСотрудника = @EmpID
	            SET @MessID = SCOPE_IDENTITY()
	            IF @MessID IS NULL OR @MessID < 1
	            BEGIN
		            RAISERROR('Не удалось добавить сообщениe',16,1)
		            RETURN 
	            END
                ELSE
	            BEGIN
                    INSERT INSERT @MessIDs (КодСообщения int)
	                SELECT @MessID
	            END


	            INSERT dbo.ПолучателиСообщенийНаОтправку (КодСообщения, КодСотрудникаПолучателя) SELECT @MessID, КодСотрудника FROM Инвентаризация.dbo.Сотрудники 
		            WHERE КодСотрудника IN (" + recipientIDs + @")
	            IF @@ROWCOUNT = 0 
	            BEGIN
		            RAISERROR('Не удалось добавить получателей сообщения',16,1)
		            RETURN 
	            END
                SELECT @MessID " +
					identityField, identityField, delegate(SqlCommand cmd)
													  {
														  AddParam(cmd, "@DocsCount", SqlDbType.Int, docsCount);
														  AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
														  AddParam(cmd, "@Message", SqlDbType.NVarChar, message);
														  AddParam(cmd, "@Person", SqlDbType.TinyInt, (personal ? 1 : 0));
													  });
			if(id > 0)
			{
				return ExecNoError("sp_ОтправкаСообщения",
								   delegate(SqlCommand cmd)
								   {
									   cmd.Parameters.AddWithValue("@КодСообщения", id);
									   cmd.CommandType = CommandType.StoredProcedure;
								   });
			}
			return false;
		}

        #endregion
    }
}