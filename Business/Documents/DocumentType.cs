using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Kesco.Lib.Win.Data.Business.Documents
{
	public class DocumentType:Base
	{
		Type type;		
		string name;
		int l;	//left
		int r;	//right

		string url;

		DocumentType[] allChildren;

		#region ACCESSORS

		/// <summary>
		/// имя типа документа
		/// </summary>
		public string Name
		{
			get
			{
				LoadIfDelayed();
				return name;
			}
		}

		public string URL
		{
			get
			{
				if (url==null) LoadIfDelayed();
				return url;
			}
		}
		
		/// <summary>
		/// Left
		/// </summary>
		public int L
		{
			get
			{
				LoadIfDelayed();
				return l;
			}
		}
		
		/// <summary>
		/// Right
		/// </summary>
		public int R
		{
			get
			{
				LoadIfDelayed();
				return r;
			}
		}


		/// <summary>
		/// Подчиненные типы, включая этот
		/// </summary>
		public DocumentType[] AllChildren
		{
			get { return allChildren ?? (allChildren = GetChildren(true)); }
		}

		public string AllChildrenIDs
		{
			get
			{
			    return AllChildren.Aggregate("", (current, t) => current + ((current.Length == 0 ? "" : ",") + t.ID));
			}
		}
		
		/// <summary>
		/// тип класса документа
		/// </summary>
		public Type Type
		{
			get
			{
				return type;
			}
		}

		#endregion

		public DocumentType[] GetChildren(bool recursive)
		{
		    DocumentType[] types = null;
		    string query = "SELECT * FROM " + FillFrom_Table + " WHERE " + nameField + " IS NOT NULL AND " + L + "<=" + lField +
		                   " AND " + rField + "<=" + R + " ORDER BY " + lField;
		    using (var da = new SqlDataAdapter(query, Settings.DS_document))
		    using (var dt = new DataTable())
		    {
		        try
		        {
		            da.Fill(dt);
		            types = new DocumentType[dt.Rows.Count];

		            for (int i = 0; i < types.Length; i++)
		            {
		                types[i] = new DocumentType(0);
		                types[i].Fill(dt.Rows[i]);
		            }
		        }
		        catch (SqlException sex)
		        {
		            Env.WriteSqlToLog(sex, da.SelectCommand);
		        }
		        catch (Exception ex)
		        {
		            Env.WriteToLog(ex);
		        }
		    }
		    return types;
		}

	    #region Fields
		protected override string FillFrom_Table
		{
			get
			{
				return "Документы.dbo.ТипыДокументов";
			}
		}
		protected override string ID_Field
		{
			get
			{
				return "КодТипаДокумента";
			}
		}
		const string nameField="ТипДокумента";
		const string lField="L";
		const string rField="R";
		const string URLField="URL";
		#endregion
		
		#region Constructors
		
		public DocumentType(int id):base(id)
		{
			switch(id)
			{
				//служебные записки
			//	case SluzhebnayaZapiskaRublevaya.TypeID: type=typeof(SluzhebnayaZapiskaRublevaya);break;
			//	case SluzhebnayaZapiskaValyutnaya.TypeID: type=typeof(SluzhebnayaZapiskaValyutnaya);break;
				//счета
#if !CLEAR
				case BuhDocs.Scheta.Predoplata.Predoplata.TypeID: type=typeof(BuhDocs.Scheta.Predoplata.Predoplata);break;
				case BuhDocs.Scheta.Faktura.TypeID : type=typeof(BuhDocs.Scheta.Faktura);break;
				//акты
				case Akts.PPNP.TypeID: type=typeof(Akts.PPNP);break;
				case OUDocs.Akts.Vyrabotka.Vyrabotka.TypeID:type=typeof(OUDocs.Akts.Vyrabotka.Vyrabotka);break;
				//заявки
				case OUDocs.Zayavki.INNP.InNP.TypeID: type=typeof(OUDocs.Zayavki.INNP.InNP);break;
				
				case Dogovora.Postavka.TypeID: type=typeof(Dogovora.Postavka);break;
				case Dogovora.Komissiya.TypeID: type=typeof(Dogovora.Komissiya);break;
				case Dogovora.Kontrakt.TypeID: type=typeof(Dogovora.Kontrakt);break;
				case Dogovora.OkazanieUslug.TypeID: type=typeof(Dogovora.OkazanieUslug);break;
				case Dogovora.KuplyaProdazha.TypeID: type=typeof(Dogovora.KuplyaProdazha);break;
				case Dogovora.Arenda.TypeID: type=typeof(Dogovora.Arenda);break;
				case Dogovora.Agentirovanie.TypeID: type=typeof(Dogovora.Agentirovanie);break;
				case Dogovora.Hranenie.TypeID: type=typeof(Dogovora.Hranenie);break;
				case Dogovora.TransportnayaEkspeditsiya.TypeID: type=typeof(Dogovora.TransportnayaEkspeditsiya);break;
				case Dogovora.Transportirovka.TypeID: type=typeof(Dogovora.Transportirovka);break;
				case Dogovora.Pererabotka.TypeID: type=typeof(Dogovora.Pererabotka);break;
				case Dogovora.ZaimResursov.TypeID: type=typeof(Dogovora.ZaimResursov);break;
				case Dogovora.Ssuda.TypeID: type=typeof(Dogovora.Ssuda);break;
				
				case 2110: type=typeof(Dogovora.Prilozhenie);break;

				case PlatezhnoePoruchenie.TypeID: type=typeof(PlatezhnoePoruchenie);break;
				case SWIFT.TypeID: type=typeof(SWIFT);break;
				case BankovskayaVipiska.TypeID: type=typeof(BankovskayaVipiska);break;
#endif				
				default: type=typeof(Document); break;
			}
		}
	
		#endregion

		protected override void Fill(DataRow row)
		{
			base.Fill (row);
			name=row.IsNull(nameField)?"Тип документа №"+ID:(string)row[nameField];
			l=row.IsNull(lField)?int.MinValue:(int)row[lField];
			r=row.IsNull(rField)?int.MinValue:(int)row[rField];
			url=row.IsNull(URLField)?"":row[URLField].ToString();
		}
	}
}

