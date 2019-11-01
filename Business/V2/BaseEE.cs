
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
				if (Row.IsNull("�������")) return null;
				return new V2.Corporate.Employee(Row["�������"].ToString());
			}
			set
			{
				if(value==null) throw new Exception("���������� ������� ���������� ������������� ���������");
				Row["�������"]=value.ID;
			}
		}
		public DateTime Edited
		{
			get
			{
				return Row.IsNull("��������")?DateTime.MinValue:(DateTime)Row["��������"];
			}
			set
			{
				Row["��������"]=value;
			}
		}
		
	
		
		
		public object EditorV3Value
		{
			get{return Row["�������"];}	
			set{Row["�������"] = value;}
		}
		public object EditedV3Value
		{
			get{return Row["��������"];}	
			set{Row["��������"] = value;}
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