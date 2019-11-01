namespace Kesco.Lib.Win.Data.Business.V2.FilterOptions
{
	/// <summary>
	/// Summary description for FOptItemFlags.
	/// </summary>
	public enum FOptItemFlags
	{
		None = 0,

		//��� Option
		IsNull = 0x1,		//	0001

		Equals = 0x2,		//	0010
		Less = 0x4,		//	0100
		More = 0x8,		//	1000
		EqualsOrLess = Equals | Less,
		EqualsOrMore = Equals | More,
		NotEquals = Less | More,

		//������������� ��� IntOption
		/// <summary>
		/// ������� ������� �������� �������� (�� ����������� ������) ���������� �������� 
		/// </summary>
		ChildOf = 0x10,		//  0001 0000
		DirectChildOf = 0x20,		//	0010 0000
		ParentOf = 0x40,		//	0100 0000
		DirectParentOf = 0x80,		//	1000 0000

		SameAs = 0x100		//  0001 0000 0000									
		//������������� ��� TextOption
	}
}