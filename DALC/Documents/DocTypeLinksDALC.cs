using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	/// <summary>
	/// DAL-компонент для доступа к таблице Документы.dbo.СвязиТиповДокументов
	/// </summary>
	public class DocTypeLinksDALC : DALC
	{
		private const string parentTypeIDField = "КодТипаДокументаОснования";
		private const string childTypeIDFeild = "КодТипаДокументаВытекающего";
		private const string fieldIDField = "КодПоляДокумента";
		private const string linkTypeField = "ТипСвязи";

		private string docTypeTableName;
		private string docTypeIDField;
		private string nameLangField;

		private string docFieldTable;
		private string docFieldNameField;
		private string docFieldNameLangField;

		public DocTypeLinksDALC(string connectionString) : base(connectionString)
		{
			tableName = "Документы.dbo.СвязиТиповДокументов";

			var docTypeData = new DocTypeDALC(connectionString);
			docTypeTableName = docTypeData.TableName;
			docTypeIDField = docTypeData.IDField;
			nameField = docTypeData.NameField;

			var fieldData = new FieldDALC(connectionString);
			docFieldTable = fieldData.TableName;
			docFieldNameField = fieldData.NameField;
			if(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("ru"))
			{
				nameLangField = docTypeData.NameField + " + ' ' + " + docTypeData.AddNameField;
				docFieldNameLangField = fieldData.NameField;
			}
			else
			{
				nameLangField = docTypeData.TypeDocField + " + ' ' + " + docTypeData.AddNameEngField;
				docFieldNameLangField = fieldData.FieldNameEngField;
			}
		}

		#region Accessors

		public string ParentTypeIDField
		{
			get { return parentTypeIDField; }
		}

		public string ChildTypeIDFeild
		{
			get { return childTypeIDFeild; }
		}

		public string LinkTypeField
		{
			get { return linkTypeField; }
		}

		public string FieldIDField
		{
			get { return fieldIDField; }
		}

		#endregion

		#region Get Data

		/// <summary>
		/// Получение всех кодов типов, связанных с заданым типом документа
		/// </summary>
		/// <param name="typeID">Код типа для получения</param>
		/// <returns></returns>
		public List<object[]> GetAllLinkedTypesID(int typeID)
		{
			return GetRecords<object[]>("SELECT " + parentTypeIDField + "," + childTypeIDFeild + " FROM " + tableName +
			   " WHERE " + parentTypeIDField + "= @TypeID" +
			   " UNION SELECT " + parentTypeIDField + "," + childTypeIDFeild + " FROM " + tableName +
			   " WHERE " + childTypeIDFeild + "= @TypeID",
			   cmd => AddParam(cmd, "@TypeID", SqlDbType.Int, typeID),
			   dr => new object[2] { dr.GetInt32(0), dr.GetInt32(1) });
		}

		public DataSet GetParentLinkedTypesID(int typeID)
		{
			return GetData("SELECT " + childTypeIDFeild + ", " + fieldIDField + " FROM " + tableName + " WHERE " + parentTypeIDField + "= @TypeID",
						delegate(SqlCommand cmd)
						{
							AddParam(cmd, "@TypeID", SqlDbType.Int, typeID);
						});
		}

		public DataTable GetLinkedTypes(int typeID)
		{
			return GetDataTable("SELECT " + childTypeIDFeild + ", " + nameLangField + " " + nameField + ", " + tableName + "." +
						fieldIDField + ", " + docFieldNameLangField + " " + docFieldNameField + " FROM " + tableName +
						" INNER JOIN " + docTypeTableName + " ON " + tableName + "." + childTypeIDFeild + " = " +
						docTypeTableName + "." + docTypeIDField + " INNER JOIN " + docFieldTable + " ON " + tableName +
						"." + fieldIDField + " = " + docFieldTable + "." + fieldIDField + " WHERE " + parentTypeIDField + " = @TypeID",
						delegate(SqlCommand cmd)
						{
							AddParam(cmd, "@TypeID", SqlDbType.Int, typeID);
						});
		}

		#endregion
	}
}
