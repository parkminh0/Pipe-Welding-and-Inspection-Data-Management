using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;

namespace SWSUploader
{
    public class CommonLogic
    {
        /// <summary>
        /// Item Master 로딩여부
        /// </summary>
        public int currCompKey = 0;

        /// <summary>
        /// ItemMaster에서 자재코드를 찾아온 경우에 Setting
        /// </summary>
        //public int MatItemKey = -1;
        public string ItemMatCode;

        /// <summary>
        /// //ItemMaster에서 자재코드를 찾아온 경우에 Setting
        /// </summary>
        public string currMatCode = string.Empty; 

        /// <summary>
        /// 현재 처리중인 MatKey
        /// </summary>
        public int currMatKey = -1; 

        /// <summary>
        /// 검사항목 리스트
        /// </summary>
        private DataTable dtItemMaster;

        public DataTable GetCompanyForTile()
        {
            DataTable dtCompany;
            string sql = string.Empty;
            sql += "select c.CompKey, c.CompName, '항목수     ' || ifnull(i.cnt, 0) ItemCount, '자재수     ' || ifnull(m.cnt, 0) MatCount ";
            sql += "  from Company c ";
            sql += "  left join (select CompKey, count(*) cnt from ItemMaster where isDeleted = 0 group by CompKey) i ";
            sql += "    on i.CompKey = c.CompKey ";
            sql += "  left join (select CompKey, count(*) cnt from Material where isDeleted = 0 group by CompKey) m ";
            sql += "    on m.CompKey = c.CompKey";
            sql += " where c.isDeleted = 0 ";
            try
            {
                dtCompany = DBManager.Instance.GetDataTable(sql);
            }
            catch (Exception)
            {
                dtCompany = null;
            }

            return dtCompany;
        }

        /// <summary>
        /// 검사대상 항목 목록 불러오기
        /// </summary>
        /// <param name="compKey"></param>
        public bool LoadedItemMaster(int compKey)
        {
            if (currCompKey == compKey)
                return true;

            /* ----------------------------------
            C# 형식 키워드	.NET 형식
            bool	System.Boolean
            byte	System.Byte
            sbyte	System.SByte
            char	System.Char
            decimal	System.Decimal
            double	System.Double
            float	System.Single
            int	    System.Int32
            uint	System.UInt32
            long	System.Int64
            ulong	System.UInt64
            short	System.Int16
            ushort	System.UInt16
            object	System.Object
            string	System.String
            ---------------------------------- */
            string sql = string.Empty;
            sql += "select ItemKey, CompKey, ItemKey, ColName, DecPoint, SeqNo, ";
            sql += "       case when ifnull(isNumeric, 0) = 0 then 'System.String' ";
            //sql += "            when ifnull(isNumeric, 0) = 1 and ifnull(DecPoint,0) = 0 then 'System.Int32' ";
            sql += "            when ifnull(isNumeric, 0) = 1 and ifnull(DecPoint,0) = 0 then 'System.Double' ";
            sql += "            when ifnull(isNumeric, 0) = 1 and ifnull(DecPoint,0) > 0 then 'System.Double' ";
            sql += "       end DataType ";
            sql += "  from ItemMaster ";
            sql += " where CompKey = " + compKey;
            sql += "   and isDeleted = 0";

            try
            {
                dtItemMaster = DBManager.Instance.GetDataTable(sql);
                DataColumn[] pk = new DataColumn[1];
                pk[0] = (DataColumn)dtItemMaster.Columns["ColName"];
                dtItemMaster.PrimaryKey = pk; //ColName으로 PK 설정
                currCompKey = compKey;        //현재 CompKey변경

                //컬럼중 자재코드(MatCode)에 해당하는 컬럼이 존재하는지 검사
                //MatItemKey = DBManager.Instance.GetIntScalar(string.Format("select ifnull(max(ItemKey),-1) ItemKey from ItemMaster where CompKey = {0} and isMatCode = 1 and isDeleted = 0", compKey));
                ItemMatCode = DBManager.Instance.GetStringScalar(string.Format("select ifnull(max(ColName),'') ColName from ItemMaster where CompKey = {0} and isMatCode = 1 and isDeleted = 0", compKey));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Column 정보 찾기
        /// </summary>
        /// <param name="compKey"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public string GetColumnType(int compKey, string colName)
        {
            string colType = string.Empty;
            if (!LoadedItemMaster(compKey))
                return colType;

            DataRow row = dtItemMaster.Rows.Find(colName);
            try
            {
                //colType = row["DataType"].ToString();
                colType = "System.String";
            }
            catch (Exception)
            {
                colType = "System.String";
            }
            return colType;
        }

        /// <summary>
        /// Item Key 가져오기
        /// </summary>
        /// <param name="compKey"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public int GetColumnKey(int compKey, string colName)
        {
            int colNo = 0;
            if (!LoadedItemMaster(compKey))
                return colNo;

            DataRow row = dtItemMaster.Rows.Find(colName);
            try
            {
                colNo = int.Parse(row["ItemKey"].ToString());
            }
            catch (Exception)
            {
                colNo = 0;
            }
            return colNo;
        }

        /// <summary>
        /// 자재Key(MatKey) 찾기
        ///  -> 파일 하나를 하나의 자재로 볼 수도 있는데, 
        ///  -> 파일내 각 Row를 자재로 볼 수 있도록 처리하자.
        /// </summary>
        /// <param name="compKey"></param>
        /// <returns></returns>
        public int GetMatKey(int compKey, string matCode)
        {
            if (currCompKey == compKey && currMatCode == matCode)
                return currMatKey;

            string sql = string.Format("select ifnull(max(MatKey),0) MatKey from Material where CompKey = {0} and MatCode = '{1}'", compKey, matCode);
            currMatKey = DBManager.Instance.GetIntScalar(sql);

            return currMatKey;
        }

        /// <summary>
        /// CSV File Loading
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DataTable GetDataTableFromCSVFile(string path)
        {
            DataTable csvData = new DataTable();

            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();

                    foreach (string column in colFields)
                    {
                        DataColumn col = new DataColumn(column, System.Type.GetType("System.String"));
                        col.AllowDBNull = true;
                        csvData.Columns.Add(col);
                    }

                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        DataRow dr = csvData.NewRow();
                        //Making empty value as empty
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == null)
                                fieldData[i] = string.Empty;

                            dr[i] = fieldData[i];
                        }
                        csvData.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return csvData;
        }

        /// <summary>
        /// Key Numbering
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        //public int GetNextKey(string category, int interval = 1)
        //{
        //    string sql = string.Format("update KeyNumbering set LastKeyNo = LastKeyNo + {0} where Category = '{1}' ",interval, category);
        //    int rs;
        //    try
        //    {
        //        rs = DBManager.Instance.ExcuteDataUpdate(sql);
        //        if (rs < 0)
        //            return rs;

        //        sql = string.Format("select LastKeyNo from KeyNumbering where Category = '{0}' ", category);
        //        rs = DBManager.Instance.GetIntScalar(sql);
        //    }
        //    catch (Exception)
        //    {
        //        rs = -1;
        //    }

        //    return rs;
        //}

    }
}
