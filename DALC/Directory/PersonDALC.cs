using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Kesco.Lib.Win.Data.DALC.Corporate;

namespace Kesco.Lib.Win.Data.DALC.Directory
{
	/// <summary>
	/// DAL-компонент для доступа к таблице Справочники.dbo.vwЛица
	/// </summary>
	public class PersonDALC : DALC
	{
		private const string typeField = "ТипЛица";

		private const string currentPersonTable = "Справочники.dbo.vwЛицаХолдингаСотрудника";

		private const string orgCardTable = "Справочники.dbo.vwКарточкиЮрЛиц";
		private const string persCardTable = "Справочники.dbo.vwКарточкиФизЛиц";
		private const string orgCardNameRusField = "КраткоеНазваниеРус";
		private const string orgCardNameEngField = "КраткоеНазваниеЛат";

		private const string persLastNameRusField = "ФамилияРус";
		private const string persLastNameEngField = "ФамилияЛат";
		private const string persFirstNameRusField = "ИмяРус";
		private const string persFirstNameEngField = "ИмяЛат";
		private const string persMiddleNameRusField = "ОтчествоРус";
		private const string persMiddleNameEngField = "ОтчествоЛат";

		private const string fromFieldName = "От";
		private const string toFieldName = "До";

		private const string notExistsField = "NOTEXISTS";

		private const string personEmployeeView = "Справочники.dbo.vwЛица_Сотрудники";

		private const string personViewName = "Справочники.dbo.vwЛицаХолдингаАктуальные";

		private const string spFindPerson = "Справочники.dbo.sp_Лица_Поиск";
		private const string fnPersonPermition = "Справочники.dbo.fn_Лица_УровеньДоступа";

		private const string nameRLField = "КличкаRL";
		private const string innField = "ИНН";
		private const string bikField = "БИК";
		private const string kcField = "КорСчет";
		private const string projectField = "КодБизнесПроекта";
		private const string areaField = "КодТерритории";

		private readonly string employeeTable;
		private readonly string employeeIDField;
		private readonly string employeeField;

		private readonly EmployeeDALC empData;

		private int personType = -1;

        public const int AREA_CODE_RUSSIAN_FEDERATION = 188;    

		public PersonDALC(string connectionString) : base(connectionString)
		{
			tableName = "Справочники.dbo.vwЛица";

			idField = "КодЛица";
			nameField = "Кличка";

			empData = new EmployeeDALC(null);

			employeeTable = empData.TableName;
			employeeIDField = empData.IDField;
			employeeField = empData.NameField;
		}

		#region Accessors

		public string OrgCardNameRusField
		{
			get { return orgCardNameRusField; }
		}

		public string NameRLField
		{
			get { return nameRLField; }
		}
		public string INNField
		{
			get { return innField; }
		}
		public string BIKField
		{
			get { return bikField; }
		}
		public string KCField
		{
			get { return kcField; }
		}

		public string ProjectField
		{
			get { return projectField; }
		}

		public string PersonViewName
		{
			get { return personViewName; }
		}

		/// <summary>
		/// Доступ к полю индификатора территории
		/// </summary>
		public string AreaField
		{
			get { return areaField; }
		}

		public int PersonType
		{
			get { return personType; }
		}

		public string OrgCardTable
		{
			get { return orgCardTable; }
		}

		public string PersCardTable
		{
			get { return persCardTable; }
		}

		public string FromFieldName
		{
			get { return fromFieldName; }
		}

		public string ToFieldName
		{
			get { return toFieldName; }
		}

		#endregion

		#region Get Data

		public DataTable FindPersons(string searchText)
		{
			return FindPersons(searchText, 5, true);
		}

