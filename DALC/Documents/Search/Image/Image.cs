using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image
{
	[Option("Image.Image", ".")]
	public class Image : Option
	{
		private Изображение oImage;
		private ДатаАрхивирования oDate;
		private ChangedBy oEditer;
		private Сохранил oSaver;
		private ДатаИзменения oEditDate;
		private ИзмененоХранилище oStoregeDate;
		private ИзменилХранилище oEditStorage;
		private РазмерИзображения oImageSize;
		private КоличествоСтраниц oImagePages;
		private Хранилище oStore;
		private ТипФайлаИзображения oFileType;

		protected Image(XmlElement el)
			: base(el)
		{
			XmlElement el0;
			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.Изображение']");
			if(el0 != null)
				oImage = (Изображение)CreateOption(el0);

			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.ДатаАрхивирования']");
			if(el0 != null)
				oDate = (ДатаАрхивирования)CreateOption(el0);

			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.ChangedBy']");
			if(el0 != null)
				oEditer = (ChangedBy)CreateOption(el0);

			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.Сохранил']");
			if(el0 != null)
				oSaver = (Сохранил)CreateOption(el0);

			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.ДатаИзменения']");
			if(el0 != null)
				oEditDate = (ДатаИзменения)CreateOption(el0);

			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.ИзмененоХранилище']");
			if(el0 != null)
				oStoregeDate = (ИзмененоХранилище)CreateOption(el0);

			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.ИзменилХранилище']");
			if(el0 != null)
				oEditStorage = (ИзменилХранилище)CreateOption(el0);
			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.Хранилище']");
			if(el0 != null)
				oStore = (Хранилище)CreateOption(el0);
			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.РазмерИзображения']");
			if(el0 != null)
				oImageSize = (РазмерИзображения)CreateOption(el0);
			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.КоличествоСтраниц']");
			if(el0 != null)
				oImagePages = (КоличествоСтраниц)CreateOption(el0);
			el0 = (XmlElement)el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.ТипФайлаИзображения']");
			if(el0 != null)
				oFileType = (ТипФайлаИзображения)CreateOption(el0);
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
                FROM Документы.dbo.vwИзображенияДокументов TI WITH(NOLOCK)
                WHERE TI.КодДокумента=T0.КодДокумента";

			if(oDate != null)
				s += " AND (" + oDate.GetSQLCondition2("TI.Сохранено") + ")";
			if(oStore != null)
				s += " AND (" + oStore.GetSQLCondition2("TI.КодХранилища") + ")";
			if(oSaver != null)
				s += " AND (" + oSaver.GetSQLCondition2("TI.Сохранил = @VAL") + ")";
			if(oEditer != null)
				s += " AND (" + oEditer.GetSQLCondition2("TI.Изменил = @VAL") + ")";
			if(oEditDate != null)
				s += " AND (" + oEditDate.GetSQLCondition2("TI.Изменено") + ")";
			if(oStoregeDate != null && oStoregeDate.Mode != MinMaxOption.Modes.None)
				s += " AND (" + oStoregeDate.GetSQLCondition2("TI.ИзмененоХранилище") + ")";
			if(oEditStorage != null && !string.IsNullOrEmpty(oEditStorage.Value))
				s += " AND (" + oEditStorage.GetSQLCondition2("TI.ИзменилХранилище = @VAL") + ")";
			if(oImageSize != null)
				s += " AND (" + oImageSize.GetSQLCondition2("TI.Размер") + ")";
			if(oImagePages != null)
				s += " AND (" + oImagePages.GetSQLCondition2("TI.Страниц") + ")";
			if(oFileType != null && (oFileType.Value.Equals("1") || oFileType.Value.Equals("2")))
				s += " AND TI.ТипИзображения = " + (oFileType.Value.Equals("1") ? "'TIF'" : "'PDF'");
			s += ")\n";

			return s;
		}
	}
}