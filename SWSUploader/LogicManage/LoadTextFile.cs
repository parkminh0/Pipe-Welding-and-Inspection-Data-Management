using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace SWSUploader
{
    class LoadTextFile
    {
        private string filePath;
        public string FilePath
        {
            get { return filePath; }
        }

        private DataTable _dtNetList;
        public DataTable dtNetList
        {
            get { return _dtNetList; }
            //set { _dtNetList = value; }
        }

        private const string startValue = "**********";
        public bool isOK;
        public string errMessage;

        /// <summary>
        /// 총 건수
        /// </summary>
        private int _TotalNetCount;
        public int TotalNetCount
        {
            get { return _TotalNetCount; }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="fPath"></param>
        public LoadTextFile(string fPath)
        {
            _dtNetList = new DataTable();
            filePath = fPath;
            CreateDataTable();
            getFileContents();
        }

        private void CreateDataTable()
        {
            DataColumn col;
            col = new DataColumn("LineNo", typeof(int));
            _dtNetList.Columns.Add(col);
            col = new DataColumn("List", typeof(string));
            _dtNetList.Columns.Add(col);
            col = new DataColumn("isShow", typeof(bool));
            _dtNetList.Columns.Add(col);
            col = new DataColumn("Keyword", typeof(string));
            _dtNetList.Columns.Add(col);
            col = new DataColumn("GName", typeof(string));
            _dtNetList.Columns.Add(col);
            col = new DataColumn("GCount", typeof(int));
            _dtNetList.Columns.Add(col);

            dtNetList.PrimaryKey = new DataColumn[] { dtNetList.Columns["Keyword"] };
        }

        /// <summary>
        /// 불러오기
        /// </summary>
        /// <returns></returns>
        private bool getFileContents()
        {
            DataRow row;
            StreamReader sr;
            string line, keyword = string.Empty, gName = string.Empty;
            int idx = 0, gCount = 0;
            bool isDiff;
            _TotalNetCount = 0; //총대상건수

            try
            {
                sr = new StreamReader(filePath, Encoding.Default);
            }
            catch (Exception)
            {
                return false;
            }

            isDiff = false;
            string[] lineData;
            try
            {
                while ((line = sr.ReadLine()) != null)
                {
                    idx++;

                    //**************
                    if (line.Trim().StartsWith(startValue) || line.Trim() == "")
                    {
                        gCount = 0;
                        isDiff = false;
                        keyword = string.Format("#{0}#", idx); //PK비대상은 임의의 값으로 채움
                        gName = string.Empty;
                    }
                    else
                    {
                        lineData = line.Split('=');
                        keyword = lineData[lineData.Length - 1].Trim();
                        gName = lineData[0].Trim();
                        _TotalNetCount++;
                        if (keyword.ToLower().Contains("null"))
                        {
                            gCount--;
                            _TotalNetCount--;
                            isDiff = false;
                            keyword = string.Format("#{0}#", idx); //PK비대상은 임의의 값으로 채움
                        }
                        //else
                        //{
                        //    keyword = keyword.Trim();
                        //}
                    }

                    try
                    {
                        row = dtNetList.NewRow();
                        row["LineNo"] = idx;
                        row["list"] = line;
                        row["isShow"] = isDiff;
                        row["Keyword"] = keyword;
                        row["GName"] = gName;
                        row["GCount"] = 0;
                        dtNetList.Rows.Add(row);
                    }
                    catch (Exception ex)
                    {
                        isOK = false;
                        errMessage = string.Format("Error[{0}]: {1}", idx, ex.Message);
                        return false;
                    }

                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                sr.Close();
            }

            //Group별 Count
            var cnt =
                from p in dtNetList.AsEnumerable()
                group p by p.Field<string>("gName") into g
                select new { gName = g.Key, gcnt = g.Count() };

            //var gcnt = from p in dtNetList.AsEnumerable()
            //           group p by new
            //           {
            //               gName = p.Field<string>("gName")
            //           } into k
            //           select k;
            foreach (var item in cnt)
            {
                if (item.gName.StartsWith(startValue) || item.gName.Trim() == "") continue;

                foreach (DataRow dr in dtNetList.Rows)
                {
                    if (dr["Gname"].ToString() == item.gName)
                    {
                        dr.BeginEdit();
                        dr["Gcount"] = item.gcnt;
                        dr.EndEdit();
                    }
                }
            }
            dtNetList.AcceptChanges();
            return true;
        }
    }
}
