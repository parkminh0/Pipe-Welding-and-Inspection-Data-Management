using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SWSAnalyzer
{
    public class CommonLogic
    {
        /// <summary>
        /// 각종 Key 채번
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public int GetNextKey(string category)
        {
            return DBManager.Instance.MDB.uspGetNextKey(category);
        }

        /// <summary>
        /// BeadDetailInfo에서 합성이미지 가져오기
        /// </summary>
        /// <param name="beadDetailKey"></param>
        /// <param name="imageType">0:합성 1:원본</param>
        /// <returns></returns>
        public byte[] GetBeadImage(int beadDetailKey, int imageType)
        {
            string ColName = (imageType == 0) ? "OedImage" : "RawImage";
            byte[] beadImage = null;
            string sql = string.Format("select bii.{0} from BeadImageInfo bii where bii.BeadDetailKey = {1}", ColName, beadDetailKey);
            DataTable dtData;
            try
            {
                dtData = DBManager.Instance.GetDataTable(sql);
                if (dtData.Rows[0][0] != DBNull.Value)
                {
                    beadImage = (byte[])dtData.Rows[0][0];
                }
            }
            catch (Exception)
            {
            }
            return beadImage;
        }

        /// <summary>
        /// BeadMaster Select Query 생성 (조건 제외)
        /// </summary>
        /// <returns></returns>
        public string GetSelectBeadMasterSql()
        {
            string sql = string.Empty;
            sql += "SELECT [BeadKey] ";
            sql += "      ,c.CompName ";
            sql += "      ,p.ProjectName ";
            sql += "      ,[LineNo] ";
            sql += "      ,[SerialNo] ";
            sql += "      ,[InspectionNo] ";
            sql += "      ,[InspectionDate] ";
            sql += "      ,[InspectionTime] ";
            sql += "      ,[InspectorID] ";
            sql += "      ,[SoftwareVersion] ";
            sql += "      ,[Material] ";
            sql += "      ,[OD] ";
            sql += "      ,[WallThickness] ";
            sql += "      ,[SDR] ";
            sql += "      ,[ProdCode] ";
            sql += "      ,[BatchNo] ";
            sql += "      ,[IDNo] ";
            sql += "      ,[ProdCode2] ";
            sql += "      ,[BatchNo2] ";
            sql += "      ,[IDNo2] ";
            sql += "      ,[Manufacturer] ";
            sql += "      ,bm.[MachineSerialNo] ";
            sql += "      ,bm.[WeldNo] ";
            sql += "      ,bm.[WeldingDate] ";
            sql += "      ,[WeldedTime] ";
            sql += "      ,[Welder] ";
            sql += "      ,[Project] ";
            sql += "      ,[Floor] ";
            sql += "      ,[Column] ";
            sql += "      ,[Location] ";
            sql += "      ,[LineSeries] ";
            sql += "      ,[Medium] ";
            sql += "      ,[OtherInfo1] ";
            sql += "      ,[AmbTemp] ";
            sql += "      ,[PipeTemp] ";
            sql += "      ,[OperatingPressure] ";
            sql += "      ,[Constructor] ";
            sql += "      ,[OtherInfo2] ";
            sql += "      ,[TestPoints] ";
            sql += "      ,[CompletionRate] ";
            sql += "      ,[PassRate] ";
            sql += "      ,[K_C] ";
            sql += "      ,[KMin] ";
            sql += "      ,[KMax] ";
            sql += "      ,[KAvg] ";
            sql += "      ,[isKPass] ";
            sql += "      ,[BMin_C] ";
            sql += "      ,[BMax_C] ";
            sql += "      ,[BMin] ";
            sql += "      ,[BMax] ";
            sql += "      ,[BAvg] ";
            sql += "      ,[isBPass] ";
            sql += "      ,[BRatio_C] ";
            sql += "      ,[B1Min] ";
            sql += "      ,[B2Min] ";
            sql += "      ,[BRatioMin] ";
            sql += "      ,[B1Max] ";
            sql += "      ,[B2Max] ";
            sql += "      ,[BRatioMax] ";
            sql += "      ,[B1Avg] ";
            sql += "      ,[B2Avg] ";
            sql += "      ,[BRatioAvg] ";
            sql += "      ,[isBRatioPass] ";
            sql += "      ,[MissAlign_C] ";
            sql += "      ,[MissAlignMin] ";
            sql += "      ,[MissAlignMax] ";
            sql += "      ,[MissAlignAvg] ";
            sql += "      ,[isMissAlignPass] ";
            sql += "      ,[Notch_C] ";
            sql += "      ,[NotchMin] ";
            sql += "      ,[NotchMax] ";
            sql += "      ,[NotchAvg] ";
            sql += "      ,[isNotchPass] ";
            sql += "      ,[isNotchEyes] ";
            sql += "      ,[NotchNote] ";
            sql += "      ,[isAngularDevEyes] ";
            sql += "      ,[AngularDevNote] ";
            sql += "      ,[isCrackEyes] ";
            sql += "      ,[CrackNote] ";
            sql += "      ,[isVoidEyes] ";
            sql += "      ,[VoidNote] ";
            sql += "      ,[isSupportPadEyes] ";
            sql += "      ,[SupportPadNote] ";
            sql += "      ,[isInterruptionEyes] ";
            sql += "      ,[InterruptionNote] ";
            sql += "      ,[isOverheatingEyes] ";
            sql += "      ,[OverheatingNote] ";
            sql += "      ,[DVS] ";
            sql += "      ,[VIScore] ";
            sql += "      ,[VIGrade] ";
            sql += "      ,[Code] ";
            sql += "      ,[Result] ";
            sql += "      ,[EvaluationScore] ";
            sql += "      ,[Grade] ";
            sql += "      ,bm.CreateDtm ";
            sql += "      ,isnull(wm.WeldKey, 0) WeldKey ";
            sql += "  FROM [dbo].[BeadMaster] bm ";
            sql += "  left join Project p ";
            sql += "    on bm.ProjectKey = p.ProjectKey ";
            sql += "  left join Company c ";
            sql += "    on p.CompKey = c.CompKey ";
            sql += "  LEFT JOIN (select MachineSerialNo, WeldNo, WeldingDate, CreateID, min(WeldKey) WeldKey from WeldMaster group by MachineSerialNo, WeldNo, WeldingDate, CreateID) wm ";
            sql += "    ON bm.MachineSerialNo = wm.MachineSerialNo and bm.WeldNo = wm.WeldNo and bm.WeldingDate = wm.WeldingDate ";
            sql += " where 1 = 1 ";

            return sql;
        }

        /// <summary>
        /// WeldMaster Select Query 생성 (조건은 별도로 생성)
        /// </summary>
        /// <returns></returns>
        public string GetSelectWeldMasterSql()
        {
            string sql = string.Empty;
            sql += "SELECT [WeldKey] ";
            sql += "      ,wm.[ProjectKey] ";
            sql += "      ,wm.[MachineSerialNo] ";
            sql += "      ,[MachineLine] ";
            sql += "      ,[MachineType] ";
            sql += "      ,[SoftwareVersion] ";
            sql += "      ,[LastMaintDate] ";
            sql += "      ,[NextMaintDate] ";
            sql += "      ,[WeldProjectDescr] ";
            sql += "      ,[AdditionalData] ";
            sql += "      ,[InstallingCompany] ";
            sql += "      ,[WelderCode] ";
            sql += "      ,[WelderName] ";
            sql += "      ,[WeldersCompany] ";
            sql += "      ,[WelderCertExpireDate] ";
            sql += "      ,wm.[WeldNo] ";
            sql += "      ,[DataSeqNo] ";
            sql += "      ,wm.[WeldingDate] ";
            sql += "      ,[WeldingDateTime] ";
            sql += "      ,[AmbientTemperature] ";
            sql += "      ,[Status] ";
            sql += "      ,[Weather] ";
            sql += "      ,[Material] ";
            sql += "      ,[Diameter] ";
            sql += "      ,[Diameter2] ";
            sql += "      ,[SDR] ";
            sql += "      ,[WallThickness] ";
            sql += "      ,[Angle] ";
            sql += "      ,[FittingManufacturer] ";
            sql += "      ,[Design] ";
            sql += "      ,[OperatingMode] ";
            sql += "      ,[Standard] ";
            sql += "      ,[Mode] ";
            sql += "      ,[FittingCode] ";
            sql += "      ,[WeldingBeadLeft] ";
            sql += "      ,[WeldingBeadRight] ";
            sql += "      ,[WeldingBeadTotal] ";
            sql += "      ,[WeldingBeadLeft2] ";
            sql += "      ,[WeldingBeadRight2] ";
            sql += "      ,[WeldingBeadTotal2] ";
            sql += "      ,[WeldingBeadLeft3] ";
            sql += "      ,[WeldingBeadRight3] ";
            sql += "      ,[WeldingBeadTotal3] ";
            sql += "      ,[Latitude] ";
            sql += "      ,[Longitude] ";
            sql += "      ,[DragForceNominalValue] ";
            sql += "      ,[DragForceNominalUnit] ";
            sql += "      ,[DragForceActualValue] ";
            sql += "      ,[DragForceActualUnit] ";
            sql += "      ,[MirrorTempNominalValue] ";
            sql += "      ,[MirrorTempActualValue] ";
            sql += "      ,[PreheatingTimeNominalValue] ";
            sql += "      ,[PreheatingTimeActualValue] ";
            sql += "      ,[BeadBuidupTimeNominalValue] ";
            sql += "      ,[BeadBuidupTimeActualValue] ";
            sql += "      ,[BeadBuildupForceNominalValue] ";
            sql += "      ,[BeadBuildupForceNominalUnit] ";
            sql += "      ,[BeadBuildupForceActualValue] ";
            sql += "      ,[BeadBuildupForceActualUnit] ";
            sql += "      ,[HeatingTimeNominaValue] ";
            sql += "      ,[HeatingTimeActualValue] ";
            sql += "      ,[HeatingForceNominalValue] ";
            sql += "      ,[HeatingForceNominalUnit] ";
            sql += "      ,[HeatingForceActualValue] ";
            sql += "      ,[HeatingForceActualUnit] ";
            sql += "      ,[ChangeOverTimeNominalValue] ";
            sql += "      ,[ChangeOverTimeActualValue] ";
            sql += "      ,[JoiningPressRampNominalValue] ";
            sql += "      ,[JoiningPressRampActualValue] ";
            sql += "      ,[JoiningForceNominalValue] ";
            sql += "      ,[JoiningForceNominalUnit] ";
            sql += "      ,[JoiningForceActualValue] ";
            sql += "      ,[JoiningForceActualUnit] ";
            sql += "      ,[CoolingTimeNominalValue] ";
            sql += "      ,[CoolingTimeActualValue] ";
            sql += "      ,[TwoLevelCoolingTimeNominalValue] ";
            sql += "      ,[TwoLevelCoolingTimeActualValue] ";
            sql += "      ,[TwoLevelCoolingForceNominalValue] ";
            sql += "      ,[TwoLevelCoolingForceNominalUnit] ";
            sql += "      ,[TwoLevelCoolingForceActualValue] ";
            sql += "      ,[TwoLevelCoolingForceActualUnit] ";
            sql += "      ,[WeldingDistance] ";
            sql += "      ,[WeldingVoltageNominalValue] ";
            sql += "      ,[WeldingVoltageActualValue] ";
            sql += "      ,[ResistanceNominalValue] ";
            sql += "      ,[ResistanceActualValue] ";
            sql += "      ,[WorkNominalValue] ";
            sql += "      ,[WorkActualValue] ";
            sql += "      ,[TotalTimeNominalValue] ";
            sql += "      ,[TotalTimeActualValue] ";
            sql += "	  ,isnull(bm.BeadKey, 0) BeadKey ";
            sql += "      ,wm.CreateID WeldCreateID ";
            sql += "      ,bm.CreateID InspectCreateID ";
            sql += "      ,wm.CreateDtm ";
            sql += "  FROM [dbo].[WeldMaster] wm ";
            sql += "  LEFT JOIN Project p ";
            sql += "    ON p.ProjectKey = wm.ProjectKey ";
            sql += "  left join Company c ";
            sql += "    on p.CompKey = c.CompKey ";
            sql += "  LEFT JOIN (select MachineSerialNo, WeldNo, WeldingDate, min(BeadKey) BeadKey, ProjectKey, CreateID ";
            sql += "               from BeadMaster ";
            sql += "			  where LTRIM(RTRIM(MachineSerialNo)) <> '' ";
            sql += "			  group by MachineSerialNo, WeldNo, WeldingDate, ProjectKey, CreateID) bm ";
            sql += "    on bm.MachineSerialNo = wm.MachineSerialNo and bm.WeldNo = wm.WeldNo and bm.WeldingDate = wm.WeldingDate ";
            sql += " WHERE 1 = 1 ";

            return sql;
        }

        /// <summary>
        /// 현재 암호 가져오기
        /// </summary>
        /// <returns></returns>
        public string GetCurrentPasswd()
        {
            string sql = "SELECT Passwd from UserInfo WHERE UserID = '" + Program.Option.LoginID + "' ";
            string passwd = DBManager.Instance.GetStringScalar(sql);
            return AES.AESDecrypt256(passwd, Program.constance.compName);
            //return DBManager.Instance.GetStringScalar(sql);
        }

    }
}
