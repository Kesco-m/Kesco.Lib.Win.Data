using System;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Kesco.Lib.Log;

namespace Kesco.Lib.Win.Data
{
	public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

	/// <summary>
	/// Окружение в котором работает приложение
	/// </summary>
	public class Env
	{
		public static Business.V2.Docs.DocsModule Docs;
		public static Business.V2.Corporate.CorporateModule Corporate;
		public static Business.V2.Buh.BuhModule Buh;

		/// <summary>
		/// Функция для отправки сообщений и ошибок в лог
		/// </summary>
		/// <param name="ex">Ошибка</param>
		public static void WriteToLog(Exception ex)
		{
			WriteToLog(ex, null);
		}

		/// <summary>
		/// Функция для отправки сообщений и ошибок в лог
		/// </summary>
		/// <param name="ad">Дополнительная информация</param>
		public static void WriteToLog(string ad)
		{
			WriteToLog(null, ad);
		}

		/// <summary>
		/// Функция для отправки сообщений и ошибок в лог
		/// </summary>
		/// <param name="ex">Ошибка</param>
		/// <param name="ad">Дополнительная информация</param>
		public static void WriteToLog(Exception ex, string ad)
		{
			if(string.IsNullOrEmpty(ad))
				Logger.WriteEx(new DetailedException(ex.Message, ex, GetPriority(ex, Priority.Error)));
			else if(ex == null)
				Logger.WriteEx(new Exception(ad));
			else
				Logger.WriteEx(new DetailedException(ad, ex, GetPriority(ex, Priority.Error)));
		}

		/// <summary>
		/// Функция для отправки SqlException в лог
		/// </summary>
		/// <param name="ex">Ошибка</param>
		/// <param name="cmd">Команда в которой произошла ошибка</param>
		public static void WriteSqlToLog(SqlException ex, SqlCommand cmd)
		{
			if( ex.Message.Equals("Internal Query Processor Error: The query processor ran out of stack space during query optimization."))
				MessageBox.Show("You have selected too many items!");
			else
				Logger.WriteEx(new DetailedException(ex.Message, ex, cmd, GetPriority(ex, Priority.Error)));
		}

		/// <summary>
		/// Отправка генерируемых программой ошибок внешнего окружения
		/// </summary>
		/// <param name="title">Заголовок ошибки</param>
		/// <param name="message">Сообщение</param>
		/// <param name="assemblyName">Сборка, в которой вызвана ошибка</param>
		/// <param name="methodBase">Метод, в котором вызвана ошибка </param>
		public static void WriteExtExToLog(string title, string message, System.Reflection.AssemblyName assemblyName, System.Reflection.MethodBase methodBase)
		{
			Logger.WriteEx(new LogicalException(title, message, assemblyName, methodBase.Name, Priority.ExternalError));
		}

		public static void WriteErrorToLog(Exception ex, SqlCommand cmd)
		{
			Logger.WriteEx(new DetailedException(ex.Message, ex, cmd, GetPriority(ex, Priority.Error)));
		}

		public static bool HasAvaliblePhoneDevices(string clientname)
		{
			bool _ret = false;
			using(var cn = new SqlConnection(Settings.DS_user))
			{
				cn.Open();
				SqlCommand cm = cn.CreateCommand();
				cm.CommandText = "EXEC sp_ДоступныеТелефоны @СетевоеИмя";
				cm.Parameters.AddWithValue("@СетевоеИмя", clientname);
				try
				{
					SqlDataReader dr = cm.ExecuteReader();
					_ret = dr.HasRows;
					dr.Close();
				}
				catch(SqlException sex)
				{
					WriteSqlToLog(sex, cm);
				}
				catch(Exception ex)
				{
					WriteToLog(ex);
				}
				finally
				{

					cn.Close();
				}
			}
			return _ret;
		}

		public static string ReverseString(string input)
		{
			char[] retVal = input.ToCharArray();
			Array.Reverse(retVal);
			return new string(retVal);
		}

