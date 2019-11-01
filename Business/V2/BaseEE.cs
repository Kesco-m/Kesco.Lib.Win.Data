
#if !CLEAR
namespace Business.V2
{
	/// <summary>
	/// Summary description for BaseEE.
	/// </summary>
	public abstract class BaseEE:V2.Base
	{
		
		public V2.Corporate.Employee Editor
		{
			get
			{
				if (Row.IsNull("Изменил")) return null;
				return new V2.Corporate.Employee(Row["Изменил"].ToString());
			}
			set
			{
				if(value==null) throw new Exception("Необходимо указать сотрудника производящего изменения");
				Row["Изменил"]=value.ID;
			}
		}
		public DateTime Edited
		{
			get
			{
				return Row.IsNull("Изменено")?DateTime.MinValue:(DateTime)Row["Изменено"];
			}
			set
			{
				Row["Изменено"]=value;
			}
		}
		
	
		
		
		public object EditorV3Value
		{
			get{return Row["Изменил"];}	
			set{Row["Изменил"] = value;}
		}
		public object EditedV3Value
		{
			get{return Row["Изменено"];}	
			set{Row["Изменено"] = value;}
		}
		
		
		public BaseEE(V2.Context context, DataRow row):base(context,row)
		{
			
		}
		public BaseEE(V2.Context context, int id):base(context,id)
		{
			
		}
	}
}
#endif