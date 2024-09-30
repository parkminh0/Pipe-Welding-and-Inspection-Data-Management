using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWSUploader
{
    class CsvDetailData
    {
        private int _BeadKey;

        public int BeadKey
        {
            get { return _BeadKey; }
            set { _BeadKey = value; }
        }

        private string _SerialNo;

        public string SerialNo
        {
            get { return _SerialNo; }
            set { _SerialNo = value; }
        }

        private int _InspectionNo;

        public int InspectionNo
        {
            get { return _InspectionNo; }
            set { _InspectionNo = value; }
        }

        private DateTime _InspectionDate;

        public DateTime InspectionDate
        {
            get { return _InspectionDate; }
            set { _InspectionDate = value; }
        }

        //---------------

        private string _PathName;
        private string _FolderName;
        private string _CsvFullPath;
        private DataRow mRow;

        private bool isOedImage;
        private bool isRawImage;
        private bool isXyLineImage;

        private List<ParamQuery> paramQueryList;
        private int _LastKey; //AutoKey생성용

        private DataTable _dtCSVFile;
        public bool isOK;
        public string ResultMessage;

        /// <summary>
        /// 생성자
        /// </summary>
        public CsvDetailData(string aPath, string foldName, DataRow dr, bool[] options)
        {
            _PathName = aPath;
            _FolderName = foldName;
            _CsvFullPath = Path.Combine(_PathName, _FolderName);
            isOedImage = options[0];
            isRawImage = options[1];
            isXyLineImage = options[2];

            mRow = dr;
            BeadKey = int.Parse(mRow["BeadKey"].ToString());
            SerialNo = mRow["SerialNo"].ToString();
            InspectionNo = int.Parse(mRow["InspectionNo"].ToString());
            InspectionDate = DateTime.Parse(mRow["InspectionDate"].ToString());

            if (!GetCsvData())
                return;
        }

        /// <summary>
        /// Csv 파일 불러오기
        /// </summary>
        /// <returns></returns>
        private bool GetCsvData()
        {
            string csvName = string.Format("{0}.csv", _FolderName);
            string csvFullFileName = Path.Combine(_CsvFullPath, csvName);

            isOK = false;
            try
            {
                //Csv로딩 및 DataTable생성
                _dtCSVFile = LogicManager.Common.GetDataTableFromCSVFile(csvFullFileName);
                if (_dtCSVFile.Rows.Count > 0)
                    isOK = true;
                else
                    ResultMessage = string.Format(LangResx.Main.msg_EmptyFile, csvName);
            }
            catch (Exception ex)
            {
                _dtCSVFile = null;
                ResultMessage = string.Format(LangResx.Main.msg_FileFormatError + "\r\n{1}", csvName, ex.Message);
            }

            if (!isOK)
                return isOK;

            if (!_dtCSVFile.Columns[1].ColumnName.ToLower().StartsWith("serial"))
                isOK = false;

            if (!_dtCSVFile.Columns[2].ColumnName.ToLower().StartsWith("inspection"))
                isOK = false;

            if (!_dtCSVFile.Columns[3].ColumnName.ToLower().StartsWith("date"))
                isOK = false;

            //체크
            if (!isOK)
                ResultMessage = string.Format(LangResx.Main.msg_FileFormatError, csvName);

            return isOK;
        }

        /// <summary>
        /// 데이터생성 메인
        /// </summary>
        public bool RunQuery()
        {
            //쿼리 생성
            MakeInsertBeadDetailSql();
            string result;            

            try
            {
                isOK = true;
                result = DBManager.Instance.ExcuteTransactionParam(paramQueryList);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    isOK = false;
                    ResultMessage = result;
                }
                paramQueryList.Clear();
            }
            catch (Exception ex)
            {
                isOK = false;
                ResultMessage = ex.Message;
            }

            return isOK;
        }

        /// <summary>
        /// 검수 상세데이터 생성
        /// </summary>
        private void MakeInsertBeadDetailSql()
        {
            try
            {
                if (_dtCSVFile.Rows.Count == 0)
                    return;
            }
            catch (Exception)
            {
                return;
            }

            paramQueryList = new List<ParamQuery>();
            ParamQuery paramQuery;

            //AutoKey값을 Row갯수만큼 한번에 채번한 후 사용하는 방식으로 하자.
            //_LastKey = LogicManager.Common.GetNextKey("BeadDetailKey", _dtCSVFile.Rows.Count);
            _LastKey = DBManager.Instance.MDB.uspGetNextKey("BeadDetailKey", _dtCSVFile.Rows.Count);
            int currKey = _LastKey - _dtCSVFile.Rows.Count + 1;

            string sql, tmpValue;
            foreach (DataRow dr in _dtCSVFile.Rows)
            {
                paramQuery = new ParamQuery();

                sql = string.Empty;
                sql += "INSERT INTO [dbo].[BeadDetail] ";
                sql += "           ([BeadDetailKey] ";
                sql += "           ,[BeadKey] ";
                sql += "           ,[LineNo] ";
                sql += "           ,[SerialNo] ";
                sql += "           ,[InspectionNo] ";
                sql += "           ,[InspectionDate] ";
                sql += "           ,[InspectionTime] ";
                sql += "           ,[Position] ";
                sql += "           ,[isPass] ";
                sql += "           ,[KValue] ";
                sql += "           ,[isKPass] ";
                sql += "           ,[BeadWidthTotal] ";
                sql += "           ,[isBWidthPass] ";
                sql += "           ,[B1Width] ";
                sql += "           ,[B2Width] ";
                sql += "           ,[BRatio] ";
                sql += "           ,[isBRatioPass] ";
                sql += "           ,[MissAlignValue] ";
                sql += "           ,[isMissAlignPass] ";
                sql += "           ,[NotchValue] ";
                sql += "           ,[isNotchPass] ";
                sql += "           ,[ContactAngle] ";
                sql += "           ,[Descr] ";
                sql += "           ,[CreateID] ";
                sql += "           ,[CreateDtm]) ";
                sql += "     VALUES ";
                sql += "           (" + currKey + " ";
                sql += "           ," + BeadKey + " ";

                tmpValue = dr[0].ToString();     //LineNo
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,0 ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'" + dr[1].ToString() + "' ";   //SerialNo
                tmpValue = dr[2].ToString();     //Insp. no
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,0 ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'" + dr[3].ToString() + "' ";   //Insp. date
                sql += "           ,'" + dr[4].ToString() + "' ";   //Insp. time
                tmpValue = dr[5].ToString();     //Position
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,0 ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'" + dr[6].ToString() + "' ";   //isPass

                tmpValue = dr[7].ToString();     //KValue
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'" + dr[8].ToString() + "' ";   //isKPass

                tmpValue = dr[9].ToString();     //BeadWidthTotal
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'" + dr[10].ToString() + "' ";   //isBWidthPass

                tmpValue = dr[11].ToString();     //B1Width
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[12].ToString();     //B2Width
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                tmpValue = dr[13].ToString();     //BRatio
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'" + dr[14].ToString() + "' ";   //isBRatioPass

                tmpValue = dr[15].ToString();     //MissAlignValue
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'" + dr[16].ToString() + "' ";   //isMissAlignPass

                tmpValue = dr[17].ToString();     //NotchValue
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'" + dr[18].ToString() + "' ";   //isNotchPass

                tmpValue = dr[19].ToString();     //ContactAngle
                if (string.IsNullOrWhiteSpace(tmpValue))
                    sql += "           ,NULL ";
                else
                    sql += "           ," + tmpValue + " ";

                sql += "           ,'' ";   //Descr
                sql += "           ,'" + Program.Option.LoginID + "' ";
                sql += "           ,GETDATE())";

                paramQuery.Sql = sql;
                paramQueryList.Add(paramQuery);

                //검수 Image정보생성 sql
                string lineNo = dr[0].ToString().PadLeft(6, '0');
                MakeBeadImage(currKey, lineNo); 

                //1씩 증가
                currKey++;
            }
        }

        /// <summary>
        /// 검수 이미지정보(좌표 포함)
        /// </summary>
        private void MakeBeadImage(int bdKey, string lineNo)
        {
            ParamQuery paramQuery = new ParamQuery();

            string lineFileName = string.Format("{0}-{1}_line.csv", _FolderName, lineNo);
            string oedFileName = string.Format("{0}-{1}_oed.jpg", _FolderName, lineNo);
            string rawFileName = string.Format("{0}-{1}_raw.jpg", _FolderName, lineNo);
            byte[] binaryLine, binaryOed, binaryRaw;

            //합성 검수이미지(oed) 로딩
            try
            {
                if (isOedImage)
                {
                    FileStream fs = new FileStream(Path.Combine(_CsvFullPath, oedFileName), FileMode.Open, FileAccess.Read);
                    int intLength = Convert.ToInt32(fs.Length);
                    binaryOed = new byte[intLength];
                    fs.Read(binaryOed, 0, intLength);
                    fs.Close();
                }
                else
                    binaryOed = new byte[1];
            }
            catch (Exception ex)
            {
                binaryOed = null;
                ResultMessage = ex.Message;
            }

            //원본 검수이미지(raw) 로딩
            try
            {
                if (isRawImage)
                {
                    FileStream fs = new FileStream(Path.Combine(_CsvFullPath, rawFileName), FileMode.Open, FileAccess.Read);
                    int intLength = Convert.ToInt32(fs.Length);
                    binaryRaw = new byte[intLength];
                    fs.Read(binaryRaw, 0, intLength);
                    fs.Close();
                }
                else
                    binaryRaw = new byte[1];
            }
            catch (Exception ex)
            {
                binaryRaw = null;
                ResultMessage = ex.Message;
            }

            //XY좌표 csv File 로딩
            try
            {
                if (isXyLineImage)
                {
                    FileStream fs = new FileStream(Path.Combine(_CsvFullPath, lineFileName), FileMode.Open, FileAccess.Read);
                    int intLength = Convert.ToInt32(fs.Length);
                    binaryLine = new byte[intLength];
                    fs.Read(binaryLine, 0, intLength);
                    fs.Close();
                }
                else
                    binaryLine = new byte[1];
            }
            catch (Exception ex)
            {
                binaryLine = null;
                ResultMessage = ex.Message;
            }

            string sql = string.Empty;
            sql += "INSERT INTO [dbo].[BeadImageInfo] ";
            sql += "           ([BeadDetailKey] ";
            sql += "           ,[RawImage] ";
            sql += "           ,[OedImage] ";
            sql += "           ,[LineInfo] ";
            sql += "           ,[CreateID] ";
            sql += "           ,[CreateDtm]) ";
            sql += "     VALUES ";
            sql += "           (" + bdKey + " ";
            sql += "           , @RawImage";
            sql += "           , @OedImage";
            sql += "           , @LineInfo";
            sql += "           ,'" + Program.Option.LoginID + "' ";
            sql += "           ,GETDATE()) ";

            paramQuery.AddParameter("@RawImage", binaryRaw);
            paramQuery.AddParameter("@OedImage", binaryOed);
            paramQuery.AddParameter("@LineInfo", binaryLine);
            paramQuery.Sql = sql;
            paramQueryList.Add(paramQuery);
        }
    }
}
