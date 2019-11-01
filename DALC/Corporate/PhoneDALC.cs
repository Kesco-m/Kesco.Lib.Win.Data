using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Corporate
{
	public class PhoneDALC : DALC
	{
		public PhoneDALC(string connectionString): base(connectionString)
		{
		}

		public string GetInternationalNumber(string number, string compName)
		{
			return GetRecord<string>("SELECT dbo.fn_НомерНабора2НомерМеждународный(@НомерНабора, null, @СетевоеИмя)",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@НомерНабора", SqlDbType.VarChar, number);
					AddParam(cmd, "@СетевоеИмя", SqlDbType.VarChar, compName);
				}
			, null);
		}
	}
}
