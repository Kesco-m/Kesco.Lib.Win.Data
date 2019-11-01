using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image
{
	[Option("Image.Image", ".")]
	public class Image : Option
	{
		private ����������� oImage;
		private ����������������� oDate;
		private ChangedBy oEditer;
		private �������� oSaver;
		private ������������� oEditDate;
		private ����������������� oStoregeDate;
		private ���������������� oEditStorage;
		private ����������������� oImageSize;
		private ����������������� oImagePages;
		private ��������� oStore;
		private ������������������� oFileType;

		protected Image(XmlElement el)
			: base(el)
		{
			XmlElement el0;
			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.�����������']");
			if(el0 != null)
				oImage = (�����������)CreateOption(el0);

			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.�����������������']");
			if(el0 != null)
				oDate = (�����������������)CreateOption(el0);

			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.ChangedBy']");
			if(el0 != null)
				oEditer = (ChangedBy)CreateOption(el0);

			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.��������']");
			if(el0 != null)
				oSaver = (��������)CreateOption(el0);

			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.�������������']");
			if(el0 != null)
				oEditDate = (�������������)CreateOption(el0);

			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.�����������������']");
			if(el0 != null)
				oStoregeDate = (�����������������)CreateOption(el0);

			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.����������������']");
			if(el0 != null)
				oEditStorage = (����������������)CreateOption(el0);
			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.���������']");
			if(el0 != null)
				oStore = (���������)CreateOption(el0);
			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.�����������������']");
			if(el0 != null)
				oImageSize = (�����������������)CreateOption(el0);
			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.�����������������']");
			if(el0 != null)
				oImagePages = (�����������������)CreateOption(el0);
			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.�������������������']");
			if(el0 != null)
				oFileType = (�������������������)CreateOption(el0);
		}

		public override string GetHTML()
		{
			return string.Empty;
		}

		public override string GetSQL(bool throwOnError)
		{
			string s =
				@"
                EXISTS (SELECT *
                FROM ���������.dbo.vw��������������������� TI WITH(NOLOCK)
                WHERE TI.������������=T0.������������";

			if(oDate != null)
				s += " AND (" + oDate.GetSQLCondition2("TI.���������") + ")";
			if(oStore != null)
				s += " AND (" + oStore.GetSQLCondition2("TI.������������") + ")";
			if(oSaver != null)
				s += " AND (" + oSaver.GetSQLCondition2("TI.�������� = @VAL") + ")";
			if(oEditer != null)
				s += " AND (" + oEditer.GetSQLCondition2("TI.������� = @VAL") + ")";
			if(oEditDate != null)
				s += " AND (" + oEditDate.GetSQLCondition2("TI.��������") + ")";
			if(oStoregeDate != null && oStoregeDate.Mode != MinMaxOption.Modes.None)
				s += " AND (" + oStoregeDate.GetSQLCondition2("TI.�����������������") + ")";
			if(oEditStorage != null && !string.IsNullOrEmpty(oEditStorage.Value))
				s += " AND (" + oEditStorage.GetSQLCondition2("TI.���������������� = @VAL") + ")";
			if(oImageSize != null)
				s += " AND (" + oImageSize.GetSQLCondition2("TI.������") + ")";
			if(oImagePages != null)
				s += " AND (" + oImagePages.GetSQLCondition2("TI.�������") + ")";
			if(oFileType != null && (oFileType.Value.Equals("1") || oFileType.Value.Equals("2")))
				s += " AND TI.�������������� = " + (oFileType.Value.Equals("1") ? "'TIF'" : "'PDF'");
			s += ")\n";

			return s;
		}
	}
}