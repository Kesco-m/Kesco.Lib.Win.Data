using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Directory
{
	/// <summary>
	/// Доступ к связям лиц
	/// </summary>
	public class PersonLinkDALC : DALC
	{
	    private const string parentPersonIDField = "КодЛицаРодителя";
        private const string childPersonIDField = "КодЛицаПотомка";

		private PersonDALC personData;

        private const string parentName = "Parent";
        private const string childName = "Child";
		private string personTableName;
		private string personIDField;
		private string personNameField;

		public PersonLinkDALC(string connectionString)
			: base(connectionString)
		{
			tableName = "Справочники.dbo.vwСвязиЛиц";

			idField = "КодСвязиЛиц";
			nameField = "Описание";

			personData = new PersonDALC(null);

			personTableName = personData.TableName;
			personIDField = personData.IDField;
			personNameField = personData.NameField;
		}

		#region Accessors

		public string ParentPersonIDField
		{
			get { return parentPersonIDField; }
		}

		public string ChildPersonIDField
		{
			get { return childPersonIDField; }
		}

		public string ParentName
		{
			get { return parentName; }
		}

		public string ChildName
		{
			get { return childName; }
		}

		#endregion

		#region Get Data

		public KeyValuePair<KeyValuePair<int, string>, KeyValuePair<int, string>> GetFormatedLink(int linkID)
		{
			return GetRecord<KeyValuePair<KeyValuePair<int, string>, KeyValuePair<int, string>>>(
				"SELECT " +
					parentPersonIDField + ", " +
					childPersonIDField + ", " +
					parentName + "." + personNameField + " " + parentName + ", " +
					childName + "." + personNameField + " " + childName +
				" FROM " + tableName + " (nolock) " +
					" INNER JOIN " + personTableName + " " + parentName + " (nolock) ON " + tableName + "." + parentPersonIDField + " = " + parentName + "." + personIDField +
					" INNER JOIN " + personTableName + " " + childName + " (nolock) ON " + tableName + "." + childPersonIDField + " = " + childName + "." + personIDField +
				" WHERE " + idField + " = @LinkID",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@LinkID", SqlDbType.Int, linkID);
				},
				delegate(IDataRecord dr)
				{
					return new KeyValuePair<KeyValuePair<int, string>, KeyValuePair<int, string>> 
						(new KeyValuePair<int, string>((int)dr[parentPersonIDField], (dr[parentName].Equals(DBNull.Value) ? "#" + dr[parentPersonIDField] : dr[parentName].ToString())), new KeyValuePair<int, string>((int)dr[childPersonIDField], (dr[childName].Equals(DBNull.Value) ? "#" + dr[childPersonIDField] : dr[childName].ToString())));
				});
		}

		public KeyValuePair<int,string> GetFormatedLink(int linkID, int personID)
		{
			return GetRecord<KeyValuePair<int, string>>("SELECT " +
					parentPersonIDField + ", " +
					childPersonIDField + ", " +
					parentName + "." + personNameField + " " + parentName + ", " +
					childName + "." + personNameField + " " + childName +
				" FROM " + tableName + " (nolock) " +
					" INNER JOIN " + personTableName + " " + parentName + " (nolock) ON " + tableName + "." + parentPersonIDField + " = " + parentName + "." + personIDField +
					" INNER JOIN " + personTableName + " " + childName + " (nolock) ON " + tableName + "." + childPersonIDField + " = " + childName + "." + personIDField +
				" WHERE " + idField + " = @LinkID",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@LinkID", SqlDbType.Int, linkID);
				}
				,
				delegate(IDataRecord dr)
				{
					if (personID.Equals(dr[0]))
						if (!dr.IsDBNull(3))
							return new KeyValuePair<int, string>(dr.GetInt32(1), dr[3].ToString());
						else
							return new KeyValuePair<int, string>(dr.GetInt32(1),"#" + dr.GetString(1));
				    if (personID.Equals(dr[1]))
				        if (!dr.IsDBNull(2))
				            return new KeyValuePair<int, string>(dr.GetInt32(0),dr.GetString(2));
				        else
				            return new KeyValuePair<int, string>(dr.GetInt32(0),"#" + dr.GetString(0));
				    return null;
				});
		}

		#endregion
	}
}