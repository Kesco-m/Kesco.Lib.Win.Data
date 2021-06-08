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
        private const string sentField = "����������";
        private const string senderField = "�����������";
        private const string senderEngField = "Sender";
        private const string recipientsField = "����������";
        private const string recipientsEngField = "Recipients";
        private const string readField = "���������";

        private const string spMessages = "sp_���������";
        private const string spSendMessage = "sp_SendMessage";

        private const string directionField = "������������������������";
        private const string employeesField = "����������";
        private const string dateMessageField = "����/���������";
        private const string dateField = "����";
        private const string titleField = "���������";

        private const string originalEmployeesField = "������������������";
        private const string originalDateMessageField = "������������/���������";

        public MessageDALC(string connectionString)
            : base(connectionString)
        {
            idField = "������������";
            nameField = "���������";
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

            AddParam(cmd, "@������������", SqlDbType.Int, docID);

            return CMD_FillDT(cmd);
        }

		#endregion

		#region Change Data

		public bool SendOneRecipientMessage(int docID, int empID, string message, int recipientID, bool personal)
		{
			int id = 0;
			string sql = @"DECLARE @MessID int
	                IF NOT EXISTS(SELECT * FROM vw��������� (nolock) WHERE ������������ = @DocID)
                    BEGIN
						RAISERROR('�������� � ����� %d ��� �� ������ ��� �� ��������',12,1, @DocID)
						RETURN 
					END
					INSERT ���������
						(���������, ������������, ������������������������, �����������, Sender, ������������, ����������, Recipients)
						SELECT @Message, @DocID, Sender.�������������, Sender.���, Sender.IOF, @Person, Recipient.���, Recipient.IOF
						FROM ��������������.dbo.���������� Recipient CROSS JOIN ��������������.dbo.���������� Sender
						WHERE Sender.������������� = @EmpID AND Recipient.������������� = @RecipientID
					SET @MessID = SCOPE_IDENTITY()
					IF @MessID IS NULL OR @MessID < 1
					BEGIN
						RAISERROR('�� ������� �������� ��������e',16,1)
						RETURN 
					END
					INSERT dbo.����������������������������� (������������, �����������������������) SELECT @MessID, ������������� FROM ��������������.dbo.���������� 
						WHERE ������������� = @RecipientID
					IF @@ROWCOUNT = 0 
					BEGIN
						RAISERROR('�� ������� �������� ����������� ���������',16,1)
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
				return ExecNoError("sp_�����������������",
								   delegate (SqlCommand cmd)
								   {
									   cmd.Parameters.AddWithValue("@������������", id);
									   cmd.CommandType = CommandType.StoredProcedure;
								   });
			}
			return false;
		}

		public bool SendMessage2(int docID, int empID, string message, string recipientIDs, string recipients, string recipientsEn, bool personal)
		{
			int id = 0;
			string sql = @"DECLARE @MessID int
	            IF NOT EXISTS(SELECT * FROM vw��������� (nolock) WHERE ������������ = @DocID)
                BEGIN
		            RAISERROR('�������� � ����� %d ��� �� ������ ��� �� ��������',12,1, @DocID)
		            RETURN 
	            END
	            INSERT ���������
                    (���������, ������������, ������������������������, �����������, Sender, ������������, ����������, Recipients)
		            SELECT @Message, @DocID, �������������, ���, IOF, @Person, '', ''
		            FROM ��������������.dbo.���������� WHERE ������������� = @EmpID
	            SET @MessID = SCOPE_IDENTITY()
	            IF @MessID IS NULL OR @MessID < 1
	            BEGIN
		            RAISERROR('�� ������� �������� ��������e',16,1)
		            RETURN 
	            END
	            INSERT dbo.����������������������������� (������������, �����������������������) SELECT @MessID, ������������� FROM ��������������.dbo.���������� 
		            WHERE ������������� IN (" +
					recipientIDs +
					@")
	            IF @@ROWCOUNT = 0 
	            BEGIN
		            RAISERROR('�� ������� �������� ����������� ���������',16,1)
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
				return ExecNoError("sp_�����������������",
								   delegate(SqlCommand cmd)
								   {
									   cmd.Parameters.AddWithValue("@������������", id);
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
                DECLARE @MessIDs (������������ int)
                DECLARE @DocIDs (������������ int)
                INSERT @DocIDs (������������ int)
	            SELECT ������������ FROM vw��������� (nolock) WHERE ������������ IN (" +
					docIDs +
					@")

	            IF (SELECT COUNT(*) FROM @DocIDs) < @DocsCount
                BEGIN
		            RAISERROR('�� ��� ��������� ���� ������� ��� ��������',16,1)
		            RETURN 
	            END
	            INSERT ���������
                    (���������, ������������, ������������������������, �����������, Sender, ������������, ����������, Recipients)
		            SELECT @Message, @DocID, �������������, ���, IOF, @Person, '', ''
		            FROM ��������������.dbo.���������� WHERE ������������� = @EmpID
	            SET @MessID = SCOPE_IDENTITY()
	            IF @MessID IS NULL OR @MessID < 1
	            BEGIN
		            RAISERROR('�� ������� �������� ��������e',16,1)
		            RETURN 
	            END
                ELSE
	            BEGIN
                    INSERT INSERT @MessIDs (������������ int)
	                SELECT @MessID
	            END


	            INSERT dbo.����������������������������� (������������, �����������������������) SELECT @MessID, ������������� FROM ��������������.dbo.���������� 
		            WHERE ������������� IN (" + recipientIDs + @")
	            IF @@ROWCOUNT = 0 
	            BEGIN
		            RAISERROR('�� ������� �������� ����������� ���������',16,1)
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
				return ExecNoError("sp_�����������������",
								   delegate(SqlCommand cmd)
								   {
									   cmd.Parameters.AddWithValue("@������������", id);
									   cmd.CommandType = CommandType.StoredProcedure;
								   });
			}
			return false;
		}

        #endregion
    }
}