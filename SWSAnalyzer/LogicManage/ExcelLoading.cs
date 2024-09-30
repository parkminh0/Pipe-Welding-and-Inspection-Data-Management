using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Excel;
using System.IO;

namespace SWSAnalyzer
{
    class ExcelLoading
    {
        private string filePath;
        private int orderSequence;
        public int OrderSequence
        {
            get { return orderSequence; }
        }

        public DataTable excelTable;
        public bool isOK;
        public string resultMessage;
        private List<string> QueryList;

        private bool isReadonly = true;

        /// <summary>
        /// 생성자
        /// </summary>
        public ExcelLoading(string aPath)
        {
            isReadonly = false;
            filePath = aPath;
            isOK = true;

            CreateTable(); //빈 테이블 생성
            if (GetExcelLoading()) //엑셀 로딩
                MakeTable(); //데이터 생성
        }
        /// <summary>
        /// 생성자2
        /// 기존 데이터 불러오기
        /// </summary>
        /// <param name="seq"></param>
        public ExcelLoading(int seq)
        {
            orderSequence = seq;
            GetOrderData();
        }

        public DataTable dtExcelData;
        /// <summary>
        /// 빈 테이블 생성
        /// </summary>
        private void CreateTable()
        {
            string sql = "SELECT * FROM CSOrder WHERE 1 = 2 ";
            dtExcelData = DBManager.Instance.GetDataTable(sql);
        }

        /// <summary>
        /// 기존 주문내역 불러오기
        /// </summary>
        private void GetOrderData()
        {
            string sql = "SELECT * FROM CSOrder WHERE OrderSequence = " + orderSequence;
            try
            {
                dtExcelData = DBManager.Instance.GetDataTable(sql);
                isOK = true;
            }
            catch (Exception ex)
            {
                isOK = false;
                resultMessage = ex.Message;
            }
        }

