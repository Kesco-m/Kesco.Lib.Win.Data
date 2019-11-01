using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
namespace Kesco.Lib.Win.Data.DALC.Documents.Search
{
	public class Options
	{
		public Options(XmlDocument xml, string cnDocument, string cnPersons)
		{

		}

		public void Add(string name)
		{

		}

		public void Remove(string name)
		{

		}

		private static ResourceManager Resources
		{
			get { return resources ?? (resources = new ResourceManager(typeof(Options))); }
		}

		public const int maxGroupCount = 10;
		public const int maxOptionsInGroupCount = 25;
		private static ResourceManager resources;

		public static int GetGroups(ref int[] IDs, ref string[] names)
		{
			//��������������� ��������� ��� ��������� ����� � ��������� ��������� ������
			int i = 0;
			IDs[i] = 0;
			names[i] = Resources.GetString("Document");
			i++;
			IDs[i] = 1;
			names[i] = Resources.GetString("Image");
			i++;
			IDs[i] = 2;
			names[i] = Resources.GetString("EForm");
			i++;
			IDs[i] = 3;
			names[i] = Resources.GetString("Messages");
			i++;
			IDs[i] = 4;
			names[i] = Resources.GetString("Transactions");
			i++;
			IDs[i] = 5;
			names[i] = Resources.GetString("ImageSings");
			i++;
			IDs[i] = 6;
			names[i] = Resources.GetString("Links");
			i++;
			IDs[i] = 7;
			names[i] = Resources.GetString("Buh");
			i++;
			return i;
		}

