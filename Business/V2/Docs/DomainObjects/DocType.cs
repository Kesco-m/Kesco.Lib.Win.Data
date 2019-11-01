using System.Collections.Specialized;
using System.Data;
using System.Text.RegularExpressions;

namespace Kesco.Lib.Win.Data.Business.V2.Docs.DomainObjects
{
    public class DocType : TreeNodeEntity
    {
        private static StringDictionary defaultMapping;

        public static StringDictionary DefaultMapping
        {
            get { return defaultMapping; }
        }

        public string _Name
        {
            get { return GetProp("name"); }
        }

        public string _NameEng
        {
            get { return GetProp("nameEng"); }
        }

        public string _NumGenMethod
        {
            get { return GetProp("numGenMethod"); }
        }

        public string _NumGenType
        {
            get { return GetProp("numGenType"); }
        }

        public string _Url
        {
            get { return GetProp("url"); }
        }

        public string _HelpUrl
        {
            get { return GetProp("helpUrl"); }
        }

        public string _ViewName
        {
            get { return GetProp("view"); }
        }

        public string _Fields
        {
            get
            {
                if (!props.ContainsKey("fields")) LoadFields();
                return props["fields"];
            }
        }

        public override TreeNodeEntity[] TreeNodeChildren
        {
            get { return Children; }
        }

        public DocType[] Children
        {
            get
            {
                StringCollection col = Str2Collection(_Children);
                var children = new DocType[col.Count];
                for (int i = 0; i < children.Length; i++) children[i] = new DocType(col[i]);
                return children;
            }
        }

        public string Name
        {
            get { return _Name; }
        }

        public string NameEng
        {
            get { return _NameEng; }
        }

        public DocType Parent
        {
            get { return _Parent.Length > 0 ? new DocType(_Parent) : null; }
        }

        public override TreeNodeEntity TreeNodeParent
        {
            get { return Parent; }
        }

        public NumGenTypes NumGenType
        {
            get
            {
                if (_NumGenType.Length == 0) return NumGenTypes.CanNotBeGenerated;
                int ngt = int.Parse(_NumGenType);
                if (ngt > 2) return new DocType(_NumGenType).NumGenType;
                return (NumGenTypes) ngt;
            }
        }


        public Field[] Fields
        {
            get
            {
                MatchCollection m = Regex.Matches(_Fields, "\\d+");
                var fields = new Field[m.Count];
                for (int i = 0; i < fields.Length; i++) fields[i] = new Field(m[i].Value);
                return fields;
            }
        }

        public override void Load()
        {
            var dt = new DataTable();
            Env.Docs.Find("SELECT * FROM �������������� WHERE ����������������=" + _ID, dt);
            if (dt.Rows.Count == 1) Populate(dt.Rows[0], DefaultMapping);
            else props["unavailable"] = "1";
        }

        public void LoadFields()
        {
            var dt = new DataTable();
            module.Find(
                "SELECT * FROM �������������� WHERE ����������������=" + _ID + " ORDER BY ��������������������", dt);
            Field fld;

            var col = new StringCollection();
            foreach (DataRow row in dt.Rows)
            {
                fld = new Field(row["����������������"].ToString());
                fld.Populate(row, Field.DefaultMapping);
                col.Add(fld._ID);
            }
            props["fields"] = Collection2Str(col);
        }

        public override void LoadChildren()
        {
            var dt = new DataTable();
            Env.Docs.Find("SELECT * FROM �������������� WHERE Parent=" + _ID, dt);

            DocType type;
            var col = new StringCollection();
            foreach (DataRow row in dt.Rows)
            {
                type = new DocType(row["����������������"].ToString());
                type.Populate(row, DefaultMapping);
                col.Add(type._ID);
            }
            props["children"] = Collection2Str(col);
        }

        public override void LoadParents()
        {
            var dt = new DataTable();
            Env.Docs.Find("SELECT * FROM �������������� WHERE L<" + _L + " AND R>" + _R, dt);

            DocType parent;
            string s = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                s += " " + dt.Rows[i]["����������������"];
                parent = new DocType(dt.Rows[i]["����������������"].ToString());
                parent.Populate(dt.Rows[i], DefaultMapping);
            }

            props["parents"] = s;
        }

        static DocType()
        {
            defaultMapping = new StringDictionary
                                 {
                                     {"����������������", "id"},
                                     {"������������", "name"},
                                     {"TypeDoc", "nameEng"},
                                     {"����������������", "view"},
                                     {"������������", "hasForm"},
                                     {"URL", "url"},
                                     {"SearchURL", "searchUrl"},
                                     {"HelpURL", "helpUrl"},
                                     {"���������", "isOutgoing"},
                                     {"����������������������", "numGenType"},
                                     {"���������������������������", "numGenMethod"},
                                     {"Parent", "parent"},
                                     {"L", "l"},
                                     {"R", "r"},
                                     {"�������", "editor"},
                                     {"��������", "edited"}
                                 };
        }

        public DocType(string id) : base(id, "DocType", Env.Docs)
        {

        }
    }


#if !CLEAR
	public class DocTypeOld:V2.BaseEE
	{
		protected override void AttachRowInContext(ref DataRow row)
		{
			row=Env.Docs.DSSchema.��������������.FindBy����������������(ID);
			if (row==null) throw new Exception("��� ��������� � ����� "+ID+" �� ������");
		}


		ArrayList fields = null;
		
		public int NumberGeneratingType
		{
			get
			{
				if (Row.IsNull("����������������������")) return 0;
				return 	(int)Row["����������������������"];
			}
		}

		
		public bool IsNumberCanBeGenerated
		{
			get
			{
				return (NumberGeneratingType>0);
			}
		}

		public ArrayList Fields
		{
			get
			{
				if (fields==null) 
				{
					fields = new ArrayList();

					DataRow[] rows = context.Docs.��������������.Select("����������������="+ID);
					Env.Docs.Fill��������������By����������������(context.Docs.��������������,ID);
					rows = context.Docs.��������������.Select("����������������="+ID);
					for(int i=0;i<rows.Length;i++)
						fields.Add(new Field(rows[i]["����������������"].ToString()));
				}
				
				return fields;
							
			}
		}
		public Field GetFieldByID(int fieldID)
		{
			foreach(Field f in Fields) if (f.ID==fieldID) return f; 
			return null;
		}
	
		public string Name
		{
			get 
			{
				return Row.IsNull("������������")?"":(string)Row["������������"];
			}
		}
		
		public DocTypeOld(V2.Context context, int id):base(context,id)
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
#endif
}
