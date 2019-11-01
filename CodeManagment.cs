using System;
using System.IO;
using Kesco.Lib.Win.Data.Business.V2;

namespace Kesco.Lib.Win.Data
{
	[Descriptor("������ ��� ����������� ������� ���� ������� � ������� ��������� Data.dll")]
	public class ReportGenerator
	{
		int typeCounter;

		public void Render(TextWriter w)
		{
			w.WriteLine("{0}",DateTime.Now);
			typeCounter=0;
			foreach(Type t in typeof(Entity).Assembly.GetTypes())
			{
				if(!t.IsClass) continue;
				
				typeCounter++;
				w.WriteLine(t.Namespace+"."+t.Name);
				var at= (DescriptorAttribute[])t.GetCustomAttributes(typeof(DescriptorAttribute),false);
				if(at.Length>0) RenderAttribute(w,at[0]);
			}
			
			
			w.WriteLine("����� �������: {0}",typeCounter);
			w.WriteLine("OK");
		}
		void RenderAttribute(TextWriter w,DescriptorAttribute at)
		{
			w.WriteLine(at.Comment);
		}
	}
    
	[Descriptor("������������ �����������, �������� ��������� � DataDll")]
	public enum Users
	{
		DrizhovSergey,
		BulychevNick
	}
	
	[Descriptor("������ ��� ���������� �������, � ���������, � ��������� ����������")]
	[AttributeUsage(AttributeTargets.All,AllowMultiple=true)]
	public class CommentAttribute:Attribute
	{
		Users user;
		DateTime date;
		string comment;
		
		public Users User		{get{return user;}}
		public DateTime Date	{get{return date;}}
		public string Comment	{get{return comment;}}
		
		
		public CommentAttribute(Users user, string date, string comment)
		{
			this.user=user;
			this.date=DateTime.Parse(date);
			this.comment=comment;
		}
	}

	[Descriptor("������ ��� �������� ������� ��� ����������� ������� � ���������� Data.dll")]
	[AttributeUsage(AttributeTargets.All,AllowMultiple=true)]
	public class DescriptorAttribute:Attribute
	{
		string comment;
		
		public string Comment {get{return comment;}}

		public DescriptorAttribute(string comment)
		{
			
			this.comment=comment;
		}
	}
}