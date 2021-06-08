using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.DALC.Corporate;
using Kesco.Lib.Win.Error;

namespace Kesco.Lib.Win.Data.DALC
{
	/// <summary>
	/// Базовый класс DAL-компонента
	/// </summary>
	public class DALC : LocalObject
	{
		protected const int cmdTimeout = 120; // таймаут в секундах

		protected string connectionString;

		private bool cancel = false;

		protected string tableName = "БазаДанных.dbo.Таблица";

		protected string idField = "Код";
		protected string nameField = "Имя";
		protected string nameEnField = "Name";
		protected string editorField = "Изменил";
		protected string editedField = "Изменено";
		protected string descriptionField = "Описание";

		protected string countField = "Count";
		protected string identityField = "_Identity";

		public DALC(string connectionString)
		{
			this.connectionString = connectionString;
		}

		#region Accessors

		public string TableName
		{
			get { return tableName; }
		}

		public string IDField
		{
			get { return idField; }
		}

		public string NameField
		{
			get { return nameField; }
		}

		public string DescriptionField
		{
			get { return descriptionField; }
		}

		public string EditedField
		{
			get { return editedField; }
		}

		public string EditorField
		{
			get { return editorField; }
		}

		public string CountField
		{
			get { return countField; }
		}

		#endregion

		#region Get Data

		/// <summary>
		/// Получение поля документа
		/// </summary>
		/// <param name="field">необходимое поле</param>
		/// <param name="id">Код</param>
		/// <returns></returns>
		public object GetField(string field, int id)
		{
			return GetField("SELECT " + field + " FROM " + tableName + " (nolock)" + " WHERE " + idField + " = @ID",
				field,
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@ID", SqlDbType.Int, id);
				});
		}

		/// <summary>
		/// Получение поля документа
		/// </summary>
		/// <param name="field">необходимое поле</param>
		/// <param name="id">Код</param>
		/// <returns></returns>
		public bool GetDocBoolField(string field, int id)
		{
			object obj = GetField("SELECT " + field + " FROM " + tableName + " (nolock)" + " WHERE " + idField + " = @ID",
				field,
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@ID", SqlDbType.Int, id);
				});

			if(obj == null || obj == DBNull.Value)
				return false;
			if(obj is bool)
				return (bool)obj;
			if(obj is byte)
				return Convert.ToBoolean((byte)obj);
			if(obj is int)
				return Convert.ToBoolean((int)obj);