		public static int GetOptionsInGroup(int groupID, ref OptionAttribute[] metas)
		{
			//��������������� ��������� ��� ��������� ����� � ��������� ���������
			int i = 0;
			switch(groupID)
			{
				case 0:
                    metas[i] = Option.GetMeta(typeof(Document.������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Document.������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Document.���������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Document.������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Document.��������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Document.�����));
					i++;
                    metas[i] = Option.GetMeta(typeof(Document.����));
					i++;
                    metas[i] = Option.GetMeta(typeof(Document.��������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Document.�������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Document.ChangedBy));
					i++;
					break;

				case 1:
                    metas[i] = Option.GetMeta(typeof(Image.�����������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.����������������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.��������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.�����������������));
					i++;
					metas[i] = Option.GetMeta(typeof(Image.ChangedBy));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.�������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.���������));
					i++;
					//metas[i] = Option.GetMeta(typeof(Image.����������������));
					//i++;
					//metas[i] = Option.GetMeta(typeof(Image.�����������������));
					//i++;
                    metas[i] = Option.GetMeta(typeof(Image.Send.�����������));
					i++;
					//metas[i] = Option.GetMeta(typeof(Image.������������));
					//i++;
					//metas[i] = Option.GetMeta(typeof(Image.�����������));
					//i++;
                    metas[i] = Option.GetMeta(typeof(Image.NotSend.��������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.NotSend.������������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.��������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.�����������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.�����������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.�������������������));
					i++;
					break;
				case 5:
                    metas[i] = Option.GetMeta(typeof(Image.Sign.��������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.Sign.������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.Sign.��������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.NoSign.����������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.NoSign.��������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.Sign.������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.Sign.�����������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.NoSign.�������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.Sign.�����������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.NoSign.��������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Image.Sign.�������������));
					i++;
					break;
				case 2:
                    metas[i] = Option.GetMeta(typeof(EForm.�������));
					i++;
                    metas[i] = Option.GetMeta(typeof(EForm.������������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(EForm.��������������));
					i++;
					metas[i] = Option.GetMeta(typeof(EForm.ChangedBy));
					i++;
                    metas[i] = Option.GetMeta(typeof(EForm.ChangedByMe));
					i++;
                    metas[i] = Option.GetMeta(typeof(EForm.�������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(EForm.������������������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Contract.LinkedToByCurator));
					i++;
                    metas[i] = Option.GetMeta(typeof(Contract.LinkedToByCuratorMe));
					i++;
                    metas[i] = Option.GetMeta(typeof(EForm.����������));
					i++;
                    metas[i] = Option.GetMeta(typeof(EForm.Sign.��������));
					i++;
                    metas[i] = Option.GetMeta(typeof(EForm.Sign.��������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(EForm.NoSign.����������));
					i++;
                    metas[i] = Option.GetMeta(typeof(EForm.Sign.��������));
					i++;
                    metas[i] = Option.GetMeta(typeof(EForm.Sign.������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(EForm.Sign.��������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(EForm.NoSign.����������));
					i++;
                    metas[i] = Option.GetMeta(typeof(EForm.NoSign.��������������));
					i++;
					break;
				case 3:
                    metas[i] = Option.GetMeta(typeof(Message.Incoming.Incoming_By));
					i++;
                    metas[i] = Option.GetMeta(typeof(Message.Incoming.Incoming_Read));
					i++;
                    metas[i] = Option.GetMeta(typeof(Message.Incoming.Incoming_Unread));
					i++;
                    metas[i] = Option.GetMeta(typeof(Message.Incoming.Incoming_Date));
					i++;
                    metas[i] = Option.GetMeta(typeof(Message.Incoming.Incoming_Text));
					i++;
                    metas[i] = Option.GetMeta(typeof(Message.Outgoing.Outgoing_By));
					i++;
                    metas[i] = Option.GetMeta(typeof(Message.Outgoing.Outgoing_Date));
					i++;
                    metas[i] = Option.GetMeta(typeof(Message.Outgoing.Outgoing_Text));
					i++;
                    metas[i] = Option.GetMeta(typeof(Message.FromTo));
					i++;
					break;
				case 4:
                    metas[i] = Option.GetMeta(typeof(Transaction.�����������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Transaction.����������������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Transaction.���������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Transaction.��������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Transaction.��������������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Transaction.������������������������));
					i++;
					break;
				case 6:
                    metas[i] = Option.GetMeta(typeof(Links.�����������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Links.������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Links.����������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Links.��������������));
					i++;
                    metas[i] = Option.GetMeta(typeof(Links.������������������));
					i++;
					break;
				case 7:
                    metas[i] = Option.GetMeta(typeof(_1C.��������));
					i++;
                    metas[i] = Option.GetMeta(typeof(_1C.����������));
					i++;
                    metas[i] = Option.GetMeta(typeof(_1C.�������������));
					i++;
					break;
			}
			return i;
		}

		private static string[] groupNames = new[]
                                                 {
                                                     "��������",
                                                     "�����������",
                                                     "���������",
                                                     "��. �����",
                                                     "�����������",
                                                     "�����",
                                                     "1C",
                                                     "Tab8",
                                                     "Tab9",
                                                     "Tab10"
                                                 };

		public static void FlatternDocument(XmlDocument doc)
		{
			if(doc.DocumentElement == null)
				return;
			XmlElement elIncoming = doc.CreateElement("Option");
			elIncoming.SetAttribute("name", "Message.Incoming");
			XmlElement elOutgoing = doc.CreateElement("Option");
			elOutgoing.SetAttribute("name", "Message.Outgoing");
			XmlElement elImage = doc.CreateElement("Option");
			elImage.SetAttribute("name", "Image.Image");
			XmlElement elSend = doc.CreateElement("Option");
			elSend.SetAttribute("name", "Image.Send");
			XmlElement elNotSend = doc.CreateElement("Option");
			elNotSend.SetAttribute("name", "Image.NotSend");
			XmlElement elSignEform = doc.CreateElement("Option");
			elSignEform.SetAttribute("name", "EForm.Sing");
			XmlElement elSignImage = doc.CreateElement("Option");
			elSignImage.SetAttribute("name", "Image.Sign");
			XmlElement elNoSignImage = doc.CreateElement("Option");
			elNoSignImage.SetAttribute("name", "Image.NoSign");


			XmlNodeList list;
			list = doc.SelectNodes("Options/Option[@name='Message.Incoming']");
			foreach(XmlElement elOption in list)
				doc.DocumentElement.RemoveChild(elOption);
			list = doc.SelectNodes("Options/Option[@name='Message.Outgoing']");
			foreach(XmlElement elOption in list)
				doc.DocumentElement.RemoveChild(elOption);

			list = doc.SelectNodes("Options/Option[@name='Image.Image']");
			foreach(XmlElement elOption in list)
				doc.DocumentElement.RemoveChild(elOption);
			list = doc.SelectNodes("Options/Option[@name='Image.Send']");
			foreach(XmlElement elOption in list)
				doc.DocumentElement.RemoveChild(elOption);
			list = doc.SelectNodes("Options/Option[@name='Image.NotSend']");
			foreach(XmlElement elOption in list)
				doc.DocumentElement.RemoveChild(elOption);
			list = doc.SelectNodes("Options/Option[@name='Image.Sign']");
			foreach(XmlElement elOption in list)
				doc.DocumentElement.RemoveChild(elOption);
			list = doc.SelectNodes("Options/Option[@name='Image.NoSign']");
			foreach(XmlElement elOption in list)
				doc.DocumentElement.RemoveChild(elOption);
			list = doc.SelectNodes("Options/Option[@name='EForm.Sing']");
			foreach(XmlElement elOption in list)
				doc.DocumentElement.RemoveChild(elOption);

			list = doc.SelectNodes("Options/Option");
			foreach(XmlElement elOption in list)
			{
				switch(elOption.GetAttribute("name"))
				{
					case "Message.Incoming.By":
					case "Message.Incoming.Read":
					case "Message.Incoming.Unread":
					case "Message.Incoming.Date":
					case "Message.Incoming.Text":
					case "Message.Incoming.Chief":
					case "Message.Incoming.Employee":
						elIncoming.SetAttribute("enabled", "true");
						break;

					case "Message.Outgoing.By":
					case "Message.Outgoing.Date":
					case "Message.Outgoing.Text":
						elOutgoing.SetAttribute("enabled", "true");
						break;

					case "Image.���������":
					case "Image.�����������":
					case "Image.�����������������":
					case "Image.ChangedBy":
					case "Image.��������":
					case "Image.�������������":
					case "Image.����������������":
					case "Image.�����������������":
					case "Image.�����������������":
					case "Image.�����������������":
					case "Image.�������������������":
						elImage.SetAttribute("enabled", "true");
						break;

					case "Image.�����������":
					case "Image.������������":
					case "Image.�����������":
						elSend.SetAttribute("enabled", "true");
						break;

					case "Image.������������������":
					case "Image.��������������":
						elNotSend.SetAttribute("enabled", "true");
						break;

					case "EForm.Sing.��������":
					case "EForm.Sing.��������������":
						elSignEform.SetAttribute("enabled", "true");
						break;

					case "Image.Sing.������������":
					case "Image.Sing.�����������������":
						elSignImage.SetAttribute("enabled", "true");
						break;
					case "Image.NoSing.����������":
					case "Image.NoSing.�������������":
					case "Image.NoSing.��������������":
					case "Image.NoSing.��������������":
						elNoSignImage.SetAttribute("enabled", "true");
						break;
				}
			}

			if(elOutgoing.Attributes.Count > 1)
				doc.DocumentElement.AppendChild(elOutgoing);
			if(elIncoming.Attributes.Count > 1)
				doc.DocumentElement.AppendChild(elIncoming);
			if(elImage.Attributes.Count > 1)
				doc.DocumentElement.AppendChild(elImage);
			if(elSend.Attributes.Count > 1)
				doc.DocumentElement.AppendChild(elSend);
			if(elNotSend.Attributes.Count > 1)
				doc.DocumentElement.AppendChild(elNotSend);
			if(elSignEform.Attributes.Count > 1)
				doc.DocumentElement.AppendChild(elSignEform);
			if(elSignImage.Attributes.Count > 1)
				doc.DocumentElement.AppendChild(elSignImage);
			if(elNoSignImage.Attributes.Count > 1)
				doc.DocumentElement.AppendChild(elNoSignImage);
		}

		public static string SelectFrom = "SELECT *	FROM ���������.dbo.vw��������� as T0 WITH(NOLOCK)";

		public static string GetSQL(XmlDocument doc)
		{
			string ret = "";
			string opt;
			FlatternDocument(doc);

			XmlNodeList list = doc.SelectNodes("Options/Option");
			Option option;

			foreach(XmlElement elOption in list)
			{
				if(elOption.GetAttribute("name") == "��������� � ������")
					elOption.SetAttribute("name", "�������");
				else if(elOption.GetAttribute("name") == "EForm.NoSing")
					continue;
				option = Option.CreateOption(elOption);
				opt = option.GetSQL(false);

				if(opt == null)
					continue;
				ret += "\n" + (ret.Length == 0 ? "WHERE" : "AND") + " (" + opt + ")";
			}

			ret = SelectFrom + "\n" + ret;

			return ret;
		}

		public static string GetSQL(string xml)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			return GetSQL(doc);
		}

		public static bool HasOption(XmlDocument doc, Type optionType)
		{
			OptionAttribute atr = Option.GetMeta(optionType);

			XmlNodeList list = doc.SelectNodes("Options/Option[@name='" + atr.Name + "']");
			if(list == null || (list.Count < 1 && atr.Name.Equals("�������")))
				list = doc.SelectNodes("Options/Option[@name='��������� � ������']");

			return list != null && list.Count > 0;
		}

		public static bool HasOption(string xml, Type optionType)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			return HasOption(doc, optionType);
		}

		public static string GetHTML(XmlDocument doc)
		{
			var sb = new StringBuilder();

			XmlNodeList list = doc.SelectNodes("Options/Option");
			Option option;

			foreach(XmlElement elOption in list)
			{
				option = Option.CreateOption(elOption);
				if(option.GetHTML().Length == 0 || option.IsSubOption())
					continue;
				if(sb.Length != 0)
					sb.Append(" " + Resources.GetString("And") + " ");
				sb.Append(option.GetHTML());
				var suboptions = option.GetSubOptions();
				if(suboptions != null && suboptions.Count > 0)
				{
					Option subOption;
					SortedList<int, Option> sOptions = new SortedList<int, Option>();
					foreach(string str in suboptions)
					{
						XmlElement subxml = (XmlElement)doc.SelectSingleNode("Options/Option[@name='" + str + "']");
						if(subxml != null)
							subOption = Option.CreateOption(subxml);
						else
						{
							var elOptions = (XmlElement)doc.SelectSingleNode("Options");
							if(elOptions != null)
							{
								subxml = doc.CreateElement("Option");
								subxml.SetAttribute("name", str);
								elOptions.AppendChild(elOption);
							}
						}
						subOption = Option.CreateOption(subxml);
						sOptions.Add(subOption.Meta.Index, subOption);
					}
					for(int k = 1; k <= sOptions.Count; k++)
					{
						sb.Append(" ");
						sb.Append(sOptions[k].GetHTML());
					}
				}
				sb.Append("<br>\n");
			}
			return sb.ToString();

		}

		public static string GetHTML(string xml)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			return GetHTML(doc);
		}

