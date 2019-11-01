using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Corporate
{
	public class RequestDALC : DALC
	{
		public RequestDALC(string connectionString) : base(connectionString)
		{
			tableName = "vwОценкиИнтерфейса";
			idField = "КодИдентификатораОценки";
		}

		public const string FnGetRating = "dbo.fn_ИтоговаяОценкаСотрудника";
		public const string VersionField = "ВерсияПО";
		public const string RatingField = "Оценка";

		public int GetRating(int empID)
		{
			return GetIntField("SELECT " +  FnGetRating + "('FL', @EmpID)", RatingField, delegate(SqlCommand cmd)
			{
				AddParam(cmd, "@EmpID", System.Data.SqlDbType.Int, empID);
			});
		}

		public bool SetRating(int r)
		{
			return Exec("INSERT INTO " + tableName + "(" + idField + "," + RatingField + ") VALUES ('FL', @Rating)", delegate(SqlCommand cmd)
			{
				AddParam(cmd, "@Rating", System.Data.SqlDbType.Int, r);
			});
		}
	}
}
