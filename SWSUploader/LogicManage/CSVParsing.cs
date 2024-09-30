using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace SWSUploader
{
    class CSVParsing
    {
        private int _CompKey;

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

        private string _MatCode;
        private int TotColumnCount;
        private int TotRowCount;
        private int CurrLineNo;
        private string CurrCellValue;
        private int CurrCellValueLength;
        private double CurrCellValueDouble;
        private string fileName;

        private DetailItemsIO filter;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="aPath"></param>
        public CSVParsing(string aPath)
        {
            fileName = Path.GetFileName(aPath);
            filter = new DetailItemsIO();
            _Querys = new List<string>();

            isOK = false;
            try
            {
                //Csv로딩 및 DataTable생성
                _dtCSVFile = LogicManager.Common.GetDataTabletFromCSVFile(aPath);
                if (_dtCSVFile.Rows.Count > 0)
                    isOK = true;
                else
                {
                    ResultMessage = string.Format("{0} 파일에 데이터가 비어있습니다.", aPath);
                    MakeResultQuery("", ResultMessage);
                }
            }
            catch (Exception ex)
            {
                _dtCSVFile = null;
                ResultMessage = string.Format("{0}은 정해진 형식의 엑셀파일이 아닙니다. 확인 후 다시 시도해 주세요.", fileName);
                MakeResultQuery(ex.Message, ResultMessage);
            }

            //검사 수행
            if (isOK)
                MainValidation();
        }

        /// <summary>
        /// 검사 수행 Main
        /// </summary>
        private void MainValidation()
        {
            TotColumnCount = _dtCSVFile.Columns.Count;
            TotRowCount = _dtCSVFile.Rows.Count;
            string colName = string.Empty;
            int itemKey = 0, matKey = 0;
            int saveItemKey = 0, saveMatKey = 0;

            //ItemMaster (검사대상 항목 리스트) Loading
            LogicManager.Common.LoadedItemMaster(_CompKey);
            if (string.IsNullOrWhiteSpace(LogicManager.Common.ItemMatCode) && string.IsNullOrWhiteSpace(_MatCode)) 
                return; //자재Key를 못찾으면 Skip

            MakeResultQuery("검사시작", string.Format("[{0}]파일 검사를 시작했습니다.", fileName));

            //----------------------------------------
            //DataTable 전체를 열단위로 검사 수행
            //----------------------------------------
            for (int i = 0; i < TotColumnCount; i++)  //Column Loop
            {
                colName = _dtCSVFile.Columns[i].ColumnName;
                itemKey = LogicManager.Common.GetColumnKey(_CompKey, colName);
                //matKey = LogicManager.Common.GetMatKey(_CompKey, _MatCode); //지정한 MatCode
                for (int j = 0; j < TotRowCount; j++) //Row Loop
                {
                    if (string.IsNullOrWhiteSpace(_MatCode))
                        matKey = LogicManager.Common.GetMatKey(_CompKey, _dtCSVFile.Rows[j][LogicManager.Common.ItemMatCode].ToString()); //Line에 있는 MatCode
                    else
                        matKey = LogicManager.Common.GetMatKey(_CompKey, _MatCode); //지정한 MatCode

                    //위치가 바뀔때마다 조건 다시 불러오기
                    if ((saveItemKey != itemKey) || (saveMatKey != matKey))
                    {
                        filter = new DetailItemsIO(_CompKey, matKey, itemKey);
                        saveItemKey = itemKey;
                        saveMatKey = matKey;
                    }

                    //검사대상이 아니면 현재 열(Column)은 생략 (continue로 할지 break로 할지 생각해보자.
                    if (!filter.isTarget)
                        continue;

                    //검사 수행
                    CurrLineNo = j + 1;
                    CurrCellValue = _dtCSVFile.Rows[j][i].ToString();
                    RunDetailCondition();
                }
            }
        }

        /// <summary>
        /// 데이터 검사 시작
        /// </summary>
        /// <param name="CellValue"></param>
        private void RunDetailCondition()
        {
            //필수 항목에 값이 비어있는지 확인
            if (filter.isNonNullable)
            {
                if (string.IsNullOrWhiteSpace(CurrCellValue))
                {
                    MakeResultQuery("", string.Format("[{0}]항목의 값이 비어있습니다!", filter.ColName));
                    return;
                }
            }

            //문자와 숫자로 구분하여 검사
            if (filter.isNumeric || filter.isLength)
                CheckNumericCondition(); //숫자 데이터검사
            else
                CheckStringCondition();  //문자 데이터검사
        }

        /// <summary>
        /// 검사 결과데이터 생성 쿼리 
        /// </summary>
        /// <param name="cellValue">실제 값</param>
        /// <param name="filterString">조합된 조건 문자열</param>
        /// <param name="errMessage">오류 메시지</param>
        private void MakeResultQuery(string filterString, string errMessage)
        {
            string sql = string.Empty;
            sql += "INSERT INTO [ResultTesting] ";
            sql += "		([FilterKey] ";
            sql += "		,[RunDate] ";
            sql += "		,[FileName] ";
            sql += "		,[LineNo] ";
            sql += "		,[ColName] ";
            sql += "		,[OrgValue] ";
            sql += "		,[FilterString] ";
            sql += "		,[isError] ";
            sql += "		,[isDeleted] ";
            sql += "		,[Descr] ";
            sql += "		,[CreateID] ";
            sql += "		,[CreateDtm]) ";
            sql += "	VALUES ";
            sql += "		(" + filter.FilterKey + " ";
            sql += "		,CURRENT_DATE ";
            sql += "		,'" + fileName + "' ";
            sql += "		," + CurrLineNo + " ";
            sql += "		,'" + filter.ColName + "' ";
            sql += "		,'" + CurrCellValue + "' ";
            sql += "		,'" + filterString + "' ";
            sql += "		,1 ";
            sql += "		,0 ";
            sql += "		,'" + errMessage + "' ";
            sql += "		,'sys' ";
            sql += "		,CURRENT_DATE)";

            Querys.Add(sql);
        }

        /// <summary>
        /// 숫자인 경우
        /// </summary>
        private void CheckNumericCondition()
        {
            CurrCellValue = CurrCellValue.Trim();
            CurrCellValueLength = CurrCellValue.Length; //문자열의 길이

            if (!isDouble(CurrCellValue) && filter.isNumeric) //숫자인지 여부
            {
                MakeResultQuery("", string.Format("[{0}]항목의 값이 숫자가 아닙니다.", filter.ColName));
                return;
            }

            //길이 체크가 아니면 숫자로 변환하는데 혹시 빈공백이 들어있다면 0으로 처리
            if (!filter.isLength)
            {
                
                if (!string.IsNullOrWhiteSpace(CurrCellValue))
                    CurrCellValueDouble = double.Parse(CurrCellValue); //문자값을 Double타입으로 변환
                else
                    CurrCellValueDouble = 0;
            }

            switch (filter.OpCode.ToLower())
            {
                case "=":
                    CompareCommEquals();
                    break;
                case "≠":
                    CompareCommNotEquals();
                    break;
                case "isblank":
                    CompareCommIsBlank();
                    break;
                case "isnotblank":
                    CompareCommIsNotBlank();
                    break;
                case ">":
                    CompareNumGreaterThan();
                    break;
                case "≥":
                    CompareNumGreaterThanOrEqual();
                    break;
                case "<":
                    CompareNumLessThan();
                    break;
                case "≤":
                    CompareNumLessThanOrEqual();
                    break;
                case "between":
                    CompareNumBetween();
                    break;
                case "notbetween":
                    CompareNumNotBetween();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 공통 - Equals (=)
        /// </summary>
        /// <param name="CellValue"></param>
        private void CompareCommEquals()
        {
            string filterString;

            //길이값 비교여부에 따라 검사
            if (filter.isLength) 
            {
                if (filter.AValue != CurrCellValueLength) //같지 않으면 오류
                {
                    filterString = string.Format("[기준조건] {0}의 길이 = {1}", filter.ColName, filter.AValue);
                    MakeResultQuery(filterString, string.Format("[{0}] 지정한 값의 길이가 아닙니다.", filter.ColName));
                    return;
                }
            }
            else
            {
                if (filter.FixValue != CurrCellValue) //같지 않으면 오류
                {
                    filterString = string.Format("[기준조건] {0} = {1}", filter.ColName, filter.FixValue);
                    MakeResultQuery(filterString, string.Format("[{0}] 지정한 값이 아닙니다.", filter.ColName));
                    return;
                }
            }
        }

        /// <summary>
        /// 공통 - Not Equals (≠)
        /// </summary>
        private void CompareCommNotEquals()
        {
            string filterString;

            //길이값 비교여부에 따라 검사
            if (filter.isLength)
            {
                if (filter.AValue == CurrCellValueLength) //같으면 오류
                {
                    filterString = string.Format("[기준조건] {0}의 길이 ≠ {1}", filter.ColName, filter.AValue);
                    MakeResultQuery(filterString, string.Format("[{0}] 지정한 값의 길이와 달라야 합니다.", filter.ColName));
                    return;
                }
            }
            else
            {
                if (filter.FixValue == CurrCellValue) //같으면 오류
                {
                    filterString = string.Format("[기준조건] {0} ≠ {1}", filter.ColName, filter.FixValue);
                    MakeResultQuery(filterString, string.Format("[{0}] 지정한 값과 달라야 합니다.", filter.ColName));
                    return;
                }
            }
        }

        /// <summary>
        /// 공통 - isBlank
        /// </summary>
        private void CompareCommIsBlank()
        {
            string filterString = string.Format("[기준조건] {0} = 빈값", filter.ColName);
            //공백이어야 함
            if (!string.IsNullOrWhiteSpace(CurrCellValue))
            {
                MakeResultQuery(filterString, string.Format("[{0}] 빈값이어야 합니다.", filter.ColName));
                return;
            }
        }

        /// <summary>
        /// 공통 - isNotBlank
        /// </summary>
        private void CompareCommIsNotBlank()
        {
            string filterString = string.Format("[기준조건] {0} ≠ 빈값", filter.ColName);
            //공백이 아니어야 함
            if (string.IsNullOrWhiteSpace(CurrCellValue))
            {
                MakeResultQuery(filterString, string.Format("[{0}] 값이 비어있습니다.", filter.ColName));
                return;
            }
        }

        /// <summary>
        /// 숫자 - Is greater than ( > )
        /// </summary>
        private void CompareNumGreaterThan()
        {
            string filterString;

            //길이값 비교여부에 따라 검사
            if (filter.isLength)
            {
                if (!(CurrCellValueLength > filter.AValue))
                {
                    filterString = string.Format("[기준조건] {0}의 길이 > {1}", filter.ColName, filter.AValue);
                    MakeResultQuery(filterString, string.Format("[{0}] 값의 길이는 지정한 길이보다 커야 합니다.", filter.ColName));
                    return;
                }
            }
            else
            {
                if (!(CurrCellValueDouble > filter.AValue))
                {
                    filterString = string.Format("[기준조건] {0} > {1}", filter.ColName, filter.AValue);
                    MakeResultQuery(filterString, string.Format("[{0}] 값은 지정한 값보다 커야 합니다.", filter.ColName));
                    return;
                }
            }
        }

        /// <summary>
        /// 숫자 - Is greater than or equal to ( ≥ )
        /// </summary>
        private void CompareNumGreaterThanOrEqual()
        {
            string filterString;

            //길이값 비교여부에 따라 검사
            if (filter.isLength)
            {
                if (!(CurrCellValueLength >= filter.AValue))
                {
                    filterString = string.Format("[기준조건] {0}의 길이 ≥ {1}", filter.ColName, filter.AValue);
                    MakeResultQuery(filterString, string.Format("[{0}] 값의 길이는 지정한 길이보다 크거나 같아야 합니다.", filter.ColName));
                    return;
                }
            }
            else
            {
                if (!(CurrCellValueDouble >= filter.AValue))
                {
                    filterString = string.Format("[기준조건] {0} ≥ {1}", filter.ColName, filter.AValue);
                    MakeResultQuery(filterString, string.Format("[{0}] 값은 지정한 값보다 크거나 같아야 합니다.", filter.ColName));
                    return;
                }
            }
        }

        /// <summary>
        /// 숫자 - Is less than ( < )
        /// </summary>
        private void CompareNumLessThan()
        {
            string filterString;

            //길이값 비교여부에 따라 검사
            if (filter.isLength)
            {
                if (!(CurrCellValueLength < filter.AValue))
                {
                    filterString = string.Format("[기준조건] {0}의 길이 < {1}", filter.ColName, filter.AValue);
                    MakeResultQuery(filterString, string.Format("[{0}] 값의 길이는 지정한 길이보다 작아야 합니다.", filter.ColName));
                    return;
                }
            }
            else
            {
                if (!(CurrCellValueDouble < filter.AValue))
                {
                    filterString = string.Format("[기준조건] {0} < {1}", filter.ColName, filter.AValue);
                    MakeResultQuery(filterString, string.Format("[{0}] 값은 지정한 값보다 작아야 합니다.", filter.ColName));
                    return;
                }
            }
        }

        /// <summary>
        /// 숫자 - Is less than or equal to ( ≤ )
        /// </summary>
        private void CompareNumLessThanOrEqual()
        {
            string filterString;

            //길이값 비교여부에 따라 검사
            if (filter.isLength)
            {
                if (!(CurrCellValueLength <= filter.AValue))
                {
                    filterString = string.Format("[기준조건] {0}의 길이 ≤ {1}", filter.ColName, filter.AValue);
                    MakeResultQuery(filterString, string.Format("[{0}] 값의 길이는 지정한 길이보다 작거나 같아야 합니다.", filter.ColName));
                    return;
                }
            }
            else
            {
                if (!(CurrCellValueDouble <= filter.AValue))
                {
                    filterString = string.Format("[기준조건] {0} ≤ {1}", filter.ColName, filter.AValue);
                    MakeResultQuery(filterString, string.Format("[{0}] 값은 지정한 값보다 작거나 같아야 합니다.", filter.ColName));
                    return;
                }
            }
        }

        /// <summary>
        /// 숫자 - between A and B ( between )
        /// </summary>
        private void CompareNumBetween()
        {
            string filterString;

            //길이값 비교여부에 따라 검사
            if (filter.isLength)
            {
                if (!(CurrCellValueLength >= filter.AValue && CurrCellValueLength <= filter.BValue))
                {
                    filterString = string.Format("[기준조건] {0}의 길이 Between {1} and {2} ", filter.ColName, filter.AValue, filter.BValue);
                    MakeResultQuery(filterString, string.Format("[{0}] 값의 길이가 범위를 벗어났습니다.", filter.ColName));
                    return;
                }
            }
            else
            {
                if (!(CurrCellValueDouble >= filter.AValue && CurrCellValueDouble <= filter.BValue))
                {
                    filterString = string.Format("[기준조건] {0} Between {1} and {2} ", filter.ColName, filter.AValue, filter.BValue);
                    MakeResultQuery(filterString, string.Format("[{0}] 값이 범위를 벗어났습니다.", filter.ColName));
                    return;
                }
            }
        }

        /// <summary>
        /// 숫자 - Not between A and B ( NotBetween )
        /// </summary>
        private void CompareNumNotBetween()
        {
            string filterString;

            //길이값 비교여부에 따라 검사
            if (filter.isLength)
            {
                if ((CurrCellValueLength >= filter.AValue && CurrCellValueLength <= filter.BValue))
                {
                    filterString = string.Format("[기준조건] {0}의 길이 Not Between {1} and {2} ", filter.ColName, filter.AValue, filter.BValue);
                    MakeResultQuery(filterString, string.Format("[{0}] 값의 길이가 지정한 범위의 길이가 아닙니다.", filter.ColName));
                    return;
                }
            }
            else
            {
                if ((CurrCellValueDouble >= filter.AValue && CurrCellValueDouble <= filter.BValue))
                {
                    filterString = string.Format("[기준조건] {0} Not Between {1} and {2} ", filter.ColName, filter.AValue, filter.BValue);
                    MakeResultQuery(filterString, string.Format("[{0}] 값이 지정한 범위의 값이 아닙니다.", filter.ColName));
                    return;
                }
            }
        }

        /// <summary>
        /// 문자 - Contains
        /// </summary>
        private void CompareStrngContains()
        {
            string filterString = string.Format("[기준조건] {0}내에 [{1}]값을 포함", filter.ColName, filter.FixValue);

            if (!CurrCellValue.Contains(filter.FixValue))
            {
                MakeResultQuery(filterString, string.Format("{0}내에 [{1}]값이 포함되어있지 않습니다.", filter.ColName, filter.FixValue));
                return;
            }
        }

        /// <summary>
        /// 문자 - Contains
        /// </summary>
        private void CompareStrngNotContains()
        {
            string filterString = string.Format("[기준조건] {0} Contains [{1}]", filter.ColName, filter.FixValue);

            if (!CurrCellValue.Contains(filter.FixValue))
            {
                MakeResultQuery(filterString, string.Format("{0}내에 [{1}]값이 포함되면 안됩니다.", filter.ColName, filter.FixValue));
                return;
            }
        }

        /// <summary>
        /// 문자 - Begins with
        /// </summary>
        private void CompareStrngBeginsWith()
        {
            string filterString = string.Format("[기준조건] {0} Begins with [{1}]", filter.ColName, filter.FixValue);

            if (!CurrCellValue.StartsWith(filter.FixValue))
            {
                MakeResultQuery(filterString, string.Format("{0}의 값은 [{1}]값으로 시작되는 문자이어야 합니다.", filter.ColName, filter.FixValue));
                return;
            }
        }

        /// <summary>
        /// 문자 - Ends with
        /// </summary>
        private void CompareStrngEndsWith()
        {
            string filterString = string.Format("[기준조건] {0} Ends with [{1}]", filter.ColName, filter.FixValue);

            if (!CurrCellValue.EndsWith(filter.FixValue))
            {
                MakeResultQuery(filterString, string.Format("{0}의 값은 [{1}]값으로 끝나는 문자이어야 합니다.", filter.ColName, filter.FixValue));
                return;
            }
        }

        /// <summary>
        /// 문자인 경우
        /// </summary>
        private void CheckStringCondition()
        {
            switch (filter.OpCode.ToLower())
            {
                case "=":
                    CompareCommEquals();
                    break;
                case "≠":
                    CompareCommNotEquals();
                    break;
                case "isblank":
                    CompareCommIsBlank();
                    break;
                case "isnotblank":
                    CompareCommIsNotBlank();
                    break;
                case "contains":
                    CompareStrngContains();
                    break;
                case "notcontains":
                    CompareStrngNotContains();
                    break;
                case "beginswith":
                    CompareStrngBeginsWith();
                    break;
                case "endswith":
                    CompareStrngEndsWith();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 문자열이 숫자 형식인지를 확인하는 함수
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public bool IsNumber(string strValue)
        {
            if (strValue == null || strValue.Length < 1)
                return false;

            //요기서 정규식 사용
            Regex reg = new Regex(@"^(\d)+$");

            return reg.IsMatch(strValue);
        }

        /// <summary>
        /// 문자열이 알파벳으로만 구성되어 있는지를 확인하는 함수
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public bool IsAlpabet(string strValue)
        {
            if (strValue == null || strValue.Length < 1)
                return false;

            Regex reg = new Regex(@"^[a-zA-Z]+$");
            return reg.IsMatch(strValue);
        }

        /// <summary>
        /// 시작문자열이 알파벳이고, 알파벳과 숫자로 이루어진 문자열인지 여부 확인하는 함수
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public bool IsAlpaNumber(string strValue)
        {
            if (strValue == null || strValue.Length < 1)
                return false;

            Regex reg = new Regex(@"^[a-zA-Z]+[0-9]*$");
            return reg.IsMatch(strValue);
        }

        /// <summary>
        /// 숫자검사
        /// </summary>
        /// <param name="orgStr"></param>
        /// <returns></returns>
        public bool isDigit(string orgStr)
        {
            if (orgStr == null) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(orgStr, "^\\d+$");
        }

        public bool isInt(string orgStr)
        {
            if (orgStr == null) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(orgStr, @"^[+-]?\d*$");
        }

        public bool isDouble(string orgStr)
        {
            if (orgStr == null) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(orgStr, @"^[+-]?\d*(\.?\d*)$");
        }

    }
}