		public static string GetText(XmlDocument doc)
		{
			var sb = new StringBuilder();
			FlatternDocument(doc);
			XmlNodeList list = doc.SelectNodes("Options/Option");
			Option option;
			string s;
			foreach(XmlElement elOption in list)
			{
				if(elOption.GetAttribute("name") == "��������� � ������")
					elOption.SetAttribute("name", "�������");
				else if(elOption.GetAttribute("name") == "EForm.NoSing")
					continue;
				option = Option.CreateOption(elOption);
				s = option.GetText();
				if(s.Length == 0 || s == ".")
					continue;

				if(sb.Length != 0)
					sb.Append("\n " + Resources.GetString("And") + " ");
				sb.Append(s);
			}
			return sb.ToString();
		}

		public static string GetText(string xml)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			return GetText(doc);
		}

		public static string GetShortText(XmlDocument doc)
		{
			var sb = new StringBuilder();
			FlatternDocument(doc);
			XmlNodeList list = doc.SelectNodes("Options/Option");
			Option option;
			string s;
			foreach(XmlElement elOption in list)
			{
				if(elOption.GetAttribute("name") == "��������� � ������")
					elOption.SetAttribute("name", "�������");
				else if(elOption.GetAttribute("name") == "EForm.NoSing")
					continue;
				option = Option.CreateOption(elOption);

				s = option.GetShortText();
				if(s.Length == 0)
					continue;
				if(sb.Length != 0)
					sb.Append(";");
				sb.Append(s);
			}
			return sb.ToString();
		}

