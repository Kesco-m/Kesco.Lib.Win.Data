using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Data.Documents
{
    public class DocImageDALC : DALC.DALC
    {
        private const string docIDField = "������������";
        private const string createDateField = "���������";
        private const string archiveIDField = "������������";
        private const string imageServerTableName = "���������.dbo.�����������������_���������������������";
        private const string logTableName = "���������.dbo.vw���������������������log";
        private const string creatorField = "��������";
        private const string printedField = "����������������";
        private const string typeSavedField = "�������������";
        private const string fileSizeField = "������";
        private const string pageCountField = "�������";
        private const string imageTypeField = "��������������";

        private const string serverIDField = "��������������������";
        private const string serverField = "����������������";
        private const string serverEngField = "ImageArchive";
        private const string networkPathField = "�����������";
        private const string netFileNameField = "����������������";
        private const string localField = "���������";
        private const string archiveEditField = "�����������������";
        private const string archiveEditorField = "����������������";

        private const string sendTimeField = "�������������";
        protected const string directionField = "Direction";
        private const string senderField = "�����������";
        private const string senderAddressField = "����������������";
        private const string recipientField = "����������";
        private const string recipientAddressField = "���������������";

        private const string spDocImageInsert = "���������.dbo.sp_���������������������_InsUpd";
        private const string spDocImageServersLocal = "���������.dbo.sp_��������������������������";
        private const string spDocImageServers = "���������.dbo.sp_��������������������������";
        private const string spDocImageCopy = "���������.dbo.sp_���������������������_Copy";
        private const string spDocImageDelete = "���������.dbo.sp_Delete_��������������������";

        // ��� �������� ������������ �����������
        private const string vw����������������������������� = "vw�����������������������������";
        private const string �����������������������Filed = "�����������������������";
        private const string �������������Filed = "�������������";
        private const string ������������Field = "������������";


        public DocImageDALC(string connectionString) : base(connectionString)
        {
            tableName = "���������.dbo.vw���������������������";

            idField = "�����������������������";
            nameField = "";
        }

        #region Accessors

        public string DocIDField
        {
            get { return docIDField; }
        }

        public string CreateDateField
        {
            get { return createDateField; }
        }

        public string ArchiveIDField
        {
            get { return archiveIDField; }
        }

        public string CreatorField
        {
            get { return creatorField; }
        }

        public string PrintedField
        {
            get { return printedField; }
        }

        public string FileSizeField
        {
            get { return fileSizeField; }
        }

        public string PageCountField
        {
            get { return pageCountField; }
        }

        public string ImageTypeField
        {
            get { return imageTypeField; }
        }

        public string ServerIDField
        {
            get { return serverIDField; }
        }

        public string ServerField
        {
            get { return serverField; }
        }

        public string ServerEngField
        {
            get { return serverEngField; }
        }

        public string NetworkPathField
        {
            get { return networkPathField; }
        }

        public string NetFileNameField
        {
            get { return netFileNameField; }
        }

        public string LocalField
        {
            get { return localField; }
        }

        public string SendTimeField
        {
            get { return sendTimeField; }
        }

        public string DirectionField
        {
            get { return directionField; }
        }

        public string SenderField
        {
            get { return senderField; }
        }

        public string SenderAddressField
        {
            get { return senderAddressField; }
        }

        public string RecipientField
        {
            get { return recipientField; }
        }

        public string RecipientAddressField
        {
            get { return recipientAddressField; }
        }

        public string ArchiveEditorField
        {
            get { return archiveEditorField; }
        }

        public string ArchiveEditField
        {
            get { return archiveEditField; }
        }

        #endregion

        #region Get Data

        public bool DocHasImages(int docID, bool canAdd)
        {
            return
                FieldExists(
                    " WHERE " + docIDField + " = @DocID" +
                    ((canAdd)
                         ? ""
                         : " AND " + printedField + " IS NULL AND NOT EXISTS (SELECT * FROM ����������������� p WHERE " +
                           tableName + "." + docIDField + " = p." + docIDField +
                           " AND " + tableName + "." + idField + " = p." + idField + ")")
                    ,
                    delegate(SqlCommand cmd)
                        {
                            AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                        });
        }

		public List<T> GetLocalServers<T>(GetResult getResult)
		{
			return GetRecords<T>(spDocImageServersLocal,
				delegate(SqlCommand cmd)
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 30;
				}, getResult);
		}

        public DataTable GetDocImages(int docID, bool canAdd)
        {
            return GetDataTable("SELECT " +
                                idField + ", " +
                                createDateField + ", " +
                                creatorField + ", " +
                                printedField + ", " +
                                archiveIDField + ", " +
                                editorField + ", " +
                                editedField + ", " +
                                imageTypeField +
                                " FROM " + tableName +
                                " WHERE " + tableName + "." + docIDField + " = @DocID" +
                                ((canAdd)
                                     ? ""
                                     : " AND " + printedField +
                                       " IS NULL AND NOT EXISTS (SELECT * FROM ����������������� p WHERE " + tableName +
                                       "." + docIDField + " = p." + docIDField +
                                       " AND " + tableName + "." + idField + " = p." + idField + ")") +
                                " ORDER BY " + createDateField + " DESC, " + idField + " DESC",
                                delegate(SqlCommand cmd)
                                    {
                                        AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                                    });
        }

        public DataTable GetDocImages(int docID)
        {
            return GetDocImages(docID, true);
        }

		public DataTable GetDocImageServers(int imageID)
		{
			return GetDataTable(spDocImageServers,
				delegate(SqlCommand cmd)
				{
					cmd.CommandType = CommandType.StoredProcedure;
					AddParam(cmd, "@�����������������������", SqlDbType.Int, imageID);
				});
		}

        public DataRow GetDocImage(int id)
        {
            return GetRow(id);
        }

        public DataRow GetLastDocImage(int empID)
        {
            return GetFirstRow("SELECT TOP 1 " +
                               idField + ", " +
                               docIDField +
                               " FROM " + tableName +
                               " WHERE " + editorField + " = @EmpID AND " + editedField +
                               " > DATEADD(n, - 3, getutcdate())" +
                               " ORDER BY " + editedField + " DESC",
                               delegate(SqlCommand cmd)
                                   {
                                       AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
                                   });
        }

        public List<Int32> GetLocalDocImageServers(int imageID, string localServers)
        {
            return GetRecords<Int32>("SELECT " + serverIDField + " FROM " + imageServerTableName +
                                     " WHERE " + idField + " = @����������������������� AND " + serverIDField + " IN (" +
                                     localServers + ")",
                                     delegate(SqlCommand cmd)
                                         { AddParam(cmd, "@�����������������������", SqlDbType.Int, imageID); });
        }

		public DataTable GetLogForImage(int imageID)
		{
			return GetDataTable("SELECT " +
								sendTimeField + ", " +
								directionField + ", " +
								senderField + ", " +
								senderAddressField + ", " +
								recipientField + ", " +
								recipientAddressField + ", " +
								descriptionField +
								" FROM " + logTableName +
								" WHERE (" + idField + " = @�����������������������)",
								delegate(SqlCommand cmd)
								{
									AddParam(cmd, "@�����������������������", SqlDbType.Int, imageID);
								});
		}

		//public DataSet GetPrintedDocImages(int docID)
		//{
		//    return GetData("SELECT " +
		//                   idField + ", " +
		//                   " FROM " + TableName +
		//                   " WHERE " + docIDField + " = @������������ AND " + printedField + " = 1",
		//                  cmd => AddParam(cmd, "@������������", SqlDbType.Int, docID));
		//}

        /// <summary>
        /// ��������, ��������� ������� �������� ����������� ���������.
        /// ���������� �������� ������������ �����������.
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="pageId"></param>
		public List<Tuple<int, int>> GetRotateOfSignedImage(int imageId)
		{
			if(imageId <= 0)
				return null;

			return GetRecords<Tuple<int, int>>("SELECT " + �������������Filed + "," + ������������Field +
								" FROM " + vw����������������������������� +
								" WHERE " + �����������������������Filed + " = @imageId",
								cmd => AddParam(cmd, "@imageId", SqlDbType.Int, imageId),
								dr => new Tuple<int, int>(Convert.ToInt32( dr[0]), (int)dr[1]));
								
		}

		/// <summary>
		/// ��������, ��������� ������� �������� ����������� ���������.
		/// ���������� �������� ������������ �����������.
		/// </summary>
		/// <param name="imageId"></param>
		/// <param name="pageId"></param>
		public int? GetRotateOfSignedImage(int imageId, int pageId)
		{
			if(imageId <= 0)
				return null;

			return GetIntField("SELECT " + ������������Field +
							   " FROM " + vw����������������������������� +
							   " WHERE " + �����������������������Filed + " = @imageId AND " + �������������Filed + " = @pageId ",
							   ������������Field,
							   cmd =>
							   {
								   AddParam(cmd, "@imageId", SqlDbType.Int, imageId);
								   AddParam(cmd, "@pageId", SqlDbType.Int, pageId);
							   });
		}

        /// <summary>
        /// ����������� ������� ������� �����������
        /// </summary>
        /// <param name="imageId">��� �����������</param>
        public bool IsImageSigned(int imageId)
        {
            // �������� ��������
			return FieldExists(@"WHERE vw���������������������.����������������������� = @�����������������������
		  AND EXISTS(SELECT * FROM ����������������� (nolock) WHERE	--������� ��.�. ����������� ����� ��������� �����������
              				(�����������������.������������ = vw���������������������.������������
              				AND �����������������.����������������������� IS NULL
              				AND �����������������.���� > vw���������������������.��������))",
			delegate(SqlCommand cmd)
			{
				AddParam(cmd, "@�����������������������", SqlDbType.Int, imageId);
			});
        }

        #endregion

        #region Change Data

        public bool SetDocImageProperties(int imageID, int docID, int archiveID)
        {
            if (docID <= 0 || imageID <= 0)
                return false;

            return SetField(archiveIDField, SqlDbType.Int, imageID, archiveID);
        }

        /// <summary>
        /// ����� ��������� �������� ��� ��������� �����������
        /// </summary>
        /// <param name="imageServer">������ �������� �����������</param>
        /// <param name="fileName">��� �����</param>
        /// <param name="imageID">��� ����������� ���������</param>
        /// <param name="docID">��� ���������</param>
        /// <param name="docTypeID">��� ���� ���������</param>
        /// <param name="name">�������� ���������</param>
        /// <param name="date">���� ���������</param>
        /// <param name="number">����� ���������</param>
        /// <param name="descr">�������� ���������</param>
        /// <param name="protect">�������-�� ��������</param>
        /// <param name="scanDate">���� ������������ ���������</param>
        /// <param name="archiveID">��� ��������� ���������</param>
        /// <param name="mainImage">�������� �����������</param>
        /// <param name="pagesCount">���������� �������</param>
        /// <returns></returns>
       

        public bool DocImageInsert(int imageServer, string fileName, ref int imageID, ref int docID, int docTypeID,
                                   string name, DateTime date, string number, string descr, bool protect,
                                   DateTime scanDate, int archiveID, bool mainImage, string imageType, int pagesCount)
        {
            return DocImageInsert(imageServer, fileName, ref imageID, ref docID, docTypeID, name, date, number, descr,
                                  protect, scanDate, archiveID, mainImage, 0, imageType, pagesCount);
        }

        /// <summary>
        /// ����� ��������� �������� ��� ��������� �����������
        /// </summary>
        /// <param name="imageServer">������ �������� �����������</param>
        /// <param name="fileName">��� �����</param>
        /// <param name="imageID">��� ����������� ���������</param>
        /// <param name="docID">��� ���������</param>
        /// <param name="docTypeID">��� ���� ���������</param>
        /// <param name="name">�������� ���������</param>
        /// <param name="date">���� ���������</param>
        /// <param name="number">����� ���������</param>
        /// <param name="descr">�������� ���������</param>
        /// <param name="protect">�������-�� ��������</param>
        /// <param name="scanDate">���� ������������ ���������</param>
        /// <param name="archiveID">��� ��������� ���������</param>
        /// <param name="mainImage">�������� �����������</param>
        /// <param name="printed">������������ ��. �����</param>
        /// <param name="pagesCount">���������� �������</param>
        /// <returns></returns>
        public bool DocImageInsert(ref int imageID, ref int docID, int docTypeID, string name, DateTime date,
                                   string number, string descr, bool protect, DateTime scanDate, int archiveID,
								   bool mainImage, string imageType, int pagesCount)
        {
            return DocImageInsert(0, "", ref imageID, ref docID, docTypeID, name, date, number, descr, protect, scanDate,
                                  archiveID, mainImage, 0, imageType, pagesCount);
        }

        /// <summary>
        /// ����� ��������� �������� ��� ��������� �����������
        /// </summary>
        /// <param name="imageID">��� ����������� ���������</param>
        /// <param name="docID">��� ���������</param>
        /// <param name="docTypeID">��� ���� ���������</param>
        /// <param name="name">�������� ���������</param>
        /// <param name="date">���� ���������</param>
        /// <param name="number">����� ���������</param>
        /// <param name="descr">�������� ���������</param>
        /// <param name="protect">�������-�� ��������</param>
        /// <param name="scanDate">���� ������������ ���������</param>
        /// <param name="archiveID">��� ��������� ���������</param>
        /// <param name="mainImage">�������� �����������</param>
        /// <param name="pagesCount">���������� �������</param>
        /// <returns></returns>
        public bool DocImageInsert(int imageServer, string fileName, ref int imageID, ref int docID, int docTypeID,
                                   string name, DateTime date, string number, string descr, bool protect,
                                   DateTime scanDate, int archiveID, bool mainImage, int printed, string imageType,
                                   int pagesCount)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(spDocImageInsert, conn) {CommandType = CommandType.StoredProcedure})
            {
                AddParam(cmd, "@��������������������", SqlDbType.Int, imageServer);
                AddParam(cmd, "@FileName", SqlDbType.VarChar, fileName);

                if (docID != 0)
                    AddParam(cmd, "@������������", SqlDbType.Int, docID);
                else
                    AddParam(cmd, "@������������", SqlDbType.Int, DBNull.Value);
                cmd.Parameters["@������������"].Direction = ParameterDirection.InputOutput;

                if (imageID > 0)
                    AddParam(cmd, "@�����������������������", SqlDbType.Int, imageID);
                else
                    AddParam(cmd, "@�����������������������", SqlDbType.Int, DBNull.Value);
                cmd.Parameters["@�����������������������"].Direction = ParameterDirection.InputOutput;

                AddParam(cmd, "@����������������", SqlDbType.Int, docTypeID);

                AddParam(cmd, "@�����������������", SqlDbType.NVarChar, name);

                if (date != DateTime.MinValue)
                    AddParam(cmd, "@�������������", SqlDbType.DateTime, date);

                AddParam(cmd, "@��������������", SqlDbType.NVarChar, number);
                AddParam(cmd, "@��������", SqlDbType.NVarChar, descr);
                AddParam(cmd, "@�������", SqlDbType.TinyInt, (protect ? (byte) 1 : (byte) 0));

                if (printed > 0)
                    AddParam(cmd, "@����������������", SqlDbType.Int, printed);

                if (archiveID != 0)
                    AddParam(cmd, "@������������", SqlDbType.Int, archiveID);

                AddParam(cmd, "@�������������������", SqlDbType.Bit, mainImage ? 1 : 0);
                AddParam(cmd, "@��������������", SqlDbType.Char, imageType);
                AddParam(cmd, "@�������", SqlDbType.Int, pagesCount);

                bool retValue = CMD_Exec(cmd);

                object obj = cmd.Parameters["@�����������������������"].Value;
                if (obj != null && !obj.Equals(DBNull.Value) && obj is int)
                    imageID = (int) obj;

                obj = cmd.Parameters["@������������"].Value;
                if (obj != null && !obj.Equals(DBNull.Value) && obj is int)
                    docID = (int) obj;

                return retValue;
            }
        }

        public bool DocImageFileUpdateExtError(int imageServer, string fileName, int docID, int imageID,
                                               string imageType, int pagesCount)
        {
            return Exec(spDocImageInsert, delegate(SqlCommand cmd)
                                              {
                                                  cmd.CommandType = CommandType.StoredProcedure;
                                                  AddParam(cmd, "@��������������������", SqlDbType.Int, imageServer);
                                                  AddParam(cmd, "@FileName", SqlDbType.VarChar, fileName);
                                                  AddParam(cmd, "@��������������", SqlDbType.Char, imageType);
                                                  AddParam(cmd, "@�������", SqlDbType.Int, pagesCount);

                                                  if (docID > 0)
                                                      AddParam(cmd, "@������������", SqlDbType.Int, docID);
                                                  else
                                                      throw new Exception("��� ��������� �� �������������");
                                                  cmd.Parameters["@������������"].Direction =
                                                      ParameterDirection.InputOutput;

                                                  if (imageID > 0)
                                                      AddParam(cmd, "@�����������������������", SqlDbType.Int, imageID);
                                                  else
                                                      throw new Exception("��� ����������� ��������� �� �������������");
                                                  cmd.Parameters["@�����������������������"].Direction =
                                                      ParameterDirection.InputOutput;
                                              }, null, delegate(SqlException sex)
                                                           {
                                                               if (sex.Number == 50000 &&
                                                                   sex.Message.EndsWith("Delete 15") ||
                                                                   sex.Message.EndsWith("Error 2")) return true;
                                                               return false;
                                                           });
        }

        public bool DocImageFileUpdate(int imageServer, string fileName, int docID, int imageID, string imageType,
                                       int pagesCount)
        {
            return Exec(spDocImageInsert, delegate(SqlCommand cmd)
                                              {
                                                  cmd.CommandType = CommandType.StoredProcedure;
                                                  AddParam(cmd, "@��������������������", SqlDbType.Int, imageServer);
                                                  AddParam(cmd, "@FileName", SqlDbType.VarChar, fileName);
                                                  AddParam(cmd, "@��������������", SqlDbType.Char, imageType);
                                                  AddParam(cmd, "@�������", SqlDbType.Int, pagesCount);

                                                  if (docID > 0)
                                                      AddParam(cmd, "@������������", SqlDbType.Int, docID);
                                                  else
                                                      throw new Exception("��� ��������� �� �������������");
                                                  cmd.Parameters["@������������"].Direction =
                                                      ParameterDirection.InputOutput;

                                                  AddParam(cmd, "@�������������������", SqlDbType.Bit, 0);
               
                                                  if (imageID > 0)
                                                      AddParam(cmd, "@�����������������������", SqlDbType.Int, imageID);
                                                  else
                                                      throw new Exception("��� ����������� ��������� �� �������������");
                                                  cmd.Parameters["@�����������������������"].Direction =
                                                      ParameterDirection.InputOutput;
                                              });
        }

        public bool DocImageCopy(int imageServer, int imageID)
        {
            return Exec(spDocImageCopy, delegate(SqlCommand cmd)
                                            {
                                                cmd.CommandType = CommandType.StoredProcedure;
                                                if (imageID > 0)
                                                    AddParam(cmd, "@�����������������������", SqlDbType.Int, imageID);
                                                else
                                                    throw new Exception("��� ��������� �� �������������");
                                                AddParam(cmd, "@��������������������To", SqlDbType.Int, imageServer);

                                                AddParam(cmd, "@Wait", SqlDbType.Int, 1);
                                                cmd.Parameters["@Wait"].Direction = ParameterDirection.InputOutput;
                                            },
                        delegate(SqlCommand cmd)
                            {
                                if ((cmd.Parameters["@Wait"].Value).Equals(1))
                                    return true;

                                MessageBox.Show(
                                    "�������� ��������� � ������� �� ��������\n � � ��������� ����� �������� � ��������� ������");
                                return false;
                            });
        }

        public override bool Delete(int id)
        {
            return Exec(spDocImageDelete, delegate(SqlCommand cmd)
                                              {
                                                  cmd.CommandType = CommandType.StoredProcedure;
                                                  AddParam(cmd, "@�����������������������", SqlDbType.Int, id);
                                              });
        }

        /// <summary>
        /// ��������, ��������� ������� �������� ����������� ���������.
        /// ���������� �������� ������������ �����������.
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="pageId"></param>
        /// <param name="angle">����������� ���� �������� �����������</param>
        public void ApplyRotateToSignedImage(int imageId, int pageId, int angle)
        {
            Exec("IF EXISTS(SELECT * FROM " + vw����������������������������� + " WHERE " + �����������������������Filed + " = @����������������������� AND " + �������������Filed + " = @�������������) " + Environment.NewLine +
                "UPDATE " + vw����������������������������� + Environment.NewLine +
                "SET " + ������������Field + " = @������������" + Environment.NewLine +
                "WHERE " + �����������������������Filed + "  = @����������������������� AND " + �������������Filed + " = @�������������" + Environment.NewLine +
                "ELSE" + Environment.NewLine +
                "INSERT " + vw����������������������������� + " (" + �����������������������Filed + ", " + �������������Filed + ", " + ������������Field + ")" + Environment.NewLine +
                "VALUES(@�����������������������, @�������������, @������������)"
            ,
            delegate(SqlCommand cmd)
            {
                AddParam(cmd, "@�����������������������", SqlDbType.Int, imageId);
                AddParam(cmd, "@�������������", SqlDbType.Int, pageId);
                AddParam(cmd, "@������������", SqlDbType.Int, angle);
            });
        }

        #endregion
    }
}