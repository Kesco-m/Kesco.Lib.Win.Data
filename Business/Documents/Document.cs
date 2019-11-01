using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace Kesco.Lib.Win.Data.Business.Documents
{
	public class Document : BaseEE
	{
		const string context_key = "DocumentKey";	//���� �� �������� ���� ��������� �������� �� ���������

		const string type_field = "����������������";
		protected string typeField = type_field;
		const string number_field = "��������������";
		const string date_field = "�������������";
		const string description_field = "��������";
		const string editor_field = "�������";
		const string edited_field = "��������";
		const string dataEditor_field = "�������1";
		const string dataEdited_field = "��������1";

		DocumentType type;	//��� ��������� (������ ����)

		DateTime date;		//���� ���������
		DateTime dateOriginal;	//
		string number;		//����� ���������
		string numberOriginal;	//
		string description;	//�������� ���������

		#region ACCESSORS

		public override bool IsNew
		{
			get
			{
				return Editor == null;	// ���������
			}
		}

		/// <summary>
		/// ��� ���������
		/// </summary>
		public DocumentType Type
		{
			get
			{
				if (type == null) LoadIfDelayed();
				return type;
			}
		}

		/// <summary>
		/// ����� ���������
		/// </summary>
		public string Number
		{
			get
			{
				LoadIfDelayed();
				return number;
			}
			set
			{
				number = value;
			}
		}

		/// <summary>
		/// ���� ��������� 
		/// </summary>
		public DateTime Date
		{
			get
			{
				LoadIfDelayed();
				return date;
			}
			set
			{
				if (date == value) return;
				date = value;
				ClearCache();
			}
		}

		/// <summary>
		/// ������������ ���� ��������� 
		/// </summary>
		public DateTime DateOriginal
		{
			get
			{
				LoadIfDelayed();
				return dateOriginal;
			}
		}

		/// <summary>
		/// �������� ���������
		/// </summary>
		public string Description
		{
			get
			{
				LoadIfDelayed();
				return description;
			}
			set
			{
				description = value;
			}
		}

		#endregion

		#region LABELS

		/// <summary>
		/// ������ ��� ��������� 
		/// ��� ������������� - [���] �[�����] �� [����]
		/// ��� ������ - [���] (�����)
		/// </summary>
		public string FullName
		{
			get
			{
			    if (ID > 0)
					return Type.Name +
						(Number.Length > 0 ? " �" + Number : "") +
						(Date == DateTime.MinValue ? "" : " �� " + Date.ToString("dd.MM.yyyy"));
			    return Type.Name + " (����� ��������)";
			}
		}

		#endregion

		#region Constructors

		//����������� ��� ������������� ���������
		public Document(int id)
			: base(id)
		{
			connectionString = Settings.DS_document;
		}

		//����������� ��� ������ ���������
		public Document(DocumentType type)
		{
			this.type = type;
			connectionString = Settings.DS_document;
			//������ ��������� �� ���������
		}

		protected override void Init()
		{
			base.Init();
			dateOriginal = DateTime.MinValue;
			numberOriginal = "";
		}

		#endregion

		#region XML

		public override void LoadFromXmlElement(XmlElement el)
		{
			base.LoadFromXmlElement(el);

			string atr;

			atr = el.GetAttribute("TypeID");
			type = new DocumentType(int.Parse(atr));

			atr = el.GetAttribute("Date");
			date = atr.Length > 0 ? DateTime.Parse(atr) : DateTime.MinValue;

			atr = el.GetAttribute("DateOriginal");
			dateOriginal = atr.Length > 0 ? DateTime.Parse(atr) : DateTime.MinValue;

			atr = el.GetAttribute("Number");
			number = atr;

			atr = el.GetAttribute("NumberOriginal");
			numberOriginal = atr;

			atr = el.GetAttribute("Description");
			description = atr;

			atr = el.GetAttribute("CurrentPerson");
		}

		#endregion

		#region DataBase
		//�������� �����

		protected override string FillFrom_Table
		{
			get
			{
				return "���������.dbo.vw���������" + " T0 LEFT OUTER JOIN " + "���������.dbo.vw���������������" + " T1 ON T0." + ID_Field + "=T1." + ID_Field;
			}
		}

		protected override string ID_Field
		{
			get
			{
				return "������������";
			}
		}

		protected override string Edited_Field
		{
			get
			{
				return dataEdited_field;
			}
		}

		protected override string Editor_Field
		{
			get
			{
				return dataEditor_field;
			}
		}

		protected override void Load()
		{
			string query = string.Format(@"SELECT * FROM {0} T0 
LEFT OUTER JOIN {1} T1 ON T0.{2}=T1.{2}
WHERE T0.{2}=@ID", new object[] { "���������.dbo.vw���������", "���������.dbo.vw���������������", "������������" });
			var dt = new DataTable();

			var da = new SqlDataAdapter(query, connectionString);
			try
			{
				da.SelectCommand.Parameters.AddWithValue("@ID", ID);
				da.Fill(dt);
			}
			catch (SqlException sex)
			{
				Env.WriteSqlToLog(sex, da.SelectCommand);
			}
			catch (Exception ex)
			{
				Env.WriteToLog(ex);
			}

			if (dt.Rows.Count == 1) Fill(dt.Rows[0]);
			else state = States.LoadFailed;
		}

		protected override void Fill(DataRow row)
		{
			base.Fill(row);
			type = new DocumentType((int)row[type_field]);
			date = row.IsNull(date_field) ? DateTime.MinValue : (DateTime)row[date_field];

			number = row.IsNull(number_field) ? "" : (string)row[number_field];
			description = row.IsNull(description_field) ? "" : (string)row[description_field];

			dateOriginal = date;
			numberOriginal = number;
		}

		#endregion
	}
}
