using Kesco.Lib.Win.Data.Business.V2.Docs.DomainObjects;

namespace Kesco.Lib.Win.Data.Business.V2.Docs
{
	/// <summary>
	/// Summary description for DocsModule.
	/// </summary>
	public class DocsModule:DBModule
	{
		public void FindДокументы(DsoDoc filter,int[] ids, ref int startRecord, ref int maxRecords)
		{
			Find(filter,ids,ref startRecord,ref maxRecords, null);
		}

		public DocsModule(string connectionString):base(connectionString)
		{
		}		
	}
}
