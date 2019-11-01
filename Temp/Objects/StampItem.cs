using System.Data;
using System.Diagnostics;
using Kesco.Lib.Win.Data.DALC.Documents;

namespace Kesco.Lib.Win.Data.Temp.Objects
{
    /// <summary>
    /// Данные штампа
    /// </summary>
    [DebuggerDisplay("TypeID = {TypeID} Page = {Page}")]
	public sealed class StampItem : IDObject
	{
	    public StampItem(int id) : base(id)
		{
		}

		public StampItem(IDataRecord dr, DocSignatureDALC data) : base((int)dr[data.IDField])
		{
			if(dr["КодШтампа"] is int)
				StampID = (int)dr["КодШтампа"];
			if(dr["Page"] is int)
			{
				Page = (int)dr["Page"];
                OriginalX = X = (int)dr["X"];
                OriginalY = Y = (int)dr["Y"];
				Zoom = (int)dr["Zoom"];
				if(dr["Rotate"] != System.DBNull.Value)
                    OriginalRotate = Rotate = (int)dr["Rotate"];
			}
			if(dr[data.ImageIdField] is int)
				ImageID = (int)dr[data.ImageIdField];
		    try
		    {
		        if (dr["ТипПодписи"] is byte)
		            TypeID = (int) (byte) dr["ТипПодписи"];
		        else
		            TypeID = 100;
		    }
		    catch
		    {
		        TypeID = 100;
		    }

			DocumentID = (int)dr[data.DocumentIDField];
			Employee = new Employee((int)dr["КодСотрудника"], null);
			if(dr["КодСотрудникаЗА"] is int)
				EmployeeFor = new Employee((int)dr["КодСотрудникаЗА"], null);
		}

	    public int DocumentID { get; set; }

	    public int ImageID { get; set; }

	    public int TypeID { get; set; }

	    public int StampID { get; set; }

	    public int Page { get; set; }

	    public int X { get; set; }

	    public int Y { get; set; }

        /// <summary>
        /// Загруженное из бд значение
        /// </summary>
        public int OriginalX { get; set; }

        /// <summary>
        /// Загруженное из бд значение
        /// </summary>
        public int OriginalY { get; set; }

	    public int Zoom { get; set; }

        /// <summary>
        /// Загруженное из бд значение поворота штампа
        /// </summary>
        public int OriginalRotate { get; set; }

        /// <summary>
        /// Текущее значение поворота штампа
        /// Может отличатся от оригинального, например в случае виртульного поворота страницы
        /// </summary>
	    public int Rotate { get; set; }

	    public Employee Employee { get; set; }

	    public Employee EmployeeFor { get; set; }

        /// <summary>
        /// Флаг. Координаты штампа изменились.
        /// </summary>
	    public bool CooordinateChanged
	    {
	        get { return X != OriginalX || Y != OriginalY || Rotate != OriginalRotate; }
	    }
	}
}
