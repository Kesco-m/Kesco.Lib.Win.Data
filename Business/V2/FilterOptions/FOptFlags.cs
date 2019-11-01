using System;

namespace Kesco.Lib.Win.Data.Business.V2.FilterOptions
{
	/// <summary>
	/// Summary description for FOptFlags.
	/// </summary>
	[Flags]
	public enum FOptFlags
	{
		None = 0,
		
		Enabled			= 0x1,		//	0001
		Fixed			= 0x2,		//	0010
		Inverse			= 0x4,		//	0100
		
		MatchAnyItem	= 0x8,		// �� ��������� ��� 

//����� �� ���������� ������
		TextBeginsWith	= 0x10,	//����� ���������� � ...
		TextContains	= 0x20,	//����� �������� .. (�� ��������� �������)	
		WordsBeginWith	= 0x30,	 //DEFAULT ����� �������� ����� (����� ������� �� ������ �� �����)
		WordsContain	= 0x40,  //����� ���������� � ������
		TextEquals		= 0x80, //����� ����� ���������

//����� �� ���������
		PersonSearchByName = 0x100,
		PersonSearchByDetails = 0x200,
		PersonSearchByContacts = 0x300,
		PersonSearchByAll = PersonSearchByName|PersonSearchByDetails|PersonSearchByContacts
			
		
		//��� ������
		//��� 
	}
}
