using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Kesco.Lib.Win.Data.Temp.Objects;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	/// <summary>
	/// Компонент доступа к штампам (факсимиле).
	/// </summary>
	public class StampDALC : DALC
	{
        private const string deleteField = "Удален";
        private const string nameEnField = "ШтампEn";
        private const string imageField = "Изображение";
        private const string fnStamp = "fn_ПраваНаУстановкуШтампов";
        private const string canInsertField = "МожноСтавить";
        private const string employeesField = "ТребуютсяПодписи";

		#region Конструкторы

		public StampDALC(string connectionString)
			: base(connectionString)
		{
			tableName = "Штампы";
			idField = "КодШтампа";
			nameField = "Штамп";
		}
		#endregion

		#region Accessors

		public string NameEnField
		{
			get { return nameEnField; }
		}

		public string CanInsertField
		{
			get { return canInsertField; }
		}

		public string EmployeesField
		{
			get { return employeesField; }
		}

		public string DeleteField
		{
			get { return deleteField; }
		}

		#endregion

		#region Get Data

		/// <summary>
		/// Получение списка штампов
		/// </summary>
		/// <returns>список штампов в формате [код, наименование]</returns>
		public Dictionary <StampAddItem, byte[]> GetStampsList(int imageID, bool all)
		{
			string sql = "SELECT " + idField + ", " + nameField + ", " + imageField;
		    sql += all ? ", 1 " : ", " + employeesField + ", ";
		    sql += canInsertField + " FROM ";
		    sql += all ? tableName + " WHERE " + deleteField + " = 0" : fnStamp + "(@ID)";

		    return GetRecords<KeyValuePair<StampAddItem, byte[]>>(sql,
				delegate(SqlCommand cmd)
				{
					if (!all)
						cmd.Parameters.AddWithValue("@ID", imageID);
				},
				delegate(IDataRecord dr)
				{
					if (!dr.IsDBNull(0))
					{
						return new KeyValuePair<StampAddItem, byte[]>(new StampAddItem(dr, this), dr[2] as byte[]);
					}
					return null;
				}).ToDictionary(x => x.Key, x => x.Value);
		}

		/// <summary>
		/// Получение изображения штампа по коду.
		/// </summary>
		/// <param name="stampId">код штампа</param>
		/// <returns>изображение из БД</returns>
		public byte[] GetStampImage(int stampId, int imageID)
		{
			string sql = string.Concat("SELECT ", imageField, " FROM fn_ШтампыНаИзображении(@ID) WHERE ", idField, " = @StampID");
			object obj = GetField(sql, imageField,
				delegate(SqlCommand cmd)
				{
					cmd.Parameters.AddWithValue("@ID", imageID);
					cmd.Parameters.Add("@stampId", SqlDbType.Int).Value = stampId;
				});
		    return obj is Array ? (byte[]) obj : null;
		}

		#endregion

		#region Set Data

        public int AddStamp(byte[] imageBytes, string name, string nameEn)
        {
            object obj = GetField("INSERT INTO " + tableName + " (" + nameField + ", " + nameEnField + ", " + imageField + ") VALUES(@Name, @NameEn, @Stamp) SELECT SCOPE_IDENTITY()", "",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@Name", SqlDbType.NVarChar, name);
					AddParam(cmd, "@NameEn", SqlDbType.VarChar, nameEn);
					AddParam(cmd, "@Stamp", SqlDbType.Image, imageBytes);
				});
            return obj != null ? (int) (decimal) obj : 0;
        }

	    public bool SetStampImage(int stampId, byte[] imageBytes, string name, string nameEn)
		{
            return Exec("UPDATE " + tableName + " SET " + nameField + "=@Name, " + nameEnField + " = @NameEn, " + imageField + "=@Stamp WHERE " + idField + "=@Id",
				delegate (SqlCommand cmd)
			{
				AddParam(cmd, "@Id", SqlDbType.Int, stampId);
				AddParam(cmd, "@Name", SqlDbType.NVarChar, name);
				AddParam(cmd, "@NameEn", SqlDbType.VarChar, nameEn);
				AddParam(cmd, "@Stamp", SqlDbType.Image, imageBytes);
			});
		}

		#endregion
	}
}