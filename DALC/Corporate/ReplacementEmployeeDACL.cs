using System.Collections.Generic;
using Kesco.Lib.Win.Data.Temp.Objects;

namespace Kesco.Lib.Win.Data.DALC.Corporate
{
	public class ReplacementEmployeeDALС : DALC
	{
		private const string GET_LEST_QUERY =
@"SELECT DISTINCT СЗА.КодСотрудника, СЗА.Сотрудник, СЗА.ФИО, СЗА.Employee, СЗА.FIO
FROM Инвентаризация.dbo.fn_ТекущийСотрудник() ЗС INNER JOIN
Инвентаризация.dbo.Сотрудники СЗА ON СЗА.КодСотрудника = ЗС.КодСотрудника";

		public ReplacementEmployeeDALС(string connectionString) : base(connectionString)
		{
		}

		public List<Employee> GetReplacementEmployees()//Objects.Employee employee
		{
			return GetRecords<Employee>(GET_LEST_QUERY,
				//cmd => cmd.Parameters.AddWithValue("@КодСотрудника", employee.ID),
				null,
				reader =>
				{
					int empID = (int)reader["КодСотрудника"];
					string fullName = (string)reader["Сотрудник"];
					string shortName = (string)reader["ФИО"];
					string fullNameEn = (string)reader["Employee"];
					string shortNameEn = (string)reader["FIO"];

					Employee emp = new Employee(empID, shortName, fullName, shortNameEn, fullNameEn, new EmployeeDALC(connectionString));
					return emp;
				});
		}
	}
}