        /// <summary>
        /// 엑셀로딩(코드플렉스 참조용)
        /// </summary>
        private bool GetExcelLoading()
        {
            DataSet ds;

            var file = new FileInfo(filePath);
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    IExcelDataReader reader = null;
                    if (file.Extension == ".xls")
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);

                    }
                    else if (file.Extension == ".xlsx")
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }

                    if (reader == null)
                        return false;
                    reader.IsFirstRowAsColumnNames = true;
                    ds = reader.AsDataSet();

                    if (ds.Tables.Count < 1)
                        return false;

                    excelTable = ds.Tables[0];
                    return true;
                }
            }
            catch (Exception ex)
            {
                isOK = false;
                resultMessage = LangResx.Main.msg_ExcelError + "\r\n" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 테이블 생성
        /// </summary>
        /// <param name="aYear"></param>
        /// <returns></returns>
        private bool MakeTable()
        {
            orderSequence = GetOrderSequence();
            int startRow = 1;
            int idx1 = 0;
            string sql = string.Empty;
            DataRow row;
            try
            {
                foreach (DataRow dr in excelTable.Rows)
                {
                    row = dtExcelData.NewRow();
                    idx1++;
                    if (idx1 < startRow) continue;
                    if (dr[4].ToString().Trim() == "") continue;  //OrderNumber가 비어있으면 끝부분으로 인식

                    string dvdate;
                    try
                    {
                        dvdate = DateTime.Parse(dr[11].ToString()).ToString("yyyy-MM-dd");
                    }
                    catch (Exception)
                    {
                        dvdate = string.Empty;
                    }                    

                    row["DetailKey"] = idx1;
                    row["OrderSequence"] = orderSequence;
                    row["OrderNumber"] = dr[4];
                    row["SoldTo"] = dr[5];
                    row["SoldToName"] = dr[6];
                    row["ItemNumber"] = dr[7];
                    row["Description1"] = dr[8];
                    row["Quantity"] = dr[9];
                    row["UOM"] = dr[10];
                    row["DeliveryDate"] = dvdate;
                    row["ShipToNo"] = dr[12];
                    row["ShipTo"] = dr[13];
                    row["ShippingInfo1"] = dr[14];
                    row["ShippingInfo2"] = dr[15];
                    row["TransactionOriginator"] = dr[16];
                    row["Descr"] = dr[17];
                    row["JobStatus"] = 0;
                    row["RouteTypeCode"] = 0;
                    row["Driver"] = 0;
                    row["Receiver"] = "";
                    row["PhoneNo"] = "";
                    row["Address"] = "";
                    row["ProdName"] = "";
                    row["Unit"] = "";
                    row["Remark1"] = "";
                    row["Remark2"] = "";
                    row["CreateDtm"] = DateTime.Now;
                    dtExcelData.Rows.Add(row);
                }
                dtExcelData.AcceptChanges();
            }
            catch (Exception ex)
            {
                isOK = false;
                resultMessage = ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 주문내역 생성
        /// </summary>
        public bool InsertCSOrder()
        {
            if (isReadonly)
            {
                isOK = false;
                resultMessage = "읽기전용 상태입니다 (ReadOnly = true)";
                return false;
            }

            QueryList = new List<string>();
            string sql = string.Empty;

            try
            {
                foreach (DataRow dr in dtExcelData.Rows)
                {
                    sql = string.Empty;
                    sql += " INSERT INTO [CSOrder] ";
                    sql += " ( ";
                    sql += "   [OrderSequence], ";
                    sql += "   [OrderNumber], ";
                    sql += "   [SoldTo], ";
                    sql += "   [SoldToName], ";
                    sql += "   [ItemNumber], ";
                    sql += "   [Description1], ";
                    sql += "   [Quantity], ";
                    sql += "   [UOM], ";
                    sql += "   [DeliveryDate], ";
                    sql += "   [ShipToNo], ";
                    sql += "   [ShipTo], ";
                    sql += "   [ShippingInfo1], ";
                    sql += "   [ShippingInfo2], ";
                    sql += "   [TransactionOriginator], ";
                    sql += "   [Descr], ";
                    sql += "   [JobStatus], ";
                    sql += "   [RouteTypeCode], ";
                    sql += "   [Driver], ";
                    sql += "   [Receiver], ";
                    sql += "   [PhoneNo], ";
                    sql += "   [Address], ";
                    sql += "   [ProdName], ";
                    sql += "   [Unit], ";
                    sql += "   [Remark1], ";
                    sql += "   [Remark2], ";
                    sql += "   [CreateDtm] ";
                    sql += " ) ";
                    sql += "      VALUES ";
                    sql += " ( ";
                    sql += "   " + orderSequence + ", ";
                    sql += "   '" + dr["OrderNumber"].ToString().Trim() + "', ";
                    sql += "   '" + dr["SoldTo"].ToString().Trim().Replace("'", "''") + "', ";
                    sql += "   '" + dr["SoldToName"].ToString().Trim().Replace("'", "''") + "', ";
                    sql += "   '" + dr["ItemNumber"].ToString().Trim() + "', ";
                    sql += "   '" + dr["Description1"].ToString().Trim().Replace("'", "''") + "', ";
                    sql += "   " + dr["Quantity"].ToString().Trim() + ", ";
                    sql += "   '" + dr["UOM"].ToString().Trim() + "', ";
                    sql += "   '" + dr["DeliveryDate"].ToString().Trim() + "', ";
                    sql += "   '" + dr["ShipToNo"].ToString().Trim().Replace("'", "''") + "', ";
                    sql += "   '" + dr["ShipTo"].ToString().Trim().Replace("'", "''") + "', ";
                    sql += "   '" + dr["ShippingInfo1"].ToString().Trim().Replace("\u3000", " ").Replace("'", "''") + "', ";
                    sql += "   '" + dr["ShippingInfo2"].ToString().Trim().Replace("'", "''") + "', ";
                    sql += "   '" + dr["TransactionOriginator"].ToString().Trim().Replace("'", "''") + "', ";
                    sql += "   '" + dr["Descr"].ToString().Trim().Replace("'", "''") + "', ";
                    sql += "   " + 0 + ", ";
                    sql += "   " + 0 + ", ";
                    sql += "   " + 0 + ", ";
                    sql += "   '', ";
                    sql += "   '', ";
                    sql += "   '', ";
                    sql += "   '', ";
                    sql += "   '', ";
                    sql += "   '', ";
                    sql += "   '', ";
                    sql += "   CURRENT_TIMESTAMP ";
                    sql += " ) ";                 

                    QueryList.Add(sql);
                }
            }
            catch (Exception ex)
            {
                isOK = false;
                resultMessage = ex.Message;
                return false;
            }

            InsertCSOrderMaster();   //요약정보 생성
            IncreaseOrderSequence(); //채번 증가 sql생성

            //DB저장 및 Commit
            try
            {
                string str = DBManager.Instance.ExcuteTransaction(QueryList);
                if (string.IsNullOrEmpty(str))
                {
                    isOK = true;                    
                }
                else
                {
                    isOK = false;
                    resultMessage = "ExcuteTransaction Error :\r\n" + str;
                }
                return isOK;
            }
            catch (Exception ex)
            {
                isOK = false;
                resultMessage = ex.Message;
                return false;
            } 
        }
        /// <summary>
        /// 주문내역 집계
        /// </summary>
        private void InsertCSOrderMaster()
        {
            string sql = "DELETE from [CSOrderMaster] where [OrderSequence] = " + orderSequence;
            QueryList.Add(sql);

            sql = string.Empty;
            sql += " INSERT INTO [CSOrderMaster] ";
            sql += " ( ";
            sql += "   [OrderSequence], ";
            sql += "   [RowCount], ";
            sql += "   [TotalQty], ";
            sql += "   [DeliverySDate], ";
            sql += "   [DeliveryEDate], ";
            sql += "   [ItemCount], ";
            sql += "   [AttrInt], ";
            sql += "   [AttriStr], ";
            sql += "   [CreateDtm] ";
            sql += " ) ";
            sql += " SELECT [OrderSequence], ";
            sql += "        COUNT(*) ROWCOUNT, ";
            sql += "        SUM([Quantity]) TotalQty, ";
            sql += "        MIN([DeliveryDate]) DeliverySDate, ";
            sql += "        MAX(DeliveryDate) DeliveryEDate, ";
            sql += "        COUNT(DISTINCT [ItemNumber]) ItemCount, ";
            sql += "        0 AttrInt, ";
            sql += "        '' AttrStr, ";
            sql += "        CURRENT_TIMESTAMP CreateDtm ";
            sql += "   FROM [CSOrder] ";
            sql += "  WHERE [OrderSequence] = " + orderSequence;
            sql += "  GROUP BY [OrderSequence]  ";
            QueryList.Add(sql);
        }

        /// <summary>
        /// OrderSequence 채번
        /// </summary>
        /// <returns></returns>
        private int GetOrderSequence()
        {
            string sql = "select ifnull(max([IntValue]), 0) OrderSequence from [Configuration] where [Category] = 'OrderSequence' ";
            return DBManager.Instance.GetIntScalar(sql);
        }

        /// <summary>
        /// 증가
        /// </summary>
        /// <returns></returns>
        private void IncreaseOrderSequence()
        {
            string sql = "update [Configuration] set [IntValue] = [IntValue] + 1 where [Category] = 'OrderSequence'";
            QueryList.Add(sql);
        }
    }
}
