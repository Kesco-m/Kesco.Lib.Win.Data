using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;
using Kesco.Lib.Win.Data.Temp;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
    [Option("СтрокаПоиска", typeof (СтрокаПоиска))]
    public class СтрокаПоиска : ValueOption
    {
        protected СтрокаПоиска(XmlElement el) : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPostfix = "";

            textItemPrefix = "'";
            textItemPostfix = "'";
        }

        public override string GetSQL(bool throwOnError)
        {
            string rez = "";
            string val = Value;
            val = Regex.Replace(val, "№|#|No.", "№ ");
            string[] words = Regex.Split(val, @"\s+");
            string typeids = "", numberwords = "", date = "", id = "";
            string word;
            for (int i = 0; i < words.Length; i++)
            {
                word = words[i];

                if (Regex.IsMatch(word, "от|ot|oт|оt|from", RegexOptions.IgnoreCase))
                    continue;
                if (Regex.IsMatch(word, "№", RegexOptions.IgnoreCase))
                    continue;
                if (IsID(words, i, ref id))
                    continue;
                if (IsType(word, ref typeids))
                    continue;

                if (i > 0 && words[i - 1].Equals("№"))
                {
                    if (IsNumber(word, ref numberwords))
                        continue;
                }
                else
                {
                    if (IsDate(word, ref date))
                        continue;
                    if (IsNumber(word, ref numberwords))
                        continue;
                }
            }

            if (id.Length > 0 && numberwords.Length == 0)
                numberwords = id;

            if (date.Length > 0)
                rez += (rez.Length == 0 ? "" : " AND ") + "(T0.ДатаДокумента = '" + date + "')";
            if (numberwords.Length > 0)
            {
                numberwords = Options.PrepareTextParameter(numberwords).Trim();
                string[] nwords = Regex.Split(numberwords, "[ ()]");
                rez += (rez.Length == 0 ? "" : " AND ");
                if (nwords.Length > 1)
                    rez += "(";
                for (int i = 0; i < nwords.Length; i++)
                {
                    if (i > 0) rez += " OR ";
                    rez += "(T0.НомерДокументаRL LIKE '" + Replacer.ReplaceRusLat(nwords[i]) + "%" +
                           "' OR T0.НомерДокументаRLReverse LIKE '" +
                           Env.ReverseString(Replacer.ReplaceRusLat(nwords[i])) +
                           "%" + "')";
                }
                if (nwords.Length > 1)
                    rez += ")";
            }
            if (typeids.Length > 0)
                rez += (rez.Length == 0 ? "" : " AND ") + "(T0.КодТипаДокумента IN (" + typeids + "))";

            if (id.Length > 0)
                rez = (rez.Length == 0 ? "" : "(" + rez + ") OR ") + "(T0.КодДокумента=" + id + ")";

            if (rez.Length == 0)
            {
                if (throwOnError)
                    throw new Exception(Resources.GetString("GetSQL"));
                return null;
            }
            return rez;
        }

        private static bool IsID(string[] words, int index, ref string id)
        {
            if (index > 0 || words.Length > 1)
                return false;
            if (!Regex.IsMatch(words[index], "^\\d+$"))
                return false;
            string sql = "SELECT КодДокумента FROM Документы.dbo.vwДокументы WHERE КодДокумента=" + words[index];
            int ind = 0;

            using (SqlCommand cmd = new SqlCommand(sql))
			using(cmd.Connection = new SqlConnection(Settings.DS_document))
			{
				try
				{
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					if(dr.HasRows)
					{
						if(ind < 2 && dr.Read())
							ind++;

						dr.Close();
						dr.Dispose();
					}
				}
				catch(SqlException sqlEx)
				{
					Env.WriteSqlToLog(sqlEx, cmd);
				}
				catch(Exception ex)
				{
					Env.WriteToLog(ex);
				}
				finally
				{
					cmd.Connection.Close();
				}
			}
            if (ind != 1)
                return false;

            id = words[index];
            return true;
        }

        private static bool IsDate(string str, ref string date)
        {
            if (!Regex.IsMatch(str, "^[0-9]{1,4}[.,/-][0-9]{1,4}[.,/-][0-9]{1,4}$"))
                return false;
            try
            {
                date = NewDateParser.Parse(str).ToString("yyyyMMdd");
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        private static bool IsType(string str, ref string typeids)
        {
            string search = Regex.Replace(" " + Options.PrepareTextParameter(str), "[ ]{1,}", "%");
            string sql =
                @"
                SELECT КодТипаДокумента FROM Документы.dbo.ТипыДокументов
                WHERE (' '+ ТипДокумента like '" +
                search + @"%' OR ' '+TypeDoc like '" + search + @"%')
                ";
            if (typeids.Length > 0)
                sql += " AND КодТипаДокумента IN(" + typeids + ")";

			using(SqlCommand cmd = new SqlCommand(sql))
			using(cmd.Connection = new SqlConnection(Settings.DS_document))
			{
				try
				{
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					bool ret = false;
					while(dr.Read())
					{
						if(!ret)
						{
							ret = true;
							typeids = "";
						}
						else if(typeids.Length > 0)
							typeids += ",";
						typeids += dr[0].ToString();
					}
					dr.Close();
					dr.Dispose();

					return ret;
				}
				catch(SqlException sex)
				{
					Env.WriteSqlToLog(sex, cmd);
					return false;
				}
				catch(Exception ex)
				{
					Env.WriteToLog(ex);
					return false;
				}
				finally
				{
					cmd.Connection.Close();
				}
			}
        }

        private static bool IsNumber(string str, ref string numberwords)
        {
            if (str.Length == 0)
                return false;

            numberwords += str + " ";
            return true;
        }
    }
}
