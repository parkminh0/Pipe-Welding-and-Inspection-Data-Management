using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace SWSUploader
{
    class CsvMasterListData
    {
        private int _ProjectKey;

        public int ProjectKey
        {
            get { return _ProjectKey; }
            set { _ProjectKey = value; }
        }

        private DataTable _dtCSVFile;
        /// <summary>
        /// CSV File Loading data
        /// </summary>
        public DataTable dtCSVFile
        {
            get { return _dtCSVFile; }
        }

        private List<string> _Querys;
        /// <summary>
        /// 검사결과 Query
        /// </summary>
        public List<string> Querys
        {
            get { return _Querys; }
            //set { _Querys = value; }
        }

        public bool isOK;
        public string ResultMessage;
        private string fileName;
        private DataTable dtDupData;
        private int dupCount;

        /// <summary>
        /// 생성자
        /// </summary>
        public CsvMasterListData(string aPath)
        {
            fileName = Path.GetFileName(aPath);
            _Querys = new List<string>();

            isOK = false;
            try
            {
                //Csv로딩 및 DataTable생성
                _dtCSVFile = LogicManager.Common.GetDataTableFromCSVFile(aPath);
                if (_dtCSVFile.Rows.Count > 0)
                    isOK = true;
                else
                    ResultMessage = string.Format(LangResx.Main.msg_EmptyFile, aPath);
            }
            catch (Exception ex)
            {
                _dtCSVFile = null;
                ResultMessage = string.Format(LangResx.Main.msg_FileFormatError + "\r\n{1}", fileName, ex.Message);
            }

            if (!isOK)
                return;

            if (!_dtCSVFile.Columns[1].ColumnName.ToLower().StartsWith("serial"))
                isOK = false;

            if (!_dtCSVFile.Columns[2].ColumnName.ToLower().StartsWith("inspection"))
                isOK = false;

            if (!_dtCSVFile.Columns[7].ColumnName.ToLower().StartsWith("material"))
                isOK = false;

            //체크
            if (!isOK)
            {
                ResultMessage = string.Format(LangResx.Main.msg_FileFormatError, fileName);
                return;
            }
        }

        /// <summary>
        /// 중복상태 업데이트
        /// </summary>
        public void UpdateDupStatus()
        {
            string sql = string.Empty;
            sql += "update TempBeadMaster set Rate = 100 ";
            sql += "  from TempBeadMaster x ";
            sql += "  JOIN ( ";
            sql += "		select tm.SerialNo, tm.InspectionNo, tm.InspectionDate ";
            sql += "		from TempBeadMaster tm ";
            sql += "		join BeadMaster bm ";
            sql += "		  on bm.SerialNo = tm.SerialNo and bm.InspectionNo = tm.InspectionNo and bm.InspectionDate = tm.InspectionDate ";
            sql += "		 where tm.CreateID = '" + Program.Option.LoginID + "' ";
            sql += "	 ) y ";
            sql += "	 on y.SerialNo = x.SerialNo and y.InspectionNo = x.InspectionNo and y.InspectionDate = x.InspectionDate";
            sql += " where x.CreateID = '" + Program.Option.LoginID + "' ";
            try
            {
                DBManager.Instance.ExcuteDataUpdate(sql);
            }
            catch (Exception)
            {
                return;
            }

            //중복 데이터 목록 추출
            sql = string.Empty;
            sql += "select UPPER(tm.SerialNo) SerialNo, tm.InspectionNo, CONVERT(VARCHAR(10), tm.InspectionDate, 120) InspectionDate ";
            sql += "  from TempBeadMaster tm ";
            sql += " where Rate = 100 and CreateID = '" + Program.Option.LoginID + "'";
            try
            {
                dtDupData = DBManager.Instance.GetDataTable(sql);
                dupCount = dtDupData.Rows.Count;
            }
            catch (Exception)
            {
                dtDupData = null;
                dupCount = 0;
            }
        }

        /// <summary>
        /// 중복 건수 확인
        /// </summary>
        /// <returns></returns>
        public int GetDupCount()
        {
            string sql = string.Empty;
            sql = string.Format("select count(*) from TempBeadMaster where Rate = 100 and CreateID = '{0}' ", Program.Option.LoginID);

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
            sql += "select top 1 (CONVERT(VARCHAR, tm.[LineNo]) + '-' + tm.SerialNo + '-' + CONVERT(VARCHAR, tm.InspectionNo) + '-' + CONVERT(VARCHAR(10), tm.InspectionDate, 120)) DupRow ";
            sql += "  from TempBeadMaster tm ";
            sql += " where Rate = 100 and CreateID = '" + Program.Option.LoginID + "'";

            try
            {
                return DBManager.Instance.GetStringScalar(sql);
            }
            catch (Exception)
            {
                return "-n/a-";
            }
        }

        /// <summary>
        /// 중복확인
        /// </summary>
        /// <returns></returns>
        private bool isDup(string serialNo, string inspNo, string inspDate)
        {
            bool flag = false;
            if (dupCount == 0)
                return flag;

            inspNo = int.Parse(inspNo).ToString(); //'0010' 현상을 10으로 변환처리
            string s, n, d;
            foreach (DataRow dr in dtDupData.Rows)
            {
                s = dr[0].ToString().Trim().ToUpper();
                n = dr[1].ToString().Trim();
                d = dr[2].ToString().Trim();
                if (s == serialNo.Trim() && n == inspNo && d == inspDate.Trim())
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        /// <summary>
        /// 기존 업로드된 하위 데이터 삭제
        /// </summary>
        public bool DeleteDetailData()
        {
            List<string> sqls = new List<string>();
            string sql = string.Empty;
            sql += "delete BeadImageInfo ";
            sql += "  from BeadImageInfo x ";
            sql += "  join ( ";
            sql += "	select bm.BeadKey, bd.BeadDetailKey ";
            sql += "	  FROM TempBeadMaster tm ";
            sql += "	  JOIN BeadMaster bm ";
            sql += "		ON bm.SerialNo = tm.SerialNo AND bm.InspectionNo = tm.InspectionNo AND bm.InspectionDate = tm.InspectionDate ";
            sql += "	  join BeadDetail bd ";
            sql += "		on bm.BeadKey = bd.BeadKey ";
            sql += "	 where tm.CreateID = '" + Program.Option.LoginID + "' ";
            sql += "	) y ";
            sql += "	on y.BeadDetailKey = x.BeadDetailKey";
            sqls.Add(sql);

            sql = string.Empty;
            sql += "delete BeadDetail ";
            sql += "  from BeadDetail x ";
            sql += "  join ( ";
            sql += "	select bm.BeadKey ";
            sql += "	  FROM TempBeadMaster tm ";
            sql += "	  JOIN BeadMaster bm ";
            sql += "		ON bm.SerialNo = tm.SerialNo AND bm.InspectionNo = tm.InspectionNo AND bm.InspectionDate = tm.InspectionDate ";
            sql += "	 where tm.CreateID = '" + Program.Option.LoginID + "' ";
            sql += "	) y ";
            sql += "	on y.BeadKey = x.BeadKey";
            sqls.Add(sql);

            string result;

            try
            {
                isOK = true;
                result = DBManager.Instance.ExcuteTransaction(sqls);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    isOK = false;
                    ResultMessage = result;
                }
            }
            catch (Exception ex)
            {
                isOK = false;
                ResultMessage = ex.Message;
            }

            return isOK;
        }

        /// <summary>
        /// 현재 업로드된 Temp Table불러오기
        /// </summary>
        /// <param name="isEmptyRequest">False가 기본, True이면 빈테이블 생성</param>
        /// <returns></returns>
        public DataTable GetTempBeadMaster(bool isEmptyRequest)
        {
            DataTable dtData;

            //빈 테이블만 생성시
            if (isEmptyRequest)
            {
                dtData = GetEmptyTempBeadMaster();
                return dtData;
            }

            string sql = string.Empty;
            sql += "SELECT isnull(bm.BeadKey, 0) BeadKey, ";
            sql += "       tm.ProjectKey, ";
            sql += "       tm.[LineNo], ";
            sql += "       tm.[SerialNo], ";
            sql += "       tm.[InspectionNo], ";
            sql += "       CONVERT(VARCHAR(10), tm.InspectionDate, 120) InspectionDate, ";
            sql += "       tm.[Material], ";
            sql += "       tm.[OD], ";
            sql += "       tm.[WallThickness], ";
            sql += "       tm.[SDR], ";
            sql += "       tm.Rate, ";
            sql += "	   isnull(x.BeadCount, 0) Status, ";
            //sql += "       '' RateStatus, ";
            sql += "       tm.CreateID, ";
            sql += "       tm.CreateDtm ";
            sql += "  FROM TempBeadMaster tm ";
            sql += "  LEFT join BeadMaster bm ";
            sql += "    ON bm.SerialNo = tm.SerialNo and bm.InspectionNo = tm.InspectionNo and bm.InspectionDate = tm.InspectionDate ";
            sql += "  LEFT JOIN (select bd.BeadKey, count(*) BeadCount from BeadDetail bd group by bd.BeadKey) x ";
            sql += "	ON x.BeadKey = bm.BeadKey ";
            sql += " WHERE tm.CreateID = '" + Program.Option.LoginID + "'";
            try
            {
                dtData = DBManager.Instance.GetDataTable(sql);
                DataColumn[] pkey = new DataColumn[1];
                pkey[0] = (DataColumn)dtData.Columns["BeadKey"];
                dtData.PrimaryKey = pkey;
            }
            catch (Exception)
            {
                dtData = null;
            }
            return dtData;
        }

        /// <summary>
        /// GetTempBeadMaster 빈 테이블 생성
        /// </summary>
        /// <returns></returns>
        private DataTable GetEmptyTempBeadMaster()
        {
            string sql = string.Empty;
            sql += "SELECT 0 as BeadKey ";
            sql += "      ,0 as ProjectKey ";
            sql += "      ,0 as [LineNo] ";
            sql += "      ,[SerialNo] ";
            sql += "      ,[InspectionNo] ";
            sql += "      ,[InspectionDate] ";
            sql += "      ,[Material] ";
            sql += "      ,[OD] ";
            sql += "      ,[WallThickness] ";
            sql += "      ,[SDR] ";
            sql += "	  ,0 Rate ";
            sql += "	  ,0 Status ";
            //sql += "      ,'' RateStatus ";
            sql += "      ,'' as CreateID ";
            sql += "      ,GETDATE() as CreateDtm ";
            sql += "  FROM [dbo].[BeadMaster] ";
            sql += " WHERE 1 = 2 "; //빈테이블

            return DBManager.Instance.GetDataTable(sql); 
        }

        /// <summary>
        /// 검수원장 생성
        /// </summary>
        /// <returns></returns>
        public bool InsertBeadMaster()
        {
            List<string> sqls = new List<string>();
            string sql, tmpValue;

            foreach (DataRow dr in _dtCSVFile.Rows)
            {
                if (isDup(dr[1].ToString(), dr[2].ToString(), dr[3].ToString())) //중복이면 생략
                    continue;

                sql = string.Empty;
                sql += "INSERT INTO [dbo].[BeadMaster] ";
                sql += "           ([ProjectKey] ";
                sql += "           ,[LineNo] ";
                sql += "           ,[SerialNo] ";
                sql += "           ,[InspectionNo] ";
                sql += "           ,[InspectionDate] ";
                sql += "           ,[InspectionTime] ";
                sql += "           ,[InspectorID] ";
                sql += "           ,[SoftwareVersion] ";
                sql += "           ,[Material] ";
                sql += "           ,[OD] ";
                sql += "           ,[WallThickness] ";
                sql += "           ,[SDR] ";
                sql += "           ,[ProdCode] ";
                sql += "           ,[BatchNo] ";
                sql += "           ,[IDNo] ";
                sql += "           ,[ProdCode2] ";
                sql += "           ,[BatchNo2] ";
                sql += "           ,[IDNo2] ";
                sql += "           ,[Manufacturer] ";
                sql += "           ,[MachineSerialNo] ";
                sql += "           ,[WeldNo] ";
                sql += "           ,[WeldingDate] ";
                sql += "           ,[WeldedTime] ";
                sql += "           ,[Welder] ";
                sql += "           ,[Project] ";
                sql += "           ,[Floor] ";
                sql += "           ,[Column] ";
                sql += "           ,[Location] ";
                sql += "           ,[LineSeries] ";
                sql += "           ,[Medium] ";
                sql += "           ,[OtherInfo1] ";
                sql += "           ,[AmbTemp] ";
                sql += "           ,[PipeTemp] ";
                sql += "           ,[OperatingPressure] ";
                sql += "           ,[Constructor] ";
                sql += "           ,[OtherInfo2] ";
                sql += "           ,[TestPoints] ";
                sql += "           ,[CompletionRate] ";
                sql += "           ,[PassRate] ";
                sql += "           ,[K_C] ";
                sql += "           ,[KMin] ";
                sql += "           ,[KMax] ";
                sql += "           ,[KAvg] ";
                sql += "           ,[isKPass] ";
                sql += "           ,[BMin_C] ";
                sql += "           ,[BMax_C] ";
                sql += "           ,[BMin] ";
                sql += "           ,[BMax] ";
                sql += "           ,[BAvg] ";
                sql += "           ,[isBPass] ";
                sql += "           ,[BRatio_C] ";
                sql += "           ,[B1Min] ";
                sql += "           ,[B2Min] ";
                sql += "           ,[BRatioMin] ";
                sql += "           ,[B1Max] ";
                sql += "           ,[B2Max] ";
                sql += "           ,[BRatioMax] ";
                sql += "           ,[B1Avg] ";
                sql += "           ,[B2Avg] ";
                sql += "           ,[BRatioAvg] ";
                sql += "           ,[isBRatioPass] ";
                sql += "           ,[MissAlign_C] ";
                sql += "           ,[MissAlignMin] ";
                sql += "           ,[MissAlignMax] ";
                sql += "           ,[MissAlignAvg] ";
                sql += "           ,[isMissAlignPass] ";
                sql += "           ,[Notch_C] ";
                sql += "           ,[NotchMin] ";
                sql += "           ,[NotchMax] ";
                sql += "           ,[NotchAvg] ";
                sql += "           ,[isNotchPass] ";
                sql += "           ,[isNotchEyes] ";
                sql += "           ,[NotchNote] ";
                sql += "           ,[isAngularDevEyes] ";
                sql += "           ,[AngularDevNote] ";
                sql += "           ,[isCrackEyes] ";
                sql += "           ,[CrackNote] ";
                sql += "           ,[isVoidEyes] ";
                sql += "           ,[VoidNote] ";
                sql += "           ,[isSupportPadEyes] ";
                sql += "           ,[SupportPadNote] ";
                sql += "           ,[isInterruptionEyes] ";
                sql += "           ,[InterruptionNote] ";
                sql += "           ,[isOverheatingEyes] ";
                sql += "           ,[OverheatingNote] ";
                sql += "           ,[DVS] ";
                sql += "           ,[VIScore] ";
                sql += "           ,[VIGrade] ";
                sql += "           ,[Code] ";
                sql += "           ,[Result] ";
                sql += "           ,[EvaluationScore] ";
                sql += "           ,[Grade] ";
                sql += "           ,[Descr] ";
                sql += "           ,[CreateID] ";
                sql += "           ,[CreateDtm]) ";
                sql += "     VALUES ";
                sql += "           (" + ProjectKey + " ";
                sql += "           ," + dr[0].ToString() + " ";     //LineNo
                sql += "           ,'" + dr[1].ToString() + "' ";   //SerialNo
                sql += "           ," + dr[2].ToString() + " ";     //InspectionNo
                sql += "           ,'" + dr[3].ToString() + "' ";   //InspectionDate
                sql += "           ,'" + dr[4].ToString() + "' ";   //Time
                sql += "           ,'" + dr[5].ToString() + "' ";   //Inspector ID
                sql += "           ,'" + dr[6].ToString() + "' ";   //Software Ver
                sql += "           ,'" + dr[7].ToString() + "' ";   //Material

                tmpValue = dr[8].ToString();    //OD
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[9].ToString();    //WallThickness
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'" + dr[10].ToString() + "' ";  //SDR
                sql += "           ,'" + dr[11].ToString() + "' ";  //Prod. Code_1
                sql += "           ,'" + dr[12].ToString() + "' ";  //Batch No._1
                sql += "           ,'" + dr[13].ToString() + "' ";  //ID No._1
                sql += "           ,'" + dr[14].ToString() + "' ";  //Prod. Code_1
                sql += "           ,'" + dr[15].ToString() + "' ";  //Batch No._1
                sql += "           ,'" + dr[16].ToString() + "' ";  //ID No._1
                sql += "           ,'" + dr[17].ToString() + "' ";  //Manufacturer
                sql += "           ,'" + dr[18].ToString() + "' ";  //Machine SN
                sql += "           ,'" + dr[19].ToString() + "' ";  //Weld No.

                tmpValue = dr[20].ToString();  //Welded Date
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                {
                    if (tmpValue.Length >= 10)
                        sql += "           ,'" + DateTime.Parse(tmpValue.Substring(0, 10)).ToString("yyyy-MM-dd") + "' ";
                    else
                        sql += "           ,'" + string.Format("20{0}", tmpValue) + "' ";
                }

                sql += "           ,'" + dr[21].ToString() + "' ";  //Welded Time
                sql += "           ,'" + dr[22].ToString() + "' ";  //Welder
                sql += "           ,'" + dr[23].ToString() + "' ";  //Project
                sql += "           ,'" + dr[24].ToString() + "' ";  //Floor
                sql += "           ,'" + dr[25].ToString() + "' ";  //Column
                sql += "           ,'" + dr[26].ToString() + "' ";  //Location
                sql += "           ,'" + dr[27].ToString() + "' ";  //Line Series
                sql += "           ,'" + dr[28].ToString() + "' ";  //Medium
                sql += "           ,'" + dr[29].ToString() + "' ";  //Other Info._1
                tmpValue = dr[30].ToString();    //AmbTemp
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[31].ToString();    //PipeTemp
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[32].ToString();    //OperatingPressure
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'" + dr[33].ToString() + "' ";  //Constructor
                sql += "           ,'" + dr[34].ToString() + "' ";  //Other Info._2
                sql += "           ,0 ";    //TestPoints
                sql += "           ,0 ";    //CompletionRate
                sql += "           ,0 ";    //PassRate

                tmpValue = dr[35].ToString();   //[K_C]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[36].ToString();    //[KMin]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[37].ToString();    //[KMax]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[38].ToString();    //[KAvg]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'" + dr[39].ToString() + "' ";    //[isKPass]

                tmpValue = dr[40].ToString();    //[BMin_C]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[41].ToString();    //[BMax_C]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[42].ToString();    //[BMin]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[43].ToString();    //[BMax]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[44].ToString();    //[BAvg]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'" + dr[45].ToString() + "' ";    //[isBPass]

                tmpValue = dr[46].ToString();    //[BRatio_C]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[47].ToString();    //[B1Min]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[48].ToString();    //[B2Min]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[49].ToString();    //[BRatioMin]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[50].ToString();    //[B1Max]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[51].ToString();    //[B2Max]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[52].ToString();    //[BRatioMax]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[53].ToString();    //[B1Avg]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[54].ToString();    //[B2Avg]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[55].ToString();    //[BRatioAvg]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'" + dr[56].ToString() + "' ";    //[isBRatioPass]

                tmpValue = dr[57].ToString();    //[MissAlign_C]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[58].ToString();    //[MissAlignMin]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[59].ToString();    //[MissAlignMax]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[60].ToString();    //[MissAlignAvg]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'" + dr[61].ToString() + "' ";    //[isMissAlignPass]

                tmpValue = dr[62].ToString();    //[Notch_C]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[63].ToString();    //[NotchMin]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[64].ToString();    //[NotchMax]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[65].ToString();    //[NotchAvg]
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'" + dr[66].ToString() + "' ";  //isNotchPass
                sql += "           ,'" + dr[67].ToString() + "' ";  //isNotchEyes
                sql += "           ,'" + dr[68].ToString() + "' ";  //NotchNote
                sql += "           ,'" + dr[69].ToString() + "' ";  //isAngularDevEyes
                sql += "           ,'" + dr[70].ToString() + "' ";  //AngularDevNote
                sql += "           ,'" + dr[71].ToString() + "' ";  //isCrackEyes
                sql += "           ,'" + dr[72].ToString() + "' ";  //CrackNote
                sql += "           ,'" + dr[73].ToString() + "' ";  //isVoidEyes
                sql += "           ,'" + dr[74].ToString() + "' ";  //VoidNote
                sql += "           ,'" + dr[75].ToString() + "' ";  //isSupportPadEyes
                sql += "           ,'" + dr[76].ToString() + "' ";  //SupportPadNote
                sql += "           ,'" + dr[77].ToString() + "' ";  //isInterruptionEyes
                sql += "           ,'" + dr[78].ToString() + "' ";  //InterruptionNote
                sql += "           ,'" + dr[79].ToString() + "' ";  //isOverheatingEyes
                sql += "           ,'" + dr[80].ToString() + "' ";  //OverheatingNote
                sql += "           ,'" + dr[81].ToString() + "' ";  //DVS

                tmpValue = dr[82].ToString();    //VIScore
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'" + dr[83].ToString() + "' ";   //VIGrade
                sql += "           ,'" + dr[84].ToString() + "' ";   //Code
                sql += "           ,'' ";   //Result
                sql += "           ,0 ";    //EvaluationScore
                sql += "           ,'' ";   //Grade
                sql += "           ,'' ";   //Descr
                sql += "           ,'" + Program.Option.LoginID + "' ";   //CreateID
                sql += "           ,GETDATE()) ";
                sqls.Add(sql);
            }

            isOK = true;
            try
            {
                ResultMessage = DBManager.Instance.ExcuteTransaction(sqls);
                if (!string.IsNullOrWhiteSpace(ResultMessage))
                {
                    isOK = false;
                }
                return isOK;
            }
            catch (Exception ex)
            {
                isOK = false;
                ResultMessage = ex.Message;
                return isOK;
            }
        }

    }
}
