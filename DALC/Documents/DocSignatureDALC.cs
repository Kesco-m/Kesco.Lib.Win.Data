using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using Kesco.Lib.Win.Data.Temp.Objects;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	/// <summary>
	/// DAL-компонент для доступа к подписям документа
	/// </summary>
	public class DocSignatureDALC : DALC
	{
		private string docIDField;
		private string imgIDField;
		private const string docSingatureTypeField = "ТипПодписи";
		private const string dataField = "Дата";

		public DocSignatureDALC(string connectionString) : base(connectionString)
		{
			tableName = "Документы.dbo.ПодписиДокументов";

			idField = "КодПодписиДокумента";
			nameField = "ПодписьДокумента"; // поле отсутствует

			docIDField = "КодДокумента";
			imgIDField = "КодИзображенияДокумента";
		}

		#region Accessories

		public string DocumentSingatureTypeField
		{
			get { return docSingatureTypeField; }
		}

		public string DocumentIDField
		{
			get { return docIDField; }
		}

		public string ImageIdField
		{
			get { return imgIDField; }
		}

		public string DataField
		{
			get { return dataField; }
		}

		#endregion

		#region Get Data

		/// <summary>
		/// Определение наличия подписи у документа
		/// </summary>
		/// <param name="docID">Код документа</param>
		public bool IsDocSigned(int docID)
		{
			return IsDocSigned(docID, false);
		}

		/// <summary>
		/// Определение наличия подписи у документа
		/// </summary>
		/// <param name="docID">Код документа</param>
		public bool IsDocSigned(int docID, bool Finished)
		{
			return FieldExists(" WHERE " + docIDField + " = @DocID" + " AND " + (Finished ? docSingatureTypeField + " = 1" : docSingatureTypeField + " <> 101"),
					delegate(SqlCommand cmd)
					{
						AddParam(cmd, "@DocID", SqlDbType.Int, docID);
					});
		}

		/// <summary>
		/// Определение наличия подписи у изображения документа
		/// </summary>
		/// <param name="docID">Код документа</param>
		public bool IsImageSigned(int imageID)
		{
			return FieldExists(" WHERE " + imgIDField + " = @ImageID",
					delegate(SqlCommand cmd)
					{
						AddParam(cmd, "@ImageID", SqlDbType.Int, imageID);
					});
		}

		/// <summary>
		/// Определение наличия штампа "ДСП" у изображения документа
		/// </summary>
		/// <param name="docID">Код документа</param>
		public bool IsDocSignedDSP(int docID, int imageID)
		{
			return FieldExists(" WHERE " + docIDField + " = @DocID AND " + ImageIdField + " = @ImageID AND " + docSingatureTypeField + " = 101",
					delegate(SqlCommand cmd)
					{
						AddParam(cmd, "@DocID", SqlDbType.Int, docID);
						AddParam(cmd, "@ImageID", SqlDbType.Int, imageID);
					});
		}

        /// <summary>
        /// Определение наличия аннулирующей подписи у изображения документа
        /// </summary>
        /// <param name="docID">Код документа</param>
		public bool IsDocSignedAnnuled(/*int docID,*/ int imageID)
		{
			if(imageID <= 0) return false;

			return FieldExists(" WHERE " + ImageIdField + " = @ImageID AND " + docSingatureTypeField + " = 2",
					delegate(SqlCommand cmd)
					{
						AddParam(cmd, "@ImageID", SqlDbType.Int, imageID);
					});
		}

		/// <summary>
		/// Определение наличия подписи изображения
		/// </summary>
		/// <param name="docID">Код документа</param>
		/// <param name="imgId">Код изображения (null - наличие подписи электронной формы документа)</param>
		public bool IsImageSign(int docID, int? imgId)
		{
			string imgCondition = imgId.HasValue ? (imgIDField + " = @ImgId AND " + docSingatureTypeField + " <> 101") : (imgIDField + " IS NULL ");

			string sql = string.Format(" WHERE {2} = @DocID AND {3}",
									   idField, tableName, docIDField, imgCondition);
			return FieldExists(sql, delegate(SqlCommand cmd)
										{
											AddParam(cmd, "@DocID", SqlDbType.Int, docID);
											if(imgId.HasValue)
												AddParam(cmd, "@ImgId", SqlDbType.Int, docID);
										});
		}

		/// <summary>
		/// Запрос для определения наличия подписей у документа и изображения документа
		/// </summary>
		/// <param name="docID">Код документа</param>
		/// <param name="isEformSign">Имеется подпись эл. формы</param>
		/// <param name="isDocSign">На документе есть подпись</param>
		public void IsSigns(int docID, out bool isEformSign, out bool isDocSign)
		{
			isEformSign = false;
			isDocSign = false;
			string query = "SELECT " + idField + ", " + imgIDField + " FROM " + tableName + " WHERE " + docSingatureTypeField + " <> 101 AND " + docIDField +
						   " = " + docID.ToString();
			using(var cmd = new SqlCommand(query, new SqlConnection(connectionString)))
			{
				cmd.CommandTimeout = cmdTimeout;
				try
				{
					cmd.Connection.Open();
					using(SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
					{
						if(dr.HasRows)
						{
							isDocSign = true;
							while(dr.Read())
							{
								if(dr.IsDBNull(1))
									isEformSign = true;
							}
						}
						dr.Close();
					}
				}
				catch(SqlException sqlEx)
				{
					ProcessSqlEx(sqlEx, cmd);
				}
				catch(Exception ex)
				{
					ErrorMessage(ex, null, "GetRecord");
				}
			}
		}

		public SignItem[ ] GetDocumentSigns(int docId, int? imageId, out bool canSign, out bool canFinalSign, out bool canSingCancel, CancellationToken token)
		{
			var result = new List<SignItem>();
			canSign = canFinalSign = canSingCancel = false;
			using(var conn = new SqlConnection(connectionString))
			using(var cmd = new SqlCommand("dbo.sp_ПодписиДокумента", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				if(token != CancellationToken.None)
					token.Register(() => cmd.Cancel());
				conn.Open();
				AddParam(cmd, "@КодДокумента", SqlDbType.Int, docId);
				if(imageId.HasValue)
					AddParam(cmd, "@КодИзображениядокумента", SqlDbType.Int, imageId.Value);

				SqlParameter canWrite = AddParam(cmd, "@МожноПодписать", SqlDbType.SmallInt, null);
				canWrite.Direction = ParameterDirection.Output;

				try
				{
					using(SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while(reader.Read())
						{
							var item = new SignItem(reader, this);
							result.Add(item);
						}
						reader.Close();
					}

					if(token.IsCancellationRequested)
						token.ThrowIfCancellationRequested();
					canSign = (Int16)canWrite.Value > -1;
					canFinalSign = canWrite.Value.Equals((Int16)1);
					canSingCancel = canWrite.Value.Equals((Int16)2);
				}
				catch(SqlException ex)
				{
					if(!token.IsCancellationRequested)
						ProcessSqlEx(ex, cmd);
				}
				catch(Exception ex)
				{
					if(!token.IsCancellationRequested)
						ErrorMessage(ex, cmd, "GetDocumentSigns");
				}
			}
			if(token.IsCancellationRequested)
				token.ThrowIfCancellationRequested();
			return result.ToArray();
		}

		public List<StampItem> GetStamps(int imageID)
		{
			return GetRecords<StampItem>("SELECT " + idField + ", КодШтампа, " + imgIDField + ", " + docIDField + ", ТипПодписи, Page, X, Y, Zoom, Rotate, КодСотрудника, КодСотрудникаЗА FROM fn_ШтампыНаИзображении(@КодИзображения)",
										 delegate(SqlCommand cmd)
										 {
											 AddParam(cmd, "@КодИзображения", SqlDbType.Int, imageID);
										 }, delegate(IDataRecord dr)
													{
														return new StampItem(dr, this);
													});
		}

		#endregion

		#region Change data

	    /// <summary>
	    /// Добавление подписи по эл. форме документа
	    /// </summary>
	    /// <param name="docId">Код документа</param>
	    /// <param name="employeeId">код сотрудника</param>
	    /// <param name="employee4Id">код сотрудника, если подпизавается за замещающего сотрудника</param>
	    /// <param name="signType">тип подписи</param>
        /// <param name="кодПодписиДокумента">КодПодписиДокумента</param>
	    public bool AddSign(int docId, int employeeId, int? employee4Id, byte signType, out int? кодПодписиДокумента)
		{
			return AddSign(docId, null, employeeId, employee4Id, signType, null, null, null, null, null, null, out кодПодписиДокумента);
		}

	    /// <summary>
	    /// Добавление подписи изображению документа
	    /// </summary>
	    /// <param name="docID">Код документа</param>
	    /// <param name="imageID">Код изображения документа</param>
	    /// <param name="employeeID">код сотрудника</param>
	    /// <param name="employee4ID">код сотрудника, если подпизавается за замещающего сотрудника</param>
	    /// <param name="signTypeID">тип подписи</param>
        /// <param name="кодПодписиДокумента">КодПодписиДокумента</param>
	    /// <returns>true - если подпись добавилась успешно</returns>
	    public bool AddSign(int docId, int imgId, int employeeId, int? employee4Id, byte signType, out int? кодПодписиДокумента)
		{
			return AddSign(docId, imgId, employeeId, employee4Id, signType, null, null, null, null, null, null, out кодПодписиДокумента);
		}

		/// <summary>
		/// Добавление штампа по изображению документа
		/// </summary>
		/// <param name="docID">Код документа</param>
		/// <param name="imageID">Код изображения документа</param>
		/// <param name="employeeID">код сотрудника</param>
		/// <param name="stampTypeID">код штампа</param>
		/// <param name="page">страница изображения</param>
		/// <param name="x">координата по x</param>
		/// <param name="y">координата по y</param>
		/// <param name="zoom">маштабирование картинки штампа</param>
		/// <param name="rotate">угол поворота картинки штампа в градусах</param>
		/// <returns>true - если штамп добавился успешно</returns>
		public bool AddStamp(int docID, int imageID, int employeeID, int stampTypeID, int page, int x, int y, int zoom, int rotate)
		{
            int? кодПодписиДокумента;
			return AddSign(docID, imageID, employeeID, null, 100, stampTypeID, page, x, y, zoom, rotate, out кодПодписиДокумента);
		}

		/// <summary>
		/// добавление штампа "ДСП"
		/// </summary>
		// <param name="docID">Код документа</param>
		/// <param name="imageID">Код изображения документа</param>
		/// <param name="employeeID">код сотрудника</param>
		/// <param name="page">страница изображения</param>
		/// <param name="x">координата по x</param>
		/// <param name="y">координата по y</param>
		/// <param name="zoom">маштабирование картинки штампа</param>
		/// <param name="rotate">угол поворота картинки штампа в градусах</param>
		/// <returns>true - если штамп добавился успешно</returns>
		public bool AddStampDSP(int docID, int imageID, int employeeID, int page, int x, int y, int zoom, int rotate)
		{
            int? кодПодписиДокумента;
			return AddSign(docID, imageID, employeeID, null, 101, null, page, x, y, zoom, rotate, out кодПодписиДокумента);
		}

	    /// <summary>
	    /// Добавление подписи или штампа по документу
	    /// </summary>
	    /// <param name="docID">Код документа</param>
	    /// <param name="imageID">Код изображения документа, если подписывается изображение или добавляется штамп</param>
	    /// <param name="employeeID">код сотрудника</param>
	    /// <param name="employee4ID">код сотрудника, если подпизавается за замещающего сотрудника</param>
	    /// <param name="signTypeID">тип подписи</param>
	    /// <param name="stampTypeID">код штампа, если добавляется штамп</param>
	    /// <param name="page">страница изображения, если добавляется штамп</param>
	    /// <param name="x">координата по x, если добавляется штамп</param>
	    /// <param name="y">координата по y, если добавляется штамп</param>
	    /// <param name="zoom">маштабирование картинки штампа, если добавляется штамп</param>
	    /// <param name="rotate">угол поворота картинки штампа в градусах, если добавляется штамп</param>
	    /// <param name="кодПодписиДокумента"> Возвращаемое значение </param>
	    /// <returns>true - если подпись или штамп добавился успешно</returns>
	    private bool AddSign(int docID, int? imageID, int employeeID, int? employee4ID, byte signTypeID, int? stampTypeID, int? page, int? x, int? y, int? zoom, int? rotate, out int? кодПодписиДокумента)
	    {
	        кодПодписиДокумента = null;

			var sb = new StringBuilder(300);
			sb.Append("INSERT ");
			sb.Append(tableName);
			sb.Append(" (КодДокумента");
			if(imageID.HasValue)
				sb.Append(",КодИзображенияДокумента");
			sb.Append(",КодСотрудника, КодСотрудникаЗА, " + dataField + ", ");
			sb.Append(docSingatureTypeField);
			if(stampTypeID.HasValue)
				sb.Append(", КодШтампа");
			if(page.HasValue)
				sb.Append(", Page, X, Y, Zoom, Rotate");
			sb.AppendLine(")");
			sb.Append("VALUES (@КодДокумента");
			if(imageID.HasValue)
				sb.Append(", @КодИзображенияДокумента");
			sb.Append(", @КодСотрудника, @КодСотрудникаЗа, GETUTCDATE(), @ТипПодписи");
			if(stampTypeID.HasValue)
				sb.Append(", @КодШтампа");
			if(page.HasValue)
				sb.Append(", @Page, @X, @Y, @Zoom, @Rotate");
			sb.Append(")");
			sb.Append(" SET @КодПодписиДокумента = SCOPE_IDENTITY()");

		    var idParam = new SqlParameter("@КодПодписиДокумента", SqlDbType.Int, 4) {Direction = ParameterDirection.Output};

	        var result = Exec(sb.ToString(), delegate(SqlCommand cmd)
	        {
	            cmd.Parameters.AddWithValue("@КодДокумента", docID);
	            if(imageID.HasValue)
	                cmd.Parameters.AddWithValue("@КодИзображенияДокумента", imageID.Value);
	            cmd.Parameters.AddWithValue("@КодСотрудника", employeeID);
	            cmd.Parameters.AddWithValue("@КодСотрудникаЗа", employee4ID.HasValue ? employee4ID : employeeID);
	            cmd.Parameters.AddWithValue("@ТипПодписи", signTypeID);
	            if(stampTypeID.HasValue)
	                cmd.Parameters.AddWithValue("@КодШтампа", stampTypeID.Value);
	            if(page.HasValue)
	            {
	                cmd.Parameters.AddWithValue("@Page", page.Value);
	                cmd.Parameters.AddWithValue("@X", x.Value);
	                cmd.Parameters.AddWithValue("@Y", y.Value);
	                cmd.Parameters.AddWithValue("@Zoom", zoom.Value);
	                cmd.Parameters.AddWithValue("@Rotate", rotate.Value);
	            }

	            cmd.Parameters.Add(idParam);
	        });

	        if (idParam.Value != DBNull.Value)
	            кодПодписиДокумента = (int) idParam.Value;

	        return result;
		}

		/// <summary>
		/// Замена координат и поворотов штампов
		/// </summary>
		/// <param name="sings">список обновляемых штампов</param>
		/// <returns></returns>
		public bool StampReplace(List<StampItem> sings)
		{
			if(sings.Count == 1)
			{
				return Exec("IF EXISTS(SELECT * FROM " + tableName + " WHERE (X <> @X OR Y <> @Y OR Rotate <> @Rotate) AND " + idField + " = @ID)" + Environment.NewLine +
						"UPDATE " + tableName + " SET X = @X, Y = @Y, Rotate = @Rotate WHERE " + idField + " = @ID",
					delegate(SqlCommand cmd)
					{
						AddParam(cmd, "@ID", SqlDbType.Int, sings[0].ID);
						AddParam(cmd, "@X", SqlDbType.Int, sings[0].X);
						AddParam(cmd, "@Y", SqlDbType.Int, sings[0].Y);
						AddParam(cmd, "@Rotate", SqlDbType.Int, sings[0].Rotate);
					});
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("UPDATE " + tableName);
				sb.AppendLine(" SET X = CASE");
				foreach(StampItem si in sings)
				{
					sb.AppendLine(" WHEN " + idField + " = " + si.ID.ToString() + " THEN " + si.X.ToString());
				}
				sb.AppendLine("END, Y = CASE ");
				foreach(StampItem si in sings)
				{
					sb.AppendLine(" WHEN " + idField + " = " + si.ID.ToString() + " THEN " + si.Y.ToString());
				}
				sb.AppendLine("END, Rotate = CASE ");
				foreach(StampItem si in sings)
				{
					sb.AppendLine(" WHEN " + idField + " = " + si.ID.ToString() + " THEN " + si.Rotate.ToString());
				}
				sb.AppendLine("END");
				sb.Append("WHERE " + idField + " IN (");
				sb.Append(String.Join(",", sings.Select(x => x.ID.ToString()).ToArray()));
				sb.AppendLine(")");
				return Exec(sb.ToString());
			}
		}

		/// <summary>
		/// Метод обновления положения штампа
		/// </summary>
		/// <param name="imageID">КодИзображения</param>
		/// <param name="x">координата по вертикали</param>
		/// <param name="y">координата по горизонтале</param>
		/// <returns>результат занесения в базу</returns>
		public bool UpdateStampDSP(int imageID, int x, int y)
		{
			return Exec("IF EXISTS(SELECT * FROM " + tableName + " WHERE (X <> @X OR Y <> @Y) AND КодИзображенияДокумента = @ImageID AND ТипПодписи = 101)" + Environment.NewLine +
					"UPDATE " + tableName + " SET X = @X, Y = @Y WHERE КодИзображенияДокумента = @ImageID AND ТипПодписи = 101",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@ImageID", SqlDbType.Int, imageID);
					AddParam(cmd, "@X", SqlDbType.Int, x);
					AddParam(cmd, "@Y", SqlDbType.Int, y);
				});
		}

		#endregion
	}
}