using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWSAnalyzer
{
    public class OptionInfo
    {
        private string _ServerAddress;
        /// <summary>
        /// DB서버주소
        /// </summary>
        public string ServerAddress
        {
            get { return _ServerAddress; }
            set { _ServerAddress = value; }
        }

        private int _CompKey;
        /// <summary>
        /// 거래처 코드
        /// </summary>
        public int CompKey
        {
            get { return _CompKey; }
            set { _CompKey = value; }
        }

        private string _cultureName = "";
        /// <summary>
        /// 언어
        /// </summary>
        public string cultureName
        {
            get { return _cultureName; }
            set { _cultureName = value; }
        }

        private int _ProjectKey;
        /// <summary>
        /// 프로젝트 코드
        /// </summary>
        public int ProjectKey
        {
            get { return _ProjectKey; }
            set { _ProjectKey = value; }
        }

        private string loginID;
        /// <summary>
        /// 사용자 ID
        /// </summary>
        public string LoginID
        {
            get { return loginID; }
            set
            {
                loginID = value;
            }
        }

        private int _AuthLevel;
        /// <summary>
        /// 권한레벨
        /// </summary>
        public int AuthLevel
        {
            get { return _AuthLevel; }
            set { _AuthLevel = value; }
        }

        /// <summary>
        /// 관리자 여부
        /// </summary>
        public bool isAdmin
        {
            get
            {
                if (AuthLevel == 0)
                    return true;
                else
                    return false;
            }
        }

        private string _CompName;
        /// <summary>
        /// 거래처명
        /// </summary>
        public string CompName
        {
            get { return _CompName; }
            set { _CompName = value; }
        }

        private string _ProjectName;
        /// <summary>
        /// 프로젝트명
        /// </summary>
        public string ProjectName
        {
            get { return _ProjectName; }
            set { _ProjectName = value; }
        }

        private bool _UploadInspectData;
        /// <summary>
        /// 검수 데이터 업로드 권한
        /// </summary>
        public bool UploadInspectData
        {
            get { return _UploadInspectData; }
            set { _UploadInspectData = value; }
        }

        private bool _UploadWeldData;
        /// <summary>
        /// 융착 데이터 업로드 권한
        /// </summary>
        public bool UploadWeldData
        {
            get { return _UploadWeldData; }
            set { _UploadWeldData = value; }
        }

        private bool _ShowInspectUploadRecord;
        /// <summary>
        /// 검수 데이터 업로드 기록 보기 권한
        /// </summary>
        public bool ShowInspectUploadRecord
        {
            get { return _ShowInspectUploadRecord; }
            set { _ShowInspectUploadRecord = value; }
        }

        private bool _ShowWeldUploadRecord;
        /// <summary>
        /// 융착 데이터 업로드 기록 보기 권한
        /// </summary>
        public bool ShowWeldUploadRecord
        {
            get { return _ShowWeldUploadRecord; }
            set { _ShowWeldUploadRecord = value; }
        }

        private bool _FormBeadMasterManage;
        /// <summary>
        /// Form 검수데이터관리 권한
        /// </summary>
        public bool FormBeadMasterManage
        {
            get { return _FormBeadMasterManage; }
            set { _FormBeadMasterManage = value; }
        }

        private bool _FormWeldMasterManage;
        /// <summary>
        /// Form 융착데이터관리 권한
        /// </summary>
        public bool FormWeldMasterManage
        {
            get { return _FormWeldMasterManage; }
            set { _FormWeldMasterManage = value; }
        }

        private bool _FormBeadDetailList;
        /// <summary>
        /// Form 검수데이터상세조회 권한
        /// </summary>
        public bool FormBeadDetailList
        {
            get { return _FormBeadDetailList; }
            set { _FormBeadDetailList = value; }
        }

        private bool _FormBeadAndWeldList;
        /// <summary>
        /// Form 검수/융착조회 권한
        /// </summary>
        public bool FormBeadAndWeldList
        {
            get { return _FormBeadAndWeldList; }
            set { _FormBeadAndWeldList = value; }
        }

        private bool _FormGeneralChart1;
        /// <summary>
        /// Form 주요현황 조회 권한
        /// </summary>
        public bool FormGeneralChart1
        {
            get { return _FormGeneralChart1; }
            set { _FormGeneralChart1 = value; }
        }

        private bool _FormPivotResult;
        /// <summary>
        /// Form 집계 Pivot 권한
        /// </summary>
        public bool FormPivotResult
        {
            get { return _FormPivotResult; }
            set { _FormPivotResult = value; }
        }

        private bool _FormDashboard;
        /// <summary>
        /// Form 요약통계 권한
        /// </summary>
        public bool FormDashboard
        {
            get { return _FormDashboard; }
            set { _FormDashboard = value; }
        }

        private bool _FormDashboard2;
        /// <summary>
        /// Form 융착통계 권한
        /// </summary>
        public bool FormDashboard2
        {
            get { return _FormDashboard2; }
            set { _FormDashboard2 = value; }
        }

        private bool _FormDashboard3;
        /// <summary>
        /// Form 검수통계 권한
        /// </summary>
        public bool FormDashboard3
        {
            get { return _FormDashboard3; }
            set { _FormDashboard3 = value; }
        }

        private bool _SaveExcel;
        /// <summary>
        /// 엑셀저장 권한
        /// </summary>
        public bool SaveExcel
        {
            get { return _SaveExcel; }
            set { _SaveExcel = value; }
        }

        private bool _DeleteInspectData;
        /// <summary>
        /// 검수데이터 삭제 권한
        /// </summary>
        public bool DeleteInspectData
        {
            get { return _DeleteInspectData; }
            set { _DeleteInspectData = value; }
        }

        private bool _DeleteWeldData;
        /// <summary>
        /// 융착데이터 삭제 권한
        /// </summary>
        public bool DeleteWeldData
        {
            get { return _DeleteWeldData; }
            set { _DeleteWeldData = value; }
        }
    }
}
