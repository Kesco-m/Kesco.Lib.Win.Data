using System;
using System.Data;
using System.Diagnostics;
using Kesco.Lib.Win.Data.DALC.Documents;

namespace Kesco.Lib.Win.Data.Temp.Objects
{
    /// <summary>
    /// Данные подписи
    /// </summary>
    [DebuggerDisplay("signType = {signType} SignText = {SignText}")]
	public sealed class SignItem
	{
		private readonly int _signId;
		private readonly SignType signType;

		private readonly int _employee;
		private readonly int _employee4;

		private readonly string _fio;
		private readonly string _fio4;
		private readonly DateTime _date;
		private readonly string signText;

		private readonly bool _canRemove;

		public int SignId
		{
			get { return _signId; }
		}

		public int Employee
		{
			get { return _employee; }
		}

		public int Employee4
		{
			get { return _employee4; }
		}

		public string FIO
		{
			get { return _fio; }
		}

		public string FIO4
		{
			get { return _fio4; }
		}

		/// <summary>
		/// Текст подписи
		/// </summary>
		public string SignText
		{
			get { return signText; }
		}

		public DateTime Date
		{
			get { return _date; }
		}

		/// <summary>
		/// Тип подписи
		/// </summary>
		public SignType SignType
		{
			get { return signType; }
		}

		public bool CanRemove
		{
			get { return _canRemove; }
		}

		public SignItem(IDataRecord record, DocSignatureDALC data)
		{
			_signId = (int)record[data.IDField];

			_employee = (int)record["КодСотрудника"];
			_employee4 = (int)record["КодСотрудникаЗА"];

			_fio = (string)record["ФИО"];
			_fio4 = (string)record["ФИОЗА"];
			signText = record["ТекстПодписи"].ToString();

			_date = (DateTime)record["Дата"];

			if(record[data.DocumentSingatureTypeField].Equals(DBNull.Value))
				signType = SignType.noSign;
			else
			switch ((byte)record[data.DocumentSingatureTypeField])
			{
				case 1:
					signType = SignType.finalSign;
					break;
				case 2:
					signType = SignType.cancelSign;
					break;
				case 3:
					signType = SignType.hzSing;
					break;
				case 100:
					signType = SignType.stampSign;
					break;
				case 101:
					signType = SignType.interanalSign;
					break;
				default:
					signType = SignType.firstSign;
					break;
			}

			_canRemove = record["МожноУдалить"].Equals((byte)1);
		}
	}
    
    /// <summary>
    /// Тип подписи
    /// </summary>
	public enum SignType
	{
		noSign = -1,
		firstSign,
		finalSign,
		cancelSign,
		secondSign,
		stampSign,
		interanalSign,
		hzSing
	}
}
