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
		
		MatchAnyItem	= 0x8,		// по умолчанию все 

//флаги по текстовому поиску
		TextBeginsWith	= 0x10,	//текст начинается с ...
		TextContains	= 0x20,	//текст содержит .. (не удаляются символы)	
		WordsBeginWith	= 0x30,	 //DEFAULT слова содержат текст (слова берутся из текста по маске)
		WordsContain	= 0x40,  //слова начинаются с текста
		TextEquals		= 0x80, //текст точно совпадает

//Флаги по сущностям
		PersonSearchByName = 0x100,
		PersonSearchByDetails = 0x200,
		PersonSearchByContacts = 0x300,
		PersonSearchByAll = PersonSearchByName|PersonSearchByDetails|PersonSearchByContacts
			
		
		//для дерева
		//для 
	}
}