		public DataTable FindPersons(string searchText, int searchType, bool force)
		{
			return GetDataTable(
				spFindPerson,
			delegate(SqlCommand cmd)
			{
				cmd.CommandType = CommandType.StoredProcedure;

				AddParam(cmd, "@Search", SqlDbType.NVarChar, searchText);
				AddParam(cmd, "@PersonWhereSearch", SqlDbType.Int, searchType);
				if(force)
				{
					AddParam(cmd, "@PersonValidAt", SqlDbType.DateTime, DateTime.Today);
					AddParam(cmd, "@PersonForSend", SqlDbType.Int, 1);
					AddParam(cmd, "@PersonSelectTop", SqlDbType.Int, 11);
				}
			});
		}

		public DataTable GetPersons(int personType)
		{
			return GetDataTable("SELECT " + tableName + "." + idField + ", " + tableName + "." + nameField + ", " + "ISNULL(" + orgCardTable + "." + orgCardNameRusField + ", " + tableName + "." + nameField + ") " + orgCardNameRusField + " FROM " + tableName + " (nolock) " + " LEFT JOIN " + orgCardTable + " (nolock) ON " + orgCardTable + "." + idField + " = " + tableName + "." + idField + " WHERE " + typeField + " = @Type" + " ORDER BY " + nameField,
				 delegate(SqlCommand cmd)
				 {
					 AddParam(cmd, "@Type", SqlDbType.Int, personType);
				 });
		}

		public string GetPerson(int id)
		{
			object obj = GetField(nameField, id);
			if((obj is string))
				return (string)obj;

			throw new Exception("Не найдено лицо с кодом " + id);
		}

		/// <summary>
		/// Возврат лица 
		/// (необходимо переделать
		/// </summary>
		/// <param name="id">код лица</param>
		/// <returns>строка полного описания лица</returns>
		public string GetFullPerson(int id)
		{
			string retString = GetRecord<string>("SELECT TOP 1 " + tableName + "." + idField + ", " +
				typeField + ", " +
				nameField + ", " +
				persLastNameRusField + ", " +
				persFirstNameRusField + ", " +
				persMiddleNameRusField + ", " +
				persLastNameEngField + ", " +
				persFirstNameEngField + ", " +
				persMiddleNameEngField + ", " +
				orgCardNameRusField + ", " +
				orgCardNameEngField + ", " +
				"CASE WHEN " + persCardTable + "." + fromFieldName + " < GETDATE() AND " + persCardTable + "." + toFieldName + " > GETDATE() THEN 0 " +
				"ELSE CASE WHEN " + orgCardTable + "." + fromFieldName + " < GETDATE() AND " + orgCardTable + "." + toFieldName + " > GETDATE() THEN 0 ELSE 1 END END " + notExistsField + " " +
				"FROM " + tableName + " (nolock) LEFT OUTER JOIN " + persCardTable + " (nolock) " +
				" ON " + tableName + "." + idField + " = " + persCardTable + "." + idField +
				" LEFT OUTER JOIN " + orgCardTable + " (nolock)" +
				" ON " + tableName + "." + idField + " = " + orgCardTable + "." + idField +
				" WHERE " + tableName + "." + idField + " = @PersonID " +
				"ORDER BY CASE WHEN " + persCardTable + "." + fromFieldName + " < GETDATE() AND " + persCardTable + "." + toFieldName + " > GETDATE() THEN 0 " +
				"ELSE CASE WHEN " + orgCardTable + "." + fromFieldName + " < GETDATE() AND " + orgCardTable + "." + toFieldName + " > GETDATE() THEN 0 ELSE 1 END END",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@PersonID", SqlDbType.Int, id);
				},
				delegate(IDataRecord dr)
				{
					personType = Convert.ToInt32(dr[typeField]);
					if(dr[notExistsField].Equals(0))
					{
						if(dr[typeField].Equals((byte)1))
						{
							if(dr[orgCardNameRusField].ToString().Length > 0)
								retString = dr[orgCardNameRusField].ToString();
							else if(dr[orgCardNameEngField].ToString().Length > 0)
								retString = dr[orgCardNameEngField].ToString();
							else
								retString = dr[nameField].ToString();
						}
						else
						{
							if((dr[persLastNameRusField].ToString().Length > 0) || (dr[persFirstNameRusField].ToString().Length > 0) || (dr[persMiddleNameRusField].ToString().Length > 0))
							{
								retString = dr[persLastNameRusField].ToString();
								retString += ((retString.Length > 0) ? " " : "") + dr[persFirstNameRusField];
								retString += ((retString.Length > 0) ? " " : "") + dr[persMiddleNameRusField];
							}
							else if((dr[persLastNameEngField].ToString().Length > 0) || (dr[persFirstNameEngField].ToString().Length > 0) || (dr[persMiddleNameEngField].ToString().Length > 0))
							{
								retString = dr[persLastNameEngField].ToString();
								retString += ((retString.Length > 0) ? " " : "") + dr[persFirstNameEngField];
								retString += ((retString.Length > 0) ? " " : "") + dr[persMiddleNameEngField];
							}
							else
							{
								retString = dr[nameField].ToString();
							}
						}
					}
					else
					{
						retString = dr[nameField] + " (Реквизиты лица не действительны)";
					}
					return retString;
				});

