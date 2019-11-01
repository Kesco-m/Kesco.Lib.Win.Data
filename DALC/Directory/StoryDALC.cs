using System;
using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Directory
{
	/// <summary>
	/// DAL-��������� ��� ������� � ������� �����������.dbo.vw������
	/// </summary>
	public class StoryDALC : DALC
	{
		protected string storeTypeTableName = "�����������.dbo.�����������";
		protected string storeTypeIDField = "�������������";
		protected string keeperIDField = "������������";
		protected string keeperField = "���������";
		protected string managerIDField = "����������������";
		protected string managerField = "�������������";


		private string personTableName;
		private string personIDField;
		private string personNameField;

		public StoryDALC(string connectionString) : base(connectionString)
		{
			tableName = "�����������.dbo.vw������";

			idField = "���������";
			nameField = "�����";
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
						throw new Exception("�� ������ ����� � ����� " + id);
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