		public static string GetShortText(string xml)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			return GetShortText(doc);
		}

		public static XmlElement GetSearchElementFormQueryString(string query)
		{
			var xml = new XmlDocument();
			XmlElement elSearch = xml.CreateElement("search");

			string key;
			string val;

			string[] pair;
			if(query != null)
				foreach(string x in query.Split('&'))
				{
					pair = x.Split('=');
					if(pair.Length != 2)
						continue;
					key = HttpUtility.UrlDecode(pair[0]);
					val = HttpUtility.UrlDecode(pair[1]).Trim();
					if(val.Equals(""))
						continue;

					elSearch.SetAttribute(key, val);
				}
			return elSearch;
		}

		public static string GetXMLFromSearchElement(XmlElement elSearch)
		{
			bool isA;
			string val;
			string[] arr;

			XmlDocument xml = GetXmlDocument(elSearch.GetAttribute("options"));
			XmlElement elOption;


			Match m;
			foreach(XmlAttribute a in elSearch.Attributes)
			{

				if((m = Regex.Match(a.Name, "^��������$", RegexOptions.IgnoreCase)).Success)
				// ������� �����// ����� ������
				{
					elOption = GetElOption(xml, "��������");
				}


				if((m = Regex.Match(a.Name, "^title$", RegexOptions.IgnoreCase)).Success)
				{
					xml.DocumentElement.SetAttribute("title", elSearch.GetAttribute(m.Groups[0].Value));
				}
				if((m = Regex.Match(a.Name, "^search$", RegexOptions.IgnoreCase)).Success)
				{
					elOption = GetElOption(xml, "������������");
					elOption.SetAttribute("value", elSearch.GetAttribute(m.Groups[0].Value));
				}
				if((m = Regex.Match(a.Name, "^([_]{0,1})types$", RegexOptions.IgnoreCase)).Success)
				{
					arr = elSearch.GetAttribute(m.Groups[0].Value).Replace('a', 'A').Split('A');
					val = arr[0];
					isA = arr.Length > 1;

					elOption = GetElOption(xml, "������������");
                    elOption.SetAttribute("value", Document.������������.NormalizeS(val));
					elOption.SetAttribute("fixed", m.Groups[1].Value.Length == 0 ? "true" : "false");
				}
				if((m = Regex.Match(a.Name, "^([_]{0,1})persons$", RegexOptions.IgnoreCase)).Success)
				{
					arr = elSearch.GetAttribute(m.Groups[0].Value).Replace('a', 'A').Split('A');
					val = arr[0];
					isA = arr.Length > 1;

					elOption = GetElOption(xml, "���������������");
					elOption.SetAttribute("value", val);
					elOption.SetAttribute("mode", isA ? "and" : "or");
					elOption.SetAttribute("fixed", m.Groups[1].Value.Length == 0 ? "true" : "false");
				}

				if((m = Regex.Match(a.Name, "^([_]{0,1})date$", RegexOptions.IgnoreCase)).Success)
				{
					isA = m.Groups[1].Value.Length == 0;
					val = elSearch.GetAttribute(m.Groups[0].Value);
					m = Regex.Match(val, "^[_]{0,1}(\\d{8})([M]{0,1})([B]{0,1})$", RegexOptions.IgnoreCase);
					if(m.Success)
					{
						DateTime date = DateTime.ParseExact(m.Groups[1].Value, "yyyyMMdd", CultureInfo.InvariantCulture);
						elOption = GetElOption(xml, "����");
						if(m.Groups[2].Value.Length > 0)
							elOption.SetAttribute("min", date.ToString("dd.MM.yyyy"));
						else
						{
							elOption.SetAttribute("max", date.ToString("dd.MM.yyyy"));
							if(m.Groups[3].Value.Length == 0)
								elOption.SetAttribute("min", date.ToString("dd.MM.yyyy"));
						}
						elOption.SetAttribute("fixed", isA ? "true" : "false");
					}
				}
			}

			return xml.OuterXml;
		}

		private static XmlElement GetElOption(XmlDocument xml, string name)
		{
			XmlElement elOption = (XmlElement)xml.SelectSingleNode("Options/Option[@name=\"" + name + "\"]");
			if(elOption == null)
			{
				elOption = xml.CreateElement("Option");
				elOption.SetAttribute("name", name);
				xml.DocumentElement.AppendChild(elOption);
			}

			return elOption;
		}

		private static XmlDocument GetXmlDocument(string xml)
		{
			var doc = new XmlDocument();
			try
			{
				doc.LoadXml(xml);
				if(doc.SelectSingleNode("Options") == null)
					throw new Exception("1");
			}
			catch
			{
				if(doc.DocumentElement != null)
					doc.RemoveChild(doc.DocumentElement);
				doc.AppendChild(doc.CreateElement("Options"));
			}
			return doc;
		}

		public static string PrepareTextParameter(string text)
		{
			string ret = text;
			using(var cm = new SqlCommand())
			using(cm.Connection = new SqlConnection(Settings.DS_document))
			{
				cm.CommandText = "SELECT ��������������.dbo.fn_SplitWords(��������������.dbo.fn_ReplaceKeySymbols(@text))";
				cm.Parameters.AddWithValue("@text", text);

				try
				{
					cm.Connection.Open();
					ret = (string)cm.ExecuteScalar();
				}
				catch(SqlException sex)
				{
					Env.WriteSqlToLog(sex, cm);
				}
				catch(Exception ex)
				{
					Env.WriteToLog(ex);
				}
				finally
				{
					cm.Connection.Close();
				}
			}
			return ret;
		}
	}
}