			if(string.IsNullOrEmpty(retString))
				throw new Exception("Не найдено лицо с кодом " + id);
			return retString;
		}

		public DataTable GetPersonEmployees(int personID)
		{
			return GetDataTable("SELECT " +
					employeeTable + "." + employeeIDField + ", " +
					employeeTable + "." + employeeField +
					" FROM " + personEmployeeView + " (nolock) " +
					" INNER JOIN " + employeeTable + " (nolock)" +
					" ON " + employeeTable + "." + employeeIDField + " = " + personEmployeeView + "." + employeeIDField +
					" WHERE " + personEmployeeView + "." + idField + " = @PersonID",
				   delegate(SqlCommand cmd)
				   {
					   AddParam(cmd, "@PersonID", SqlDbType.Int, personID);
				   });
		}

		public DataTable GetCurrentPersons()
		{
			return GetDataTable("SELECT " +
					 currentPersonTable + "." + idField + ", " +
					 currentPersonTable + "." + nameField +
					 " FROM " + currentPersonTable + " (nolock)" + " ORDER BY " + nameField, null);

		}

		public bool CanAddContact(int personID)
		{
			object obj = GetField("select " + fnPersonPermition + "(@PersonID) " + countField,
				countField,
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@PersonID", SqlDbType.Int, personID);
				});
			if(obj is int)
				return ((int)obj > 1);
			try
			{
				return Convert.ToInt32(obj) > 1;
			}
			catch
			{
				return false;
			}
		}

		#endregion

		#region Check Data

		public bool CheckPersons(int[] personIDs)
		{
			if(personIDs.Length == 0)
				return false;

			string personIDstr = "";

			for(int i = 0; i < personIDs.Length; i++)
			{
				personIDstr += ((personIDstr.Length > 0) ? ", " : "");
				personIDstr += personIDs[i].ToString();
			}

			return GetIntField("SELECT TOP 1 " + idField +
				" FROM " + personViewName + " WITH (NOLOCK) WHERE " + idField + " IN (" + personIDstr + ")",
				idField,
				null) > 0;
		}

        // TODO: Реализовать с IDataRecord
        public List<T> CheckPersonsCountry<T>(string personIDs, bool checkin, GetResult del)
        {
            return null;
        }

        public int CheckPersonsCountry(string personIDs, bool checkin)
        {
            return GetIntField("SELECT TOP 1 КодТерритории FROM " + TableName + " (NOLOCK) WHERE КодЛица IN (" + personIDs + ") AND " + AreaField + ((!checkin) ? "=" : "<>") + AREA_CODE_RUSSIAN_FEDERATION, AreaField, null);
        }

		#endregion
    }
}