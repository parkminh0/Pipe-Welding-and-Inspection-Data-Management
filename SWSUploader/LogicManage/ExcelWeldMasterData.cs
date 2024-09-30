using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Excel;
using System.IO;
//using EasyXLS;
using System.Globalization;
using DevExpress.DataAccess.Excel;
using DevExpress.Spreadsheet;
using System.ComponentModel;
using System.Collections;

namespace SWSUploader
{
    class ExcelWeldMasterData
    {
        private int projectKey;
        private string filePath;
        public DataTable excelTable;    //엑셀데이터
        public DataTable dtExcelData;   //재가공한 데이터
        public bool isOK;
        public string ResultMessage;

        //생성된 데이터가 존재하는지 여부
        private bool isCreated;

        private const int MachineSerialNoPos = 0;  //MachineSerialNo 엑셀상의 위치
        private const int WeldNoPos = 13;  //WeldNo 엑셀상의 위치
        private const int WeldingDatePos = 15;  //WeldingDate 엑셀상의 위치

        /// <summary>
        /// 생성자
        /// </summary>
        public ExcelWeldMasterData(string aPath, int projectKey)
        {
            filePath = aPath;
            isOK = true;
            isCreated = false;
            this.projectKey = projectKey;
            if (!GetExcelLoading()) //엑셀 File 로딩
                return;
        }       

        /// <summary>
        /// 엑셀데이터 생성
        /// </summary>
        /// <returns></returns>
        public bool MakeData()
        {
            CreateTempTable(); //빈 Temp테이블 생성
            MakeTempTable(); //데이터 생성

            if (!isOK)
                return isOK;

            if (!isFindHeader && !isCreated)
            {
                isOK = false;
                ResultMessage = LangResx.Main.msg_FileError;
            }

            return isOK;
        }

        /// <summary>
        /// 빈 테이블 생성
        /// </summary>
        private void CreateTempTable()
        {
            string sql = "SELECT * FROM TempWeldMaster WHERE 1 = 2 ";
            dtExcelData = DBManager.Instance.GetDataTable(sql);
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

                    reader.IsFirstRowAsColumnNames = false;
                    
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
                ResultMessage = LangResx.Main.msg_ExcelError + "\r\n" + ex.Message;
                return false;
            }
        }
        