			return false;
		}

		/// <summary>
		/// Получение поля документа типа int
		/// </summary>
		/// <param name="field">необходимое поле</param>
		/// <param name="id">Код</param>
		/// <param name="def">Значение по-умолчанию</param>
		/// <returns></returns>
		public int GetDocIntField(string field, int id, int def = 0)
		{
			object obj = GetField("SELECT " +
					field +
				" FROM " + tableName + " (nolock)" +
				" WHERE " + idField + " = @ID",
				field,
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@ID", SqlDbType.Int, id);
				});

			if(obj != null && obj != DBNull.Value && obj is int)
				return (int)obj;

			return def;
		}

		/// <summary>
		/// Показывает наличие данных по коду
		/// </summary>
		/// <param name="id">Код</param>
		/// <returns>Наличие данных</returns>
		public bool FieldExists(int id)
		{
			return FieldExists("WHERE " + idField + " = @ID",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@ID", SqlDbType.Int, id);
				});
		}

		/// <summary>
		/// Проверка наличия значения в базе
		/// </summary>
		/// <param name="whereString"></param>
		/// <param name="addParams"></param>
		/// <returns></returns>
		internal protected bool FieldExists(string whereString, AddParams addParams)
		{
			return GetIntField("SELECT TOP 1 " + idField +
				" FROM " + tableName + " WITH (NOLOCK)\n" +
				whereString, idField,
				addParams) > 0;
		}

		public DataRow GetRow(int id)
		{
			return GetFirstRow("SELECT TOP 1 * FROM " + tableName + " (nolock) WHERE " + idField + " = @ID", delegate(SqlCommand cmd)
			{
				AddParam(cmd, "@ID", SqlDbType.Int, id);
			});
		}

		public string GetEditor(int id, bool fullName)
		{
			object result = GetField(editorField, id);

			if(result != null && result is int)
			{
				var empDALC = new EmployeeDALC(connectionString);
				return empDALC.GetEmployee((int)result, fullName);
			}

			return null;
		}

		public delegate void AddParams(SqlCommand cmd);

		internal protected DataSet GetData(string query, AddParams addParams)
		{
			using(var sda = new SqlDataAdapter())
			using(var conn = new SqlConnection(connectionString))
			using(sda.SelectCommand = new SqlCommand(query, conn))
			{
				if(addParams != null)
					addParams(sda.SelectCommand);
				return CMD_FillDS(sda);
			}
		}

		internal protected DataTable GetDataTable(string query, AddParams addParams)
		{
			using(var conn = new SqlConnection(connectionString))
			using(var cmd = new SqlCommand(query, conn))
			{
				if(addParams != null)
					addParams(cmd);
				return CMD_FillDT(cmd);
			}
		}

		internal protected DataTable GetDataTable(string query, AddParams addParams, out SqlCommand command)
		{
			using(var conn = new SqlConnection(connectionString))
			using(var cmd = new SqlCommand(query, conn))
			{
				command = cmd;
				addParams?.Invoke(cmd);
				return CMD_FillDT(cmd);
			}
		}

		internal protected DataTable GetDataTable(string query, AddParams addParams, CancellationToken ct)
		{
			using(var conn = new SqlConnection(connectionString))
			using(var cmd = new SqlCommand(query, conn))
			{
				if(ct != CancellationToken.None)
					ct.Register(() => cmd.Cancel());
				if(addParams != null)
					addParams(cmd);
				DataTable td = CMD_FillDT(cmd);
				return td;
			}
		}

		public delegate object GetResult(IDataRecord dr);

		internal protected TResult GetRecord<TResult>(string query, AddParams addParams, GetResult getResult)
		{
			return GetRecord<TResult>(query, addParams, getResult, CancellationToken.None);
		}

		internal protected TResult GetRecord<TResult>(string query, AddParams addParams, GetResult getResult, CancellationToken ct)
		{
			TResult retVal = default(TResult);
			using(SqlConnection conn = new SqlConnection(connectionString))
			using(SqlCommand cmd = new SqlCommand(query, conn))
			{
				if(ct != CancellationToken.None)
					ct.Register(() => cmd.Cancel());
				if(addParams != null)
					addParams(cmd);
				cmd.CommandTimeout = cmdTimeout;

				try
				{
#if AdvancedLogging
                    Log.Logger.EnterMethod(this, "TResult GetRecord<TResult>(string query, AddParams addParams, GetResult getResult) query= " + query);
#endif

					cmd.Connection.Open();
					if(getResult != null)
					{
						using(SqlDataReader dr = cmd.ExecuteReader())
							if(dr.Read())
								retVal = (TResult)getResult(dr);
					}
					else
						retVal = (TResult)cmd.ExecuteScalar();
				}
				catch(SqlException sqlEx)
				{
					if((sqlEx.Class == 11 && sqlEx.Number == 0) || (sqlEx.Class == 16 && sqlEx.Number == 3980))
						retVal = default(TResult);
					else
						ProcessSqlEx(sqlEx, cmd);
				}
				catch(Exception ex)
				{
					ErrorMessage(ex, cmd, "GetRecord");
				}
				finally
				{
					cmd.Connection.Close();

#if AdvancedLogging
                    Log.Logger.LeaveMethod(this, "TResult GetRecord<TResult>(string query, AddParams addParams, GetResult getResult) query= " + query);
#endif
				}
			}

			return retVal;
		}

		/// <summary>
		/// Метод возвращает первое значение запроса приведенное к заданому типу
		/// </summary>
		/// <typeparam name="T">Тип возврата</typeparam>
		/// <param name="query">запрос</param>
		/// <param name="addParams">Добавление дополнительных параметров к запросу</param>
		/// <returns>первый значение запроса</returns>
		internal protected List<T> GetRecords<T>(string query, AddParams addParams)
		{
			return GetRecords<T>(query, addParams, null);
		}

		internal protected List<T> GetRecords<T>(string query, AddParams addParams, GetResult getResult)
		{
			var result = new List<T>();

			using(var conn = new SqlConnection(connectionString))
			using(var cmd = new SqlCommand(query, conn))
			{
				if(addParams != null)
					addParams(cmd);
				cmd.CommandTimeout = cmdTimeout;
				bool? repeat = null;
				while(!repeat.HasValue || repeat.Value)
					try
					{
						cmd.Connection.Open();
						using(SqlDataReader dr = cmd.ExecuteReader())
						{
							while(dr.Read())
							{
								if(getResult != null)
									result.Add((T)getResult(dr));
								else
								{
									if(dr[0] != DBNull.Value)
										result.Add((T)dr[0]);
								}
							}
							dr.Close();
						}
						repeat = false;
					}
					catch(SqlException sqlEx)
					{
						if((sqlEx.Number != 6005 && sqlEx.Number != 10054) || (repeat.HasValue && repeat.Value))
						{
							repeat = false;
							ProcessSqlEx(sqlEx, cmd);
						}
						else
							repeat = true;
					}
					catch(Exception ex)
					{
						repeat = false;
						ErrorMessage(ex, null, "GetRecords");
					}
					finally
					{
						cmd.Connection.Close();
					}
			}

			return result;
		}

		public string GetAsyncConnectionString()
		{
			return connectionString.Contains("Asynchronous Processing=true")
					   ? connectionString
					   : connectionString + ";Asynchronous Processing=true";
		}

		internal protected DataRow GetFirstRow(string query, AddParams addParams)
		{
			using(SqlConnection conn = new SqlConnection(connectionString))
			using(SqlCommand cmd = new SqlCommand(query, conn))
			{
				if(addParams != null)
					addParams(cmd);
				return CMD_GetFirstRow(cmd);
			}
		}

		/// <summary>
		/// метод-обертка для запуска команды на получение значения поля
		/// </summary>
		/// <param name="query"></param>
		/// <param name="field"></param>
		/// <param name="addParams"></param>
		/// <returns></returns>
		internal protected object GetField(string query, string field, AddParams addParams)
		{
			using(var cn = new SqlConnection(connectionString))
			using(var cmd = new SqlCommand(query, cn))
			{
				if(addParams != null)
					addParams(cmd);
				return CMD_GetField(cmd, field);
			}
		}

		/// <summary>
		/// метод-обертка для запуска команды на получение значения поля
		/// </summary>
		/// <param name="query"></param>
		/// <param name="field"></param>
		/// <param name="addParams"></param>
		/// <returns></returns>
		internal protected object GetField(string query, string field, AddParams addParams, CancellationToken ct)
		{
			using(var cn = new SqlConnection(connectionString))
			using(var cmd = new SqlCommand(query, cn))
			{
				if(ct != CancellationToken.None)
					ct.Register(() => cmd.Cancel());
				if(addParams != null)
					addParams(cmd);
				return CMD_GetField(cmd, field);
			}
		}

		/// <summary>
		/// метод-обертка для запуска команды на получение численного значения поля
		/// </summary>
		/// <param name="query"></param>
		/// <param name="field"></param>
		/// <param name="addParams"></param>
		/// <returns></returns>
		internal protected int GetIntField(string query, string field, AddParams addParams)
		{
			return GetIntField(query, field, addParams, CancellationToken.None);
		}

		/// <summary>
		/// метод-обертка для запуска команды на получение численного значения поля
		/// </summary>
		/// <param name="query"></param>
		/// <param name="field"></param>
		/// <param name="addParams"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		internal protected int GetIntField(string query, string field, AddParams addParams, CancellationToken ct)
		{
			object obj = GetField(query, field, addParams, ct);
			return obj is int ? (int)obj : 0;
		}

		/// <summary>
		/// метод-обертка для запуска команды на получение ключевого поля. Должно содержать поле IdentityField
		/// </summary>
		/// <param name="query"></param>
		/// <param name="addParams"></param>
		/// <returns></returns>
		internal protected int GetIdentityField(string query, AddParams addParams)
		{
			object obj = GetField(query, identityField, addParams);
			return obj is decimal || obj is int ? Convert.ToInt32(obj) : 0;
		}

		/// <summary>
		/// метод-обертка для запуска команды на получение количества. Должно содержать поле CountField
		/// </summary>
		/// <param name="query">sql запрос</param>
		/// <param name="addParams">дополнительные параметры</param>
		/// <returns>количество</returns>
		public int GetCount(string query, AddParams addParams)
		{
			return GetIntField(query, countField, addParams);
		}

		public int ExecID(string query, AddParams addParams, int id)
		{
			return id > 0 ? (Exec(query, addParams) ? id : 0) : GetIdentityField(query, addParams);
		}

		#endregion

		#region Change Data

		/// <summary>
		/// Выполнение CMD_Exec с обработкой результата
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public bool Exec(string query)
		{
			return Exec(query, null);
		}

		/// <summary>
		///  Выполнение CMD_Exec с обработкой результата
		/// </summary>
		/// <param name="query"></param>
		/// <param name="addParams"></param>
		/// <returns></returns>
		public bool Exec(string query, AddParams addParams)
		{
			return Exec(query, addParams, null);
		}

		/// <summary>
		/// Выполнение CMD_Exec с обработкой результата
		/// </summary>
		/// <param name="query"></param>
		/// <param name="addParams"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public bool Exec(string query, AddParams addParams, Func<SqlCommand, bool> result)
		{
			return Exec<bool>(query, addParams, result ?? delegate { return true; }, null);
		}

		/// <summary>
		/// Выполнение CMD_Exec с обработкой результата
		/// </summary>
		/// <param name="query"></param>
		/// <param name="addParams"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public bool Exec(string query, AddParams addParams, Func<SqlCommand, bool> result, Func<SqlException, bool> errorCheck)
		{
			return Exec<bool>(query, addParams, result ?? delegate { return true; }, errorCheck);
		}

		/// <summary>
		/// Выполнение CMD_Exec с обработкой результата
		/// </summary>
		/// <param name="query"></param>
		/// <param name="addParams"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public T Exec<T>(string query, AddParams addParams, Func<SqlCommand, T> result, Func<SqlException, bool> errorCheck)
		{
			using(var cn = new SqlConnection(connectionString))
			using(var cmd = new SqlCommand(query, cn))
			{
				if(addParams != null)
					addParams(cmd);

				if(CMD_Exec(cmd, errorCheck))
					return result != null ? result(cmd) : default(T);

				return default(T);
			}
		}

		/// <summary>
		/// Выполнение CMD_Exec без обработки результата
		/// </summary>
		/// <param name="query"></param>
		/// <param name="addParams"></param>
		/// <returns></returns>
		public bool ExecNoError(string query, AddParams addParams)
		{
			using(var cn = new SqlConnection(connectionString))
			using(var cmd = new SqlCommand(query, cn))
			{
				if(addParams != null)
					addParams(cmd);
				return CMD_ExecNoError(cmd);
			}
		}

		public bool SetField(string field, SqlDbType type, int id, object val)
		{
			return Exec(
				"UPDATE " + tableName +
				" SET " + field + " = @Value" +
				" WHERE " + idField + " = @ID",
				delegate(SqlCommand cmd)
				{
					if(type == SqlDbType.Bit)
						val = ((bool)val ? 1 : 0);

					AddParam(cmd, "@Value", type, val);
					AddParam(cmd, "@ID", SqlDbType.Int, id);
				});
		}

		public bool SetField(string field, SqlDbType type, string ids, object val)
		{
			return Exec(
				"UPDATE " + tableName +
				" SET " + field + " = @Value" +
				" WHERE " + idField + " in (" + ids + ")",
				delegate(SqlCommand cmd)
				{
					if(type == SqlDbType.Bit)
						val = ((bool)val ? 1 : 0);

					AddParam(cmd, "@Value", type, val);
				});
		}

		public virtual bool Rename(int id, string name)
		{
			return SetField(nameField, SqlDbType.NVarChar, id, name);
		}

		public virtual bool Delete(int id)
		{
			return Exec("DELETE FROM " + tableName + " WHERE " + idField + " = @ID",
			delegate(SqlCommand cmd)
			{
				AddParam(cmd, "@ID", SqlDbType.Int, id);
			});
		}

		#endregion

		#region CMD_Get...

		protected DataRow CMD_GetFirstRow(SqlCommand cmd)
		{
			cmd.CommandTimeout = cmdTimeout;
			var table = new DataTable();
			table.BeginLoadData();
			try
			{
				cmd.Connection.Open();

				using(SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
				{
					table.Load(reader);
					reader.Close();
				}
			}
			catch(SqlException sqlEx)
			{
				ProcessSqlEx(sqlEx, cmd);
			}
			catch(Exception ex)
			{
				ErrorMessage(ex, null, "CMD_GetFirstRow");
			}
			finally
			{
				if(cmd.Connection.State != ConnectionState.Closed)
					cmd.Connection.Close();
				cmd.Dispose();
				table.EndLoadData();
			}
			return table.Rows.Count == 1 ? table.Rows[0] : null;
		}

		private int CMD_GetIntField(SqlCommand cmd, string field)
		{
			object obj = CMD_GetField(cmd, field);
			return obj != null && obj != DBNull.Value && obj is int ? (int)obj : 0;
		}

		private object CMD_GetField(SqlCommand cmd, string field)
		{
			cmd.CommandTimeout = cmdTimeout;
			bool? repeat = null;
			while(!repeat.HasValue || repeat.Value)
			try
			{
				
				cmd.Connection.Open();

#if AdvancedLogging
                using (Log.Logger.DurationMetter("DALC CMD_GetField " + cmd.CommandText))
#endif
				repeat = false;
				return cmd.ExecuteScalar();
			}
			catch(SqlException sqlEx)
			{
				if((sqlEx.Number != 6005 && sqlEx.Number != 10054) || (repeat.HasValue && repeat.Value))
				{
					repeat = false;
					if((sqlEx.Class == 11 && sqlEx.Number == 0) || (sqlEx.Class == 16 && sqlEx.Number == 3980))
						return null;
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
			finally
			{
				cmd.Connection.Close();
			}

			return null;
		}

		#endregion

		#region CMD_FillDS

		protected DataSet CMD_FillDS(SqlDataAdapter cmd)
		{
			return CMD_FillDS(cmd, tableName);
		}

		protected DataSet CMD_FillDS(SqlDataAdapter cmd, string table)
		{
			cmd.SelectCommand.CommandTimeout = cmdTimeout;

			try
			{
				var ds = new DataSet();

#if AdvancedLogging
                using (Log.Logger.DurationMetter("DALC CMD_FillDS " + cmd.SelectCommand.CommandText))
#endif

				cmd.Fill(ds, table);
				return ds;
			}
			catch(SqlException sqlEx)
			{
				if((sqlEx.Class == 11 && sqlEx.Number == 0) || (sqlEx.Class == 16 && sqlEx.Number == 3980))
					return null;
				else
					ProcessSqlEx(sqlEx, cmd.SelectCommand);
			}
			catch(InvalidOperationException ioEx)
			{
				ErrorMessage(ioEx, null, "CMD_FillDS", false);
			}
			catch(Exception ex)
			{
				ErrorMessage(ex, null, "CMD_FillDS");
			}
			finally
			{
				cmd.Dispose();
			}
			return null;
		}

		protected DataTable CMD_FillDT(SqlCommand cmd)
		{
			return CMD_FillDT(cmd, tableName);
		}

		protected DataTable CMD_FillDT(SqlCommand cmd, string tableName)
		{

			var table = new DataTable(tableName);
			table.BeginLoadData();

			cmd.CommandTimeout = cmdTimeout;
			bool? repeat = null;
			while(!repeat.HasValue || repeat.Value)
			try
			{
				cmd.Connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
				{
					table.Load(reader);
					reader.Close();
				}
				for(int i = 0; i < table.Columns.Count; i++)
				{
					if(table.Columns[i].ReadOnly)
						table.Columns[i].ReadOnly = false;
					if(!table.Columns[i].AllowDBNull)
						table.Columns[i].AllowDBNull = true;
				}
				repeat = false;
			}
			catch(SqlException sqlEx)
			{
				if((sqlEx.Number != 6005 && sqlEx.Number != 10054) || (repeat.HasValue && repeat.Value))
				{
					repeat = false;
					if((sqlEx.Class == 11 && sqlEx.Number == 0) || (sqlEx.Class == 16 && sqlEx.Number == 3980))
						table.Clear();
					else
						ProcessSqlEx(sqlEx, cmd);
				}
				else
					repeat = true;
			}
			catch(Exception ex)
			{
				repeat = false;
				ErrorMessage(ex, null, "CMD_FillDT");
			}
			finally
			{
				if(cmd.Connection.State != ConnectionState.Closed)
					cmd.Connection.Close();
				table.EndLoadData();
			}
			return table;
		}

		#endregion

		#region CMD_Exec

		protected bool CMD_Exec(SqlCommand cmd)
		{
			return CMD_Exec(cmd, false, cmdTimeout, null);
		}

		protected bool CMD_Exec(SqlCommand cmd, Func<SqlException, bool> errorCheck)
		{
			return CMD_Exec(cmd, false, cmdTimeout, errorCheck);
		}

		protected bool CMD_ExecNoError(SqlCommand cmd)
		{
			cmd.CommandTimeout = cmdTimeout;

			try
			{
				cmd.Connection.Open();
				cmd.ExecuteNonQuery();
				return true;
			}
			catch
			{
			}
			finally
			{
				cmd.Connection.Close();
				cmd.Dispose();
			}

			return true;
		}

		protected bool CMD_Exec(SqlCommand cmd, bool throwError)
		{
			return CMD_Exec(cmd, throwError, cmdTimeout, null);
		}

		protected bool CMD_Exec(SqlCommand cmd, bool throwError, int timeOut, Func<SqlException, bool> errorCheck)
		{
			bool result = false;
			cmd.CommandTimeout = timeOut;

			bool error = false;
			while(!result)
			{
				try
				{
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();

					result = true;
					break;
				}
				catch(SqlException sqlEx)
				{
					if(errorCheck != null && errorCheck(sqlEx))
						break;
					if(sqlEx.Number == 1205 && !error)//deadlock
					{
						error = true;
						continue;
					}

					if(sqlEx.Number == -2 && !error)//timeout
					{
						error = true;
						continue;
					}

					ProcessSqlEx(sqlEx, cmd, "CMD_Exec", throwError);
					break;
				}
				catch(InvalidOperationException ioEx)
				{
					ErrorMessage(throwError, ioEx, null, "CMD_Exec", false);
					break;
				}
				catch(Exception ex)
				{
					ErrorMessage(throwError, ex, null, "CMD_Exec");
					break;
				}
				finally
				{
					cmd.Connection.Close();
				}
			}

			return result;
		}


		protected bool CMD_ExecOpenConn(SqlCommand cmd)
		{
			bool result = false;
			cmd.CommandTimeout = cmdTimeout;

			bool error = false;

			while(!result)
			{
				try
				{
					cmd.ExecuteNonQuery();

					result = true;
					break;
				}
				catch(SqlException sqlEx)
				{
					if(sqlEx.Number == 1205 && !error)//deadlock
					{
						error = true;
						continue;
					}

					if(sqlEx.Number == -2 && !error)//timeout
					{
						error = true;
						continue;
					}

					ProcessSqlEx(sqlEx, cmd, "CMD_Exec", false);
					break;
				}
				catch(InvalidOperationException ioEx)
				{
					ErrorMessage(false, ioEx, null, "CMD_Exec", false);
					break;
				}
				catch(Exception ex)
				{
					ErrorMessage(false, ex, null, "CMD_Exec");
					break;
				}
			}
			return result;
		}

		#endregion

		#region Add Param

		protected SqlParameter AddParam(SqlCommand cmd, string name, SqlDbType type, object val)
		{
			if(!cmd.Parameters.Contains(name))
			{
				var param = new SqlParameter(name, type) { Value = val };
				cmd.Parameters.Add(param);
				return param;
			}
			else
			{
				SqlParameter param = cmd.Parameters[cmd.Parameters.IndexOf(name)];
				param.SqlDbType = type;
				param.Value = val;
				return param;
			}
		}

		#endregion

		#region Errors & Exceptions

		protected void ProcessSqlEx(SqlException sqlEx, SqlCommand cmd)
		{
			ProcessSqlEx(sqlEx, cmd, "", false);
		}

		protected void ProcessSqlEx(SqlException sqlEx, SqlCommand cmd, string source, bool throwError)
		{
			if(sqlEx.Number != 229 && sqlEx.Number != 6005 && sqlEx.Number != 10054)
			{
				Env.WriteSqlToLog(sqlEx, cmd);
			}
			try
			{
				if(sqlEx.Number == 229)
					ErrorShower.OnShowError(null, "У вас недостаточно прав для выполнения этой операции." + Environment.NewLine + Environment.NewLine + sqlEx.Message, "Ошибка: Отказано в доступе");
				else if(sqlEx.Number == 6005 || sqlEx.Number == 10054)
					ErrorShower.OnShowError(null, "Сервер перезапускается.\nС Архивом можно будет работать через одну-две минуты", "Ошибка: Отказано в доступе");
				else if(sqlEx.Number == 11 || sqlEx.Number == 2 || sqlEx.Number == 64)
					ErrorShower.OnShowError(null, "Сервер не доступен.", "Ошибка: нет доступа");
				else
					if(throwError)
						throw sqlEx;
					else
						ErrorShower.OnShowError(null, sqlEx.Message, "Ошибка выполнения");
			}
			catch(SqlException ex)
			{
				throw ex;
			}
			catch { }
		}

		protected void ErrorMessage(bool throwError, Exception ex, SqlCommand cmd, string source)
		{
			ErrorMessage(throwError, ex, cmd, source, true);
		}

		protected void ErrorMessage(Exception ex, SqlCommand cmd, string source)
		{
			ErrorMessage(false, ex, cmd, source, true);
		}

		protected void ErrorMessage(Exception ex, SqlCommand cmd, string source, bool write)
		{
			ErrorMessage(false, ex, cmd, source, write);
		}

		protected void ErrorMessage(bool throwError, Exception ex, SqlCommand cmd, string source, bool write)
		{
			if(ex is SqlException)
				ProcessSqlEx((SqlException)ex, cmd, source, throwError);
			if(write)
				Env.WriteToLog(ex);
			if(throwError)
				throw ex;
			try
			{
				ErrorShower.OnShowError(null, ex.Message, "Ошибка (" + source + ")");
			}
			catch
			{
			}
		}

		protected void ErrorMessage(string message, bool check, string title)
		{
			Env.WriteToLog(null, message);

			MessageBox.Show(message, title);
		}

		#endregion

		#region Test Connection

		public bool TestConnection()
		{
			using(var c = new SqlConnection(connectionString))
				try
				{
					c.Open();
					return true;
				}
				catch { }
				finally
				{
					if(c.State == ConnectionState.Open)
						c.Close();
				}
			return false;
		}

		public static bool TestConnection(string cString)
		{
			var d = new DALC(cString);
			return d.TestConnection();
		}

		#endregion

		#region SQLDataReader

		public string ReadString(string query, AddParams addParams)
		{
			var sb = new StringBuilder();
			using(var connection = new SqlConnection(connectionString))
			using(var command = new SqlCommand(query, connection))
			{
				try
				{
					if(addParams != null)
						addParams(command);

					connection.Open();

					using(SqlDataReader reader = command.ExecuteReader())
					{
						try
						{
							while(reader.Read())
							{
								sb.Append(reader.GetValue(0));
								sb.Append(",");
							}
						}
						finally
						{
							reader.Close();
						}
					}
				}
				catch(SqlException sqlEx)
				{
					ProcessSqlEx(sqlEx, command);
				}
				catch(InvalidOperationException ioEx)
				{
					ErrorMessage(ioEx, null, "ReadString", false);
				}
				catch(Exception ex)
				{
					ErrorMessage(ex, null, "ReadString");
				}
				finally
				{
					connection.Close();
				}
			}
			return sb.ToString().TrimEnd(',');
		}

		#endregion
	}
}