		private enum SqlErrors
		{
			SQL_CONNECTION_FAILED = -2,
			SQL_CONNECTION_FAILED8 = -1,
			SQL_CONNECTION_FAILED2 = 53,
			SQL_CONNECTION_FAILED10 = 51,
			SQL_CONNECTION_FAILED1 = 2,
			SQL_CONNECTION_FAILED5 = 10060,
			SQL_CONNECTION_FAILED6 = 10061,
			SQL_CONNECTION_FAILED7 = 11001,
			SQL_INIT_SRVC_PAUSED = 17142,
			SQL_STARTUP_SERVER_KILLED = 17147,
			SQL_STARTUP_SERVER_UNINSTALL = 17148,
			SQL_SRV_NO_FREE_CONN = 17809,
			SQL_CONNECTION_FAILED4 = 233,
			SQL_CONNECTION_FAILED11 = 64,
			SQL_LOGON_INVALID_CONNECT1 = 18452,
			SQL_LOGON_INVALID_CONNECT2 = 18456,
			SQL_DB_UFAIL_FATAL = 4064,
			SQL_PERM_DEN = 229,
			SQL_CONNECTION_FAILED3 = 10054,
			SQL_MEMTIME_OUT = 8645,
			SQL_DEADLOCK = 1205,
			SQL_CONNECTION_FAILED9 = 17806,
			SQL_LOGON_INVALID_CONNECT3 = 1385,
			SQL_SERVER_SHUTDOWN = 6005,
			SQL_LOG_FULL = 9002
		};

		private const int SQL_USERMESSAGE = unchecked((int)0xC350);

		// HRESULTs:
		private const int COR_E_NOTSUPPORTED = unchecked((int)0x80131515);
		//private const int COR_E_INVALOPERATION = unchecked((int)0x80131509);
		private const int COR_E_UNAUTHORIZEDACCESS = unchecked((int)0x80070005);
		private const int COR_E_TIMEOUT = unchecked((int)0x80131505);
		private const int COR_E_LOGIN_FAILED = unchecked((int)0x80131904);
		private const int COR_E_LOGIN_FAILED1 = unchecked((int)0x80040E4D);
		private const int COR_E_SRV_REFUSED = unchecked((int)0x80131501);
		private const int COR_E_NET_NOT_REACHED = unchecked((int)0x800704d0);

		private enum TcpErrors
		{
			REMOTE_CONNECTION_FAILED3 = 10054,
			REMOTE_CONNECTION_FAILED7 = 11001,  /* No such host is known*/
			REMOTE_CONNECTION_FAILED6 = 10061,
			REMOTE_CONNECTION_HOST_UNAVAILABLE = 10065,
			REMOTE_CONNECTION_TIMEOUT_FAILED = 10060
		};

		/// <summary>
		/// Возвращает проставляемы приоритет ошибки в соответствии с регламентом
		/// </summary>
		/// <param name="ex">Пойманная ошибка</param>
		/// <param name="pr">Приоритет по-умолчанию</param>
		public static Priority GetPriority(Exception ex, Priority pr)
		{
			if(ex == null)
				return pr;

			if(ex is SqlException && Enum.IsDefined(typeof(SqlErrors), ((SqlException)ex).Number))
				return Priority.ExternalError;

			if(ex.Message.Contains("provider: Named Pipes Provider, error: 40") ||
				ex.Message.Contains("provider: Поставщик именованных каналов, error: 40"))
				return Priority.ExternalError;

			if(ex is InvalidOperationException && ex.StackTrace.Contains("System.Data.SqlClient.SqlConnection.Open()"))
				return Priority.ExternalError;

			if(ex.Message.Contains("Ошибка при создании объекта KescoDocs"))
				return Priority.ExternalError;

			if(ex is System.IO.IOException)
				return Priority.ExternalError;

			int hr = Marshal.GetHRForException(ex);
			if(hr == COR_E_NOTSUPPORTED || hr == COR_E_UNAUTHORIZEDACCESS || hr == COR_E_TIMEOUT)
				return Priority.ExternalError;

			if(ex is SqlException && ((SqlException)ex).Number != SQL_USERMESSAGE && (hr == COR_E_LOGIN_FAILED || hr == COR_E_LOGIN_FAILED1))
				return Priority.ExternalError;

			if(ex is System.Web.Services.Protocols.SoapException && ex.StackTrace.Contains("ReportingService") && hr == COR_E_SRV_REFUSED)
				return Priority.ExternalError;

			if(ex is SocketException && Enum.IsDefined(typeof(TcpErrors), ((SocketException)ex).ErrorCode))
				return Priority.ExternalError;

			if(ex is System.IO.IOException && hr == COR_E_NET_NOT_REACHED)
				return Priority.ExternalError;

			if(ex is System.Net.WebException)
			{
				System.Net.WebException we = ex as System.Net.WebException;
				if(we.Status != System.Net.WebExceptionStatus.ProtocolError && we.Status != System.Net.WebExceptionStatus.UnknownError)
					return Priority.ExternalError;
			}

			return pr;
		}
	}
}