        private bool isFindHeader;
        /// <summary>
        /// 테이블 생성
        /// </summary>
        /// <param name="aYear"></param>
        /// <returns></returns>
        private bool MakeTempTable()
        {
            int idx1 = 0;
            List<string> sqls = new List<string>();
            string sql = string.Empty;

            isFindHeader = false; //Header를 찾았는지 여부
            string MachineSerialNo, WeldNo, WeldingDate;
            DataRow row;
            try
            {
                foreach (DataRow dr in excelTable.Rows)
                {
                    idx1++;
                    MachineSerialNo = dr[MachineSerialNoPos].ToString();
                    WeldNo = dr[WeldNoPos].ToString();
                    WeldingDate = dr[WeldingDatePos].ToString();

                    if (idx1 == 1) //헤더검사
                    {
                        if (MachineSerialNo.ToLower() == "machine" && WeldNo.ToLower().StartsWith("sequence") && WeldingDate.ToLower().Replace(" ", "") == "weldingdate-time")
                        {
                            isFindHeader = true;
                            continue;
                        }
                        else
                            break; //Header가 안맞으면 종료처리
                    }

                    DateTime wd;
                    //try
                    //{
                    //    wd = DateTime.FromOADate(double.Parse(WeldingDate));
                    //}
                    //catch (Exception)
                    //{
                    //    WeldingDate = dr[WeldingDatePos].ToString().Substring(0, 10).Replace(".", "-");
                    //    try
                    //    {
                    //        wd = DateTime.Parse(WeldingDate);
                    //    }
                    //    catch (Exception)
                    //    {
                    //        wd = DateTime.ParseExact(WeldingDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    //    }
                    //}
                    //WeldingDate = wd.ToShortDateString();

                    if (WeldingDate.Length > 10)
                        WeldingDate = WeldingDate.Substring(0, 10);
                    try
                    {
                        wd = DateTime.Parse(WeldingDate);
                    }
                    catch (Exception)
                    {
                        wd = DateTime.Parse("1900-01-01");
                    }

                    row = dtExcelData.NewRow();
                    row["MachineSerialNo"] = MachineSerialNo;
                    row["WeldNo"] = WeldNo;
                    row["WeldingDate"] = wd;
                    row["Status"] = 0;
                    row["CreateID"] = Program.Option.LoginID;
                    row["CreateDtm"] = DateTime.Now.ToShortDateString();
                    dtExcelData.Rows.Add(row);
                    isCreated = true;
                }
                dtExcelData.AcceptChanges();
            }
            catch (Exception ex)
            {
                isOK = false;
                ResultMessage = ex.Message;
                return false;
            }

            try
            {
                if (isCreated)
                    DBManager.Instance.MDB.BulkCopyDI(dtExcelData, "TempWeldMaster");

                return true;
            }
            catch (Exception ex)
            {
                isOK = false;
                ResultMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 중복상태 업데이트 
        /// </summary>
        public void UpdateDupStatus()
        {
            string sql = string.Empty;
            sql += "update TempWeldMaster set Status = 1 ";
            sql += "  from TempWeldMaster x ";
            sql += "  JOIN ( ";
            sql += "		select tm.MachineSerialNo, tm.WeldNo, tm.WeldingDate ";
            sql += "		  from TempWeldMaster tm ";
            sql += "		  join WeldMaster bm ";
            sql += "		    on bm.MachineSerialNo = tm.MachineSerialNo and bm.WeldNo = tm.WeldNo and bm.WeldingDate = tm.WeldingDate ";
            sql += "		 where tm.CreateID = '" + Program.Option.LoginID + "' ";
            sql += "	 ) y ";
            sql += "    on y.MachineSerialNo = x.MachineSerialNo and y.WeldNo = x.WeldNo and y.WeldingDate = x.WeldingDate ";
            sql += " where x.CreateID = '" + Program.Option.LoginID + "' ";
            try
            {
                DBManager.Instance.ExcuteDataUpdate(sql);

                //다시 로딩
                dtExcelData = DBManager.Instance.GetDataTable("select DISTINCT twm.MachineSerialNo, twm.WeldNo, twm.WeldingDate, twm.Status from TempWeldMaster twm ");

                //Primary Key 설정
                DataColumn[] pk = new DataColumn[3];
                pk[0] = (DataColumn)dtExcelData.Columns["MachineSerialNo"];
                pk[1] = (DataColumn)dtExcelData.Columns["WeldNo"];
                pk[2] = (DataColumn)dtExcelData.Columns["WeldingDate"];
                dtExcelData.PrimaryKey = pk;
            }
            catch (Exception)
            {
                return;
            }            
        }

        /// <summary>
        /// 중복 건수 확인
        /// </summary>
        /// <returns></returns>
        public int GetDupCount()
        {
            string sql = string.Empty;
            sql = string.Format("select count(*) from TempWeldMaster where Status = 1 and CreateID = '{0}' ", Program.Option.LoginID);

            try
            {
                return DBManager.Instance.GetIntScalar(sql);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// 중복 데이터 1건 추출
        /// </summary>
        /// <returns></returns>
        public string GetDupFirstRow()
        {
            string sql = string.Empty;
            sql += "select top 1 (tm.MachineSerialNo + '-' + CONVERT(VARCHAR, tm.WeldNo) + '-' + CONVERT(VARCHAR(10), tm.WeldingDate, 120)) DupRow ";
            sql += "  from TempWeldMaster tm ";
            sql += " where tm.Status = 1 and CreateID = '" + Program.Option.LoginID + "'";

            try
            {
                return DBManager.Instance.GetStringScalar(sql);
            }
            catch (Exception)
            {
                return "-n/a-";
            }
        }

        public static int cntErrorDate;
        /// <summary>
        /// 융착정보 Master 생성
        /// </summary>
        public bool InsertWeldMaster()
        {
            List<string> sqls = new List<string>();
            string sql, tmpValue;
            int status, idx = 0;
            DataRow tmpRow;
            cntErrorDate = 0;
            foreach (DataRow dr in excelTable.Rows)
            {
                idx++;
                if (idx == 1)  //Header
                    continue;

                //중복여부 확인후 신규분만 생성하도록 함
                //string wDate, tmpDate = dr[WeldingDatePos].ToString().Substring(0, 10).Replace(".", "-");
                string wDate, tmpDate = dr[WeldingDatePos].ToString();
                DateTime wd;
                try
                {
                    wd = DateTime.Parse(tmpDate);
                }
                catch (Exception)
                {
                    wd = DateTime.Parse("1900-01-01");
                }
                //try
                //{
                //    wd = DateTime.FromOADate(double.Parse(tmpDate));  //from julianDate
                //}
                //catch (Exception)
                //{
                //    tmpDate = dr[WeldingDatePos].ToString().Substring(0, 10).Replace(".", "-");
                //    try
                //    {
                //        wd = DateTime.Parse(tmpDate);
                //    }
                //    catch (Exception)
                //    {
                //        wd = DateTime.ParseExact(tmpDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                //    }
                //}

                wDate = wd.ToShortDateString();
                string[] pk = new string[] { dr[MachineSerialNoPos].ToString(), dr[WeldNoPos].ToString(), wDate };
                try
                {
                    tmpRow = dtExcelData.Rows.Find(pk);
                    status = int.Parse(tmpRow["Status"].ToString());
                }
                catch (Exception)
                {
                    status = 1; //확인필요
                }

                if (status == 1)
                    continue; //중복이면 생략처리

                sql = string.Empty;
                sql += "INSERT INTO [dbo].[WeldMaster] ";
                sql += "           ([MachineSerialNo] ";
                sql += "           ,[MachineLine] ";
                sql += "           ,[MachineType] ";
                sql += "           ,[SoftwareVersion] ";
                sql += "           ,[LastMaintDate] ";
                sql += "           ,[NextMaintDate] ";
                sql += "           ,[WeldProjectDescr] ";
                sql += "           ,[AdditionalData] ";
                sql += "           ,[InstallingCompany] ";
                sql += "           ,[WelderCode] ";
                sql += "           ,[WelderName] ";
                sql += "           ,[WeldersCompany] ";
                sql += "           ,[WelderCertExpireDate] ";
                sql += "           ,[WeldNo] ";
                sql += "           ,[DataSeqNo] ";
                sql += "           ,[WeldingDate] ";
                sql += "           ,[WeldingDateTime] ";
                sql += "           ,[AmbientTemperature] ";
                sql += "           ,[Status] ";
                sql += "           ,[Weather] ";
                sql += "           ,[Material] ";
                sql += "           ,[Diameter] ";
                sql += "           ,[Diameter2] ";
                sql += "           ,[SDR] ";
                sql += "           ,[WallThickness] ";
                sql += "           ,[Angle] ";
                sql += "           ,[FittingManufacturer] ";
                sql += "           ,[Design] ";
                sql += "           ,[OperatingMode] ";
                sql += "           ,[Standard] ";
                sql += "           ,[Mode] ";
                sql += "           ,[FittingCode] ";
                sql += "           ,[WeldingBeadLeft] ";
                sql += "           ,[WeldingBeadRight] ";
                sql += "           ,[WeldingBeadTotal] ";
                sql += "           ,[WeldingBeadLeft2] ";
                sql += "           ,[WeldingBeadRight2] ";
                sql += "           ,[WeldingBeadTotal2] ";
                sql += "           ,[WeldingBeadLeft3] ";
                sql += "           ,[WeldingBeadRight3] ";
                sql += "           ,[WeldingBeadTotal3] ";
                sql += "           ,[Latitude] ";
                sql += "           ,[Longitude] ";
                sql += "           ,[DragForceNominalValue] ";
                sql += "           ,[DragForceNominalUnit] ";
                sql += "           ,[DragForceActualValue] ";
                sql += "           ,[DragForceActualUnit] ";
                sql += "           ,[MirrorTempNominalValue] ";
                sql += "           ,[MirrorTempActualValue] ";
                sql += "           ,[PreheatingTimeNominalValue] ";
                sql += "           ,[PreheatingTimeActualValue] ";
                sql += "           ,[BeadBuidupTimeNominalValue] ";
                sql += "           ,[BeadBuidupTimeActualValue] ";
                sql += "           ,[BeadBuildupForceNominalValue] ";
                sql += "           ,[BeadBuildupForceNominalUnit] ";
                sql += "           ,[BeadBuildupForceActualValue] ";
                sql += "           ,[BeadBuildupForceActualUnit] ";
                sql += "           ,[HeatingTimeNominaValue] ";
                sql += "           ,[HeatingTimeActualValue] ";
                sql += "           ,[HeatingForceNominalValue] ";
                sql += "           ,[HeatingForceNominalUnit] ";
                sql += "           ,[HeatingForceActualValue] ";
                sql += "           ,[HeatingForceActualUnit] ";
                sql += "           ,[ChangeOverTimeNominalValue] ";
                sql += "           ,[ChangeOverTimeActualValue] ";
                sql += "           ,[JoiningPressRampNominalValue] ";
                sql += "           ,[JoiningPressRampActualValue] ";
                sql += "           ,[JoiningForceNominalValue] ";
                sql += "           ,[JoiningForceNominalUnit] ";
                sql += "           ,[JoiningForceActualValue] ";
                sql += "           ,[JoiningForceActualUnit] ";
                sql += "           ,[CoolingTimeNominalValue] ";
                sql += "           ,[CoolingTimeActualValue] ";
                sql += "           ,[TwoLevelCoolingTimeNominalValue] ";
                sql += "           ,[TwoLevelCoolingTimeActualValue] ";
                sql += "           ,[TwoLevelCoolingForceNominalValue] ";
                sql += "           ,[TwoLevelCoolingForceNominalUnit] ";
                sql += "           ,[TwoLevelCoolingForceActualValue] ";
                sql += "           ,[TwoLevelCoolingForceActualUnit] ";
                sql += "           ,[WeldingDistance] ";
                sql += "           ,[WeldingVoltageNominalValue] ";
                sql += "           ,[WeldingVoltageActualValue] ";
                sql += "           ,[ResistanceNominalValue] ";
                sql += "           ,[ResistanceActualValue] ";
                sql += "           ,[WorkNominalValue] ";
                sql += "           ,[WorkActualValue] ";
                sql += "           ,[TotalTimeNominalValue] ";
                sql += "           ,[TotalTimeActualValue] ";
                sql += "           ,[CreateID] ";
                sql += "           ,[CreateDtm] "; 
                sql += "           ,[ProjectKey] ) ";
                sql += "     VALUES ";
                sql += $"           ( '" + dr[0].ToString() + "'  ";  // [MachineSerialNo]
                sql += "           ,'" + dr[1].ToString() + "'  ";  // [MachineLine]
                sql += "           ,'" + dr[2].ToString() + "'  ";  // [MachineType]
                sql += "           ,'" + dr[3].ToString() + "'  ";  // [SoftwareVersion]

                tmpValue = dr[4].ToString();  // [LastMaintDate]
                if (string.IsNullOrWhiteSpace(tmpValue))
                {
                    sql += "           ,NULL ";
                }
                else
                {
                    try
                    {
                        if (tmpValue.Length > 10)
                            tmpValue = tmpValue.Substring(0, 10);
                        wd = DateTime.Parse(tmpValue);
                    }
                    catch (Exception)
                    {
                        wd = DateTime.Parse("1900-01-01");
                    }
                    //try
                    //{
                    //    wd = DateTime.FromOADate(double.Parse(tmpValue));  //from julianDate
                    //}
                    //catch (Exception)
                    //{
                    //    try
                    //    {
                    //        tmpValue = tmpValue.Substring(0, 10).Replace(".", "-");
                    //        wd = DateTime.Parse(tmpValue);
                    //    }
                    //    catch (Exception)
                    //    {
                    //        wd = DateTime.Parse("1900-01-01");
                    //    }
                    //}
                    sql += "           ,'" + wd.ToShortDateString() + "' ";
                }

                tmpValue = dr[5].ToString();  // [NextMaintDate]
                if (string.IsNullOrWhiteSpace(tmpValue))
                {
                    sql += "           ,NULL ";
                }
                else
                {
                    try
                    {
                        if (tmpValue.Length > 10)
                            tmpValue = tmpValue.Substring(0, 10);
                        wd = DateTime.Parse(tmpValue);
                    }
                    catch (Exception)
                    {
                        wd = DateTime.Parse("1900-01-01");
                    }
                    sql += "           ,'" + wd.ToShortDateString() + "' ";
                }

                sql += "           ,'" + dr[6].ToString() + "'  ";  // [WeldProjectDescr]
                sql += "           ,'" + dr[7].ToString() + "'  ";  // [AdditionalData]
                sql += "           ,'" + dr[8].ToString() + "'  ";  // [InstallingCompany]
                sql += "           ,'" + dr[9].ToString() + "'  ";  // [WelderCode]
                sql += "           ,'" + dr[10].ToString() + "'  ";  // [WelderName]
                sql += "           ,'" + dr[11].ToString() + "'  ";  // [WeldersCompany]

                tmpValue = dr[12].ToString();  // [WelderCertExpireDate]
                if (string.IsNullOrWhiteSpace(tmpValue))
                {
                    sql += "           ,NULL ";
                }
                else
                {
                    try
                    {
                        if (tmpValue.Length > 10)
                            tmpValue = tmpValue.Substring(0, 10);
                        wd = DateTime.Parse(tmpValue);
                    }
                    catch (Exception)
                    {
                        wd = DateTime.Parse("1900-01-01");
                    }
                    sql += "           ,'" + wd.ToShortDateString() + "' ";
                }

                sql += "           ,'" + dr[13].ToString() + "'  ";  // [WeldNo]
                sql += "           ,'" + dr[14].ToString() + "'  ";  // [DataSeqNo]
                tmpValue = dr[15].ToString();  // [WeldingDate]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                {
                    try
                    {
                        if (DateTime.Compare(DateTime.Parse(tmpValue), new DateTime(1910, 01, 01)) == -1)
                        {
                            wDate = "1900-01-01";
                            cntErrorDate++;
                        }
                    }
                    catch
                    {
                        wDate = "1900-01-01";
                    }
                    sql += "           ,'" + wDate + "' ";
                }

                tmpValue = dr[15].ToString();  // [WeldingDateTime]
                if (string.IsNullOrWhiteSpace(tmpValue))
                {
                    sql += "           ,NULL ";
                }
                else
                {
                    try
                    {
                        wd = DateTime.Parse(tmpValue); //Time포함
                        if (DateTime.Compare(wd, new DateTime(1910, 01, 01)) == -1)
                        {
                            wd = DateTime.Parse("1900-01-01 00:00:00");
                        }
                    }
                    catch (Exception)
                    {
                        wd = DateTime.Parse("1900-01-01 00:00:00");
                    }
                    sql += "           ,'" + wd.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                }

                sql += "           ," + MakeTmpSql(dr[16].ToString()) + "  ";   // [AmbientTemperature]
                sql += "           ,'" + dr[17].ToString() + "'  ";  // [Status]
                sql += "           ,'" + dr[18].ToString() + "'  ";  // [Weather]
                sql += "           ,'" + dr[19].ToString() + "'  ";  // [Material]
                sql += "           ," + MakeTmpSql(dr[20].ToString()) + "  ";  // [Diameter]
                sql += "           ," + MakeTmpSql(dr[21].ToString()) + "  ";  // [Diameter2]
                sql += "           ," + MakeTmpSql(dr[22].ToString()) + "  ";  // [SDR]
                sql += "           ," + MakeTmpSql(dr[23].ToString()) + "  ";  // [WallThickness]
                sql += "           ," + MakeTmpSql(dr[24].ToString()) + "  ";  // [Angle]
                sql += "           ,'" + dr[25].ToString() + "'  ";  // [FittingManufacturer]
                sql += "           ,'" + dr[26].ToString() + "'  ";  // [Design]
                sql += "           ,'" + dr[27].ToString() + "'  ";  // [OperatingMode]
                sql += "           ,'" + dr[28].ToString() + "'  ";  // [Standard]
                sql += "           ,'" + dr[29].ToString() + "'  ";  // [Mode]
                sql += "           ,'" + dr[30].ToString() + "'  ";  // [FittingCode]
                sql += "           ," + MakeTmpSql(dr[31].ToString()) + "  ";   // [WeldingBeadLeft]
                sql += "           ," + MakeTmpSql(dr[32].ToString()) + "  ";   // [WeldingBeadRight]
                sql += "           ," + MakeTmpSql(dr[33].ToString()) + "  ";   // [WeldingBeadTotal]
                sql += "           ," + MakeTmpSql(dr[34].ToString()) + "  ";   // [WeldingBeadLeft2]
                sql += "           ," + MakeTmpSql(dr[35].ToString()) + "  ";   // [WeldingBeadRight2]
                sql += "           ," + MakeTmpSql(dr[36].ToString()) + "  ";   // [WeldingBeadTotal2]
                sql += "           ," + MakeTmpSql(dr[37].ToString()) + "  ";   // [WeldingBeadLeft3]
                sql += "           ," + MakeTmpSql(dr[38].ToString()) + "  ";   // [WeldingBeadRight3]
                sql += "           ," + MakeTmpSql(dr[39].ToString()) + "  ";   // [WeldingBeadTotal3]
                sql += "           ,'" + dr[40].ToString() + "'  ";  // [Latitude]
                sql += "           ,'" + dr[41].ToString() + "'  ";  // [Longitude]
                sql += "           ," + MakeTmpSql(dr[42].ToString()) + "  ";   // [DragForceNominalValue]
                sql += "           ,'" + dr[43].ToString() + "'  ";  // [DragForceNominalUnit]
                sql += "           ," + MakeTmpSql(dr[44].ToString()) + "  ";   // [DragForceActualValue]
                sql += "           ,'" + dr[45].ToString() + "'  ";  // [DragForceActualUnit]
                sql += "           ," + MakeTmpSql(dr[46].ToString()) + "  ";   // [MirrorTempNominalValue]
                sql += "           ," + MakeTmpSql(dr[47].ToString()) + "  ";   // [MirrorTempActualValue]
                sql += "           ," + MakeTmpSql(dr[48].ToString()) + "  ";   // [PreheatingTimeNominalValue]
                sql += "           ," + MakeTmpSql(dr[49].ToString()) + "  ";   // [PreheatingTimeActualValue]
                sql += "           ," + MakeTmpSql(dr[50].ToString()) + "  ";   // [BeadBuidupTimeNominalValue]
                sql += "           ," + MakeTmpSql(dr[51].ToString()) + "  ";   // [BeadBuidupTimeActualValue]
                sql += "           ," + MakeTmpSql(dr[52].ToString()) + "  ";   // [BeadBuildupForceNominalValue]
                sql += "           ,'" + dr[53].ToString() + "'  ";  // [BeadBuildupForceNominalUnit]
                sql += "           ," + MakeTmpSql(dr[54].ToString()) + "  ";   // [BeadBuildupForceActualValue]
                sql += "           ,'" + dr[55].ToString() + "'  ";  // [BeadBuildupForceActualUnit]
                sql += "           ," + MakeTmpSql(dr[56].ToString()) + "  ";   // [HeatingTimeNominaValue]
                sql += "           ," + MakeTmpSql(dr[57].ToString()) + "  ";   // [HeatingTimeActualValue]
                sql += "           ," + MakeTmpSql(dr[58].ToString()) + "  ";   // [HeatingForceNominalValue]
                sql += "           ,'" + dr[59].ToString() + "'  ";  // [HeatingForceNominalUnit]
                sql += "           ," + MakeTmpSql(dr[60].ToString()) + "  ";   // [HeatingForceActualValue]
                sql += "           ,'" + dr[61].ToString() + "'  ";  // [HeatingForceActualUnit]
                sql += "           ," + MakeTmpSql(dr[62].ToString()) + "  ";   // [ChangeOverTimeNominalValue]
                sql += "           ," + MakeTmpSql(dr[63].ToString()) + "  ";   // [ChangeOverTimeActualValue]
                sql += "           ," + MakeTmpSql(dr[64].ToString()) + "  ";   // [JoiningPressRampNominalValue]
                sql += "           ," + MakeTmpSql(dr[65].ToString()) + "  ";   // [JoiningPressRampActualValue]
                sql += "           ," + MakeTmpSql(dr[66].ToString()) + "  ";   // [JoiningForceNominalValue]
                sql += "           ,'" + dr[67].ToString() + "'  ";  // [JoiningForceNominalUnit]
                sql += "           ," + MakeTmpSql(dr[68].ToString()) + "  ";   // [JoiningForceActualValue]
                sql += "           ,'" + dr[69].ToString() + "'  ";  // [JoiningForceActualUnit]
                sql += "           ," + MakeTmpSql(dr[70].ToString()) + "  ";   // [CoolingTimeNominalValue]
                sql += "           ," + MakeTmpSql(dr[71].ToString()) + "  ";   // [CoolingTimeActualValue]
                //엑셀상 4칸 중복으로 인해 번호가 71 --> 76으로 뛰었음.
                sql += "           ," + MakeTmpSql(dr[76].ToString()) + "  ";   // [TwoLevelCoolingTimeNominalValue]
                sql += "           ," + MakeTmpSql(dr[77].ToString()) + "  ";   // [TwoLevelCoolingTimeActualValue]
                sql += "           ," + MakeTmpSql(dr[78].ToString()) + "  ";   // [TwoLevelCoolingForceNominalValue]
                sql += "           ,'" + dr[79].ToString() + "'  ";  // [TwoLevelCoolingForceNominalUnit]
                sql += "           ," + MakeTmpSql(dr[80].ToString()) + "  ";   // [TwoLevelCoolingForceActualValue]
                sql += "           ,'" + dr[81].ToString() + "'  ";  // [TwoLevelCoolingForceActualUnit]
                sql += "           ," + MakeTmpSql(dr[82].ToString()) + "  ";   // [WeldingDistance]
                sql += "           ," + MakeTmpSql(dr[83].ToString()) + "  ";   // [WeldingVoltageNominalValue]
                sql += "           ," + MakeTmpSql(dr[84].ToString()) + "  ";   // [WeldingVoltageActualValue]
                sql += "           ," + MakeTmpSql(dr[85].ToString()) + "  ";   // [ResistanceNominalValue]
                sql += "           ," + MakeTmpSql(dr[86].ToString()) + "  ";   // [ResistanceActualValue]
                sql += "           ," + MakeTmpSql(dr[87].ToString()) + "  ";   // [WorkNominalValue]
                sql += "           ," + MakeTmpSql(dr[88].ToString()) + "  ";   // [WorkActualValue]
                sql += "           ," + MakeTmpSql(dr[89].ToString()) + "  ";   // [TotalTimeNominalValue]
                sql += "           ," + MakeTmpSql(dr[90].ToString()) + "  ";   // [TotalTimeActualValue]
                sql += "           ,'" + Program.Option.LoginID + "'  ";  // [CreateID]
                sql += "           ,GETDATE() "; // [CreateDtm]
                sql += $"          ,{projectKey} ) ";   
                sqls.Add(sql);
            }

            //DB저장 및 Commit
            isOK = true;
            try
            {
                ResultMessage = DBManager.Instance.ExcuteTransaction(sqls);
                if (!string.IsNullOrWhiteSpace(ResultMessage))
                    isOK = false;

                return isOK;
            }
            catch (Exception ex)
            {
                isOK = false;
                ResultMessage = ex.Message;
                return isOK;
            }
        }      

        /// <summary>
        /// 숫자컬럼에 Null과 Value값을 매칭
        /// </summary>
        /// <returns></returns>
        private string MakeTmpSql(string tmpValue)
        {
            string sql = string.Empty;
            if (string.IsNullOrWhiteSpace(tmpValue))
                return "NULL";
            else
                return tmpValue;
        }
    }

}
