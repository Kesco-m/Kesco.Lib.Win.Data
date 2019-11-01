using System;
using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Directory
{
	/// <summary>
	/// DAL-компонент для доступа к таблице Справочники.dbo.vwСклады
	/// </summary>
	public class StoryDALC : DALC
	{
		protected string storeTypeTableName = "Справочники.dbo.ТипыСкладов";
		protected string storeTypeIDField = "КодТипаСклада";
		protected string keeperIDField = "КодХранителя";
		protected string keeperField = "Хранитель";
		protected string managerIDField = "КодРаспорядителя";
		protected string managerField = "Распорядитель";


		private string personTableName;
		private string personIDField;
		private string personNameField;

		public StoryDALC(string connectionString) : base(connectionString)
		{
			tableName = "Справочники.dbo.vwСклады";

			idField = "КодСклада";
			nameField = "Склад";
			var personData = new PersonDALC( connectionString);
			personTableName = personData.TableName;
			personIDField = personData.IDField;
			personNameField = personData.NameField;
		}

		#region Accessors


		#endregion

		public string StoreTypeIDField
		{
			get { return storeTypeIDField;}
		}

		public string KeeperIDField
		{
			get { return keeperIDField;}
		}

		public string KeeperField
		{
			get { return keeperField;}
		}

		#region Get Data

		public string GetStory(int id)
		{
			return GetRecord<string>("SELECT TOP 1 " + storeTypeIDField + ", " + nameField + ", " + keeperIDField + ", " + managerIDField + ", " +
				personNameField + " " + keeperField +
				" FROM " + tableName + " (nolock) INNER JOIN " +
				personTableName + " (nolock) ON " + tableName + "." + keeperIDField + " = " + personTableName + "." + personIDField +
				" WHERE " + idField + " = @ID", delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@ID", SqlDbType.Int, id);
				},
				delegate(IDataRecord dr)
				{
					string storeStr;
					if(dr[nameField] is string)
						storeStr = (string)dr[nameField];
					else
						throw new Exception("Не найден склад с кодом " + id);
					if(dr[storeTypeIDField] is int && (int)dr[storeTypeIDField] < 10)
					{
						if(dr[keeperField] is string)
							storeStr += " " + dr[keeperField] + "";
						else
							storeStr += " #" + dr[keeperIDField] + "";
					}

					return storeStr;
				});
			}

		#endregion
	}
}
