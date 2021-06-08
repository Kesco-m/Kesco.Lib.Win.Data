using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Data.Documents
{
    public class DocImageDALC : DALC.DALC
    {
        private const string docIDField = "КодДокумента";
        private const string createDateField = "Сохранено";
        private const string archiveIDField = "КодХранилища";
        private const string imageServerTableName = "Документы.dbo.АрхивыИзображений_ИзображенияДокументов";
        private const string logTableName = "Документы.dbo.vwИзображенияДокументовlog";
        private const string creatorField = "Сохранил";
        private const string printedField = "КодПечатнойФормы";
        private const string typeSavedField = "ТипСохранения";
        private const string fileSizeField = "Размер";
        private const string pageCountField = "Страниц";
        private const string imageTypeField = "ТипИзображения";

        private const string serverIDField = "КодАрхиваИзображений";
        private const string serverField = "АрхивИзображений";
        private const string serverEngField = "ImageArchive";
        private const string networkPathField = "СетевойПуть";
        private const string netFileNameField = "СетевойПутьФайла";
        private const string localField = "Локальный";
        private const string archiveEditField = "ИзмененоХранилище";
        private const string archiveEditorField = "ИзменилХранилище";

        private const string sendTimeField = "ВремяОтправки";
        protected const string directionField = "Direction";
        private const string senderField = "Отправитель";
        private const string senderAddressField = "АдресОтправителя";
        private const string recipientField = "Получатель";
        private const string recipientAddressField = "АдресПолучателя";

        private const string spDocImageInsert = "Документы.dbo.sp_ИзображенияДокументов_InsUpd";
        private const string spDocImageServersLocal = "Документы.dbo.sp_АрхивыИзображенийЛокальные";
        private const string spDocImageServers = "Документы.dbo.sp_АрхивыИзображенияДокумента";
        private const string spDocImageCopy = "Документы.dbo.sp_ИзображенияДокументов_Copy";
        private const string spDocImageDelete = "Документы.dbo.sp_Delete_ИзображениеДокумента";

        // Для поворота подписанного изображения
        private const string vwИзображенияДокументовСтраницы = "vwИзображенияДокументовСтраницы";
        private const string КодИзображенияДокументаFiled = "КодИзображенияДокумента";
        private const string НомерСтраницыFiled = "НомерСтраницы";
        private const string УголПоворотаField = "УголПоворота";


        public DocImageDALC(string connectionString) : base(connectionString)
        {
            tableName = "Документы.dbo.vwИзображенияДокументов";

            idField = "КодИзображенияДокумента";
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
                         : " AND " + printedField + " IS NULL AND NOT EXISTS (SELECT * FROM ПодписиДокументов p WHERE " +
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
                                       " IS NULL AND NOT EXISTS (SELECT * FROM ПодписиДокументов p WHERE " + tableName +
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
					AddParam(cmd, "@КодИзображенияДокумента", SqlDbType.Int, imageID);
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
                                     " WHERE " + idField + " = @КодИзображенияДокумента AND " + serverIDField + " IN (" +
                                     localServers + ")",
                                     delegate(SqlCommand cmd)
                                         { AddParam(cmd, "@КодИзображенияДокумента", SqlDbType.Int, imageID); });
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
								" WHERE (" + idField + " = @КодИзображенияДокумента)",
								delegate(SqlCommand cmd)
								{
									AddParam(cmd, "@КодИзображенияДокумента", SqlDbType.Int, imageID);
								});
		}

		//public DataSet GetPrintedDocImages(int docID)
		//{
		//    return GetData("SELECT " +
		//                   idField + ", " +
		//                   " FROM " + TableName +
		//                   " WHERE " + docIDField + " = @КодДокумента AND " + printedField + " = 1",
		//                  cmd => AddParam(cmd, "@КодДокумента", SqlDbType.Int, docID));
		//}

        /// <summary>
        /// Создание, изменение свойств страницы изображения документа.
        /// Реализация поворота подписанного изображения.
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="pageId"></param>
		public List<Tuple<int, int>> GetRotateOfSignedImage(int imageId)
		{
			if(imageId <= 0)
				return null;

			return GetRecords<Tuple<int, int>>("SELECT " + НомерСтраницыFiled + "," + УголПоворотаField +
								" FROM " + vwИзображенияДокументовСтраницы +
								" WHERE " + КодИзображенияДокументаFiled + " = @imageId",
								cmd => AddParam(cmd, "@imageId", SqlDbType.Int, imageId),
								dr => new Tuple<int, int>(Convert.ToInt32( dr[0]), (int)dr[1]));
								
		}

		/// <summary>
		/// Создание, изменение свойств страницы изображения документа.
		/// Реализация поворота подписанного изображения.
		/// </summary>
		/// <param name="imageId"></param>
		/// <param name="pageId"></param>
		public int? GetRotateOfSignedImage(int imageId, int pageId)
		{
			if(imageId <= 0)
				return null;

			return GetIntField("SELECT " + УголПоворотаField +
							   " FROM " + vwИзображенияДокументовСтраницы +
							   " WHERE " + КодИзображенияДокументаFiled + " = @imageId AND " + НомерСтраницыFiled + " = @pageId ",
							   УголПоворотаField,
							   cmd =>
							   {
								   AddParam(cmd, "@imageId", SqlDbType.Int, imageId);
								   AddParam(cmd, "@pageId", SqlDbType.Int, pageId);
							   });
		}

        /// <summary>
        /// Определение наличия подписи изображения
        /// </summary>
        /// <param name="imageId">Код изображения</param>
        public bool IsImageSigned(int imageId)
        {
            // Проверка подписей
			return FieldExists(@"WHERE vwИзображенияДокументов.КодИзображенияДокумента = @КодИзображенияДокумента
		  AND EXISTS(SELECT * FROM ПодписиДокументов (nolock) WHERE	--наличие эл.ф. подписанной позже изменения изображения
              				(ПодписиДокументов.КодДокумента = vwИзображенияДокументов.КодДокумента
              				AND ПодписиДокументов.КодИзображенияДокумента IS NULL
              				AND ПодписиДокументов.Дата > vwИзображенияДокументов.Изменено))",
			delegate(SqlCommand cmd)
			{
				AddParam(cmd, "@КодИзображенияДокумента", SqlDbType.Int, imageId);
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
        /// Вызов процедуры создания или изменения изображения
        /// </summary>
        /// <param name="imageServer">сервер хранения изображения</param>
        /// <param name="fileName">имя файла</param>
        /// <param name="imageID">код изображения документа</param>
        /// <param name="docID">код документа</param>
        /// <param name="docTypeID">код типа документа</param>
        /// <param name="name">название документа</param>
        /// <param name="date">дата документа</param>
        /// <param name="number">номер документа</param>
        /// <param name="descr">описание документа</param>
        /// <param name="protect">зашишен-ли документ</param>
        /// <param name="scanDate">дата сканирования документа</param>
        /// <param name="archiveID">код хранилища оригинала</param>
        /// <param name="mainImage">основное изображение</param>
        /// <param name="pagesCount">количество страниц</param>
        /// <returns></returns>
       

        public bool DocImageInsert(int imageServer, string fileName, ref int imageID, ref int docID, int docTypeID,
                                   string name, DateTime date, string number, string descr, bool protect,
                                   DateTime scanDate, int archiveID, bool mainImage, string imageType, int pagesCount)
        {
            return DocImageInsert(imageServer, fileName, ref imageID, ref docID, docTypeID, name, date, number, descr,
                                  protect, scanDate, archiveID, mainImage, 0, imageType, pagesCount);
        }

        /// <summary>
        /// Вызов процедуры создания или изменения изображения
        /// </summary>
        /// <param name="imageServer">сервер хранения изображения</param>
        /// <param name="fileName">имя файла</param>
        /// <param name="imageID">код изображения документа</param>
        /// <param name="docID">код документа</param>
        /// <param name="docTypeID">код типа документа</param>
        /// <param name="name">название документа</param>
        /// <param name="date">дата документа</param>
        /// <param name="number">номер документа</param>
        /// <param name="descr">описание документа</param>
        /// <param name="protect">зашишен-ли документ</param>
        /// <param name="scanDate">дата сканирования документа</param>
        /// <param name="archiveID">код хранилища оригинала</param>
        /// <param name="mainImage">основное изображение</param>
        /// <param name="printed">распечатаная эл. форма</param>
        /// <param name="pagesCount">количество страниц</param>
        /// <returns></returns>
        public bool DocImageInsert(ref int imageID, ref int docID, int docTypeID, string name, DateTime date,
                                   string number, string descr, bool protect, DateTime scanDate, int archiveID,
								   bool mainImage, string imageType, int pagesCount)
        {
            return DocImageInsert(0, "", ref imageID, ref docID, docTypeID, name, date, number, descr, protect, scanDate,
                                  archiveID, mainImage, 0, imageType, pagesCount);
        }

        /// <summary>
        /// Вызов процедуры создания или изменения изображения
        /// </summary>
        /// <param name="imageID">код изображения документа</param>
        /// <param name="docID">код документа</param>
        /// <param name="docTypeID">код типа документа</param>
        /// <param name="name">название документа</param>
        /// <param name="date">дата документа</param>
        /// <param name="number">номер документа</param>
        /// <param name="descr">описание документа</param>
        /// <param name="protect">зашишен-ли документ</param>
        /// <param name="scanDate">дата сканирования документа</param>
        /// <param name="archiveID">код хранилища оригинала</param>
        /// <param name="mainImage">основное изображение</param>
        /// <param name="pagesCount">количество страниц</param>
        /// <returns></returns>
        public bool DocImageInsert(int imageServer, string fileName, ref int imageID, ref int docID, int docTypeID,
                                   string name, DateTime date, string number, string descr, bool protect,
                                   DateTime scanDate, int archiveID, bool mainImage, int printed, string imageType,
                                   int pagesCount)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(spDocImageInsert, conn) {CommandType = CommandType.StoredProcedure})
            {
                AddParam(cmd, "@КодАрхиваИзображений", SqlDbType.Int, imageServer);
                AddParam(cmd, "@FileName", SqlDbType.VarChar, fileName);

                if (docID != 0)
                    AddParam(cmd, "@КодДокумента", SqlDbType.Int, docID);
                else
                    AddParam(cmd, "@КодДокумента", SqlDbType.Int, DBNull.Value);
                cmd.Parameters["@КодДокумента"].Direction = ParameterDirection.InputOutput;

                if (imageID > 0)
                    AddParam(cmd, "@КодИзображенияДокумента", SqlDbType.Int, imageID);
                else
                    AddParam(cmd, "@КодИзображенияДокумента", SqlDbType.Int, DBNull.Value);
                cmd.Parameters["@КодИзображенияДокумента"].Direction = ParameterDirection.InputOutput;

                AddParam(cmd, "@КодТипаДокумента", SqlDbType.Int, docTypeID);

                AddParam(cmd, "@НазваниеДокумента", SqlDbType.NVarChar, name);

                if (date != DateTime.MinValue)
                    AddParam(cmd, "@ДатаДокумента", SqlDbType.DateTime, date);

                AddParam(cmd, "@НомерДокумента", SqlDbType.NVarChar, number);
                AddParam(cmd, "@Описание", SqlDbType.NVarChar, descr);
                AddParam(cmd, "@Защищен", SqlDbType.TinyInt, (protect ? (byte) 1 : (byte) 0));

                if (printed > 0)
                    AddParam(cmd, "@КодПечатнойФормы", SqlDbType.Int, printed);

                if (archiveID != 0)
                    AddParam(cmd, "@КодХранилища", SqlDbType.Int, archiveID);

                AddParam(cmd, "@ОсновноеИзображение", SqlDbType.Bit, mainImage ? 1 : 0);
                AddParam(cmd, "@ТипИзображения", SqlDbType.Char, imageType);
                AddParam(cmd, "@Страниц", SqlDbType.Int, pagesCount);

                bool retValue = CMD_Exec(cmd);

                object obj = cmd.Parameters["@КодИзображенияДокумента"].Value;
                if (obj != null && !obj.Equals(DBNull.Value) && obj is int)
                    imageID = (int) obj;

                obj = cmd.Parameters["@КодДокумента"].Value;
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
                                                  AddParam(cmd, "@КодАрхиваИзображений", SqlDbType.Int, imageServer);
                                                  AddParam(cmd, "@FileName", SqlDbType.VarChar, fileName);
                                                  AddParam(cmd, "@ТипИзображения", SqlDbType.Char, imageType);
                                                  AddParam(cmd, "@Страниц", SqlDbType.Int, pagesCount);

                                                  if (docID > 0)
                                                      AddParam(cmd, "@КодДокумента", SqlDbType.Int, docID);
                                                  else
                                                      throw new Exception("Код документа не положительный");
                                                  cmd.Parameters["@КодДокумента"].Direction =
                                                      ParameterDirection.InputOutput;

                                                  if (imageID > 0)
                                                      AddParam(cmd, "@КодИзображенияДокумента", SqlDbType.Int, imageID);
                                                  else
                                                      throw new Exception("Код изображения документа не положительный");
                                                  cmd.Parameters["@КодИзображенияДокумента"].Direction =
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
                                                  AddParam(cmd, "@КодАрхиваИзображений", SqlDbType.Int, imageServer);
                                                  AddParam(cmd, "@FileName", SqlDbType.VarChar, fileName);
                                                  AddParam(cmd, "@ТипИзображения", SqlDbType.Char, imageType);
                                                  AddParam(cmd, "@Страниц", SqlDbType.Int, pagesCount);

                                                  if (docID > 0)
                                                      AddParam(cmd, "@КодДокумента", SqlDbType.Int, docID);
                                                  else
                                                      throw new Exception("Код документа не положительный");
                                                  cmd.Parameters["@КодДокумента"].Direction =
                                                      ParameterDirection.InputOutput;

                                                  AddParam(cmd, "@ОсновноеИзображение", SqlDbType.Bit, 0);
               
                                                  if (imageID > 0)
                                                      AddParam(cmd, "@КодИзображенияДокумента", SqlDbType.Int, imageID);
                                                  else
                                                      throw new Exception("Код изображения документа не положительный");
                                                  cmd.Parameters["@КодИзображенияДокумента"].Direction =
                                                      ParameterDirection.InputOutput;
                                              });
        }

        public bool DocImageCopy(int imageServer, int imageID)
        {
            return Exec(spDocImageCopy, delegate(SqlCommand cmd)
                                            {
                                                cmd.CommandType = CommandType.StoredProcedure;
                                                if (imageID > 0)
                                                    AddParam(cmd, "@КодИзображенияДокумента", SqlDbType.Int, imageID);
                                                else
                                                    throw new Exception("Код документа не положительный");
                                                AddParam(cmd, "@КодАрхиваИзображенийTo", SqlDbType.Int, imageServer);

                                                AddParam(cmd, "@Wait", SqlDbType.Int, 1);
                                                cmd.Parameters["@Wait"].Direction = ParameterDirection.InputOutput;
                                            },
                        delegate(SqlCommand cmd)
                            {
                                if ((cmd.Parameters["@Wait"].Value).Equals(1))
                                    return true;

                                MessageBox.Show(
                                    "Документ поставлен в очередь на загрузку\n и в ближайшее время появится в локальном архиве");
                                return false;
                            });
        }

        public override bool Delete(int id)
        {
            return Exec(spDocImageDelete, delegate(SqlCommand cmd)
                                              {
                                                  cmd.CommandType = CommandType.StoredProcedure;
                                                  AddParam(cmd, "@КодИзображенияДокумента", SqlDbType.Int, id);
                                              });
        }

        /// <summary>
        /// Создание, изменение свойств страницы изображения документа.
        /// Реализация поворота подписанного изображения.
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="pageId"></param>
        /// <param name="angle">Виртуальный угол поворота изображения</param>
        public void ApplyRotateToSignedImage(int imageId, int pageId, int angle)
        {
            Exec("IF EXISTS(SELECT * FROM " + vwИзображенияДокументовСтраницы + " WHERE " + КодИзображенияДокументаFiled + " = @КодИзображенияДокумента AND " + НомерСтраницыFiled + " = @НомерСтраницы) " + Environment.NewLine +
                "UPDATE " + vwИзображенияДокументовСтраницы + Environment.NewLine +
                "SET " + УголПоворотаField + " = @УголПоворота" + Environment.NewLine +
                "WHERE " + КодИзображенияДокументаFiled + "  = @КодИзображенияДокумента AND " + НомерСтраницыFiled + " = @НомерСтраницы" + Environment.NewLine +
                "ELSE" + Environment.NewLine +
                "INSERT " + vwИзображенияДокументовСтраницы + " (" + КодИзображенияДокументаFiled + ", " + НомерСтраницыFiled + ", " + УголПоворотаField + ")" + Environment.NewLine +
                "VALUES(@КодИзображенияДокумента, @НомерСтраницы, @УголПоворота)"
            ,
            delegate(SqlCommand cmd)
            {
                AddParam(cmd, "@КодИзображенияДокумента", SqlDbType.Int, imageId);
                AddParam(cmd, "@НомерСтраницы", SqlDbType.Int, pageId);
                AddParam(cmd, "@УголПоворота", SqlDbType.Int, angle);
            });
        }

        #endregion
    }
}