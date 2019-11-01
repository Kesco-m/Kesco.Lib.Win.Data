using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace Kesco.Lib.Win.Data.DALC.Corporate
{
	/// <summary>
	/// DAL-компонент для доступа к таблице Инвентаризация.dbo.Сотрудники
	/// </summary>
	public class EmployeeDALC : DALC
	{
        private const string fioField = "ФИО";
        private const string iofField = "ИОФ";
        private const string fioEngField = "FIO";
        private const string iofEngField = "IOF";
        private const string fField = "Фамилия";
        private const string iField = "Имя";
        private const string oField = "Отчество";
        private const string fEngField = "LastName";
        private const string iEngField = "FirstName";
        private const string oEngField = "MiddleName";

        private const string employeeField = "Employee";

        private const string languageField = "Язык";

        private const string emailField = "Email";

        private const string loginField = "Login";
        private const string loginFullField = "LoginFull";
        private const string sidField = "SID";

        private const string accountDisabledField = "AccountDisabled";
        private const string statusField = "Состояние";

        private const string fnCurrentRoles = "Инвентаризация.dbo.fn_ТекущиеРоли()";
        private const string fnCurrentEmployee = "Инвентаризация.dbo.fn_ТекущийСотрудник()";
        private const string roleIDField = "КодРоли";

        private const string currentEmpIDField = "КодСотрудника";

		private readonly string personIDField;

		public EmployeeDALC(string connectionString) : base(connectionString)
		{
			tableName = "Инвентаризация.dbo.Сотрудники";

			idField = "КодСотрудника";
			nameField = "Сотрудник";
			personIDField = "КодЛица";
		}

		#region Accessors

		public string FIOField
		{
			get { return fioField; }
		}

		public string IOFField
		{
			get { return iofField; }
		}

		public string FIOEngField
		{
			get { return fioEngField; }
		}

		public string IOFEngField
		{
			get { return iofEngField; }
		}

		public string FField
		{
			get { return fField; }
		}

		public string IField
		{
			get { return iField; }
		}

		public string FEngField
		{
			get { return fEngField; }
		}

		public string IEngField
		{
			get { return iEngField; }
		}

		public string OField
		{
			get { return oField; }
		}

		public string OEngField
		{
			get { return oEngField; }
		}

		public string EmloyeeField
		{
			get { return employeeField; }
		}

		public string LanguageField
		{
			get { return languageField; }
		}

		public string LoginField
		{
			get { return loginField; }
		}

		public string LoginFullField
		{
			get { return loginFullField; }
		}

		public string CurrentEmpIDField
		{
			get { return currentEmpIDField; }
		}

		public string AccountDisabledField
		{
			get { return accountDisabledField; }
		}

		public string StatusField
		{
			get { return statusField; }
		}

		public string SIDField
		{
			get { return sidField; }
		}

		#endregion

		#region Get Data

		public DataTable GetCompEmployees(bool dismiss)
		{
			return GetDataTable(
				"SELECT " +
					idField + ", " +
					nameField + ", " +
					fioField + ", " +
					fEngField + ", " +
					iEngField + ", " +
					employeeField + ", " +
					fField + "RL " + fField + ", " +
					iField + "RL " + iField + ", " +
					oField + "RL " + oField + ", " +
					loginField +
				" FROM " + tableName +
				" WHERE " + statusField + (dismiss ? " < 5" : (" < 3" +
				" AND " + accountDisabledField + " IS NOT NULL")),
				null);
		}

		public string GetEmployee(int id, bool fullName)
		{
			return (string)GetField(fullName ? nameField : fioField, id);
		}

		public string GetEmployeeEng(int id, bool fullName)
		{
            return (string)GetField(fullName ? employeeField : iofEngField, id);
		}

		public string GetEmployeeLanguage(int id)
		{
			return (string)GetField(languageField, id);
		}

		public int GetSystemEmployeeID()
		{
			return GetIntField("SELECT TOP 1 " + idField + " FROM " + tableName + " (nolock) WHERE " + sidField + " = SUSER_SID()", idField, null);
		}

		public string GetSystemEmployee()
		{
		    int curEmpID = GetSystemEmployeeID();

		    return curEmpID != 0 ? GetEmployee(curEmpID, false) : null;
		}

	    public int[] GetCurrentEmployeeIDs()
		{
			return GetRecords<int>("SELECT DISTINCT " + currentEmpIDField + " FROM " + fnCurrentEmployee, null).ToArray();
		}

		#region IsInRole

		public bool IsInRole(Roles role)
		{
			return IsInRole((int)role, null, false);
		}

		public bool IsInRole(Roles role, string personIDs)
		{
			return IsInRole((int)role, personIDs, true);
		}

		public bool IsInRole(int role, string personIDs, bool check)
		{
			return IsInRole((int)role, personIDs, check, CancellationToken.None);
		}

		public bool IsInRole(int role, string personIDs, bool check, CancellationToken ct)
		{
			if(!string.IsNullOrEmpty(personIDs) && !personIDs.Trim().StartsWith(","))
				personIDs = "," + personIDs.Replace(" ", "");
			return GetIntField("SELECT " +
				"TOP 1 " + roleIDField +
				" FROM " + fnCurrentRoles +
				" WHERE " + roleIDField + " = @RuleID " +
				(check ? " AND " + personIDField + ((personIDs == null) ? " = 0" : " IN (0" + personIDs + ")") : ""),
				roleIDField,
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@RuleID", SqlDbType.Int, role);
				}, ct) > 0;
		}

		public bool IsSecretary()
		{
			return IsInRole(Roles.FaxSender, null);
		}

		public bool IsFaxReceiver()
		{
			return IsInRole(Roles.FaxReceiver);
		}

		public bool IsFaxSender()
		{
            return IsInRole(Roles.FaxSender) || IsInRole(Roles.FaxReceiver);
		}

		public bool IsFaxSender(string docPersonsIDs)
		{
            return IsInRole(Roles.FaxSender, docPersonsIDs) || IsInRole(Roles.FaxReceiver, docPersonsIDs);
		}

		public bool IsDocDeleter()
		{
			return IsInRole(Roles.DocDeleter);
		}

		#endregion

		public string GetRoles()
		{
			return ReadString("SELECT * FROM " + fnCurrentRoles, null);
		}

		/// <summary>
		/// Получение списка Фио сотрудников по кодам упорядоченое по фио
		/// </summary>
		/// <param name="RecipientIDs">коды сотрудников для получения списка</param>
		/// <returns>строка сотрудников</returns>
		public string GetEmployeesFIO(string RecipientIDs)
		{
			return ReadString("SELECT REPLACE(" + fioField + ",' ',CHAR(160)) Получатели FROM " + tableName + " WHERE " + idField + " IN(" + RecipientIDs + ") ORDER BY " + fioField, null).Replace(",", ", ");
		}

		/// <summary>
		/// Получение списка IOF сотрудников по кодам упорядоченое по IOF
		/// </summary>
		/// <param name="RecipientIDs">коды сотрудников для получения списка</param>
		/// <returns>строка сотрудников</returns>
		public string GetEmployeesIOFEn(string RecipientIDs)
		{
            return ReadString( "SELECT REPLACE(" + iofEngField + ",' ',CHAR(160)) Получатели FROM " + tableName + " WHERE " + idField + " IN(" + RecipientIDs + ") ORDER BY " + iofEngField , null).Replace(",", ", " );
		}

		/// <summary>
		/// Получение контактов сотрудника
		/// </summary>
		/// <param name="id">Код сотрудника</param>
		/// <returns>Датасет с контактами сотрудника</returns>
		public DataTable GetContacts(int id)
		{
			return GetDataTable("Инвентаризация.dbo.sp_КонтактыСотрудника",
				delegate(SqlCommand cmd)
				{
					cmd.CommandType = CommandType.StoredProcedure;
					AddParam(cmd, "@КодСотрудника", SqlDbType.Int, id);
				});
		}

		/// <summary>
		/// Email сотрудника
		/// </summary>
		/// <param name="id">код сотрудника</param>
		/// <returns>email или null</returns>
		public string GetEmpEMail(int id)
		{
			return GetField(emailField, id) as string;
		}

		#endregion
	}
}